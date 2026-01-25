using System.Linq;
using ProjectFiles.Code.LevelGeneration;
using UnityEngine;

namespace ProjectFiles.Code.Other
{
    public class BoonSpawner : MonoBehaviour
    {
        private float modifier;
        
        private Room room;
    
        private void Start()
        {
            room = GetComponentInParent<Room>();
            room.OnPlayerEnteredRoom += SpawnBoon;
        }
        

        public void SetLoot(string level)
        {
            switch (level)
            {
                case "First":
                    modifier = 1f;
                    break;
                case "Second":
                    modifier = 1.2f;
                    break;
                case "Third":
                    modifier = 1.4f;
                    break;
                default:
                    modifier = 1f;
                    break;
            }   
            
        }

        private void SpawnBoon()
        {
            var rand = UnityEngine.Random.Range(0f, 1f);
            RarityType rarity = BoonDatabase.Instance.rarityWeight.First(kvp => kvp.Value <= rand).Key;
            BoonBase boon = BoonDatabase.Instance.GetBoonByRarity(rarity);
            
            Instantiate(boon, this.transform.position, Quaternion.identity, transform.parent);
            room.OnPlayerEnteredRoom -= SpawnBoon;
        }
    }
}