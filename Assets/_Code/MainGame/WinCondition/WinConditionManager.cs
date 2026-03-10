using UnityEngine;
using UnityEngine.Events;

namespace _Code.MainGame.WinCondition
{
    [RequireComponent(typeof(Timer))]
    public class WinConditionManager : MonoBehaviour
    {
        [SerializeField] private PlayerStateChannel playerStateChannel;

        [SerializeField] private UnityEvent winConditionEvent = new UnityEvent();
        [SerializeField] private UnityEvent loseConditionEvent = new UnityEvent();

        private Timer _timer;

        private bool _isPlaying = true;

        private void Awake()
        {
            _timer = GetComponent<Timer>();
            playerStateChannel.SubscribeToPlayerDied(OnLoseCondition);
            _timer.SubscribeToEvent(OnWinCondition);
            _timer.StartTimer();
        }

        private void OnWinCondition()
        {
            if (!_isPlaying) return;
            _isPlaying = false;
            Debug.Log("OnWinCondition");
            winConditionEvent.Invoke();
        }

        private void OnLoseCondition(Vector2 hitPosition)
        {
            if (!_isPlaying) return;
            _isPlaying = false;
            Debug.Log("OnLoseCondition " + hitPosition);
            loseConditionEvent.Invoke();
        }
    }
}