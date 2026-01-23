using System;
using UnityEngine;

namespace ProjectFiles.Code.LevelGeneration
{
    public class NorthWall : MonoBehaviour
    {
        private Room parentRoom;

        private void Awake()
        {
            parentRoom = GetComponentInParent<Room>(true);
            foreach (var c in parentRoom.ConnectionPoints)
            {
                c.OnStateFinalized += DisableOnActionFire;
            }
        }

        private void DisableOnActionFire(Direction direction)
        {
            if (direction != Direction.North) return;
            this.gameObject.SetActive(false);
        }
    }
}