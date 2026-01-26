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
    
    private void Start()
    {
        room = GetComponentInParent<Room>();
        room.OnPlayerEnteredRoom += Spawn;
        isSpawningEngaged = false;
        
    }

    public void SetEnemy(string name)
    {
        enemyToSpawn = enemyDb.GetEnemyByName(name);
    }

    private void Spawn()
    {
        room.OnPlayerEnteredRoom -= Spawn;
        
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
        AsyncSpawn(position).Forget();
        
    }

    private async UniTask AsyncSpawn(Vector3 position)
    {
        await UniTask.Delay(
            TimeSpan.FromSeconds(spawnRandomize),
            cancellationToken: this.GetCancellationTokenOnDestroy()
        );

        var enemy = Instantiate(enemyToSpawn, position, Quaternion.identity, transform.parent);
        enemy.SetTarget(GameController.Instance.GetPlayerReference);

        room.SubscribeEnemyToRoom();
        enemy.onEntityDeath += room.HandleEnemyDeath;
        enemy.onEntityDeath += BloodManager.Instance.HealPlayer;

        Destroy(gameObject);
    }
}
