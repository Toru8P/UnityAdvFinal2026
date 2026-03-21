using UnityEngine;

namespace _Code.UI
{
    public class UIToggleHelper : MonoBehaviour
    {
        public static void Toggle(GameObject target)
        {
            if (!target) return;
            target.SetActive(!target.activeSelf);
        }

        public static void Set(GameObject target, bool state)
        {
            if (!target) return;
            target.SetActive(state);
        }
    }
}