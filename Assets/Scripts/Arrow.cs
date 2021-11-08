namespace AFSInterview
{
    using System.Collections;
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

            var interceptionPoint = FirstOrderIntercept(firePoint, Vector3.zero, speed, targetPos, targetVelocity);
            var horizontalDistance = Vector3.Distance(firePoint.ResetY(), interceptionPoint.ResetY());
            var heightDifference = interceptionPoint.y - firePoint.y;
            
            var shootingDirection = (interceptionPoint - firePoint).ResetY().normalized;
            if (!CalculateTrajectory(horizontalDistance, heightDifference, speed, out var trajectoryAngle))
            {
                // projectile's speed isn't enough to reach the target
                Destroy(gameObject);
                return;
            }
            
            Vector3 targetVector = trajectoryAngle < 0f ? Vector3.down : Vector3.up;
            shootingDirection = Vector3.Slerp(shootingDirection, targetVector, Mathf.Abs(trajectoryAngle) / 90f);
            GetComponent<Rigidbody>().velocity = speed * shootingDirection;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            var collidedGameObject = other.gameObject;
            if (collidedGameObject.layer.IsLayerInMask(Utils.GroundMask))
            {
                StartCoroutine(DelayedDestroy());
                return;
            }
            
            if (collidedGameObject == target)
            {
                Destroy(gameObject);
                Destroy(collidedGameObject);
            }
        }
        
        private IEnumerator DelayedDestroy()
        {
            yield return new WaitForSeconds(0.5f);
            Destroy(gameObject);
        }
        
        //first-order intercept using absolute target position
        public static Vector3 FirstOrderIntercept(Vector3 shooterPosition, Vector3 shooterVelocity, float shotSpeed, Vector3 targetPosition, Vector3 targetVelocity)  
        {
            Vector3 targetRelativePosition = targetPosition - shooterPosition;
            Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
            float t = FirstOrderInterceptTime(shotSpeed, targetRelativePosition, targetRelativeVelocity);
            return targetPosition + t*(targetRelativeVelocity);
        }
        
        //first-order intercept using relative target position
        public static float FirstOrderInterceptTime(float shotSpeed, Vector3 targetRelativePosition, Vector3 targetRelativeVelocity)
        {
            float velocitySquared = targetRelativeVelocity.sqrMagnitude;
            if (velocitySquared < 0.001f)
                return 0f;
 
            float a = velocitySquared - shotSpeed*shotSpeed;
 
            //handle similar velocities
            if (Mathf.Abs(a) < 0.001f)
            {
                float t = -targetRelativePosition.sqrMagnitude / (2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition));
                return Mathf.Max(t, 0f); //don't shoot back in time
            }
 
            float b = 2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
            float c = targetRelativePosition.sqrMagnitude;
            float determinant = b * b - 4f * a * c;
 
            if (determinant > 0f) //determinant > 0; two intercept paths (most common)
            {
                float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a), t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);
                if (t1 > 0f) 
                {
                    if (t2 > 0f)
                        return Mathf.Min(t1, t2); //both are positive
                    return t1; //only t1 is positive
                }
                return Mathf.Max(t2, 0f); //don't shoot back in time
            } 
            if (determinant < 0f) //determinant < 0; no intercept path
                return 0f;
            //determinant = 0; one intercept path, pretty much never happens
            return Mathf.Max(-b/(2f*a), 0f); //don't shoot back in time
        }
        
        public static bool CalculateTrajectory(float horizontalDistance, float heightDifference, float projectileVelocity, out float calculatedAngle)
        {
            float gravity = -Physics.gravity.y;
            float velocitySqr = projectileVelocity * projectileVelocity;

            float sqrt = Mathf.Sqrt(velocitySqr * velocitySqr - gravity * (gravity * horizontalDistance * horizontalDistance + 2 * heightDifference * velocitySqr));
            
            float numeratorA = velocitySqr + sqrt;
            float numeratorB = velocitySqr - sqrt;
            
            float denominator = gravity * horizontalDistance;
            
            calculatedAngle = Mathf.Atan(numeratorB / denominator) * Mathf.Rad2Deg;
            if (float.IsNaN(calculatedAngle))
            {
                calculatedAngle = 0;
                return false;
            }  
            return true;
        }
    }
}
