using UnityEngine;

namespace _Code.MainGame.Camera
{
    public class CameraPlayerFollower : MonoBehaviour
    {
    
        [SerializeField] private Transform target;
    
        void Update()
        {
            if (target)
            {
                Vector3 newPosition = target.position;
                newPosition.z = transform.position.z;
                transform.position = newPosition;
            }
        }
    }
}
