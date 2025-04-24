using System.Diagnostics;
using MelonLoader;

namespace ScheduleOneMods.Logging;

public static partial class Log
{
    private static MelonLogger.Instance? _logger;
    public static void SetLogger<T>() where T : MelonBase => _logger = Melon<T>.Logger;
    
    [Conditional("DEBUG")]
    public static void Trace(string message) => Debug(message);
    
    public static void Debug(string message)
    {
        if (!LoaderConfig.Current.Loader.DebugMode)
            return;
        
        if (_logger is not null)
            _logger.Msg($"[DEBUG] {message}"); // Section isn't exposed in the MelonLogger API so add it manually
        else
            MelonDebug.Msg(message);
    }

    public static void Info(string message)
    {
        if (_logger is not null)
            _logger.Msg(message);
        else
            MelonLogger.Msg(message);
    }

    public static void Warning(string message)
    {
        if (_logger is not null)
            _logger.Warning(message);
        else
            MelonLogger.Warning(message);
    }

    public static void Error(string message)
    {
        if (_logger is not null)
            _logger.Error(message);
        else
            MelonLogger.Error(message);
    }
}