using System;
using ProjectFiles.Code.Controllers;
using ProjectFiles.Code.LevelGeneration;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyDatabase enemyDb;
    [SerializeField] private EnemyEntityBase enemyToSpawn;

    private Room room;
    
    private void Start()
    {
        room = GetComponentInParent<Room>();
        room.OnPlayerEnteredRoom += Spawn;
        //Spawn();
    }

    public void SetEnemy(string name)
    {
        enemyToSpawn = enemyDb.GetEnemyByName(name);
    }
    
    private void Spawn()
    {
        if(!enemyToSpawn) enemyToSpawn = enemyDb.GetEnemyByName(name);
        
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
        
        var enemyInstance = Instantiate(enemyToSpawn, position, Quaternion.identity, transform.parent);
        enemyInstance.SetTarget(GameController.Instance.GetPlayerReference);
        room.SubscribeEnemyToRoom();
        enemyInstance.onEntityDeath += room.HandleEnemyDeath;
        room.OnPlayerEnteredRoom -= Spawn;
        Destroy(gameObject);
    }
}
