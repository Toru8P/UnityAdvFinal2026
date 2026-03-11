using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace _Scripts.Editor.HierarchyPainter
{
    [FilePath("ProjectSettings/HierarchyPainterSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class HierarchyPainterSettings : ScriptableSingleton<HierarchyPainterSettings>
    {
        public Vector2 offset = new Vector2(20, 1);
        public List<HierarchyPaint> paints = new List<HierarchyPaint>();

        public void SaveToDisk() => Save(true); // call when modified
    }

    [Serializable]
    public struct HierarchyPaint
    {
        public string name;
        
        public Color textColor;
        public bool boldText;

        public Color backgroundColor;
    }
}