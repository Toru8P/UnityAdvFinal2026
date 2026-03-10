using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Code.UI
{
    public class SceneLoader : MonoBehaviour
    {
        
        public void LoadLevel(int levelIndex)
        {
            SceneManager.LoadScene(levelIndex);
        }

    }
}
