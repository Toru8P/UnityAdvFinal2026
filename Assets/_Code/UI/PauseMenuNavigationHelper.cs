using UnityEngine;
using UnityEngine.UI;

namespace _Code.UI
{
    public class PauseMenuNavigationHelper : MonoBehaviour
    {
        [Header("Pause menu buttons (direct order)")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button exitToMenuButton;

        private MenuNavigationHelper _helper;

        private void Awake()
        {
            _helper = GetComponent<MenuNavigationHelper>();
            if (_helper == null)
                _helper = gameObject.AddComponent<MenuNavigationHelper>();
        }

        private void OnEnable()
        {
            BuildOrderAndSetup();
        }

        private void BuildOrderAndSetup()
        {
            ResolveButtonsIfNeeded();

            int count = 0;
            if (resumeButton) count++;
            if (restartButton) count++;
            if (exitToMenuButton) count++;

            var order = new Selectable[count];
            int i = 0;
            if (resumeButton) order[i++] = resumeButton;
            if (restartButton) order[i++] = restartButton;
            if (exitToMenuButton) order[i++] = exitToMenuButton;

            _helper.SetButtonsOrder(order);
        }

        private void ResolveButtonsIfNeeded()
        {
            if (resumeButton && restartButton && exitToMenuButton) return;

            var buttons = GetComponentsInChildren<Button>(true);
            foreach (var b in buttons)
            {
                var name = b.gameObject.name.ToLowerInvariant();
                if (!resumeButton && (name.Contains("resume")))
                    resumeButton = b;
                else if (!restartButton && (name.Contains("restart") || name.Contains("retry")))
                    restartButton = b;
                else if (!exitToMenuButton && (name.Contains("exit") || name.Contains("menu") || name.Contains("main")))
                    exitToMenuButton = b;
            }

            if (resumeButton && restartButton && exitToMenuButton) return;
            for (int i = 0; i < buttons.Length && (resumeButton == null || restartButton == null || exitToMenuButton == null); i++)
            {
                if (resumeButton == null) resumeButton = buttons[i];
                else if (restartButton == null) restartButton = buttons[i];
                else if (exitToMenuButton == null) exitToMenuButton = buttons[i];
            }
        }
    }
}
