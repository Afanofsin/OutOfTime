using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyEntityBase enemyToSpawn;
    [SerializeField] private GameObject target;

    private void Start()
    {
        Spawn();
    }
    
    private void Spawn()
    {
        var enemyInstance = Instantiate(enemyToSpawn);
        enemyInstance.transform.position = gameObject.transform.position;
        enemyInstance.SetTarget(target);
        Destroy(gameObject);
    }
}
