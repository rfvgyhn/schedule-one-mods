using System.Collections;
using MelonLoader;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Quests;
using Il2CppScheduleOne.UI.Phone;
using ScheduleOneMods.Logging;

namespace ScheduleOneMods.DealsSummary;

public sealed class Mod : MelonMod
{
    private SummaryRefs? _summaryRefs;
    private UiRefs? _uiRefs;
    private Calculator? _calculator;
    private int _lastContractCount = -1;
    private readonly List<Contract> _activeContracts = new();
#if DEBUG
    private bool _logged;
#endif
    
    public override void OnInitializeMelon()
    {
        base.OnInitializeMelon();
        Log.SetLogger<Mod>();
    }

    public override void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
        base.OnSceneWasInitialized(buildIndex, sceneName);
        if (sceneName != "Main")
            return;

        MelonCoroutines.Start(InitUi());
        _calculator = new(new() // TODO: figure out a way to look these packaging defs up at runtime
        {
            [1] = "baggie",
            [5] = "jar",
            [20] = "brick" // TODO: doesn't have an "empty" icon. Need to figure out what the icon names are per product
        });
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!PlayerSingleton<JournalApp>.InstanceExists || _summaryRefs is null || _uiRefs is null)
            return;

        var journalApp = PlayerSingleton<JournalApp>.Instance;
        if (!journalApp.isOpen)
            return;

        _activeContracts.Clear();
        foreach (var c in Il2CppScheduleOne.Quests.Contract.Contracts)
        {
            if (c.Dealer is null && c.QuestState == EQuestState.Active)
                _activeContracts.Add(new(c));
        }

        if (_lastContractCount == _activeContracts.Count)
            return;
        _lastContractCount = _activeContracts.Count;
        Log.Debug($"Contract count: {_activeContracts.Count}");

        var totals = _calculator!.CalculateTotals(_activeContracts);

        Log.Trace("Updating summary");
        UI.UpdateSummary(_summaryRefs, _uiRefs, totals);
        Log.Trace("Summary updated");

#if DEBUG
        if (!_logged)
        {
            // Log.Unity.WriteGameObject("journalApp.txt", journalApp.transform);
            // Log.Unity.WriteGameObject("GenericQuestEntry.txt", journalApp.GenericQuestEntry.transform);
            // Log.Unity.WriteGameObject("EntryContainer.txt", journalApp.EntryContainer);
            // Log.Unity.WriteGameObject("GenericDetailsPanel.txt", journalApp.GenericDetailsPanel.transform);
            _logged = true;
        }
#endif
    }

    private IEnumerator InitUi()
    {
        while (!PlayerSingleton<JournalApp>.InstanceExists)
            yield return null;
        Log.Trace("JournalApp instance exists");

        _uiRefs = UI.FindRequiredUiElements(PlayerSingleton<JournalApp>.Instance);
        if (_uiRefs is null)
        {
            Log.Error("Expected UI elements not found");
            yield break;
        }

        yield return null;

        Log.Trace("Creating new UI");
        _summaryRefs = UI.CreateSummaryPanel(_uiRefs);

        Log.Trace("Adjusting original UI elements");
        UI.AdjustTasksPanel(_uiRefs.TasksPanel.transform);
    }
}