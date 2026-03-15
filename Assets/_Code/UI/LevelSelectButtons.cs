using UnityEngine;

namespace _Code.UI
{
    // Parameterless entry points for level buttons so they can be wired in the editor or by tools.
    public class LevelSelectButtons : MonoBehaviour
    {
        [SerializeField] private SceneLoader sceneLoader;

        private void Awake()
        {
            if (sceneLoader == null)
                sceneLoader = FindAnyObjectByType<SceneLoader>();
        }

        public void LoadLevel1() => (sceneLoader ?? FindAnyObjectByType<SceneLoader>())?.LoadLevel(1);
        public void LoadLevel2() => (sceneLoader ?? FindAnyObjectByType<SceneLoader>())?.LoadLevel(2);
        public void LoadLevel3() => (sceneLoader ?? FindAnyObjectByType<SceneLoader>())?.LoadLevel(3);
    }
}
