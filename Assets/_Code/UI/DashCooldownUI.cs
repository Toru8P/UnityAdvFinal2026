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
        [SerializeField] private Image fillImage;

        private Animator _animator;

        private float _cooldownDuration;
        private float _elapsedTime;
        private float _currentAmount;

        private bool _isCoolingDown;
        private Coroutine _cooldownRoutine;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            SetFillAmount(1f);
        }

        private void Update()
        {
            HandleAnimator();
        }

        private void OnEnable()
        {
            // If the object was disabled during cooldown, resume the coroutine.
            if (_isCoolingDown && _cooldownRoutine == null)
            {
                _cooldownRoutine = StartCoroutine(CooldownRoutine());
            }
        }

        private void OnDisable()
        {
            // Coroutines stop automatically on disable, but clearing the reference
            // lets us restart it properly in OnEnable.
            if (_cooldownRoutine != null)
            {
                StopCoroutine(_cooldownRoutine);
                _cooldownRoutine = null;
            }
        }

        public void StartCooldown(float cd)
        {
            _cooldownDuration = cd;
            _elapsedTime = 0f;

            if (_cooldownRoutine != null)
            {
                StopCoroutine(_cooldownRoutine);
            }

            if (_cooldownDuration <= 0f)
            {
                _isCoolingDown = false;
                _cooldownRoutine = null;
                SetFillAmount(1f);
                return;
            }

            _isCoolingDown = true;
            _cooldownRoutine = StartCoroutine(CooldownRoutine());
        }

        private IEnumerator CooldownRoutine()
        {
            SetFillAmount(_elapsedTime / _cooldownDuration);

            while (_elapsedTime < _cooldownDuration)
            {
                _elapsedTime += Time.deltaTime;

                var progress = _elapsedTime / _cooldownDuration;
                SetFillAmount(progress);

                yield return null;
            }

            _elapsedTime = _cooldownDuration;
            SetFillAmount(1f);

            _isCoolingDown = false;
            _cooldownRoutine = null;
        }

        private void HandleAnimator()
        {
            _animator.SetBool(DashIsReady, _currentAmount >= 1f);
        }

        private void SetFillAmount(float value)
        {
            if (!fillImage)
            {
                return;
            }

            var amount = Mathf.Clamp01(value);
            fillImage.fillAmount = amount;
            _currentAmount = amount;
        }
    }
}