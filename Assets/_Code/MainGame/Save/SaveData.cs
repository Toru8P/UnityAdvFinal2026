using System;

namespace _Code.MainGame.Save
{
    // Persistent progress data. HighestCompletedLevel is 1-based (1 = Level 1 completed).
    [Serializable]
    public class SaveData
    {
        public const int MinLevel = 1;
        public const int MaxLevel = 3;
        
        public float MasterVolume = 0f;
        public float MusicVolume = 0f;
        public float EffectsVolume = 0f;
        
        public bool MinimapOpened = true;


        // Highest level the player has completed (1 = Level 1, 2 = Level 2, 3 = Level 3). 0 = none.
        public int HighestCompletedLevel;

        public SaveData()
        {
            HighestCompletedLevel = 0;
        }

        // Whether level N (1-based) is unlocked for selection. Level 1 is always unlocked.
        public bool IsLevelUnlocked(int levelNumber)
        {
            if (levelNumber < MinLevel || levelNumber > MaxLevel)
                return false;
            if (levelNumber == 1)
                return true;
            return HighestCompletedLevel >= levelNumber - 1;
        }

        // Number of levels available for selection (1 to this value inclusive). At least 1.
        public int UnlockedLevelCount => Math.Clamp(HighestCompletedLevel + 1, 1, MaxLevel);
    }
}
