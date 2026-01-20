using System;
using ProjectFiles.Code.LevelGeneration;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyEntityBase enemyToSpawn;
    [SerializeField] private GameObject target;

    private Room room;
    
    private void Start()
    {
        room = GetComponentInParent<Room>();
        room.OnPlayerEnteredRoom += Spawn;
        //Spawn();
    }
    
    private void Spawn()
    {
        var enemyInstance = Instantiate(enemyToSpawn);
        enemyInstance.transform.position = gameObject.transform.position;
        enemyInstance.SetTarget(target);
        room.SubscribeEnemyToRoom();
        enemyInstance.OnEntityDeath += room.HandleEnemyDeath;
        Destroy(gameObject);
    }
}
