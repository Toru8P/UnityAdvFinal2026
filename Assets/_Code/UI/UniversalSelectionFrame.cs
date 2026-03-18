using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace _Code.UI
{
    // One shared frame that follows EventSystem.currentSelectedGameObject. Add to one Canvas (e.g. root); works in all menus.
    [RequireComponent(typeof(RectTransform))]
    public class UniversalSelectionFrame : MonoBehaviour
    {
        [SerializeField] private Color borderColor = Color.black;
        [SerializeField] private float borderThickness = 4f;
        [SerializeField] private float activateStickThreshold = 0.25f;

        private RectTransform _frameRoot;
        private RectTransform _myRt;
        private Selectable _lastSelected;
        private bool _built;
        private bool _activatedByStick;

        private void Awake()
        {
            _myRt = (RectTransform)transform;
            BuildFrame();
        }

        private void BuildFrame()
        {
            if (_built) return;

            var go = new GameObject("SelectionFrame");
            go.transform.SetParent(transform, false);
            _frameRoot = go.AddComponent<RectTransform>();
            _frameRoot.anchorMin = Vector2.zero;
            _frameRoot.anchorMax = Vector2.one;
            _frameRoot.offsetMin = Vector2.zero;
            _frameRoot.offsetMax = Vector2.zero;

            var sprite = CreatePixelSprite();
            float t = borderThickness;
            CreateEdge("Left", _frameRoot, sprite, new Vector2(0, 0), new Vector2(0, 1), new Vector2(-t, 0), Vector2.zero);
            CreateEdge("Right", _frameRoot, sprite, new Vector2(1, 0), new Vector2(1, 1), Vector2.zero, new Vector2(t, 0));
            CreateEdge("Top", _frameRoot, sprite, new Vector2(0, 1), new Vector2(1, 1), Vector2.zero, new Vector2(0, t));
            CreateEdge("Bottom", _frameRoot, sprite, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, -t), Vector2.zero);

            _frameRoot.gameObject.SetActive(false);
            _built = true;
        }

        private void Update()
        {
            if (_activatedByStick)
                return;

            if (HasStickInput())
                _activatedByStick = true;
        }

        private bool HasStickInput()
        {
            var gp = Gamepad.current;
            if (gp != null)
            {
                if (gp.leftStick.ReadValue().sqrMagnitude >= activateStickThreshold * activateStickThreshold)
                    return true;
            }

            var js = Joystick.current;
            if (js != null)
            {
                if (js.stick.ReadValue().sqrMagnitude >= activateStickThreshold * activateStickThreshold)
                    return true;
            }

            return false;
        }

        private static Sprite CreatePixelSprite()
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
        }

        private void CreateEdge(string name, RectTransform parent, Sprite sprite, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = offsetMin;
            rt.offsetMax = offsetMax;
            var img = go.AddComponent<Image>();
            img.sprite = sprite;
            img.color = borderColor;
            img.raycastTarget = false;
        }

        private void LateUpdate()
        {
            if (_frameRoot == null) return;
            if (!_activatedByStick)
            {
                _frameRoot.gameObject.SetActive(false);
                _lastSelected = null;
                return;
            }

            var es = EventSystem.current;
            GameObject selected = es != null ? es.currentSelectedGameObject : null;
            Selectable sel = selected != null ? selected.GetComponent<Selectable>() : null;

            if (sel == null || !sel.interactable)
            {
                _frameRoot.gameObject.SetActive(false);
                _lastSelected = null;
                return;
            }

            var targetRt = selected.transform as RectTransform;
            if (targetRt == null)
            {
                _frameRoot.gameObject.SetActive(false);
                return;
            }

            bool needReparent = _lastSelected != sel;
            _lastSelected = sel;

            if (needReparent)
            {
                _frameRoot.SetParent(targetRt.parent, false);
                _frameRoot.SetSiblingIndex(targetRt.GetSiblingIndex() + 1);
            }

            float t = borderThickness;
            _frameRoot.anchorMin = targetRt.anchorMin;
            _frameRoot.anchorMax = targetRt.anchorMax;
            _frameRoot.offsetMin = targetRt.offsetMin - new Vector2(t, t);
            _frameRoot.offsetMax = targetRt.offsetMax + new Vector2(t, t);

            _frameRoot.gameObject.SetActive(true);
        }
    }
}
