using UnityEngine;
using UnityEngine.InputSystem;

namespace _Code.UI.MenuNavigation
{
    public class MenuInputRouter : MonoBehaviour
    {
        [SerializeField] private GlobalMenuNavigator navigator;

        private float _lastMoveTime;
        private const float repeatDelay = 0.2f; // tweak if needed

        public void OnNavigate(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;

            Vector2 v = ctx.ReadValue<Vector2>();
            if (Time.unscaledTime - _lastMoveTime < repeatDelay)
                return;

            if (v.y > 0.5f)
            {
                navigator.Move(-1);
                _lastMoveTime = Time.unscaledTime;
            }
            else if (v.y < -0.5f)
            {
                navigator.Move(+1);
                _lastMoveTime = Time.unscaledTime;
            }
        }
    }
}