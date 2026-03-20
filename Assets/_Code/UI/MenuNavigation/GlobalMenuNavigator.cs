using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Code.UI.MenuNavigation
{
    public class GlobalMenuNavigator : MonoBehaviour
    {
        [Header("Ordered buttons (any canvas)")]
        [SerializeField] private List<Selectable> orderedButtons = new();
        
        [Header("Canvases")]
        [SerializeField] private List<Canvas> canvases = new();

        private readonly Dictionary<Canvas, UniversalSelectionFrame> _frames = new Dictionary<Canvas, UniversalSelectionFrame>();

        private int _index = -1;

        private void Awake()
        {
            CreateFramesForAllCanvases();
        }
        
        public void SubmitCurrent()
        {
            if (_index < 0 || _index >= orderedButtons.Count)
                return;

            var sel = orderedButtons[_index];
            if (!IsSelectableValid(sel))
                return;

            // Support Buttons
            var button = sel.GetComponent<Button>();
            if (button)
            {
                button.onClick.Invoke();
                return;
            }

            // Support all Selectables (Dropdown, Toggle, Slider…)
            ExecuteEvents.Execute(sel.gameObject,
                new BaseEventData(EventSystem.current),
                ExecuteEvents.submitHandler);
        }

 
        private void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(null); // prevent Unity auto-selection
            SelectFirstInteractable();
        }

        private void CreateFramesForAllCanvases()
        {
            _frames.Clear();

            foreach (var canvas in canvases)
            {
                var go = new GameObject("UniversalSelectionFrame");
                go.transform.SetParent(canvas.transform, false);

                var frame = go.AddComponent<UniversalSelectionFrame>();
                frame.Hide();

                _frames.Add(canvas, frame);
            }
        }

        public void Move(int direction)
        {
            if (orderedButtons.Count == 0) return;

            int start = _index < 0 ? 0 : _index;
            int i = start;

            do
            {
                i = (i + direction + orderedButtons.Count) % orderedButtons.Count;

                var sel = orderedButtons[i];
                if (IsSelectableValid(sel))
                {
                    SelectIndex(i);
                    return;
                }

            } while (i != start);
        }
        
        
        private bool IsSelectableValid(Selectable sel)
        {
            if (!sel) return false;
            if (!sel.interactable) return false;

            // UI object enabled?
            if (!sel.enabled) return false;

            // GameObject active?
            if (!sel.gameObject.activeInHierarchy) return false;

            return true;
        }


        private void SelectFirstInteractable()
        {
            for (int i = 0; i < orderedButtons.Count; i++)
            {
                if (orderedButtons[i] && orderedButtons[i].interactable)
                {
                    SelectIndex(i);
                    return;
                }
            }
        }

        private void SelectIndex(int i)
        {
            _index = i;
            var sel = orderedButtons[i];

            EventSystem.current.SetSelectedGameObject(sel.gameObject);

            var rt = sel.transform as RectTransform;
            if (!rt) return;

            var canvas = sel.GetComponentInParent<Canvas>();
            if (!canvas) return;

            foreach (var kv in _frames)
            {
                if (kv.Key == canvas)
                    kv.Value.Show(rt);
                else
                    kv.Value.Hide();
            }
        }
    }
}