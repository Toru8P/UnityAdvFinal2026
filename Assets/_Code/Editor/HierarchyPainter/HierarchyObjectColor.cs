using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace _Scripts.Editor.HierarchyPainter
{
    [InitializeOnLoad]
    public class HierarchyObjectColor
    {
        static Dictionary<string, HierarchyPaint> _paintByName;
        static bool _cacheDirty = true;

        static GUIStyle _labelStyle;

        static HierarchyObjectColor()
        {
            // Called for every visible item in the Hierarchy window.
            EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
        }

        // Call this after changing settings in your SettingsProvider (recommended).
        public static void InvalidateCacheAndRepaint()
        {
            _cacheDirty = true;
            EditorApplication.RepaintHierarchyWindow();
        }

        static void EnsureCache()
        {
            if (!_cacheDirty && _paintByName != null) return;

            var settings = HierarchyPainterSettings.instance;

            _paintByName = new Dictionary<string, HierarchyPaint>(settings.paints.Count);
            foreach (var p in settings.paints)
            {
                if (!string.IsNullOrEmpty(p.name))
                {
                    _paintByName[p.name] = p; // last one wins if duplicates
                }
            }

            if (_labelStyle == null)
            {
                _labelStyle = new GUIStyle(EditorStyles.label) { fontStyle = FontStyle.Bold };
            }

            _cacheDirty = false;
        }

        static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            EnsureCache();

            var go = EditorUtility.EntityIdToObject(instanceID) as GameObject;
            if (go == null) return;

            if (!_paintByName.TryGetValue(go.name, out var paint))
                return;

            if (paint.backgroundColor.a <= 0f && paint.textColor.a <= 0f)
                return;

            var settings = HierarchyPainterSettings.instance;

            if (paint.backgroundColor.a > 0f)
            {
                var bgRect = selectionRect;
                bgRect.width += 50f; 
                EditorGUI.DrawRect(bgRect, paint.backgroundColor);
            }

            var labelRect = selectionRect;
            labelRect.xMin += 16f;
            labelRect.y += settings.offset.y;
            labelRect.x += settings.offset.x;

            var tc = paint.textColor;
            tc.a = 1f;

            _labelStyle.normal.textColor = tc;
            _labelStyle.fontStyle = paint.boldText ? FontStyle.Bold : FontStyle.Normal;

            EditorGUI.LabelField(labelRect, go.name, _labelStyle);
        }

    }
}
