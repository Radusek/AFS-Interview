namespace AFSInterview
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class BurstTower : Tower
    {
        [SerializeField] private int bulletsInBurst;
        [SerializeField] private float midBurstInterval;

        private readonly HashSet<Enemy> shotTargets = new HashSet<Enemy>();

        protected override void Attack(Enemy target)
        {
            StartCoroutine(Burst(target));
        }

        private IEnumerator Burst(Enemy target)
        {
            var delay = new WaitForSeconds(midBurstInterval);
            for (int i = 0; i < bulletsInBurst; i++)
            {
                var nextTarget = i == 0 ? target : FindTargetEnemy();
                if (!nextTarget)
                {
                    shotTargets.Clear();
                    yield break;
                }
                
                shotTargets.Add(nextTarget);
                base.Attack(nextTarget);
                yield return delay;
            }
            shotTargets.Clear();
        }

        protected override Enemy FindTargetEnemy()
        {
            Enemy closestEnemy = null;
            var closestDistance = float.MaxValue;

            foreach (var enemy in enemies)
            {
                var distance = (enemy.transform.position - transform.position).magnitude;
                if (distance <= firingRange && distance <= closestDistance && !shotTargets.Contains(enemy))
                {
                    closestEnemy = enemy;
                    closestDistance = distance;
                }
            }

            return closestEnemy;
        }
    }
}