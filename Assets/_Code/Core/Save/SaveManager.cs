using System;
using System.IO;
using UnityEngine;

namespace _Code.Save
{
    // Saves and loads progress to JSON in Application.persistentDataPath.
    public static class SaveManager
    {
        private const string FileName = "progress.json";

        private static SaveData _cached;
        private static bool _loaded;

        // Loads progress from disk (or returns cached). Never returns null.
        public static SaveData Load()
        {
            if (_loaded)
                return _cached ?? new SaveData();

            _loaded = true;
            string path = GetSavePath();

            try
            {
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    _cached = JsonUtility.FromJson<SaveData>(json);
                    if (_cached == null)
                        _cached = new SaveData();
                    return _cached;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SaveManager] Load failed: {e.Message}");
            }

            _cached = new SaveData();
            return _cached;
        }

        // Marks the given level (scene build index) as completed and saves.
        // Level 1 = build index 1, Level 2 = 2, Level 3 = 3.
        public static void CompleteLevel(int sceneBuildIndex)
        {
            SaveData data = Load();
            int levelNumber = sceneBuildIndex;
            if (levelNumber < SaveData.MinLevel || levelNumber > SaveData.MaxLevel)
                return;
            if (levelNumber > data.HighestCompletedLevel)
            {
                data.HighestCompletedLevel = levelNumber;
                Save(data);
            }
        }

        // Writes current progress to disk.
        public static void Save(SaveData data)
        {
            if (data == null)
                return;
            _cached = data;
            _loaded = true;

            try
            {
                string path = GetSavePath();
                string json = JsonUtility.ToJson(data, prettyPrint: true);
                File.WriteAllText(path, json);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SaveManager] Save failed: {e.Message}");
            }
        }

        // Clears cached data so next Load() reads from disk.
        public static void InvalidateCache()
        {
            _loaded = false;
            _cached = null;
        }

        private static string GetSavePath()
        {
            return Path.Combine(Application.persistentDataPath, FileName);
        }
    }
}
