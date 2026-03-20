using _Code.UI.MenuNavigation;
using UnityEngine;
using UnityEngine.EventSystems;
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

        private GameObject _currentlyActive;
        
        public void Awake()
        {
            Reset();
            startCanvas?.SetActive(true);
            if (turnOnTutorial)
            {
                tutorialCanvas?.SetActive(true);
                gameCanvas?.SetActive(true);
            }
            _currentlyActive = startCanvas;
            SelectFirstInActiveCanvas();
        }
        
        public void OnGameLost()
        {
            Reset();
            lostCanvas.SetActive(true);
            _currentlyActive = lostCanvas;
            SelectFirstInActiveCanvas();
        }

        public void OnGameWon()
        {
            Reset();
            winCanvas.SetActive(true);
            _currentlyActive = winCanvas;
            SelectFirstInActiveCanvas();
        }

        public void OnGameStart()
        {
            Reset();
            gameCanvas.SetActive(true);
            _currentlyActive = gameCanvas;

            SelectFirstInActiveCanvas();
        }
        
        public void OnPauseMenu()
        {
            Reset();
            pauseCanvas?.SetActive(true);
            _currentlyActive = pauseCanvas;
            SelectFirstInActiveCanvas();
        }

        public void OnResume()
        {
            Reset();
            gameCanvas?.SetActive(true);
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
    }
}