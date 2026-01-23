using System;
using ProjectFiles.Code.Events;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CameraController : MonoBehaviour
{
    private Transform _playerPos;
    
    private float _camHeight;
    private float _camWidth;

    private int _currentX;
    private int _currentY;
    private bool playerExists;

    private void OnEnable()
    {
        playerExists = false;
        GameEvents.OnPlayerCreated += GainPlayerReference;
    }
        
    private void OnDisable()
    {
        GameEvents.OnPlayerCreated -= GainPlayerReference;
    }
    
    private void Start()
    {
        var cam = GetComponent<Camera>();
        _playerPos = GameObject.FindGameObjectWithTag("Player")?.transform;
        if(_playerPos != null) playerExists = true;
        _camHeight = cam.orthographicSize * 2;
        _camWidth = _camHeight * cam.aspect;
        
        UpdateCell(true);
    }

    private void LateUpdate()
    {
        if(!playerExists) return;
        UpdateCell(false);
    }

    private void UpdateCell(bool force)
    {
        var cellX = Mathf.FloorToInt(_playerPos.position.x / _camWidth);
        var cellY = Mathf.FloorToInt(_playerPos.position.y / _camHeight);
        
        if (!force && cellX == _currentX && cellY == _currentY) return;

        _currentX = cellX;
        _currentY = cellY;

        var targetPos = new Vector3(cellX * _camWidth + _camWidth * 0.5f, cellY * _camHeight + _camHeight * 0.5f,
            transform.position.z);

        transform.position = targetPos;
    }
    
    private void GainPlayerReference(Transform player)
    {
        float zPos = this.transform.position.z;
        _playerPos = player;
        Vector3 moveTo = _playerPos.position;
        moveTo.z = zPos;
        this.transform.position = moveTo;
        playerExists = true;
        
    }
}
