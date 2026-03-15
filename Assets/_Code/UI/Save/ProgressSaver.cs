using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Code.UI.Save
{
    /// <summary>
    /// Call from WinConditionManager's winConditionEvent to record level completion and persist progress.
    /// </summary>
    public class ProgressSaver : MonoBehaviour
    {
        /// <summary>
        /// Call this when the player wins the level (e.g. via UnityEvent from WinConditionManager).
        /// </summary>
        public void ReportLevelComplete()
        {
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            SaveManager.CompleteLevel(buildIndex);
        }
    }
}
