using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace _Code.MainGame.WinCondition
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;

        [SerializeField] private float timeLimit = 60f;
        public float currentTime;

        private bool _stopTimer = true;
        
        private readonly UnityEvent _onTimerEnd = new UnityEvent();

        
        public void StartTimer()
        {
            currentTime = timeLimit;
            _stopTimer = false;
        }

        public void StopTimer()
        {
            _stopTimer = true;
        }
        
        public void SubscribeToEvent(UnityAction action)
        {
            _onTimerEnd.AddListener(action);
        }

        void LateUpdate()
        {
            if (_stopTimer) return;
        
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
            {
                _stopTimer = true;
                _onTimerEnd.Invoke();
                return;
            }
            UpdateTimerText();
        }
    
        private void UpdateTimerText()
        {
            if (!timerText)
            {
                Debug.LogWarning("No TextMeshProUGUI component found on WinConditionTimer.");
                return;
            }
            timerText.text = $"Time: {currentTime:F1}s";
        }
    }
}
