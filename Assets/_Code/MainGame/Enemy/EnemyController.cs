using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

namespace _Code.MainGame.Enemy
{
    [RequireComponent(typeof(Collider2D), typeof(NavMeshAgent), typeof(SpriteRenderer))]
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private GameObject minimapPointer;

        [Header("Light Settings")]
        [SerializeField] private bool turnOnLight = true;
        [SerializeField] private Light2D lights;
        
        private NavMeshAgent _agent;
        private SpriteRenderer _spriteRenderer;
        private Vector2 _currentLightDir = Vector2.right;
        
        private float? _pendingSpeed = null;
        
        private float _knockTime;
        private Vector2 _knockVel;
        private bool _knocked;


        public void Knock(Vector2 away, float strength = 6f, float time = 0.12f)
        {
            _knocked = true;
            _knockTime = time;
            _knockVel = away.normalized * strength;

            _agent.isStopped = true;
            _agent.ResetPath();
        }


        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
            if (target)
            {
                _agent.SetDestination(target.position);
            }

            if (_pendingSpeed.HasValue)
            {
                _agent.speed = _pendingSpeed.Value;
            }
            
            if (lights)
            {
                lights.enabled = turnOnLight;
            }

#if UNITY_EDITOR
            if (minimapPointer) SceneVisibilityManager.instance.Hide(minimapPointer, false);
#endif
        }

        private void LateUpdate()
        {
                        
            if (_knocked)
            {
                Vector2 d = _knockVel * Time.deltaTime;
                _agent.Move(new Vector3(d.x, 0f, d.y));


                _knockTime -= Time.deltaTime;
                if (_knockTime <= 0f)
                {
                    _knocked = false;
                    _agent.isStopped = false;
                }
                return;
            }
            
            if (target)
            {
                _agent.SetDestination(target.position);
            }
            
            if (_spriteRenderer && Mathf.Abs(_agent.velocity.x) > 0.01f)
            {
                _spriteRenderer.flipX = _agent.velocity.x > 0;
            }
            
            if (target && turnOnLight && lights)
            {
                Vector2 toTarget = (transform.position - target.position).normalized;
                _currentLightDir = Vector2.Lerp(_currentLightDir, toTarget, 10f * Time.deltaTime);
                lights.transform.right = _currentLightDir;
            }
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            if (_agent && target)
            {
                _agent.SetDestination(target.position);
            }
        }

        public void SetSpeed(float speed)
        {
            _pendingSpeed = speed;
        }
        
        public void TurnLight(bool on)
        {
            turnOnLight = on;
        }
    }
}