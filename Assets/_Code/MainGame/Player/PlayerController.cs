using _Code.MainGame.Buff;
using _Code.MainGame.Enemy;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Code.MainGame.Player
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerStateChannel channel;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 4f;

        [Header("Input Settings")]
        [SerializeField] private InputActionReference moveAction;

        [Header("Hit Detection Settings")]
        [SerializeField] private float skin = 0.02f;
        [SerializeField] private LayerMask obstacleMask;

        [Header("Footsteps Settings")]
        public float stepInterval = 0.4f;
        private float _stepTimer = 0f;
        public AudioSource footstepSource;
        public AudioClip[] footstepClips;

        private bool _isAlive = true;

        private Collider2D _col;
        private Vector2 _moveInput;
        private readonly RaycastHit2D[] _hits = new RaycastHit2D[8];

        private InputAction.CallbackContext _context;

        [CanBeNull] private BuffAttachment _activeBuff;

        public bool IsAlive => _isAlive;
        public Vector2 MoveInput => _moveInput;
        public bool IsMoving => _moveInput.sqrMagnitude > 0.01f;

        public BuffType? ActiveBuffType => _activeBuff != null ? _activeBuff.Type : null;
        public float ActiveBuffValue => _activeBuff != null ? _activeBuff.Value : 0f;

        private void Awake()
        {
            _col = GetComponent<Collider2D>();
        }

        private void OnEnable()
        {
            if (moveAction)
                moveAction.action.Enable();
        }

        private void OnDisable()
        {
            if (moveAction)
                moveAction.action.Disable();
        }

        private void Update()
        {
            if (!_isAlive) return;

            // Buff lifetime update stays gameplay-side
            if (_activeBuff != null)
            {
                _activeBuff.Update(Time.deltaTime);
                if (_activeBuff.IsExpired) _activeBuff = null;
            }

            // Input + movement stays gameplay-side
            _moveInput = moveAction.action.ReadValue<Vector2>();
            _moveInput = Vector2.ClampMagnitude(_moveInput, 1f);

            float speed = moveSpeed;
            if (_activeBuff != null && _activeBuff.Type == BuffType.SpeedBoost)
                speed += _activeBuff.Value;

            Vector2 delta = _moveInput * (speed * Time.deltaTime);
            MoveWithCollision(delta);

            // Footsteps stays gameplay-side (or can also be moved to visuals)
            HandleFootsteps();
        }

        private void HandleFootsteps()
        {
            if (!IsMoving || ActiveBuffType == BuffType.SpeedBoost)
            {
                _stepTimer = 0f;
                return;
            }

            _stepTimer -= Time.deltaTime;
            if (_stepTimer <= 0f)
            {
                PlayFootstep();
                _stepTimer = stepInterval;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_isAlive) return;
            if (!other.CompareTag("Enemy")) return;
            
            if (_activeBuff != null && _activeBuff.Type == BuffType.Immunity)
            {
                _activeBuff = null;

                
                Vector2 away = (other.ClosestPoint(transform.position) - (Vector2)transform.position).normalized;
                other.GetComponentInParent<EnemyController>()?.Knock(away);


                return;
            }


            Vector2 hitPoint = other.ClosestPoint(transform.position);
            channel.NotifyPlayerDied(hitPoint);
            moveAction.action.Disable();
            _isAlive = false;
        }

        private void MoveWithCollision(Vector2 delta)
        {
            if (delta == Vector2.zero) return;

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
                if (!_hits[i].collider) continue;

                float d = _hits[i].distance - skin;
                if (d < allowed)
                    allowed = Mathf.Max(0f, d);
            }

            transform.position += (Vector3)(dir * allowed);
        }

        private void PlayFootstep()
        {
            if (footstepClips == null || footstepClips.Length == 0) return;
            if (!footstepSource) return;

            footstepSource.clip = footstepClips[Random.Range(0, footstepClips.Length)];
            footstepSource.Play();
        }

        public bool AttachBuff(BuffAttachment attachment)
        {
            _activeBuff = attachment;
            return true;
        }
    }
}