using System;
using ProjectFiles.Code.LevelGeneration;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyEntityBase enemyToSpawn;

    private Room room;
    
    private void Start()
    {
        room = GetComponentInParent<Room>();
        room.OnPlayerEnteredRoom += Spawn;
        //Spawn();
    }
    
    private void Spawn()
    {
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
        enemyInstance.SetTarget(Controller.Instance.player);
        room.SubscribeEnemyToRoom();
        enemyInstance.OnEntityDeath += room.HandleEnemyDeath;
        room.OnPlayerEnteredRoom -= Spawn;
        Destroy(gameObject);
    }
}
