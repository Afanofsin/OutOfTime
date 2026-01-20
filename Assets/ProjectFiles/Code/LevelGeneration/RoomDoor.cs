using System;
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

        public void Open()
        {
            CloseAnim?.SetActive(false);
            OpenAnim?.SetActive(true);
        }

        public void Close()
        {
            Debug.Log($"{Direction} closed door;");
            OpenAnim?.SetActive(false);
            CloseAnim?.SetActive(true);
        }
    }
}