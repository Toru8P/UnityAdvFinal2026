using System;
using UnityEngine;

namespace _Code.MainGame
{
    public class PauseManager : MonoBehaviour
    {
        [SerializeField] private bool startPaused = true;
        private float _previousTimeScale = 1f;
        public static bool IsPaused = false;

        private void Awake()
        {
            if (startPaused)
            {
                PauseGame();
            }
        }

        public void ResumeGame()
        {
            if (!IsPaused) return;
            IsPaused = false;
            Debug.Log("Resume");


            Time.timeScale = Mathf.Approximately(_previousTimeScale, 0f) ? 1f : _previousTimeScale;
        }

        public void PauseGame()
        {
            if (IsPaused) return;
            IsPaused = true;
            Debug.Log("Pause");

            _previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }
    }
}