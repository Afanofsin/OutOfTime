using System;
using ProjectFiles.Code.Events;
using UnityEngine;

namespace ProjectFiles.Code.Controllers
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }

        [SerializeField] private GameObject playerPrefab;
        
        private GameObject playerReference;
        private Transform ControllerTransform;
        
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            ControllerTransform = transform;
            InstantiatePlayer();
        }

        private void Start()
        {
            
        }
        
        public void GenerateLevel()
        {
            LevelGenerator.Instance.GenerateLevel();
            if (!LevelGenerator.Instance.IsBossRoomGenerated &&
                LevelGenerator.Instance.SpecialRoomsGenerated != 2)
            {
                LevelGenerator.Instance.GenerateLevel();
            }
        }

        private void InstantiatePlayer()
        {
            playerReference = Instantiate(playerPrefab, ControllerTransform.position, Quaternion.identity, ControllerTransform);
            playerReference.SetActive(false);
        }

        public void SpawnPlayer(Vector3 spawn)
        {
            if (playerReference == null)
            {
                Debug.Log($"SpawnPlayer called. playerReference is null: {playerReference == null}");
                InstantiatePlayer();
            }
            playerReference.transform.position = spawn;
            playerReference.SetActive(true);
            GameEvents.OnPlayerCreated?.Invoke(playerReference.transform);
        }
    }
}