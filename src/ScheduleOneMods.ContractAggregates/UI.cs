using MelonLoader;
using ScheduleOne;
using ScheduleOne.Product;
using ScheduleOne.UI.Phone;
using ScheduleOneMods.Logging;
using UnityEngine;
using UnityEngine.UI;

namespace ScheduleOneMods.ContractAggregates;

public record SummaryRefs(Transform ContentContainer, GameObject NoDealsLabel);

public record UiRefs(
    Transform JournalContainer,
    GameObject TasksPanel,
    GameObject EntryContainer,
    Text EntryText);

public static class UI
{
    private const string EntryTitlePath = "Title";
    private const string TasksTitlePath = "Title";
    private const string TasksContentPath = "Scroll View/Viewport/Content";
    private const string TasksNoDetailsPath = "Scroll View/Viewport/NoTasks";

    /// <summary>
    /// Ensures the layout elements this mod is based on exist in their expected places.
    /// </summary>
    /// <remarks>
    /// Layout could be broken/wonky if they aren't in the expected locations as there could be missing or incorrect
    /// GameObject/Component clones
    /// </remarks>
    public static UiRefs? FindRequiredUiElements(JournalApp app)
    {
        if (app.GenericQuestEntry is null)
        {
            Log.Debug("GenericQuestEntry is null");
            return null;
        }

        var entryTitle = app.GenericQuestEntry.transform.Find(EntryTitlePath);
        if (entryTitle is null)
        {
            Log.Debug("GenericQuestEntry/Title is null");
            return null;
        }

        var text = entryTitle.GetComponent<Text>();
        if (text is null)
        {
            Log.Debug("GenericQuestEntry/Title[Text] is null");
            return null;
        }

        var entryText = UnityEngine.Object.Instantiate(text);
        var entryContainer = UnityEngine.Object.Instantiate(app.GenericQuestEntry);
        DeleteAllChildren(entryContainer.transform);

        var journalContainer = app.transform.Find("Container");
        if (journalContainer is null)
        {
            Log.Debug("Container not found");
            return null;
        }

        var tasksContainer = journalContainer.transform.Find("Tasks");
        if (tasksContainer is null)
        {
            Log.Debug("Container/Tasks is null");
            return null;
        }

        var tasksTitle = tasksContainer.transform.Find(TasksTitlePath);
        if (tasksTitle is null)
        {
            Log.Debug("Container/Tasks/Title is null");
            return null;
        }

        var noTasks = tasksContainer.transform.Find(TasksNoDetailsPath);
        if (noTasks is null)
        {
            Log.Debug("Container/Tasks/Scroll View/Viewport/NoTasks is null");
            return null;
        }

        var tasksContent = tasksContainer.transform.Find(TasksContentPath);
        if (tasksContent is null)
        {
            Log.Debug("Container/Tasks/Scroll View/Viewport/Content is null");
            return null;
        }

        return new UiRefs(journalContainer, tasksContainer.gameObject, entryContainer.gameObject, entryText);
    }

    public static void UpdateSummary(SummaryRefs sRefs, UiRefs uiRefs, Summary[] summaries)
    {
        Log.Trace("Destroying active summary objects");
        DeleteAllChildren(sRefs.ContentContainer);

        sRefs.NoDealsLabel.SetActive(summaries.Length == 0);

        foreach (var summary in summaries)
            AddEntry(sRefs.ContentContainer, uiRefs, summary);

        return;

        static void AddEntry(Transform collection, UiRefs uiRefs, Summary summary)
        {
            var product = Registry.GetItem<ProductDefinition>(summary.ProductId);
            var container = AddContainer(product.ID, uiRefs.EntryContainer, collection);
            var leftSide = AddSide("Left", container.transform, true);
            var rightSide = AddSide("Right", container.transform, false);

            AddIcon("ProductIcon", leftSide.transform, product.Icon);
            AddText("Product", leftSide.transform, uiRefs.EntryText,
                string.Format("{0}x {1}", summary.Total, product.Name));

            foreach (var agg in summary.Aggregates)
            {
                AddIcon(agg.Key, rightSide.transform, Registry.GetItem(agg.Key).Icon);
                AddText(agg.Key, rightSide.transform, uiRefs.EntryText, agg.Value.ToString());
            }

            return;

            static GameObject AddContainer(string name, GameObject template, Transform parent)
            {
                var gameObject = UnityEngine.Object.Instantiate(template);
                gameObject.transform.SetParent(parent, false);
                gameObject.name = name;

                var layout = gameObject.AddComponent<HorizontalLayoutGroup>();
                layout.childControlWidth = true;
                layout.childForceExpandWidth = true;

                return gameObject;
            }

            static GameObject AddSide(string name, Transform parent, bool alignLeft)
            {
                var container = new GameObject(name);
                container.transform.SetParent(parent, false);
                container.name = name;

                var rect = container.AddComponent<RectTransform>();
                rect.anchorMin = alignLeft ? Vector2.zero : new Vector2(0.5f, 0);
                rect.anchorMax = alignLeft ? new Vector2(0.5f, 1) : Vector2.one;

                var layout = container.AddComponent<HorizontalLayoutGroup>();
                layout.spacing = 0;
                layout.childAlignment = alignLeft ? TextAnchor.MiddleLeft : TextAnchor.MiddleRight;
                layout.childControlWidth = true;
                layout.childForceExpandWidth = false;

                return container;
            }

            static void AddText(string name, Transform container, Text textTmpl, string content)
            {
                var obj = new GameObject(name, typeof(RectTransform), typeof(Text));
                obj.transform.SetParent(container.transform, false);

                var rect = obj.GetComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;

                var text = obj.GetComponent<Text>();
                text.text = content;
                text.alignment = TextAnchor.MiddleLeft;
                text.fontSize = textTmpl.fontSize;
                text.font = textTmpl.font;
                text.color = textTmpl.color;
            }

            static void AddIcon(string name, Transform parent, Sprite sprite)
            {
                const int size = 30;
                var obj = new GameObject(name);
                obj.transform.SetParent(parent, false);

                var image = obj.AddComponent<Image>();
                image.sprite = sprite;
                image.preserveAspect = true;

                var rect = obj.GetComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.sizeDelta = new Vector2(size, size);

                var layout = obj.AddComponent<LayoutElement>();
                layout.preferredHeight = size;
                layout.preferredWidth = size;
            }
        }
    }

    public static void AdjustTasksPanel(Transform tasksPanel)
    {
        var rTransform = tasksPanel.GetComponent<RectTransform>();
        rTransform.anchorMin = new Vector2(rTransform.anchorMin.x, 0.3f);
        rTransform.offsetMin = new Vector2(rTransform.offsetMin.x, rTransform.offsetMin.y - 70);
    }

    public static SummaryRefs CreateSummaryPanel(UiRefs uiRefs)
    {
        Log.Trace("Creating outer container");
        var summaryPanel = UnityEngine.Object.Instantiate(uiRefs.TasksPanel);
        summaryPanel.gameObject.name = "Summary";
        summaryPanel.transform.SetParent(uiRefs.JournalContainer, false);
        AdjustPanel(summaryPanel, uiRefs);

        var contentContainer = summaryPanel.transform.Find(TasksContentPath);
        MelonCoroutines.Start(AdjustContentContainer(contentContainer.gameObject));

        return new(contentContainer, summaryPanel.transform.Find(TasksNoDetailsPath).gameObject);

        static void AdjustPanel(GameObject panel, UiRefs uiRefs)
        {
            var rTransform = panel.GetComponent<RectTransform>();
            rTransform.anchorMax = new Vector2(uiRefs.TasksPanel.GetComponent<RectTransform>()!.anchorMax.x, 0.3f);

            panel.transform.Find(TasksTitlePath)!.GetComponent<Text>()!.text = "Deal Summary";
            panel.transform.Find(TasksNoDetailsPath)!.GetComponent<Text>()!.text = "No active deals";
        }

        static System.Collections.IEnumerator AdjustContentContainer(GameObject container)
        {
            DeleteAllChildren(container.transform);
            UnityEngine.Object.Destroy(container.transform.GetComponent<VerticalLayoutGroup>());
            yield return null;

            var rect = container.transform.GetComponent<RectTransform>();
            rect.offsetMax = new Vector2(-25, rect.offsetMax.y); // Prevent scroll bar overlap

            var layout = container.AddComponent<GridLayoutGroup>();
            layout.padding = new RectOffset(5, 10, 5, 5);
            layout.cellSize = new Vector2(0, 30);
            layout.constraintCount = 2;
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.spacing = new Vector2(5, 5);

            container.AddComponent<PercentageWidthGridLayout>();
        }
    }

    private static void DeleteAllChildren(Transform parent)
    {
        Log.Trace(string.Format("Deleting all children({0}) from {1}", parent.childCount, parent.name));

        for (var i = 0; i < parent.childCount; i++)
            UnityEngine.Object.Destroy(parent.GetChild(i).gameObject);

        Log.Trace(string.Format("Deleted all children from {0}", parent.name));
    }
}