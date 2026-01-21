using System;
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
            DontDestroyOnLoad(gameObject);
            ControllerTransform = transform;
        }

        private void Start()
        {
            
        }

        private void InstantiatePlayer()
        {
            playerReference = Instantiate(playerPrefab, ControllerTransform.position, Quaternion.identity, ControllerTransform);
            playerReference.SetActive(false);
        }
    }
}