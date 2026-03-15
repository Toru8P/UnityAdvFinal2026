// Assets/Editor/HierarchyPainterSettingsProvider.cs

using UnityEditor;

namespace _Code.Editor.HierarchyPainter
{
    static class HierarchyPainterSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider Create()
        {
            return new SettingsProvider("Project/Hierarchy Painter", SettingsScope.Project)
            {
                guiHandler = _ =>
                {
                    var settings = HierarchyPainterSettings.instance;
                    var so = new SerializedObject(settings);

                    so.Update();

                    EditorGUILayout.PropertyField(so.FindProperty("offset"));
                    EditorGUILayout.PropertyField(so.FindProperty("paints"), includeChildren: true);

                    if (so.ApplyModifiedProperties())
                    {
                        settings.SaveToDisk();

                        HierarchyObjectColor.InvalidateCacheAndRepaint();
                    }
                }
            };
        }
    }
}