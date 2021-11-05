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