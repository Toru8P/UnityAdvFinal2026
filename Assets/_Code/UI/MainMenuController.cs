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

        [Header("Gamepad/Joystick navigation (first selected = New Game, then levels)")]
        [Tooltip("New Game button - put first in navigation order so it gets the highlight when menu opens.")]
        [SerializeField] private Button newGameButton;
        [Tooltip("If set, buttons are configured in this order for joystick/gamepad (New Game → Level 1 → 2 → 3).")]
        [SerializeField] private MenuNavigationHelper menuNavigationHelper;

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

            SetupMenuNavigation(progress);
        }

        private void SetupMenuNavigation(SaveData progress)
        {
            if (menuNavigationHelper == null) return;

            // Direct order: New Game first, then Level 1, Level 2, Level 3; Level Select if unlocked.
            int count = 0;
            if (newGameButton) count++;
            if (level1Button) count++;
            if (level2Button) count++;
            if (level3Button) count++;
            if (levelSelectButton != null && levelSelectButton.gameObject.activeSelf) count++;

            var order = new Selectable[count];
            int i = 0;
            if (newGameButton) order[i++] = newGameButton;
            if (level1Button) order[i++] = level1Button;
            if (level2Button) order[i++] = level2Button;
            if (level3Button) order[i++] = level3Button;
            if (levelSelectButton != null && levelSelectButton.gameObject.activeSelf)
                order[i++] = levelSelectButton;

            menuNavigationHelper.SetButtonsOrder(order);
        }

        private static void SetLevelButtonInteractable(Button button, SaveData progress, int levelNumber)
        {
            if (button != null)
                button.interactable = progress.IsLevelUnlocked(levelNumber);
        }
    }
}
