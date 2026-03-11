using _Code.MainGame.Map;

namespace _Code.Editor.Map
{
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Code.Editor.Map
{
    [CustomEditor(typeof(MapPainter))]
    public class MapPainterEditor : UnityEditor.Editor
    {
        private Texture2D _preview;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var mapPainter = (MapPainter)target;

            EditorGUILayout.Space();

            if (GUILayout.Button("Rebuild Preview"))
            {
                BuildPreview(mapPainter);
            }

            if (_preview != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Map Preview", EditorStyles.boldLabel);

                float size = Mathf.Min(EditorGUIUtility.currentViewWidth - 40f, 256f);
                Rect rect = GUILayoutUtility.GetRect(size, size, GUILayout.ExpandWidth(false));
                EditorGUI.DrawPreviewTexture(rect, _preview, null, ScaleMode.ScaleToFit);

                EditorGUILayout.LabelField($"Texture Size: {_preview.width} x {_preview.height}");
            }
        }

        private void OnEnable()
        {
            BuildPreview((MapPainter)target);
        }

        private void BuildPreview(MapPainter mapPainter)
        {
            Tilemap ground = mapPainter.GroundTilemap;
            Tilemap obstacle = mapPainter.ObstacleTilemap;

            if (ground == null && obstacle == null)
            {
                _preview = null;
                return;
            }

            if (ground != null) ground.CompressBounds();
            if (obstacle != null) obstacle.CompressBounds();

            BoundsInt bounds = GetCombinedBounds(ground, obstacle);

            if (bounds.size.x <= 0 || bounds.size.y <= 0)
            {
                _preview = null;
                return;
            }

            _preview = new Texture2D(bounds.size.x, bounds.size.y, TextureFormat.RGBA32, false);
            _preview.filterMode = FilterMode.Point;
            _preview.wrapMode = TextureWrapMode.Clamp;

            Color clear = new Color(0, 0, 0, 0);

            for (int y = 0; y < bounds.size.y; y++)
            {
                for (int x = 0; x < bounds.size.x; x++)
                {
                    _preview.SetPixel(x, y, clear);
                }
            }

            for (int y = 0; y < bounds.size.y; y++)
            {
                for (int x = 0; x < bounds.size.x; x++)
                {
                    Vector3Int cell = new Vector3Int(bounds.xMin + x, bounds.yMin + y, 0);

                    Color color = clear;

                    if (ground != null && ground.HasTile(cell))
                        color = Color.green;

                    if (obstacle != null && obstacle.HasTile(cell))
                        color = Color.red;

                    _preview.SetPixel(x, y, color);
                }
            }

            _preview.Apply();
        }

        private BoundsInt GetCombinedBounds(Tilemap a, Tilemap b)
        {
            bool hasA = a != null;
            bool hasB = b != null;

            if (hasA && !hasB) return a.cellBounds;
            if (!hasA && hasB) return b.cellBounds;

            BoundsInt ba = a.cellBounds;
            BoundsInt bb = b.cellBounds;

            int xMin = Mathf.Min(ba.xMin, bb.xMin);
            int yMin = Mathf.Min(ba.yMin, bb.yMin);
            int xMax = Mathf.Max(ba.xMax, bb.xMax);
            int yMax = Mathf.Max(ba.yMax, bb.yMax);

            return new BoundsInt(xMin, yMin, 0, xMax - xMin, yMax - yMin, 1);
        }
    }
}

}