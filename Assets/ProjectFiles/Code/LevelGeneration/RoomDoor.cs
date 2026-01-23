using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ProjectFiles.Code.LevelGeneration
{
    public class RoomDoor : MonoBehaviour
    {
        [field: SerializeField] public Direction Direction {get; private set;}
        [SerializeField] private GameObject OpenAnim;
        [SerializeField] private GameObject CloseAnim;
        private Room parentRoom;
        
        private void Awake()
        {
            parentRoom = GetComponentInParent<Room>();
        }

        private void Start()
        {
            parentRoom.Doors.Add(this);
        }

        public void SetDirection(string direction)
        {
            switch (direction)
            {
                case "North":
                    Direction = Direction.North;
                    break;
                case "South":
                    Direction = Direction.South;
                    var sprites = CloseAnim.GetComponentsInChildren<SpriteRenderer>(true);
                    foreach (var sprite in sprites )
                    {
                        sprite.sortingLayerName = "Entities";
                        sprite.sortingOrder = 100;
                    }
                    break;
                case "East":
                    Direction = Direction.East;
                    break;
                case "West":
                    Direction = Direction.West;
                    break;
                default:
                    Debug.LogWarning("Wrong Direction Name in imported door.");
                    break;
            }
        }

        public async UniTask Open()
        {
            CloseAnim?.SetActive(false);
            OpenAnim?.SetActive(true);
            await UniTask.Delay(1500);
            Destroy();
        }

        public void Close()
        {
            Debug.Log($"{Direction} closed door;");
            OpenAnim?.SetActive(false);
            CloseAnim?.SetActive(true);
        }

        private void Destroy() => DestroyImmediate(this.gameObject);
    }
}