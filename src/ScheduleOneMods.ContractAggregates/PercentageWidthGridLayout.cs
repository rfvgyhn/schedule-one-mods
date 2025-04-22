using UnityEngine;
using UnityEngine.UI;

namespace ScheduleOneMods.ContractAggregates;

[RequireComponent(typeof(GridLayoutGroup))]
public class PercentageWidthGridLayout : MonoBehaviour, ILayoutController
{
    // ReSharper disable once InconsistentNaming (follow Unity naming convention)
    public readonly float cellWidthPercentage = 0.5f;
    private RectTransform _rectTransform = null!;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>()!;
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
    }

    public void SetLayoutHorizontal()
    {
        var layoutGroup = GetComponent<GridLayoutGroup>();
        var parentWidth = _rectTransform.rect.width;
        var cellSize = new Vector2(parentWidth * cellWidthPercentage, layoutGroup.cellSize.y);
        layoutGroup.cellSize = cellSize;
    }

    public void SetLayoutVertical() { }
}