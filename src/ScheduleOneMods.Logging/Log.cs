using MelonLoader;

namespace ScheduleOneMods.Logging;

public static partial class Log
{
    public static void Trace(string message)
    {
#if DEBUG
        MelonDebug.Msg(message);
#endif
    }

    public static void Debug(string message) => MelonDebug.Msg(message);
    public static void Info(string message) => MelonLogger.Msg(message);
    public static void Warning(string message) => MelonLogger.Warning(message);
    public static void Error(string message) => MelonLogger.Error(message);
}