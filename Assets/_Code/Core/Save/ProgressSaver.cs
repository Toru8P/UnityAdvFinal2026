using UnityEngine;
using UnityEngine.SceneManagement;
using _Code.Save;

namespace _Code.Save
{
    // Used from WinConditionManager's winConditionEvent to record level completion and persist progress.
    public class ProgressSaver : MonoBehaviour
    {
        public void ReportLevelComplete()
        {
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            SaveManager.CompleteLevel(buildIndex);
        }
    }
}
