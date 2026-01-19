using NaughtyAttributes;
using UnityEngine;

namespace ProjectFiles.Code.LevelGeneration
{
    public class Controller : MonoBehaviour
    {
        [Header("DEBUG")] 
        [SerializeField] private bool GenerateOnce = false;
        [SerializeField] LevelGenerator _generator = null;

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
    }
}