using Il2CppScheduleOne.UI.Phone;
using Il2CppScheduleOne.UI.Phone.Messages;
using ScheduleOneMods.Logging;
using UnityEngine;
using UnityEngine.UI;

namespace ScheduleOneMods.CounterPriceButton;

public record UiRefs(
    Transform ButtonsContainer,
    GameObject Minus10Button,
    GameObject Plus10Button);

public static class UI
{
    private const string ButtonsContainerPath = "Shade/Content/Price/Buttons";
    private const string Minus100ButtonPath = "-100";
    private const string Plus100ButtonPath = "+100";
    private const int ButtonValueChange = 100;
    private const int Padding = 10;

    /// <summary>
    /// Ensures the layout elements this mod is based on exist in their expected places.
    /// </summary>
    /// <remarks>
    /// Layout could be broken/wonky if they aren't in the expected locations as there could be missing or incorrect
    /// GameObject/Component clones
    /// </remarks>
    public static UiRefs? FindRequiredUiElements(MessagesApp app)
    {
        if (app.CounterofferInterface is null)
        {
            Log.Debug("CounterofferInterface is null");
            return null;
        }

        var buttonsContainer = app.CounterofferInterface.Container.transform.Find(ButtonsContainerPath);
        if (buttonsContainer is null)
        {
            Log.Debug($"{ButtonsContainerPath} is null");
            return null;
        }

        var minusButton = buttonsContainer.transform.Find(Minus100ButtonPath);
        if (minusButton is null)
        {
            Log.Debug($"{Minus100ButtonPath} not found");
            return null;
        }

        if (minusButton.childCount != 1)
        {
            Log.Debug($"{Minus100ButtonPath} has an unexpected number of children");
            return null;
        }

        var plusButton = buttonsContainer.transform.Find(Plus100ButtonPath);
        if (plusButton is null)
        {
            Log.Debug($"{Plus100ButtonPath} not found");
            return null;
        }

        if (plusButton.childCount != 1)
        {
            Log.Debug($"{Plus100ButtonPath} has an unexpected number of children");
            return null;
        }

        return new UiRefs(buttonsContainer, minusButton.gameObject, plusButton.gameObject);
    }

    public static (Text minus, Text plus) AddNewButtons(UiRefs uiRefs, CounterofferInterface offerInterface)
    {
        Log.Trace("Creating minus button");
        var minusText = AddButton(uiRefs.Minus10Button, uiRefs.ButtonsContainer, offerInterface, true);

        Log.Trace("Creating plus button");
        var plusText = AddButton(uiRefs.Plus10Button, uiRefs.ButtonsContainer, offerInterface, false);

        return (minusText, plusText);

        static Text AddButton(GameObject buttonTemplate, Transform parent, CounterofferInterface offerInterface,
            bool minus)
        {
            var (prefix, modifier) = minus ? ('-', -1) : ('+', 1);
            var newButton = UnityEngine.Object.Instantiate(buttonTemplate);
            newButton.name = $"{prefix}Price";
            newButton.transform.SetParent(parent, false);

            var rect = newButton.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y - rect.rect.height - Padding);

            var button = newButton.GetComponent<Button>();
            // RemoveAllListeners doesn't remove the cloned persistent handler
            // Offset by ButtonValueChange button already has a listener that changes the amount
            button.onClick.AddListener(new Action(() =>
                offerInterface.ChangePrice(modifier * offerInterface.selectedProduct.Price -
                                           modifier * ButtonValueChange)));

            var text = newButton.transform.GetChild(0).GetComponent<Text>();
            Log.Trace("setting text");
            text.text = $"{prefix}Price";
            Log.Trace("set text");
            return text;
        }
    }
}