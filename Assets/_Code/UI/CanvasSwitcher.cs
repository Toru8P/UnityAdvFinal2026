using _Code.UI.MenuNavigation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace _Code.UI
{
    public class CanvasSwitcher : MonoBehaviour
    {
        [SerializeField] private bool turnOnTutorial = false;

        [Header("General Menus")]
        [SerializeField] private GameObject startCanvas;
        [SerializeField] private GameObject tutorialCanvas;
        [SerializeField] private GameObject gameCanvas;
        [SerializeField] private GameObject pauseCanvas;
        [SerializeField] private GameObject lostCanvas;
        [SerializeField] private GameObject winCanvas;

        [Header("Gameplay")]
        [SerializeField] private GameObject gameplayRoot;

        private GameObject _currentlyActive;

        private bool IsPaused => pauseCanvas && pauseCanvas.activeSelf;
        private bool IsGameRunning => gameplayRoot && gameplayRoot.activeSelf && gameCanvas && gameCanvas.activeSelf && !IsPaused;

        public void Awake()
        {
            Reset();
            Time.timeScale = 1f;

            startCanvas?.SetActive(true);

            if (turnOnTutorial)
            {
                Time.timeScale = 0f;

                tutorialCanvas?.SetActive(true);
                gameCanvas?.SetActive(true);
                gameplayRoot?.SetActive(false);

                _currentlyActive = tutorialCanvas ? tutorialCanvas : gameCanvas;
                SelectFirstInActiveCanvas();
                return;
            }

            _currentlyActive = startCanvas;
            SelectFirstInActiveCanvas();
        }

        private void Update()
        {
            if (Keyboard.current == null)
            {
                return;
            }

            if (!Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                return;
            }

            if (IsPaused)
            {
                OnResume();
            }
            else if (IsGameRunning)
            {
                OnPauseMenu();
            }
        }

        public void OnGameLost()
        {
            Reset();
            Time.timeScale = 1f;

            lostCanvas?.SetActive(true);
            _currentlyActive = lostCanvas;
            SelectFirstInActiveCanvas();
        }

        public void OnGameWon()
        {
            Reset();
            Time.timeScale = 1f;

            winCanvas?.SetActive(true);
            _currentlyActive = winCanvas;
            SelectFirstInActiveCanvas();
        }

        public void OnGameStart()
        {
            Reset();
            Time.timeScale = 1f;

            gameCanvas?.SetActive(true);
            gameplayRoot?.SetActive(true);
            _currentlyActive = gameCanvas;

            SelectFirstInActiveCanvas();
        }

        public void OnPauseMenu()
        {
            Time.timeScale = 0f;

            startCanvas?.SetActive(false);
            if (tutorialCanvas) tutorialCanvas.SetActive(false);
            lostCanvas?.SetActive(false);
            winCanvas?.SetActive(false);

            gameCanvas?.SetActive(true);
            pauseCanvas?.SetActive(true);
            gameplayRoot?.SetActive(false);

            _currentlyActive = pauseCanvas;
            SelectFirstInActiveCanvas();
        }

        public void OnResume()
        {
            Time.timeScale = 1f;

            startCanvas?.SetActive(false);
            if (tutorialCanvas) tutorialCanvas.SetActive(false);
            lostCanvas?.SetActive(false);
            winCanvas?.SetActive(false);

            pauseCanvas?.SetActive(false);
            gameCanvas?.SetActive(true);
            gameplayRoot?.SetActive(true);

            _currentlyActive = gameCanvas;
            SelectFirstInActiveCanvas();
        }

        private void Reset()
        {
            startCanvas?.SetActive(false);
            if (tutorialCanvas) tutorialCanvas.SetActive(false);
            gameCanvas?.SetActive(false);
            pauseCanvas?.SetActive(false);
            lostCanvas?.SetActive(false);
            winCanvas?.SetActive(false);

            gameplayRoot?.SetActive(false);

            _currentlyActive = null;
        }

        private void SelectFirstInActiveCanvas()
        {
            if (!EventSystem.current) return;
            if (!_currentlyActive) return;

            Selectable selectable = _currentlyActive.GetComponentInChildren<Selectable>(true);
            if (!selectable) return;

            while (true)
            {
                if (!selectable.interactable)
                {
                    selectable = selectable.FindSelectableOnDown();
                    if (!selectable) break;
                }
                else
                {
                    EventSystem.current.SetSelectedGameObject(selectable.gameObject);
                    break;
                }
            }
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
        }
    }
}