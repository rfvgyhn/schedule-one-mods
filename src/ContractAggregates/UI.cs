using ScheduleOne;
using ScheduleOne.Product;
using UnityEngine;
using UnityEngine.UI;

namespace ContractAggregates;

public static class UI
{
    public static GameObject CreateSummary(Transform parent, Summary[] summaries)
    {
        var container = AddContainer(parent);

        foreach (var summary in summaries)
            AddEntry(container.transform, summary);

        return container;

        static GameObject AddContainer(Transform parent)
        {
            var obj = new GameObject("Summary");
            obj.transform.SetParent(parent, false);

            var transform = obj.AddComponent<RectTransform>();
            transform.anchorMin = Vector2.zero;
            transform.anchorMax = Vector2.one;
            transform.pivot = Vector2.up;
            transform.sizeDelta = Vector2.zero;
            transform.offsetMin = new Vector2(5, 5);
            transform.offsetMax = new Vector2(-5, -5);

            var bg = obj.AddComponent<Image>();
            bg.color = Color.blue;

            var layout = obj.AddComponent<GridLayoutGroup>();
            layout.cellSize = new Vector2(0, 30);
            layout.constraintCount = 2;
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layout.childAlignment = TextAnchor.UpperLeft;

            obj.AddComponent<PercentageWidthGridLayout>();

            return obj;
        }

        static void AddEntry(Transform parent, Summary summary)
        {
            var product = Registry.GetItem<ProductDefinition>(summary.ProductId);
            var container = AddContainer(product.ID, parent);

            AddIcon("ProductIcon", container.transform, product.Icon);
            AddText("Product", container.transform, string.Format("{0}x {1}", summary.Total, product.Name));

            foreach (var agg in summary.Aggregates)
            {
                AddIcon(agg.Key, container.transform, Registry.GetItem(agg.Key).Icon);
                AddText(agg.Key, container.transform, agg.Value.ToString());
            }

            return;

            static GameObject AddContainer(string name, Transform parent)
            {
                var container = new GameObject(name);
                container.transform.SetParent(parent, false);

                var transform = container.AddComponent<RectTransform>();
                transform.anchorMin = Vector2.zero;
                transform.anchorMax = Vector2.one;
                transform.offsetMin = new Vector2(5, 5);
                transform.offsetMax = new Vector2(-5, -5);
                transform.pivot = Vector2.up;
                transform.sizeDelta = Vector2.zero;

                container.AddComponent<Image>().color = Color.green;

                var layout = container.AddComponent<HorizontalLayoutGroup>();
                layout.spacing = 0;
                layout.childAlignment = TextAnchor.MiddleLeft;
                layout.childControlWidth = true;
                layout.childForceExpandWidth = true;

                return container;
            }

            static void AddText(string name, Transform parent, string content)
            {
                var obj = new GameObject(name, typeof(RectTransform), typeof(Text));
                obj.transform.SetParent(parent.transform, false);

                var rect = obj.GetComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.pivot = Vector2.up;
                rect.sizeDelta = Vector2.zero;

                var text = obj.GetComponent<Text>();
                text.text = content;
                text.alignment = TextAnchor.MiddleLeft;
                text.fontSize = 14;
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.color = Color.black;

                var layout = obj.AddComponent<LayoutElement>();
                layout.flexibleWidth = 9999;
            }

            static void AddIcon(string name, Transform parent, Sprite sprite)
            {
                var obj = new GameObject(name);
                obj.transform.SetParent(parent, false);

                var image = obj.AddComponent<Image>();
                image.sprite = sprite;
                image.preserveAspect = true;

                var rect = obj.GetComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.pivot = Vector2.up;
                rect.sizeDelta = new Vector2(30, 30); // Fixed size

                var layout = obj.AddComponent<LayoutElement>();
                layout.preferredHeight = 30;
                layout.preferredWidth = 30;
            }
        }
    }

    public static void AdjustTasksPanel(Transform tasksPanel)
    {
        var rTransform = tasksPanel.GetComponent<RectTransform>();
        rTransform.anchorMin = new Vector2(rTransform.anchorMin.x, 0.3f);
        rTransform.offsetMin = new Vector2(rTransform.offsetMin.x, rTransform.offsetMin.y - 60);
    }

    public static Transform CreateSummarySection(Transform parent, Transform tasksPanel, GameObject detailsPanel)
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

        static void CleanupClone(Transform clone)
        {
            var noDetailsLabel = clone.transform.Find("NoDetails");
            if (noDetailsLabel is not null)
                UnityEngine.Object.Destroy(noDetailsLabel.gameObject);
            else
                Log.Trace("NoDetails not found");
        }
    }
}