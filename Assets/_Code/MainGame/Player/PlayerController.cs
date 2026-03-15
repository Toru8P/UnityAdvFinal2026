using _Code.MainGame.Buff;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace _Code.MainGame.Player
{
    [RequireComponent(typeof(Collider2D), typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerStateChannel channel;
        
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 4f;
        
        [Header("Input Settings")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference pauseAction;
        [SerializeField] private UnityEvent onPausePressed;
        
        [Header("Hit Detection Settings")]  
        [SerializeField] private float skin = 0.02f;
        [SerializeField] private LayerMask obstacleMask;
        
        [Header("Visual Settings")]
        [SerializeField] private SpriteRenderer skinRenderer;
        [Header("Immunity Buff")]
        [Tooltip("Sprite shown around the player while immunity is active. Assign bubble sprite directly; a child with SpriteRenderer is created at runtime.")]
        [SerializeField] private Sprite immunityBubbleSprite;
        
        [Header("Light Settings")]
        [SerializeField] private bool turnOnLight = true;
        [SerializeField] private Light2D lights;
        
        [Header("Footsteps Settings")]
        public float stepInterval = 0.4f; // seconds between steps
        private float stepTimer = 0f;
        public AudioSource footstepSource;
        public AudioClip[] footstepClips;
        
        private bool _isAlive = true;
    
        private Collider2D _col;
        private Vector2 _moveInput;
        private readonly RaycastHit2D[] _hits = new RaycastHit2D[8];
        private Animator _animator;
        private Vector2 _currentLightDir = Vector2.right;

        [CanBeNull] private BuffAttachment _activeBuff;
        [CanBeNull] private BuffAttachment _immunityBuff;
        private GameObject _immunityBubble;
        
        private void Awake()
        {
            _col = GetComponent<Collider2D>();
            _animator = GetComponent<Animator>();
            ResolveImmunityBubble();
            if (turnOnLight && lights)
            {
                lights.enabled = true;
            }
        }

        private void ResolveImmunityBubble()
        {
            var bubble = transform.Find("ImmunityBubble");
            if (bubble == null)
                bubble = FindInChildren(transform, "ImmunityBubble");
            if (bubble != null)
            {
                _immunityBubble = bubble.gameObject;
                return;
            }
            if (immunityBubbleSprite != null)
            {
                _immunityBubble = CreateImmunityBubble();
                if (_immunityBubble != null)
                    _immunityBubble.SetActive(false);
            }
        }

        private GameObject CreateImmunityBubble()
        {
            if (immunityBubbleSprite == null) return null;
            var go = new GameObject("ImmunityBubble");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one * 1.2f;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = immunityBubbleSprite;
            sr.color = new Color(1f, 1f, 1f, 0.5f);
            sr.sortingOrder = 100;
            if (skinRenderer != null)
                sr.sortingLayerID = skinRenderer.sortingLayerID;
            return go;
        }

        private static Transform FindInChildren(Transform parent, string name)
        {
            if (parent.name == name) return parent;
            for (int i = 0; i < parent.childCount; i++)
            {
                var found = FindInChildren(parent.GetChild(i), name);
                if (found != null) return found;
            }
            return null;
        }

        private void OnEnable()
        {
            moveAction.action.Enable();
        }

        private void OnDisable()
        {
            moveAction.action.Disable();
        }

        private void Update()
        {
            if (!_isAlive) return;

            if (_activeBuff != null)
            {
                _activeBuff.Update(Time.deltaTime);
                if (_activeBuff.IsExpired)
                    _activeBuff = null;
            }
            if (_immunityBuff != null)
            {
                _immunityBuff.Update(Time.deltaTime);
                if (_immunityBuff.IsExpired)
                    _immunityBuff = null;
            }
            bool hasImmunity = _immunityBuff != null && !_immunityBuff.IsExpired;
            if (_immunityBubble != null)
                _immunityBubble.SetActive(hasImmunity);

            if (pauseAction && pauseAction.action.WasPressedThisFrame())
            {
                onPausePressed?.Invoke();
                Debug.Log("Pause Pressed");
            }
            
            _moveInput = moveAction.action.ReadValue<Vector2>();
            _moveInput = Vector2.ClampMagnitude(_moveInput, 1f);
            bool hasSpeedBoost = _activeBuff != null && _activeBuff.Type == BuffType.SpeedBoost;
            float speed = moveSpeed;
            if (hasSpeedBoost)
            {
                speed += _activeBuff.Value;
            }
            
            Vector2 delta = _moveInput * (speed * Time.deltaTime);
            MoveWithCollision(delta);

            if (_moveInput.x != 0)
            {
                skinRenderer.flipX = !(_moveInput.x > 0);
            }
            
            bool isMoving = _moveInput.sqrMagnitude > 0.01f;

            
            if (lights && isMoving)
            {
                Vector2 dir = _moveInput.normalized;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Quaternion targetRot = Quaternion.Euler(0f, 0f, angle);

                lights.transform.rotation = Quaternion.RotateTowards(
                    lights.transform.rotation,
                    targetRot,
                    720f * Time.deltaTime
                );
            }
            
            if (_animator)
            {
                _animator.SetBool("Walk", _moveInput != Vector2.zero);
                _animator.SetBool("SpeedBoost", hasSpeedBoost);
            }
            

            if (isMoving)
            {
                stepTimer -= Time.deltaTime;

                if (stepTimer <= 0f)
                {
                    PlayFootstep();
                    stepTimer = stepInterval;
                }
            }
            else
            {
                stepTimer = 0f;
            }
        }
    
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_isAlive) return;
            if (!other.CompareTag("Enemy")) return;
            if (_immunityBuff != null && !_immunityBuff.IsExpired)
            {
                Destroy(other.gameObject);
                return;
            }
            Vector2 hitPoint = other.ClosestPoint(transform.position);
            channel.NotifyPlayerDied(hitPoint);
            moveAction.action.Disable();
            _isAlive = false;
        }

        private void MoveWithCollision(Vector2 delta)
        {
            if (delta == Vector2.zero)
                return;

            Vector2 dir = delta.normalized;
            float dist = delta.magnitude;

            ContactFilter2D filter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = obstacleMask
            };
            int hitCount = _col.Cast(dir, filter, _hits, dist + skin);

            float allowed = dist;

            for (int i = 0; i < hitCount; i++)
            {
                if (!_hits[i].collider)
                    continue;

                float d = _hits[i].distance - skin;
                if (d < allowed)
                    allowed = Mathf.Max(0f, d);

                // Debug.Log("Hit: " + hits[i].collider.name + " dist: " + hits[i].distance);
            }

            transform.position += (Vector3)(dir * allowed);
        }
        
        void PlayFootstep()
        {
            if (footstepClips.Length == 0) return;

            footstepSource.clip = footstepClips[Random.Range(0, footstepClips.Length)];
            footstepSource.Play();
        }

        public bool AttachBuff(BuffAttachment attachment)
        {
            if (attachment.Type == BuffType.Immunity)
            {
                if (_immunityBuff == null)
                {
                    _immunityBuff = attachment;
                    if (_immunityBubble == null)
                        ResolveImmunityBubble();
                    if (_immunityBubble != null)
                        _immunityBubble.SetActive(true);
                    return true;
                }
                return false;
            }
            if (_activeBuff == null)
            {
                _activeBuff = attachment;
                if (_animator && attachment.Type == BuffType.SpeedBoost)
                    _animator.SetBool("SpeedBoost", true);
                return true;
            }
            return false;
        }
        
    }
}