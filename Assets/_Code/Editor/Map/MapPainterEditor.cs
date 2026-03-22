using System.Collections.Generic;
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

            MapPainter mapPainter = (MapPainter)target;

            EditorGUILayout.Space();

            if (GUILayout.Button("Rebuild Preview"))
            {
                BuildPreview(mapPainter);
            }

            if (_preview)
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
            Tilemap spawnZone = mapPainter.SpawnZoneTilemap;

            if (!ground && !obstacle)
            {
                _preview = null;
                return;
            }

            if (ground) ground.CompressBounds();
            if (obstacle) obstacle.CompressBounds();
            if (spawnZone) spawnZone.CompressBounds();

            BoundsInt bounds = GetCombinedBounds(new List<Tilemap>() { ground, obstacle, spawnZone });

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

                    if (ground && ground.HasTile(cell))
                        color = Color.green;

                    if (obstacle && obstacle.HasTile(cell))
                        color = Color.red;
                    
                    if (spawnZone && spawnZone.HasTile(cell))
                        color = Color.cyan;

                    _preview.SetPixel(x, y, color);
                }
            }

            _preview.Apply();
        }

        private BoundsInt GetCombinedBounds(IList<Tilemap> tilemaps)
        {
            bool hasAny = false;

            int xMin = 0, yMin = 0;
            int xMax = 0, yMax = 0;

            foreach (Tilemap tm in tilemaps)
            {
                if (!tm) 
                    continue;

                BoundsInt b = tm.cellBounds;

                if (!hasAny)
                {
                    xMin = b.xMin;
                    yMin = b.yMin;
                    xMax = b.xMax;
                    yMax = b.yMax;
                    hasAny = true;
                }
                else
                {
                    xMin = Mathf.Min(xMin, b.xMin);
                    yMin = Mathf.Min(yMin, b.yMin);
                    xMax = Mathf.Max(xMax, b.xMax);
                    yMax = Mathf.Max(yMax, b.yMax);
                }
            }

            if (!hasAny)
                return new BoundsInt();

            return new BoundsInt(
                xMin,
                yMin,
                0,
                xMax - xMin,
                yMax - yMin,
                1
            );
        }
    }
}

}