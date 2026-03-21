using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Code.MainGame.Save
{
    public class ProgressSaver : MonoBehaviour
    {
        public void ReportLevelComplete()
        {
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            SaveManager.CompleteLevel(buildIndex);
        }
    }
}
