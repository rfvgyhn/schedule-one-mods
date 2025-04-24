using System.Collections;
using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Messaging;
using Il2CppScheduleOne.Product;
using Il2CppScheduleOne.UI.Phone;
using Il2CppScheduleOne.UI.Phone.Messages;
using MelonLoader;
using ScheduleOneMods.Logging;
using UnityEngine.UI;

namespace ScheduleOneMods.CounterPriceButton;

public static class MessagesAppPatches
{
    private static Text? _minusText;
    private static Text? _plusText;
    private static UiRefs? _uiRefs;

    [HarmonyPatch(typeof(MessagesApp), nameof(MessagesApp.Start))]
    public static class Start
    {
        public static void Prefix() => MelonCoroutines.Start(InitUi());
    }

    [HarmonyPatch(typeof(CounterofferInterface), nameof(CounterofferInterface.Open))]
    public static class Open
    {
#pragma warning disable IDE0060
        public static void Postfix(ProductDefinition product,
            int quantity,
            float price,
            // ReSharper disable once InconsistentNaming
            MSGConversation _conversation,
            // ReSharper disable once InconsistentNaming
            Action<ProductDefinition, int, float> _orderConfirmedCallback)
#pragma warning restore IDE0060
        {
            SetButtonText(product.Price);
        }
    }

    [HarmonyPatch(typeof(CounterofferInterface), nameof(CounterofferInterface.SetProduct))]
    public static class SetProduct
    {
        // Harmony requires specific param name for patching to work properly
        // ReSharper disable once InconsistentNaming
#pragma warning disable IDE0060
        public static void Postfix(CounterofferInterface __instance, ProductDefinition newProduct)
#pragma warning restore IDE0060
        {
            SetButtonText(newProduct.Price);
        }
    }

    private static void SetButtonText(float price)
    {
        if (_minusText is not null)
            _minusText.text = $"-{price}";

        if (_plusText is not null)
            _plusText.text = $"+{price}";
    }

    private static IEnumerator InitUi()
    {
        while (!PlayerSingleton<MessagesApp>.InstanceExists)
            yield return null;
        Log.Trace("MessagesApp instance exists");

        var messagesApp = PlayerSingleton<MessagesApp>.Instance;
        _uiRefs = UI.FindRequiredUiElements(messagesApp);
        if (_uiRefs is null)
        {
            Log.Error("Expected UI elements not found");
            yield break;
        }

        yield return null;

        Log.Trace("Creating new UI");
        (_minusText, _plusText) = UI.AddNewButtons(_uiRefs, messagesApp.CounterofferInterface);
    }
}