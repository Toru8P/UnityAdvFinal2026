using UnityEngine;
using UnityEngine.UI;
using _Code.Save;

namespace _Code.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Level select")]
        [SerializeField] private GameObject levelSelectPanel;
        [SerializeField] private Button levelSelectButton;
        [SerializeField] private Button level1Button;
        [SerializeField] private Button level2Button;
        [SerializeField] private Button level3Button;

        private void Awake()
        {
            // Hide Level Select until we know progress; avoids showing it before Start.
            if (levelSelectButton)
                levelSelectButton.gameObject.SetActive(false);
            if (levelSelectPanel)
                levelSelectPanel.SetActive(false);
        }

        private void Start()
        {
            SaveManager.InvalidateCache();
            SaveData progress = SaveManager.Load();
            int highest = progress.HighestCompletedLevel;

            // Show Level Select button only when at least one level has been completed.
            if (levelSelectButton)
                levelSelectButton.gameObject.SetActive(highest >= 1);

            SetLevelButtonInteractable(level1Button, progress, 1);
            SetLevelButtonInteractable(level2Button, progress, 2);
            SetLevelButtonInteractable(level3Button, progress, 3);

            if (levelSelectButton && levelSelectPanel)
                levelSelectButton.onClick.AddListener(() => levelSelectPanel.SetActive(true));
        }

        private static void SetLevelButtonInteractable(Button button, SaveData progress, int levelNumber)
        {
            if (button != null)
                button.interactable = progress.IsLevelUnlocked(levelNumber);
        }
    }
}
