using System;

namespace _Code.UI.Save
{
    /// <summary>
    /// Persistent progress data. HighestCompletedLevel is 1-based (1 = Level 1 completed).
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public const int MinLevel = 1;
        public const int MaxLevel = 3;

        /// <summary>
        /// Highest level the player has completed (1 = Level 1, 2 = Level 2, 3 = Level 3). 0 = none.
        /// </summary>
        public int HighestCompletedLevel;

        public SaveData()
        {
            HighestCompletedLevel = 0;
        }

        /// <summary>
        /// Whether level N (1-based) is unlocked for selection. Level 1 is always unlocked.
        /// </summary>
        public bool IsLevelUnlocked(int levelNumber)
        {
            if (levelNumber < MinLevel || levelNumber > MaxLevel)
                return false;
            if (levelNumber == 1)
                return true;
            return HighestCompletedLevel >= levelNumber - 1;
        }

        /// <summary>
        /// Number of levels available for selection (1 to this value inclusive). At least 1.
        /// </summary>
        public int UnlockedLevelCount => Math.Clamp(HighestCompletedLevel + 1, 1, MaxLevel);
    }
}
