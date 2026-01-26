using System;
using FSM;
using FSM.PlayerStates;
using ProjectFiles.Code.Events;
using UnityEngine;

public class BloodManager : MonoBehaviour
{
    public static BloodManager Instance;
    private PlayerController PlayerController => PlayerController.Instance;
    
    private float _tickAmount;
    private const float TickCooldown = 1f;
    private float _elapsedTime;
    private bool isInitialized;

    private void OnEnable()
    {
        GameEvents.OnPlayerCreated += TurnOn;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
        
        _elapsedTime = 0;
        isInitialized = false;
    }

    private void Update()
    {
        if (!isInitialized) return;
        _elapsedTime += Time.deltaTime;
        
        if (PlayerController.CurrentState is PlayerIdleState)
        {
            _tickAmount = 1.5f;
        }
        else if (PlayerController.CurrentState is not PlayerIdleState)
        {
            _tickAmount = 3f;
        }
        
        if (_elapsedTime >= TickCooldown)
        {
            PlayerController.Instance.CurrentPlayer.TickSubtract(_tickAmount);
            _elapsedTime = 0;
        }
    }

    public void HealPlayer() => PlayerController.Instance.CurrentPlayer.TickAdd(25);
    public void TurnOn(GameObject playerReference) => isInitialized = true;

    private void OnDisable()
    {
        GameEvents.OnPlayerCreated -= TurnOn;
    }
}
