namespace AFSInterview
{
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    public class GameplayManager : MonoBehaviour
    {
        [Header("Prefabs")] 
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private GameObject simpleTowerPrefab;
        [SerializeField] private GameObject burstTowerPrefab;

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
        private TextMeshProUGUI scoreTextComponent;
        private TextMeshProUGUI enemiesCountTextComponent;

        private void Awake()
        {
            enemies = new List<Enemy>();
            camera = Camera.main;
            scoreTextComponent = scoreText.GetComponent<TextMeshProUGUI>();
            enemiesCountTextComponent = enemiesCountText.GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            // simplifying timer logic (minor optimization)
            if (Time.time >= nextEnemySpawnTime)
            {
                SpawnEnemy();
                nextEnemySpawnTime = Time.time + enemySpawnRate;
            }

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                // caching Camera.main since it's not being cached by default in this Unity version
                var ray = camera.ScreenPointToRay(Input.mousePosition);

                // having ground layer mask cached
                if (Physics.Raycast(ray, out var hit, Utils.GroundMask))
                {
                    var spawnPosition = hit.point;
                    spawnPosition.y = simpleTowerPrefab.transform.position.y;

                    var towerToSpawn = Input.GetMouseButtonDown(0) ? simpleTowerPrefab : burstTowerPrefab;
                    SpawnTower(towerToSpawn, spawnPosition);
                }
            }

            // avoiding GC allocation for new strings whenever possible
            if (score != lastScore)
            {
                // having component references cached instead of calling GetComponent<>()
                // got rid of string concatenation that used '+' operator
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

        private void SpawnTower(GameObject towerPrefab, Vector3 position)
        {
            var tower = Instantiate(towerPrefab, position, Quaternion.identity).GetComponent<Tower>();
            tower.Initialize(enemies);
        }
    }
}