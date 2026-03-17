using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Code.UI
{
    public class SelectFirstOnEnable : MonoBehaviour
    {
        [SerializeField] private Selectable firstSelected;

        private void OnEnable()
        {
            if (firstSelected == null || !firstSelected.interactable)
                return;
            if (EventSystem.current == null)
                return;

            EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);
        }
    }
}
