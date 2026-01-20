using System;
using UnityEngine;

namespace ProjectFiles.Code.LevelGeneration
{
    public class ZoneCollider : MonoBehaviour
    {
        private Room parentRoom;

        private void Awake()
        {
            parentRoom = GetComponentInParent<Room>(true);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log($"Player {other.name} is entering room");
                parentRoom.OnPlayerEnteringRoom();
            }
        }
    }
}