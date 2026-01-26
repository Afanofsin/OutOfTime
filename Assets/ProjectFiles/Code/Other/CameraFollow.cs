using System;
using ProjectFiles.Code.Events;
using UnityEngine;

namespace ProjectFiles.Code.Other
{
    public class CameraFollow : MonoBehaviour
    {
        
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float zAxis;
        [Range(2, 100)]
        [SerializeField]
        float m_cameraDivider;
        private bool playerExists;
        private Transform playerTransform;
        private void OnEnable()
        {
            playerExists = false;
            GameEvents.OnPlayerCreated += GainPlayerReference;
        }
        
        private void OnDisable()
        {
            GameEvents.OnPlayerCreated -= GainPlayerReference;
        }

        private void GainPlayerReference(GameObject Player)
        {
            var player = Player.transform;
            playerExists = true;
            playerTransform = player;
        }

        private void Update()
        {
            if (!playerExists) return;
            
            //var mousePosition = mainCamera.ScreenToWorldPoint();
            //var cameraTargetPosition = (mousePosition + (m_cameraDivider - 1) * playerTransform.position) / m_cameraDivider;
            //cameraTargetPosition.z = zAxis;
            //transform.position = cameraTargetPosition;
        }
    }
}