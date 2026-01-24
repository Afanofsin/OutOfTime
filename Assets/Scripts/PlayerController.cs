using System;
using FSM;
using FSM.PlayerStates;
using Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float moveSpeed;
    private Vector2 _moveInput;
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private Player player;
    
    public static Vector2 WorldMousePos => Camera.main!.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    private Rigidbody2D PlayerRb => gameObject.GetOrAddComponent<Rigidbody2D>();
    private bool IsMoving => _moveInput.sqrMagnitude != 0;
    private bool _isDashing;
    private bool _isAttacking;
    public IState CurrentState => _stateMachine.GetState();
    private StateMachine _stateMachine;
        
    private PlayerIdleState _idleState;
    private PlayerMovingState _movingState;
    private PlayerAttackingState _attackingState;
    private PlayerInteractingState _interactingState;
    private PlayerDashingState _dashingState;

    public static PlayerController Instance; 
    
    private void InitializeStateMachine()
    {
        _stateMachine = new StateMachine();
        _idleState = new PlayerIdleState();
        _movingState = new PlayerMovingState();
        _attackingState = new PlayerAttackingState(()=> Mathf.Max(0.1f,player.HeldWeapon.AttackSpeed * player.PlayerStats.AttackSpeed / 100), player);
        _interactingState = new PlayerInteractingState();
        _dashingState = new PlayerDashingState(PlayerRb, ()=> _moveInput);
        
        _stateMachine.AddTransition(_idleState, _movingState, new FuncPredicate(
            () => IsMoving
        ));
        _stateMachine.AddTransition(_movingState, _idleState, new FuncPredicate(
            () => !IsMoving
        ));
        
        _stateMachine.AddTransition(_dashingState, _idleState, new FuncPredicate(
            () => _dashingState.IsComplete && !_isDashing && !IsMoving
        ));
        
        _stateMachine.AddTransition(_dashingState, _movingState, new FuncPredicate(
            () => _dashingState.IsComplete && !_isDashing && IsMoving
        ));
        
        _stateMachine.AddTransition(_movingState, _dashingState, new FuncPredicate(
            () => !_dashingState.IsComplete && _isDashing && IsMoving
        ));
        
        _stateMachine.AddTransition(_idleState, _attackingState, new FuncPredicate(
            () => !_attackingState.IsComplete && _isAttacking && !IsMoving
        ));
        
        _stateMachine.AddTransition(_attackingState, _idleState, new FuncPredicate(
            () => _attackingState.IsComplete && !_isAttacking && !IsMoving
        ));
        
        _stateMachine.AddTransition(_movingState, _attackingState, new FuncPredicate(
            () => !_attackingState.IsComplete && _isAttacking && IsMoving
        ));
        
        _stateMachine.AddTransition(_attackingState, _movingState, new FuncPredicate(
            () => _attackingState.IsComplete && !_isAttacking && IsMoving
        ));
        
        _stateMachine.SetState(_idleState);
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        
        Instance = this;
        
        InitializeStateMachine();
    }

    private void Start()
    {
        moveSpeed = player.PlayerStats.Speed;
    }
    
    private void Update()
    {
        _stateMachine.Update();
        Move(_moveInput);
        if (CurrentState != _attackingState && player.HeldWeapon != null)
        {
            player.HeldWeapon.transform.localRotation =
                Quaternion.Slerp(
                    player.HeldWeapon.transform.localRotation,
                    Quaternion.Euler(0f, 0f, GetAngle()),
                    8f * Time.deltaTime
                );
        }
    }

    private void LateUpdate()
    {
        moveSpeed = player.PlayerStats.Speed;
    }


    public void Move(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.action.WasPressedThisFrame())
        {
            _isDashing = true;
        }
        
        if (context.action.WasReleasedThisFrame())
        {
            _isDashing = false;
        }
    }
    
    public void Interact(InputAction.CallbackContext context)
    {
        if (context.action.WasPressedThisFrame())
        {
            CheckInteraction();
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if(CurrentState == _dashingState) return;
        
        if (context.action.WasPressedThisFrame())
        {
            _isAttacking = true;
        }
        
        if (context.action.WasReleasedThisFrame())
        {
            _isAttacking = false;
        }
    }

    public void Previous(InputAction.CallbackContext context)
    {
        if(CurrentState == _attackingState || CurrentState == _dashingState) return;
        
        if (context.action.WasReleasedThisFrame())
        {
            player.Previous();
        }
    }
    
    public void Next(InputAction.CallbackContext context)
    {
        if(CurrentState == _attackingState || CurrentState == _dashingState) return;
        
        if (context.action.WasReleasedThisFrame())
        {
            player.Next();
        }
    }

    public void Drop(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            player.Drop();
        }
    }
    
    private void Move(Vector3 direction)
    {
        if (CurrentState == _dashingState) return;
        
        if (direction.sqrMagnitude > 0.01f)
        {
            PlayerRb.linearVelocity = direction * moveSpeed;
        }
        else
        {
            PlayerRb.linearVelocity = Vector3.zero;
        }
    }

    private void CheckInteraction()
    {
        var hit = Physics2D.OverlapCircle(transform.position, 1.5f, interactableMask);
        if (hit != null)
        {
            if (hit.TryGetComponent<IInteractable>(out var interactable))
            {
                interactable.Interact(gameObject);
            }

            if (CurrentState == _attackingState) return;
            
            if (hit.TryGetComponent<IPickable>(out var pickable))
            {
                player.PickUp(pickable);
            }
        }
    }
    
    public float GetAngle()
    {
        Vector2 direction = WorldMousePos - (Vector2)transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return angle < 0 ? angle + 360f : angle;
    }

    public Quaternion GetQuaternion()
    {
        return Quaternion.Euler(0f, 0f, GetAngle());
    }

    public Vector3 GetPlayerPos()
    {
        return player.transform.position;
    }
}
