using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using _Code.UI.MenuNavigation;

namespace _Code.UI
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

        private void OnEnable()
        {
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
                if (sel && sel.interactable)
                {
                    SelectIndex(i);
                    return;
                }

            } while (i != start);
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