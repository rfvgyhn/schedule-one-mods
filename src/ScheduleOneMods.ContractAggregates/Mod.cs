using System.Collections;
using ScheduleOneMods.ContractAggregates;
using MelonLoader;
using ScheduleOne.DevUtilities;
using ScheduleOne.Quests;
using ScheduleOne.UI.Phone;
using UnityEngine;
using ScheduleOneMods.Logging;
using UnityEngine.UI;

[assembly: MelonInfo(typeof(Mod), "Contract Aggregates", "0.0.1", "rfvgyhn")]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace ContractAggregates;

public sealed class Mod : MelonMod
{
    private Transform? _container;
    private Calculator? _calculator;
    private int _lastContractCount = -1;
    private GameObject? _summary;
    private bool _logged;

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
            [20] = "brick"
        });
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!PlayerSingleton<JournalApp>.InstanceExists || _container is null)
            return;

        var journalApp = PlayerSingleton<JournalApp>.Instance;
        if (!journalApp.isOpen)
            return;

        var activeContracts = Contract.Contracts.Where(c => c.Dealer is null && c.QuestState == EQuestState.Active)
            .ToArray();
        if (_lastContractCount == activeContracts.Length)
            return;
        Log.Debug(string.Format("Contract count: {0}", activeContracts.Length));

        _lastContractCount = activeContracts.Length;
        if (_summary is not null)
        {
            Log.Trace("Destroying active summary object");
            UnityEngine.Object.Destroy(_summary);
            _summary = null;
        }

        var totals = _calculator!.CalculateTotals(activeContracts);
        
        Log.Trace("Creating summary");
        _summary = UI.CreateSummary(_container, totals);
        Log.Trace("Summary created");

        if (!_logged)
        {
            // Log.Unity.WriteGameObject("journal-app.txt", journalApp.transform);
            // Log.Unity.WriteGameObject("GenericQuestEntry.txt", journalApp.GenericQuestEntry.transform);
            // Log.Unity.WriteGameObject("EntryContainer.txt", journalApp.EntryContainer);
            // Log.Unity.WriteGameObject("GenericDetailsPanel.txt", journalApp.GenericDetailsPanel.transform);
            _logged = true;
        }
    }

    private IEnumerator InitUi()
    {
        while (!PlayerSingleton<JournalApp>.InstanceExists)
            yield return null;
        Log.Trace("JournalApp instance exists");

        var existingElements = FindRequiredUiElements(PlayerSingleton<JournalApp>.Instance);
        if (existingElements is null)
        {
            Log.Error("Required UI elements not found");
            yield break;
        }

        var (journalContainer, tasksPanel, detailsPanel) = existingElements.Value;

        Log.Trace("Creating new UI");
        _container = UI.CreateSummarySection(journalContainer, tasksPanel, detailsPanel);

        Log.Trace("Adjusting original UI elements");
        UI.AdjustTasksPanel(tasksPanel);
    }

    private static (Transform journalContainer, Transform tasksPanel, GameObject detailsPanel)? FindRequiredUiElements(
        JournalApp app)
    {
        if (app.DetailsPanelContainer is null)
        {
            Log.Error("Details panel container is null");
            return null;
        }

        var detailsPanel = app.DetailsPanelContainer?.parent;
        if (detailsPanel is null)
        {
            Log.Debug("Details panel is null");
            return null;
        }

        var journalContainer = detailsPanel.parent;
        if (journalContainer is null)
        {
            Log.Debug("Journal container is null");
            return null;
        }

        var tasksPanel = journalContainer.transform.Find("Tasks");
        if (tasksPanel is null)
        {
            Log.Debug("Tasks panel is null");
            return null;
        }

        if (tasksPanel.transform.Find("Title").GetComponent<Text>() is null)
        {
            Log.Debug("Title not found");
            return null;
        }

        return (journalContainer, tasksPanel, detailsPanel.gameObject);
    }
}