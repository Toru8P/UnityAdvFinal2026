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
        
        public void Awake()
        {
            if (GetComponent<UniversalSelectionFrame>() == null)
                gameObject.AddComponent<UniversalSelectionFrame>();
            Reset();
            startCanvas?.SetActive(true);
            if (turnOnTutorial) tutorialCanvas?.SetActive(true);
            gameCanvas?.SetActive(true);
            SelectFirstInActiveCanvas();
        }
        
        public void OnGameLost()
        {
            Reset();
            lostCanvas.SetActive(true);
            SelectFirstInActiveCanvas();
        }

        public void OnGameWon()
        {
            Reset();
            winCanvas.SetActive(true);
            SelectFirstInActiveCanvas();
        }

        public void OnGameStart()
        {
            Reset();
            gameCanvas.SetActive(true);
            SelectFirstInActiveCanvas();
        }
        
        public void OnPauseMenu()
        {
            Reset();
            pauseCanvas?.SetActive(true);
            SelectFirstInActiveCanvas();
        }

        public void OnResume()
        {
            Reset();
            gameCanvas?.SetActive(true);
            SelectFirstInActiveCanvas();
        }

        private void Reset()
        {
            startCanvas?.SetActive(false);
            tutorialCanvas?.SetActive(false);
            gameCanvas?.SetActive(false);
            pauseCanvas?.SetActive(false);
            
            lostCanvas?.SetActive(false);
            winCanvas?.SetActive(false);
        }

        private void SelectFirstInActiveCanvas()
        {
            if (EventSystem.current == null) return;

            var active = startCanvas != null && startCanvas.activeInHierarchy ? startCanvas
                : gameCanvas != null && gameCanvas.activeInHierarchy ? gameCanvas
                : pauseCanvas != null && pauseCanvas.activeInHierarchy ? pauseCanvas
                : lostCanvas != null && lostCanvas.activeInHierarchy ? lostCanvas
                : winCanvas != null && winCanvas.activeInHierarchy ? winCanvas
                : null;

            if (active == null) return;

            var selectable = active.GetComponentInChildren<Selectable>(true);
            while (selectable != null && !selectable.interactable)
                selectable = selectable.FindSelectableOnDown();
            if (selectable != null && selectable.interactable)
                EventSystem.current.SetSelectedGameObject(selectable.gameObject);
        }
    }
}