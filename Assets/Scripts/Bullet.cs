namespace AFSInterview
{
    using UnityEngine;

    public class Bullet : MonoBehaviour, IBullet
    {
        [SerializeField] private float speed;

        private GameObject targetObject;

        public void Initialize(GameObject target)
        {
            targetObject = target;
        }

        private void Update()
        {
            if (!targetObject)
            {
                // could have alternatively pick the nearest enemy alive if such exists and set it as the new target
                // instead of destroying the projectile but I decided to keep it simple
                Destroy(gameObject);
                return;
            }
        
            var direction = (targetObject.transform.position - transform.position).normalized;

            transform.position += direction * speed * Time.deltaTime;

            if ((transform.position - targetObject.transform.position).magnitude <= 0.2f)
            {
                Destroy(gameObject);
                Destroy(targetObject);
            }
        }
    }
}