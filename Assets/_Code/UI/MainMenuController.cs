using UnityEngine;
using UnityEngine.UI;
using _Code.UI.Save;

namespace _Code.UI
{
    /// <summary>
    /// Shows level select panel and configures level buttons based on saved progress.
    /// </summary>
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
            if (levelSelectButton != null)
                levelSelectButton.gameObject.SetActive(false);
            if (levelSelectPanel != null)
                levelSelectPanel.SetActive(false);
        }

        private void Start()
        {
            SaveData progress = SaveManager.Load();
            int highest = progress.HighestCompletedLevel;

            // Show Level Select button only when at least one level has been completed.
            if (levelSelectButton != null)
                levelSelectButton.gameObject.SetActive(highest >= 1);

            SetLevelButtonInteractable(level1Button, progress, 1);
            SetLevelButtonInteractable(level2Button, progress, 2);
            SetLevelButtonInteractable(level3Button, progress, 3);

            if (levelSelectButton != null && levelSelectPanel != null)
                levelSelectButton.onClick.AddListener(() => levelSelectPanel.SetActive(true));
        }

        private static void SetLevelButtonInteractable(Button button, SaveData progress, int levelNumber)
        {
            if (button != null)
                button.interactable = progress.IsLevelUnlocked(levelNumber);
        }
    }
}
