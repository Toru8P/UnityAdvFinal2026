using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Code.UI
{
    public class MenuNavigationHelper : MonoBehaviour
    {
        [Tooltip("Buttons in navigation order (top to bottom). First is selected when menu opens.")]
        [SerializeField] private Selectable[] buttonsInOrder = new Selectable[0];

        private void OnEnable()
        {
            if (buttonsInOrder == null || buttonsInOrder.Length == 0)
                return;

            SetupNavigation();
            SelectFirstInteractable();
        }

        public void SetButtonsOrder(Selectable[] order)
        {
            buttonsInOrder = order ?? new Selectable[0];
            if (buttonsInOrder.Length == 0) return;

            SetupNavigation();
            SelectFirstInteractable();
        }

        private void SetupNavigation()
        {
            for (int i = 0; i < buttonsInOrder.Length; i++)
            {
                if (buttonsInOrder[i] == null) continue;

                var nav = buttonsInOrder[i].navigation;
                nav.mode = Navigation.Mode.Explicit;
                nav.selectOnUp = i > 0 ? buttonsInOrder[i - 1] : null;
                nav.selectOnDown = i < buttonsInOrder.Length - 1 ? buttonsInOrder[i + 1] : null;
                nav.selectOnLeft = null;
                nav.selectOnRight = null;
                buttonsInOrder[i].navigation = nav;
            }
        }

        private void SelectFirstInteractable()
        {
            if (EventSystem.current == null) return;

            foreach (var b in buttonsInOrder)
            {
                if (b != null && b.interactable)
                {
                    EventSystem.current.SetSelectedGameObject(b.gameObject);
                    return;
                }
            }
        }
    }
}
