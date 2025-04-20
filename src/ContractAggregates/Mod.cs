using System.Collections;
using ContractAggregates;
using MelonLoader;
using ScheduleOne.DevUtilities;
using ScheduleOne.Quests;
using ScheduleOne.UI.Phone;
using UnityEngine;
using ScheduleOne;
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
        _calculator = new(new()
        {
            [1] = "bag",
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
        _summary = CreateSummary(_container, totals);
        Log.Trace("Summary created");

        if (!_logged)
        {
            //Log.Journal.LogObjectPaths(journalApp);
            //Log.Journal.LogUiElements();
            _logged = true;
        }
    }

    private static GameObject CreateSummary(Transform parent, Summary[] aggregates)
    {
        var summary = new GameObject("Summary");
        summary.transform.SetParent(parent, false);

        var transform = summary.AddComponent<RectTransform>();
        transform.anchorMin = Vector2.zero;
        transform.anchorMax = Vector2.one;
        transform.anchoredPosition = new Vector2(60, -60);

        var t = aggregates.Select(s => string.Format("{0}x{1}: {2}", 
            s.Total,
            Registry.GetItem(s.ProductId).Name,
            string.Join(' ', s.Aggregates.Select(v => string.Format("{0}x {1}", v.Value, v.Key)))));

        var text = summary.AddComponent<Text>();
        text.text = string.Join(", ", t);
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 14;
        text.color = Color.gray;
        text.alignment = TextAnchor.UpperLeft;

        return summary;
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
        _container = CreateUi(journalContainer, tasksPanel, detailsPanel);

        Log.Trace("Adjusting original UI elements");
        AdjustTasksPanel(tasksPanel);
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

    private static void AdjustTasksPanel(Transform tasksPanel)
    {
        var rTransform = tasksPanel.GetComponent<RectTransform>();
        rTransform.anchorMin = new Vector2(rTransform.anchorMin.x, 0.3f);
        rTransform.offsetMin = new Vector2(rTransform.offsetMin.x, rTransform.offsetMin.y - 60);
    }

    private static void CleanupClone(Transform clone)
    {
        var noDetailsLabel = clone.transform.Find("NoDetails");
        if (noDetailsLabel is not null)
            UnityEngine.Object.Destroy(noDetailsLabel.gameObject);
        else
            Log.Trace("NoDetails not found");
    }

    private static Transform CreateUi(Transform parent, Transform tasksPanel, GameObject detailsPanel)
    {
        Log.Trace("Creating outer container");
        var summaryPanel = UnityEngine.Object.Instantiate(detailsPanel);
        summaryPanel.gameObject.name = "Summary";
        summaryPanel.transform.SetParent(parent, false);

        var container = summaryPanel.transform.Find("Container");
        CleanupClone(container);

        summaryPanel.transform.Find("Title")!.GetComponent<Text>()!.text = "Deal Summary";
        var rTransform = summaryPanel.GetComponent<RectTransform>();
        rTransform.anchorMin = new Vector2(0, 0);
        rTransform.anchorMax = new Vector2(tasksPanel.GetComponent<RectTransform>()!.anchorMax.x, 0.3f);
        rTransform.offsetMax = new Vector2(10, rTransform.offsetMax.y);

        return container.transform;
    }
}