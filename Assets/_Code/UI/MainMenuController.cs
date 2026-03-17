using UnityEngine;
using UnityEngine.UI;
using _Code.Save;

namespace _Code.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Level select")]
        [SerializeField] private Button level1Button;
        [SerializeField] private Button level2Button;
        [SerializeField] private Button level3Button;

        [Header("Gamepad/Joystick navigation (first selected = New Game, then levels)")]
        [Tooltip("New Game button - put first in navigation order so it gets the highlight when menu opens.")]
        [SerializeField] private Button newGameButton;
        [Tooltip("If set, buttons are configured in this order for joystick/gamepad (New Game → Level 1 → 2 → 3).")]
        [SerializeField] private MenuNavigationHelper menuNavigationHelper;

        private void Start()
        {
            SaveManager.InvalidateCache();
            SaveData progress = SaveManager.Load();

            SetLevelButtonInteractable(level1Button, progress, 1);
            SetLevelButtonInteractable(level2Button, progress, 2);
            SetLevelButtonInteractable(level3Button, progress, 3);

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

            var order = new Selectable[count];
            int i = 0;
            if (newGameButton) order[i++] = newGameButton;
            if (level1Button) order[i++] = level1Button;
            if (level2Button) order[i++] = level2Button;
            if (level3Button) order[i++] = level3Button;

            menuNavigationHelper.SetButtonsOrder(order);
        }

        private static void SetLevelButtonInteractable(Button button, SaveData progress, int levelNumber)
        {
            if (button != null)
                button.interactable = progress.IsLevelUnlocked(levelNumber);
        }
    }
}
