using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private EnemyData[] enemyDatas; // 생성할 적 종류
    [SerializeField] private Transform[] spawnPoints; // 스폰 위치들
    [SerializeField] private float spawnInterval = 5f; // 생성 간격
    [SerializeField] private int maxEnemies = 10; // 동시에 존재 가능한 최대 수

    [Header("웨이브 설정 (선택)")]
    [SerializeField] private bool useWave = false;
    [SerializeField] private int enemiesPerWave = 5;
    [SerializeField] private float waveDelay = 10f;

    private readonly List<GameObject> activeEnemies = new List<GameObject>();
    private bool spawning = false;

    void Start()
    {
        if (enemyDatas.Length == 0 || spawnPoints.Length == 0)
        {
            Debug.LogWarning("EnemySpawner: EnemyData나 SpawnPoint가 설정되지 않았습니다.");
            return;
        }

        StartCoroutine(useWave ? SpawnWaveRoutine() : SpawnContinuousRoutine());
    }

    IEnumerator SpawnContinuousRoutine()
    {
        spawning = true;
        while (spawning)
        {
            yield return new WaitForSeconds(spawnInterval);
            CleanupDeadEnemies();
            if (activeEnemies.Count < maxEnemies)
                SpawnEnemy();
        }
    }

    IEnumerator SpawnWaveRoutine()
    {
        spawning = true;
        while (spawning)
        {
            CleanupDeadEnemies();

            int spawnCount = Mathf.Min(enemiesPerWave, maxEnemies - activeEnemies.Count);
            for (int i = 0; i < spawnCount; i++)
                SpawnEnemy();

            yield return new WaitForSeconds(waveDelay);
        }
    }

    void SpawnEnemy()
    {
        // 랜덤 데이터와 위치 선택
        EnemyData data = enemyDatas[Random.Range(0, enemyDatas.Length)];
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

        if (data.prefab == null)
        {
            Debug.LogWarning($"EnemyData {data.name}의 prefab이 비어있습니다.");
            return;
        }

        GameObject enemy = Instantiate(data.prefab, point.position, point.rotation);
        if (enemy.TryGetComponent(out EnemyBase baseComp))
        {
            // 데이터 직접 주입 (prefab에 없을 때 대비)
            var field = typeof(EnemyBase).GetField("data", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(baseComp, data);
        }

        activeEnemies.Add(enemy);
    }

    void CleanupDeadEnemies()
    {
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (activeEnemies[i] == null || !activeEnemies[i].activeInHierarchy)
                activeEnemies.RemoveAt(i);
        }
    }

    // 에디터 디버그용
    private void OnDrawGizmos()
    {
        if (spawnPoints == null) return;
        Gizmos.color = Color.green;
        foreach (var p in spawnPoints)
        {
            if (p != null)
                Gizmos.DrawWireSphere(p.position, 0.5f);
        }
    }
}
