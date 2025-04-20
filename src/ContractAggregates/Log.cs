using System.Text;
using MelonLoader;
using ScheduleOne.UI.Phone;
using UnityEngine;

namespace ContractAggregates;

public static class Log
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

    public static class Journal
    {
        public static void LogObjectPaths(JournalApp journalApp)
        {
            if (File.Exists("detailsPanelContainer.txt"))
                File.Delete("detailsPanelContainer.txt");
            var allChildren = journalApp.DetailsPanelContainer.GetComponentsInChildren<Transform>(true);
            foreach (var child in allChildren)
            {
                var fullPath = GetFullPath(child);
                File.AppendAllText("detailsPanelContainer.txt", fullPath + Environment.NewLine);
            }

            if (File.Exists("appcanvas.txt"))
                File.Delete("appcanvas.txt");
            var gameObject1 = GameObject.Find("Player_Local");
            if (gameObject1 is null)
            {
                File.WriteAllText("appcanvas.txt", "couldnt find root object");
                return;
            }

            var appCanvas =
                gameObject1.transform.Find("CameraContainer/Camera/OverlayCamera/GameplayMenu/Phone/phone/AppsCanvas");
            allChildren = appCanvas.GetComponentsInChildren<Transform>(true);

            foreach (var child in allChildren)
            {
                var fullPath = GetFullPath(child);
                File.AppendAllText("appcanvas.txt", fullPath + Environment.NewLine);
            }
        }

        private static string GetFullPath(Transform transform)
        {
            var sb = new StringBuilder();
            while (transform != null)
            {
                if (sb.Length > 0)
                    sb.Insert(0, '/');
                sb.Insert(0, transform.name);
                transform = transform.parent;
            }

            return sb.ToString();
        }

        public static void LogUiElements()
        {
            var journal = GameObject.Find("Journal");
            var container = journal?.transform.Find("Container");
            var background = container?.transform.Find("Background");
            var topBar = container?.transform.Find("Topbar");
            var tasks = container?.transform.Find("Tasks");
            var deatils = container?.transform.Find("Details");
            LogUiElement("journal", journal);
            LogUiElement("journal-container", container?.gameObject);
            LogUiElement("journal-container-background", background?.gameObject);
            LogUiElement("journal-container-topbar", topBar?.gameObject);
            LogUiElement("journal-container-details", deatils?.gameObject);
            LogUiElement("journal-container-tasks", tasks?.gameObject);
            return;

            static void LogUiElement(string name, GameObject? gameObject)
            {
                var fileName = name + ".txt";
                if (File.Exists(fileName))
                    File.Delete(fileName);

                if (gameObject is null)
                {
                    File.AppendAllText(fileName, "null");
                    return;
                }

                File.WriteAllLines(fileName, gameObject.GetComponents(typeof(Component)).Select(c => c.name));
                File.WriteAllText(fileName, "\n-----\n");
                var rTransform = gameObject.GetComponent<RectTransform>();
                File.AppendAllText(fileName, string.Format("anchorMax: {0}\n", rTransform.anchorMax));
                File.AppendAllText(fileName, string.Format("anchorMin: {0}\n", rTransform.anchorMin));
                File.AppendAllText(fileName, string.Format("anchoredPosition3D: {0}\n", rTransform.anchoredPosition3D));
                File.AppendAllText(fileName, string.Format("anchoredPosition: {0}\n", rTransform.anchoredPosition));
                File.AppendAllText(fileName, string.Format("offsetMax: {0}\n", rTransform.offsetMax));
                File.AppendAllText(fileName, string.Format("offsetMin: {0}\n", rTransform.offsetMin));
                File.AppendAllText(fileName, string.Format("pivot: {0}\n", rTransform.pivot));
                File.AppendAllText(fileName, string.Format("rect: {0}\n", rTransform.rect));
                File.AppendAllText(fileName, string.Format("sizeDelta: {0}\n", rTransform.sizeDelta));
            }
        }
    }
}