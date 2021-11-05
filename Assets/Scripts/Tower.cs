namespace AFSInterview
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Tower : MonoBehaviour
    {
        [SerializeField] protected GameObject bulletPrefab;
        [SerializeField] protected Transform bulletSpawnPoint;
        [SerializeField] protected float firingRate;
        [SerializeField] protected float firingRange;
        
        protected IReadOnlyList<Enemy> enemies;
        protected float fireTimer;
        
        public void Initialize(IReadOnlyList<Enemy> enemies)
        {
            this.enemies = enemies;
            fireTimer = firingRate;
        }
        
        private void Update()
        {
            Enemy targetEnemy = FindTargetEnemy();
            if (targetEnemy != null)
            {
                var lookRotation = Quaternion.LookRotation(targetEnemy.transform.position - transform.position);
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, lookRotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            }

            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0f)
            {
                if (targetEnemy != null)
                {
                    Attack(targetEnemy);
                }

                fireTimer = firingRate;
            }
        }

        protected virtual Enemy FindTargetEnemy()
        {
            Enemy closestEnemy = null;
            var closestDistance = float.MaxValue;

            foreach (var enemy in enemies)
            {
                var distance = (enemy.transform.position - transform.position).magnitude;
                if (distance <= firingRange && distance <= closestDistance)
                {
                    closestEnemy = enemy;
                    closestDistance = distance;
                }
            }

            return closestEnemy;
        }

        protected virtual void Attack(Enemy target)
        {
            var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity).GetComponent<IBullet>();
            bullet.Initialize(target.gameObject);
        }
    }
}