using System;
using ProjectFiles.Code.Events;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private bool isFollowing = false;
    [SerializeField]
    float m_cameraDivider =  5;

    [SerializeField] private float boundry = 2;
    [SerializeField] private float time = 1f;
    float m_cameraZaxis = -10;
    
    private Transform _playerPos;
    
    private float _camHeight;
    private float _camWidth;

    private int _currentX;
    private int _currentY;
    private bool playerExists;
    
    private Vector3 velocity = Vector3.zero;
    [SerializeField] private float minSmoothTime = 0.05f; // Fast when player is far
    [SerializeField] private float maxSmoothTime = 0.3f; // Slow when player is close
    [SerializeField] private float maxPlayerDistanceFromCamera = 6f;

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

        if (isFollowing) return;
        UpdateCell(true);
    }

    private void LateUpdate()
    {
        if(!playerExists) return;
        
        if (isFollowing)
        {
            FollowPlayer();
            return;
        }
        UpdateCell(false);
    }

    private void FollowPlayer()
    {
        var mousePos = PlayerController.WorldMousePos;
        Vector3 mousePos3d = new Vector3(mousePos.x, mousePos.y, m_cameraZaxis);

        Vector3 cameraTargetPosition;
        
        Vector3 playerPos3d = new Vector3(_playerPos.position.x, _playerPos.position.y, m_cameraZaxis);
        float distanceToPlayerSqr = (mousePos3d - playerPos3d).sqrMagnitude;
        
        if (distanceToPlayerSqr < boundry)
        {
            cameraTargetPosition = _playerPos.position;
        }
        else
        {
            cameraTargetPosition = (mousePos3d + (m_cameraDivider - 1) * _playerPos.position) / m_cameraDivider;
        }
        
        cameraTargetPosition.z = m_cameraZaxis;
        
        float currentDistance = Vector3.Distance(transform.position, playerPos3d);
        float dynamicSmoothTime = Mathf.Lerp(minSmoothTime, maxSmoothTime, 
            Mathf.InverseLerp(maxPlayerDistanceFromCamera, 0, currentDistance));
    
        transform.position = Vector3.SmoothDamp(transform.position, cameraTargetPosition, 
            ref velocity, dynamicSmoothTime);
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
    
    private void GainPlayerReference(GameObject Player)
    {
        var player = Player.transform;
        float zPos = this.transform.position.z;
        _playerPos = player;
        Vector3 moveTo = _playerPos.position;
        moveTo.z = zPos;
        this.transform.position = moveTo;
        playerExists = true;
        
    }
}
