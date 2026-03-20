using UnityEngine;
using UnityEngine.UI;

namespace _Code.UI.MenuNavigation
{
    public class UniversalSelectionFrame : MonoBehaviour
    {
        [SerializeField] private Color borderColor = Color.black;
        [SerializeField] private float borderThickness = 4f;

        private RectTransform _frameRoot;
        private bool _built;

        private void Awake()
        {
            BuildFrame();
        }

        public void Show(RectTransform targetRt)
        {
            if (!targetRt || !_frameRoot) return;

            _frameRoot.SetParent(targetRt, false);
            _frameRoot.SetAsLastSibling();

            float t = borderThickness;
            _frameRoot.anchorMin = Vector2.zero;
            _frameRoot.anchorMax = Vector2.one;
            _frameRoot.offsetMin = new Vector2(-t, -t);
            _frameRoot.offsetMax = new Vector2(t, t);

            _frameRoot.gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (_frameRoot)
                _frameRoot.gameObject.SetActive(false);
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

            CreateEdge("Left",   new Vector2(0, 0), new Vector2(0, 1), new Vector2(-t, 0), Vector2.zero, sprite);
            CreateEdge("Right",  new Vector2(1, 0), new Vector2(1, 1), Vector2.zero, new Vector2(t, 0), sprite);
            CreateEdge("Top",    new Vector2(0, 1), new Vector2(1, 1), Vector2.zero, new Vector2(0, t), sprite);
            CreateEdge("Bottom", new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, -t), Vector2.zero, sprite);

            _frameRoot.gameObject.SetActive(false);
            _built = true;
        }

        private void CreateEdge(
            string name,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 offsetMin,
            Vector2 offsetMax,
            Sprite sprite)
        {
            var go = new GameObject(name);
            go.transform.SetParent(_frameRoot, false);

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

        private static Sprite CreatePixelSprite()
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
        }
    }
}