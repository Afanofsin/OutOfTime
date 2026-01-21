using System;
using UnityEngine;

namespace ProjectFiles.Code.LevelGeneration
{
    public class ZoneCollider : MonoBehaviour
    {
        private Room parentRoom;
        private bool isCleared = false;

        private void Awake()
        {
            parentRoom = GetComponentInParent<Room>(true);
        }

        private void Start()
        {
            parentRoom.OnRoomCleared += FlipCleared;
        }

        private void FlipCleared()
        {
            parentRoom.OnRoomCleared -= FlipCleared;
            isCleared = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !isCleared)
            {
                Debug.Log($"Player {other.name} is entering room");
                parentRoom.OnPlayerEnteringRoom();
            }
        }
    }
}