using MelonLoader;
using ScheduleOneMods.Logging;

namespace ScheduleOneMods.CounterPriceButton;

public sealed class Mod : MelonMod
{
    public override void OnInitializeMelon()
    {
        base.OnInitializeMelon();
        Log.SetLogger<Mod>();
    }
}