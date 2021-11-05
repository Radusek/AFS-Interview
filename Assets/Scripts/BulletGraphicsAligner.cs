namespace AFSInterview
{
    using UnityEngine;

    public class BulletGraphicsAligner : MonoBehaviour
    {
        [SerializeField] private Transform graphicsTransform;

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            graphicsTransform.rotation = Quaternion.LookRotation(rb.velocity);
        }
    }
}
