using System;
using ProjectFiles.Code.LevelGeneration;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyBase enemyToSpawn;
    [SerializeField] private GameObject target;

    private Room room;
    
    private void Start()
    {
        room = GetComponentInParent<Room>();
        room.OnPlayerEnteringRoom += Spawn;
        //Spawn();
    }
    
    private void Spawn()
    {
        var enemyInstance = Instantiate(enemyToSpawn);
        enemyInstance.transform.position = gameObject.transform.position;
        enemyInstance.SetTarget(target);
        Destroy(gameObject);
    }
}
