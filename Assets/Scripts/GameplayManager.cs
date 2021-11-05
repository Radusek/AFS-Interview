namespace AFSInterview
{
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    public class GameplayManager : MonoBehaviour
    {
        [Header("Prefabs")] 
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private GameObject towerPrefab;

        [Header("Settings")] 
        [SerializeField] private Vector2 boundsMin;
        [SerializeField] private Vector2 boundsMax;
        [SerializeField] private float enemySpawnRate;

        [Header("UI")] 
        [SerializeField] private GameObject enemiesCountText;
        [SerializeField] private GameObject scoreText;
        
        private List<Enemy> enemies;
        private float nextEnemySpawnTime;
        private int score;
        private int lastScore;
        private int lastEnemiesCount;
        private new Camera camera;
        private int groundMask;
        private TextMeshProUGUI scoreTextComponent;
        private TextMeshProUGUI enemiesCountTextComponent;

        private void Awake()
        {
            enemies = new List<Enemy>();
            camera = Camera.main;
            groundMask = LayerMask.GetMask("Ground");
            scoreTextComponent = scoreText.GetComponent<TextMeshProUGUI>();
            enemiesCountTextComponent = enemiesCountText.GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            if (Time.time >= nextEnemySpawnTime)
            {
                SpawnEnemy();
                nextEnemySpawnTime = Time.time + enemySpawnRate;
            }

            if (Input.GetMouseButtonDown(0))
            {
                var ray = camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit, groundMask))
                {
                    var spawnPosition = hit.point;
                    spawnPosition.y = towerPrefab.transform.position.y;

                    SpawnTower(spawnPosition);
                }
            }

            if (score != lastScore)
            {
                scoreTextComponent.SetText("Score: {0}", score);
                lastScore = score;
            }

            int enemiesCount = enemies.Count;
            if (enemiesCount != lastEnemiesCount)
            {
                enemiesCountTextComponent.SetText("Enemies: {0}", enemiesCount);
                lastEnemiesCount = enemiesCount;
            }
        }

        private void SpawnEnemy()
        {
            var position = new Vector3(Random.Range(boundsMin.x, boundsMax.x), enemyPrefab.transform.position.y, Random.Range(boundsMin.y, boundsMax.y));
            
            var enemy = Instantiate(enemyPrefab, position, Quaternion.identity).GetComponent<Enemy>();
            enemy.OnEnemyDied += Enemy_OnEnemyDied;
            enemy.Initialize(boundsMin, boundsMax);

            enemies.Add(enemy);
        }

        private void Enemy_OnEnemyDied(Enemy enemy)
        {
            enemies.Remove(enemy);
            score++;
        }

        private void SpawnTower(Vector3 position)
        {
            var tower = Instantiate(towerPrefab, position, Quaternion.identity).GetComponent<SimpleTower>();
            tower.Initialize(enemies);
        }
    }
}