
using _Code.MainGame.Buff;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace _Code.MainGame.Player
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerVisualController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer skinRenderer;

        [Header("Immunity Buff Visual")]
        [SerializeField] private GameObject immunityBubble;

        [Header("Light Settings")]
        [SerializeField] private bool turnOnLight = true;
        [SerializeField] private Light2D lights;

        [Header("Animator Params")]
        [SerializeField] private string walkParam = "Walk";
        [SerializeField] private string speedBoostParam = "SpeedBoost";

        private PlayerController _player;

        private void Awake()
        {
            _player = GetComponent<PlayerController>();

            if (!animator) animator = GetComponent<Animator>();
            if (turnOnLight && lights) lights.enabled = true;
        }

        private void Update()
        {
            if (!_player.IsAlive)
            {
                // Optional: you can set death anim state here if you have one
                SetWalk(false);
                SetSpeedBoost(false);
                if (immunityBubble) immunityBubble.SetActive(false);
                return;
            }

            var move = _player.MoveInput;
            bool isMoving = _player.IsMoving;

            // Flip sprite
            if (skinRenderer && Mathf.Abs(move.x) > 0.001f)
                skinRenderer.flipX = !(move.x > 0);

            // Buff visuals & animation flags
            bool hasSpeedBoost = _player.ActiveBuffType == BuffType.SpeedBoost;
            bool hasImmunity = _player.ActiveBuffType == BuffType.Immunity;

            if (immunityBubble)
                immunityBubble.SetActive(hasImmunity);

            SetWalk(isMoving);
            SetSpeedBoost(hasSpeedBoost);

            // Light rotation (optional)
            if (lights && isMoving)
            {
                Vector2 dir = move.normalized;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Quaternion targetRot = Quaternion.Euler(0f, 0f, angle);

                lights.transform.rotation = Quaternion.RotateTowards(
                    lights.transform.rotation,
                    targetRot,
                    720f * Time.deltaTime
                );
            }
        }

        private void SetWalk(bool value)
        {
            if (animator) animator.SetBool(walkParam, value);
        }

        private void SetSpeedBoost(bool value)
        {
            if (animator) animator.SetBool(speedBoostParam, value);
        }
    }
}
