using UnityEngine;

namespace _Code.UI
{
    public class CanvasSwitcher : MonoBehaviour
    {
        [Header("General Menus")] 
        [SerializeField] private GameObject startCanvas;
        [SerializeField] private GameObject tutorialCanvas;
        [SerializeField] private GameObject gameCanvas;
        [SerializeField] private GameObject pauseCanvas;
        
        [SerializeField] private GameObject lostCanvas;
        [SerializeField] private GameObject winCanvas;
        
        public void Awake()
        {
            Reset();
            startCanvas?.SetActive(true);
            tutorialCanvas?.SetActive(true);
            gameCanvas?.SetActive(true);
        }
        
        public void OnGameLost()
        {
            Reset();
            lostCanvas.SetActive(true);
        }

        public void OnGameWon()
        {
            Reset();
            winCanvas.SetActive(true);
        }

        public void OnGameStart()
        {
            Reset();
            gameCanvas.SetActive(true);
        }
        
        public void OnPauseMenu()
        {
            Reset();
            pauseCanvas?.SetActive(true);
        }

        public void OnResume()
        {
            Reset();
            gameCanvas?.SetActive(true);
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
    }
}