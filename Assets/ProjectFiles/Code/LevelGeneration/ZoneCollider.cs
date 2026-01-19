using System;
using UnityEngine;

namespace ProjectFiles.Code.LevelGeneration
{
    public class ZoneCollider : MonoBehaviour
    {
        private Room parentRoom;

        private void Awake()
        {
            parentRoom = this.GetComponentInParent<Room>();
            
        }
    }
}