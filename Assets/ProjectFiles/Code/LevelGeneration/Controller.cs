using System;
using Cysharp.Threading.Tasks;
using NavMeshPlus.Components;
using Sirenix.OdinInspector;   
using UnityEngine;

namespace ProjectFiles.Code.LevelGeneration
{
    public class Controller : MonoBehaviour
    {
        public static Controller Instance { get; private set; }
        
        [Header("DEBUG")] 
        [SerializeField] private bool GenerateOnce = false;
        [SerializeField] private LevelGenerator _generator = null;
        [SerializeField] private NavMeshSurface surface = null;
        [SerializeField] public GameObject player; 
        private Vector3 offset = new Vector3(0.5f, 0.5f, 0);

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private async void Start()
        {
            await UniTask.Delay(1000);
            await AsyncGenerate();
            Physics2D.SyncTransforms();

            Vector3 originalPos = _generator.transform.position;
            _generator.transform.position = originalPos + offset;
            
            await surface.BuildNavMeshAsync();
            
            _generator.transform.position = originalPos;
        }

        public void ReferencePlayer(GameObject pl) => player = pl;

        [Button]
        public void GenerateLevel()
        {
            _generator.GenerateLevel();
            if (GenerateOnce) return;
            
            if (!_generator.IsBossRoomGenerated &&
                _generator.SpecialRoomsGenerated != 2)
            {
                _generator.GenerateLevel();
            }
        }

        private async UniTask AsyncGenerate()
        {
            await UniTask.WaitForEndOfFrame();
            _generator.GenerateLevel();
            if (GenerateOnce) return;
            
            if (!_generator.IsBossRoomGenerated &&
                _generator.SpecialRoomsGenerated != 2)
            {
                _generator.GenerateLevel();
            }
        }
    }
}