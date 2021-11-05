using System;

namespace AFSInterview
{
    using UnityEngine;

    public class Arrow : MonoBehaviour, IBullet
    {
        [SerializeField] private float speed;

        private GameObject target;
        
        public void Initialize(GameObject target)
        {
            this.target = target;
            var targetPos = target.transform.position;
            var targetVelocity = target.GetComponent<Enemy>().Velocity;
            var firePoint = transform.position;
            GetComponent<Rigidbody>().velocity = GetInterceptCourseDirection(targetPos, targetVelocity, firePoint, speed);
        }
        
        private Vector3 GetInterceptCourseDirection(Vector3 targetPos, Vector3 targetVelocity, Vector3 firePoint, float projectileSpeed)
        {
            Vector3 targetDir = targetPos - firePoint;
            targetDir.y = 0f;
            
            float iSpeed2 = projectileSpeed * projectileSpeed;
            float tSpeed2 = targetVelocity.sqrMagnitude;
            float fDot1 = Vector3.Dot(targetDir, targetVelocity);
            float targetDist2 = targetDir.sqrMagnitude;
            float d = (fDot1 * fDot1) - targetDist2 * (tSpeed2 - iSpeed2);
            
            if (d < 0.1f) // negative == no possible course because the interceptor isn't fast enough
            {
                return Vector3.zero;
            }
            
            float sqrt = Mathf.Sqrt(d);
            float S1 = (-fDot1 - sqrt) / targetDist2;
            float S2 = (-fDot1 + sqrt) / targetDist2;
            
            if (S1 < 0.0001f)
            {
                if (S2 < 0.0001f)
                {
                    return Vector3.zero;
                }
                return (S2) * targetDir + targetVelocity;
            }

            if (S2 < 0.0001f)
            {
                return (S1) * targetDir + targetVelocity;
            }
            
            if (S1 < S2)
            {
                return (S2) * targetDir + targetVelocity;
            }
            
            return (S1) * targetDir + targetVelocity;
        }

        private void OnTriggerEnter(Collider other)
        {
            var collidedGameObject = other.gameObject;
            if (collidedGameObject.layer.IsLayerInMask(Utils.GroundMask))
            {
                Destroy(gameObject);
                return;
            }
            
            if (collidedGameObject == target)
            {
                Destroy(gameObject);
                Destroy(collidedGameObject);
            }
        }
    }
}
