using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace _Code.UI
{
    [RequireComponent(typeof(Animator))]
    public class DashCooldownUI : MonoBehaviour
    {
        private static readonly int DashIsReady = Animator.StringToHash("DashIsReady");

        [Header("References")]
        // Filled image that shows the cooldown progress.
        [SerializeField] private Image fillImage;
        
        private float _cooldownDuration;
        private Coroutine _cooldownRoutine;

        private float _currentAmount;
        
        private Animator _animator;

        private void Awake()
        {
            // Start full so the bar shows dash is ready.
            SetFillAmount(1f);
            _animator =  GetComponent<Animator>();
        }

        private void Update()
        {
            HandleAnimator();
        }

        private void HandleAnimator()
        {
            _animator.SetBool(DashIsReady, _currentAmount == 1f);
        }

        // Called by the player's OnStartDash UnityEvent.
        public void StartCooldown(float cd)
        {
            _cooldownDuration = cd;
            // Stops the old cooldown if dash somehow starts again before it finished.
            if (_cooldownRoutine != null)
                StopCoroutine(_cooldownRoutine);

            _cooldownRoutine = StartCoroutine(CooldownRoutine());
        }

        private IEnumerator CooldownRoutine()
        {
            if (_cooldownDuration <= 0f)
            {
                SetFillAmount(1f);
                _cooldownRoutine = null;
                yield break;
            }

            // Dash just started, so the bar begins empty.
            SetFillAmount(0f);

            var timer = 0f;

            // Fill the bar over the cooldown time.
            while (timer < _cooldownDuration)
            {
                timer += Time.deltaTime;

                var progress = timer / _cooldownDuration;
                SetFillAmount(progress);

                yield return null;
            }

            SetFillAmount(1f);
            _cooldownRoutine = null;
        }

        private void SetFillAmount(float value)
        {
            if (!fillImage) return;

            float amount = Mathf.Clamp01(value);
            fillImage.fillAmount = amount;
            _currentAmount = amount;
        }
    }
}