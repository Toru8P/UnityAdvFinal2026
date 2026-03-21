using _Code.MainGame.Save;
using UnityEngine;
using UnityEngine.UI;

namespace _Code.UI.MenuNavigation
{
    public class MainMenuSaveChecker : MonoBehaviour
    {
        [Header("Level select")]
        [SerializeField] private Button level1Button;
        [SerializeField] private Button level2Button;
        [SerializeField] private Button level3Button;
        
        private void Start()
        {
            SaveManager.InvalidateCache();
            SaveData progress = SaveManager.Load();

            SetLevelButtonInteractable(level1Button, progress, 1);
            SetLevelButtonInteractable(level2Button, progress, 2);
            SetLevelButtonInteractable(level3Button, progress, 3);
        }

        private static void SetLevelButtonInteractable(Button button, SaveData progress, int levelNumber)
        {
            if (button) button.interactable = progress.IsLevelUnlocked(levelNumber);
        }
    }
}