using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CameraController : MonoBehaviour
{
    private Transform _playerPos;
    
    private float _camHeight;
    private float _camWidth;

    private int _currentX;
    private int _currentY;

    private void Start()
    {
        var cam = GetComponent<Camera>();
        _playerPos = GameObject.FindGameObjectWithTag("Player").transform;

        _camHeight = cam.orthographicSize * 2;
        _camWidth = _camHeight * cam.aspect;
        
        UpdateCell(true);
    }

    private void LateUpdate()
    {
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
}
