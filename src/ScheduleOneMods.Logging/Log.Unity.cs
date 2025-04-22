using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ScheduleOneMods.Logging;

public static partial class Log
{
    public static class Unity
    {
        private const int IndentSize = 4;
        
        public static void WriteGameObject(string fileName, Transform gameObject)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);

            using var writer = File.AppendText(fileName);
            LogGameObject(writer, gameObject);
        }
        
        public static void LogGameObject(StreamWriter writer, Transform gameObject, int depth = 0,
            HashSet<string>? seen = null)
        {
            seen ??= [];
            if (depth > 50)
            {
                writer.WriteLine("Max depth reached");
                return;
            }

            var path = GetFullPath(gameObject);
            if (!seen.Add(path))
                return;

            writer.WriteLine(path);

            foreach (var component in gameObject.GetComponents<Component>())
                LogComponent(writer, component, 1);

            writer.WriteLine();

            for (var i = 0; i < gameObject.childCount; i++)
                LogGameObject(writer, gameObject.GetChild(i), depth + 1, seen);
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

        private static void WriteProp<T>(StreamWriter writer, int indent, string prop, T? value) =>
            writer.WriteLine("{0}{1} = {2}", new string(' ', indent * IndentSize), prop, value?.ToString() ?? "null");

        public static void LogComponent(StreamWriter writer, Component c, int indent = 0)
        {
            writer.WriteLine("{0}{1}", new string(' ', indent * IndentSize), c.GetType().FullName);
            switch (c)
            {
                case RectTransform t:
                    LogComponent(writer, t, indent + 1);
                    break;
                case Text t:
                    LogComponent(writer, t, indent + 1);
                    break;
                case Image i:
                    LogComponent(writer, i, indent + 1);
                    break;
                case HorizontalOrVerticalLayoutGroup g:
                    LogComponent(writer, g, indent + 1);
                    break;
                case ScrollRect s:
                    LogComponent(writer, s, indent + 1);
                    break;
                case Mask m:
                    LogComponent(writer, m, indent + 1);
                    break;
                case LayoutElement e:
                    LogComponent(writer, e, indent + 1);
                    break;
                case ContentSizeFitter f:
                    LogComponent(writer, f, indent + 1);
                    break;
            }
        }

        public static void LogComponent(StreamWriter writer, RectTransform t, int indent = 0)
        {
            Write(nameof(t.anchorMax), t.anchorMax);
            Write(nameof(t.anchorMin), t.anchorMin);
            Write(nameof(t.anchoredPosition3D), t.anchoredPosition3D);
            Write(nameof(t.anchoredPosition), t.anchoredPosition);
            Write(nameof(t.offsetMax), t.offsetMax);
            Write(nameof(t.offsetMin), t.offsetMin);
            Write(nameof(t.pivot), t.pivot);
            Write(nameof(t.sizeDelta), t.sizeDelta);
            Write(nameof(t.rect), t.rect);
            return;

            void Write<T>(string prop, T value) => WriteProp(writer, indent, prop, value);
        }

        public static void LogComponent(StreamWriter writer, ScrollRect r, int indent = 0)
        {
            Write(nameof(r.horizontal), r.horizontal);
            Write(nameof(r.vertical), r.vertical);
            Write(nameof(r.movementType), r.movementType);
            Write(nameof(r.elasticity), r.elasticity);
            Write(nameof(r.inertia), r.inertia);
            Write(nameof(r.decelerationRate), r.decelerationRate);
            Write(nameof(r.scrollSensitivity), r.scrollSensitivity);
            Write(nameof(r.horizontalScrollbar), r.horizontalScrollbar);
            Write(nameof(r.verticalScrollbar), r.verticalScrollbar);
            Write(nameof(r.horizontalScrollbarVisibility), r.horizontalScrollbarVisibility);
            Write(nameof(r.verticalScrollbarVisibility), r.verticalScrollbarVisibility);
            Write(nameof(r.horizontalScrollbarSpacing), r.horizontalScrollbarSpacing);
            Write(nameof(r.verticalScrollbarSpacing), r.verticalScrollbarSpacing);
            Write(nameof(r.velocity), r.velocity);
            Write(nameof(r.minHeight), r.minHeight);
            Write(nameof(r.minWidth), r.minWidth);
            Write(nameof(r.preferredWidth), r.preferredWidth);
            Write(nameof(r.preferredHeight), r.preferredHeight);
            Write(nameof(r.flexibleHeight), r.flexibleHeight);
            Write(nameof(r.flexibleWidth), r.flexibleWidth);
            return;

            void Write<T>(string prop, T value) => WriteProp(writer, indent, prop, value);
        }

        public static void LogComponent(StreamWriter writer, Mask m, int indent = 0)
        {
            Write(nameof(m.showMaskGraphic), m.showMaskGraphic);
            return;

            void Write<T>(string prop, T value) => WriteProp(writer, indent, prop, value);
        }

        public static void LogComponent(StreamWriter writer, LayoutElement e, int indent = 0)
        {
            Write(nameof(e.ignoreLayout), e.ignoreLayout);
            Write(nameof(e.minWidth), e.minWidth);
            Write(nameof(e.minHeight), e.minHeight);
            Write(nameof(e.preferredWidth), e.preferredWidth);
            Write(nameof(e.preferredHeight), e.preferredHeight);
            Write(nameof(e.flexibleWidth), e.flexibleWidth);
            Write(nameof(e.flexibleHeight), e.flexibleHeight);
            Write(nameof(e.layoutPriority), e.layoutPriority);
            return;

            void Write<T>(string prop, T value) => WriteProp(writer, indent, prop, value);
        }

        public static void LogComponent(StreamWriter writer, ContentSizeFitter f, int indent = 0)
        {
            Write(nameof(f.horizontalFit), f.horizontalFit);
            Write(nameof(f.verticalFit), f.verticalFit);
            return;

            void Write<T>(string prop, T value) => WriteProp(writer, indent, prop, value);
        }

        public static void LogComponent(StreamWriter writer, Text t, int indent = 0)
        {
            Write(nameof(t.text), t.text);
            Write(nameof(t.alignment), t.alignment);
            Write(nameof(t.fontSize), t.fontSize);
            Write(nameof(t.font), t.font.name);
            Write(nameof(t.color), t.color);
            return;

            void Write<T>(string prop, T value) => WriteProp(writer, indent, prop, value);
        }

        public static void LogComponent(StreamWriter writer, Image i, int indent = 0)
        {
            Write(nameof(i.sprite), i.sprite?.name);
            Write(nameof(i.type), i.type);
            Write(nameof(i.preserveAspect), i.preserveAspect);
            Write(nameof(i.minHeight), i.minHeight);
            Write(nameof(i.minWidth), i.minWidth);
            Write(nameof(i.preferredHeight), i.preferredHeight);
            Write(nameof(i.preferredWidth), i.preferredWidth);
            Write(nameof(i.flexibleHeight), i.flexibleHeight);
            Write(nameof(i.flexibleWidth), i.flexibleWidth);
            Write(nameof(i.fillAmount), i.fillAmount);
            Write(nameof(i.fillCenter), i.fillCenter);
            Write(nameof(i.fillOrigin), i.fillOrigin);
            Write(nameof(i.fillClockwise), i.fillClockwise);
            Write(nameof(i.fillMethod), i.fillMethod);
            Write(nameof(i.fillOrigin), i.fillOrigin);
            Write(nameof(i.color), i.color);
            return;

            void Write<T>(string prop, T value) => WriteProp(writer, indent, prop, value);
        }

        public static void LogComponent(StreamWriter writer, HorizontalOrVerticalLayoutGroup g, int indent = 0)
        {
            Write(nameof(g.spacing), g.spacing);
            Write(nameof(g.childForceExpandWidth), g.childForceExpandWidth);
            Write(nameof(g.childForceExpandHeight), g.childForceExpandHeight);
            Write(nameof(g.childControlHeight), g.childControlHeight);
            Write(nameof(g.childControlWidth), g.childControlWidth);
            Write(nameof(g.childScaleHeight), g.childScaleHeight);
            Write(nameof(g.childScaleWidth), g.childScaleWidth);
            Write(nameof(g.reverseArrangement), g.reverseArrangement);
            return;

            void Write<T>(string prop, T value) => WriteProp(writer, indent, prop, value);
        }
    }
}