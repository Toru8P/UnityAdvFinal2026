using _Code.MainGame.Buff;
using _Code.MainGame.Enemy;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace _Code.MainGame.Player
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerStateChannel channel;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 4f;

        [Header("Dash Settings")]
        [SerializeField] private float dashSpeed = 15f;
        [SerializeField] private float dashTime = 0.25f;
        [SerializeField] private float dashCooldown = 3f;
        [SerializeField] private UnityEvent<float> onStartDash;

        
        [Header("Death settings")]
        [SerializeField] private UnityEvent onDeath;
        
        [Header("Hit Detection Settings")]
        [SerializeField] private float skin = 0.02f;
        [SerializeField] private LayerMask obstacleMask;

        [Header("Footsteps Settings")]
        public float stepInterval = 0.4f;
        private float _stepTimer = 0f;
        public AudioSource footstepSource;
        public AudioClip[] footstepClips;

        private bool _isAlive = true;
        private bool _isDashing;

        private float _dashTimeLeft;
        private float _dashCooldownLeft;

        private Collider2D _col;
        private Vector2 _moveInput;
        private Vector2 _dashDirection;
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

        // Called by the PlayerInput Unity Event.
        // This only stores the latest move direction.
        public void Move(InputAction.CallbackContext context)
        {
            if (!_isAlive) return;

            _moveInput = context.ReadValue<Vector2>();
            _moveInput = Vector2.ClampMagnitude(_moveInput, 1f);
        }

        // Called by the PlayerInput Unity Event.
        // Dash stores the direction once, then keeps using that same direction until it ends.
        public void Dash(InputAction.CallbackContext context)
        {
            if (!_isAlive) return;
            if (!context.performed) return;
            if (_isDashing) return;
            if (_dashCooldownLeft > 0f) return;

            // Dash needs a move direction at the moment the button is pressed.
            if (_moveInput.sqrMagnitude <= 0.01f) return;

            _isDashing = true;
            _dashTimeLeft = dashTime;
            _dashCooldownLeft = dashCooldown;
            _dashDirection = _moveInput.normalized;

            onStartDash?.Invoke(dashCooldown);
        }

        private void Update()
        {
            if (!_isAlive) return;

            UpdateDashCooldown();

            // Buff lifetime still updates here so timing stays frame-based.
            if (_activeBuff != null)
            {
                _activeBuff.Update(Time.deltaTime);
                if (_activeBuff.IsExpired) _activeBuff = null;
            }

            // Normal movement is paused while dash is active.
            // Move() can still receive input, but dash uses its stored direction.
            if (_isDashing)
                UpdateDash();
            else
                UpdateMovement();

            HandleFootsteps();
        }

        private void UpdateDashCooldown()
        {
            if (_dashCooldownLeft <= 0f) return;

            _dashCooldownLeft -= Time.deltaTime;
            if (_dashCooldownLeft < 0f)
                _dashCooldownLeft = 0f;
        }

        private void UpdateMovement()
        {
            float speed = moveSpeed;
            if (_activeBuff != null && _activeBuff.Type == BuffType.SpeedBoost)
                speed += _activeBuff.Value;

            Vector2 delta = _moveInput * (speed * Time.deltaTime);
            MoveWithCollision(delta);
        }

        private void UpdateDash()
        {
            // Dash uses the saved direction from the frame the dash started.
            // Releasing move keys during dash will not stop it.
            Vector2 delta = _dashDirection * (dashSpeed * Time.deltaTime);

            // Dash goes through the same collision path as normal movement,
            // so it still stops at walls and does not pass through them.
            MoveWithCollision(delta);

            _dashTimeLeft -= Time.deltaTime;
            if (_dashTimeLeft > 0f) return;

            _dashTimeLeft = 0f;
            _isDashing = false;
        }

        private void HandleFootsteps()
        {
            // Footsteps stop during dash so the short burst does not sound like normal walking.
            if (_isDashing || !IsMoving || ActiveBuffType == BuffType.SpeedBoost)
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

            _moveInput = Vector2.zero;
            _dashDirection = Vector2.zero;
            _isDashing = false;


            onDeath.Invoke();
            _isAlive = false;
        }

        private void MoveWithCollision(Vector2 delta)
        {
            if (delta == Vector2.zero) return;

            // Straight movement can still use one simple axis move.
            if (Mathf.Approximately(delta.x, 0f) || Mathf.Approximately(delta.y, 0f))
            {
                MoveAlongAxis(delta);
                return;
            }

            // Diagonal movement needs a small wall-slide check.
            // If one axis is blocked, the other axis gets the full move speed.
            MoveWithWallSlide(delta);
        }

        private void MoveWithWallSlide(Vector2 delta)
        {
            float totalDist = delta.magnitude;

            Vector2 xDir = Vector2.right * Mathf.Sign(delta.x);
            Vector2 yDir = Vector2.up * Mathf.Sign(delta.y);

            float xRequested = Mathf.Abs(delta.x);
            float yRequested = Mathf.Abs(delta.y);

            // Check each axis by itself first.
            // This tells us if a wall is blocking only one side of the diagonal move.
            float xAllowed = GetAllowedDistance(xDir, xRequested);
            float yAllowed = GetAllowedDistance(yDir, yRequested);

            bool xBlocked = xAllowed + 0.0001f < xRequested;
            bool yBlocked = yAllowed + 0.0001f < yRequested;

            if (xBlocked && yBlocked)
                return;

            // If vertical is blocked but horizontal is open,
            // slide horizontally at full speed instead of diagonal speed.
            if (yBlocked && !xBlocked)
            {
                MoveAlongAxis(xDir * totalDist);
                return;
            }

            // If horizontal is blocked but vertical is open,
            // slide vertically at full speed instead of diagonal speed.
            if (xBlocked && !yBlocked)
            {
                MoveAlongAxis(yDir * totalDist);
                return;
            }

            // When both axes are open, keep the normal diagonal move.
            // Larger axis goes first to make corners feel a bit more stable.
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                MoveAlongAxis(Vector2.right * delta.x);
                MoveAlongAxis(Vector2.up * delta.y);
            }
            else
            {
                MoveAlongAxis(Vector2.up * delta.y);
                MoveAlongAxis(Vector2.right * delta.x);
            }
        }

        private void MoveAlongAxis(Vector2 axisDelta)
        {
            if (axisDelta == Vector2.zero) return;

            // Same collision test as before, now reused for one axis at a time.
            Vector2 dir = axisDelta.normalized;
            float dist = axisDelta.magnitude;

            float allowed = GetAllowedDistance(dir, dist);
            transform.position += (Vector3)(dir * allowed);
        }

        private float GetAllowedDistance(Vector2 dir, float dist)
        {
            if (dist <= 0f) return 0f;

            // Cast the collider forward and stop right before the wall.
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

            return allowed;
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