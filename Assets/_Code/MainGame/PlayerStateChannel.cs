using UnityEngine;
using UnityEngine.Events;

namespace _Code.MainGame
{
    public class PlayerStateChannel : MonoBehaviour
    {
        [SerializeField] private UnityEvent<Vector2> playerDiedEvent = new UnityEvent<Vector2>();
        
        public void SubscribeToPlayerDied(UnityAction<Vector2> action)
        {
            playerDiedEvent.AddListener(action);
        }
        
        public void UnsubscribeFromPlayerDied(UnityAction<Vector2> action)
        {
            playerDiedEvent.RemoveListener(action);
        }
        
        public void NotifyPlayerDied(Vector2 hitPosition)
        {
            playerDiedEvent.Invoke(hitPosition);
        }
    }
}
