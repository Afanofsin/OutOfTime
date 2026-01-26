using System;
using Cysharp.Threading.Tasks;
using ProjectFiles.Code.Controllers;
using ProjectFiles.Code.LevelGeneration;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyDatabase enemyDb;
    [SerializeField] private EnemyEntityBase enemyToSpawn;
    [SerializeField] private float maxSpawnTime = 3f;

    private bool isSpawningEngaged = false;
    private float spawnRandomize;
    private Room room;
    private bool hasSpawned;
    
    private void Awake()
    {
        room = GetComponentInParent<Room>();
    }

    private void OnEnable()
    {
        hasSpawned = false;
        room.OnPlayerEnteredRoom += HandlePlayerEnteredRoom;
    }

    private void OnDisable()
    {
        room.OnPlayerEnteredRoom -= HandlePlayerEnteredRoom;
    }

    public void SetEnemy(string name)
    {
        enemyToSpawn = enemyDb.GetEnemyByName(name);
    }

    private void HandlePlayerEnteredRoom()
    {
        if (hasSpawned) return;
        hasSpawned = true;
        room.OnPlayerEnteredRoom -= HandlePlayerEnteredRoom;
        
        if(!enemyToSpawn) enemyToSpawn = enemyDb.GetEnemyByName(name);
        isSpawningEngaged = true;
        var playerRef = GameController.Instance.GetPlayerReference;
        Vector3 position;
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            position = hit.position;
        }
        else
        {
            position = transform.position;
        }
        spawnRandomize = UnityEngine.Random.Range(0f, maxSpawnTime);
        float delay = UnityEngine.Random.Range(0f, maxSpawnTime);
        SpawnAfterDelayAsync(position, delay).Forget();
    }

    private async UniTaskVoid SpawnAfterDelayAsync(Vector3 position, float delay)
    {
        try
        {
            await UniTask.Delay(
                TimeSpan.FromSeconds(delay),
                cancellationToken: this.GetCancellationTokenOnDestroy()
            );

            SpawnEnemy(position);
            Destroy(gameObject);
        }
        catch (OperationCanceledException)
        {
            // Spawner was destroyed before spawn — this is expected and safe
        }
    }
    
    private void SpawnEnemy(Vector3 position)
    {
        var enemy = Instantiate(enemyToSpawn, position, Quaternion.identity, room.transform);

        enemy.SetTarget(GameController.Instance.GetPlayerReference);

        room.SubscribeEnemyToRoom();
        enemy.onEntityDeath += room.HandleEnemyDeath;
        enemy.onEntityDeath += BloodManager.Instance.HealPlayer;
    }
}
