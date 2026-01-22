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
    
    public static Vector2 WorldMousePos => Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    public static float WorldMouseAngle;
    private Rigidbody2D PlayerRb => gameObject.GetOrAddComponent<Rigidbody2D>();
    private bool IsMoving => _moveInput.sqrMagnitude != 0;
    private bool _isDashing;
    private bool _isAttacking;
    public IState CurrentState => stateMachine.GetState();
    private StateMachine stateMachine;
        
    private PlayerIdleState idleState;
    private PlayerMovingState movingState;
    private PlayerAttackingState attackingState;
    private PlayerInteractingState interactingState;
    private PlayerDashingState dashingState;

    public static PlayerController Instance; 
    
    private void InitializeStateMachine()
    {
        stateMachine = new StateMachine();
        idleState = new PlayerIdleState();
        movingState = new PlayerMovingState();
        attackingState = new PlayerAttackingState(()=> Mathf.Max(0.1f,player.HeldWeapon.AttackSpeed * player.PlayerStats.AttackSpeed / 100), player);
        interactingState = new PlayerInteractingState();
        dashingState = new PlayerDashingState(PlayerRb, ()=> _moveInput);
        
        stateMachine.AddTransition(idleState, movingState, new FuncPredicate(
            () => IsMoving
        ));
        stateMachine.AddTransition(movingState, idleState, new FuncPredicate(
            () => !IsMoving
        ));
        
        stateMachine.AddTransition(dashingState, idleState, new FuncPredicate(
            () => dashingState.IsComplete && !_isDashing && !IsMoving
        ));
        
        stateMachine.AddTransition(dashingState, movingState, new FuncPredicate(
            () => dashingState.IsComplete && !_isDashing && IsMoving
        ));
        
        stateMachine.AddTransition(movingState, dashingState, new FuncPredicate(
            () => !dashingState.IsComplete && _isDashing && IsMoving
        ));
        
        stateMachine.AddTransition(idleState, attackingState, new FuncPredicate(
            () => !attackingState.IsComplete && _isAttacking && !IsMoving
        ));
        
        stateMachine.AddTransition(attackingState, idleState, new FuncPredicate(
            () => attackingState.IsComplete && !_isAttacking && !IsMoving
        ));
        
        stateMachine.AddTransition(movingState, attackingState, new FuncPredicate(
            () => !attackingState.IsComplete && _isAttacking && IsMoving
        ));
        
        stateMachine.AddTransition(attackingState, movingState, new FuncPredicate(
            () => attackingState.IsComplete && !_isAttacking && IsMoving
        ));
        
        stateMachine.SetState(idleState);
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
        stateMachine.Update();
        Move(_moveInput);
        if (CurrentState != attackingState)
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
        if(CurrentState == dashingState) return;
        
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
        if(CurrentState == attackingState || CurrentState == dashingState) return;
        
        if (context.action.WasReleasedThisFrame())
        {
            
        }
    }
    
    public void Next(InputAction.CallbackContext context)
    {
        if(CurrentState == attackingState || CurrentState == dashingState) return;
        
        if (context.action.WasReleasedThisFrame())
        {
            
        }
    }
    
    private void Move(Vector3 direction)
    {
        if (CurrentState == dashingState) return;
        
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
        if (angle < 0f) angle += 360f;
        return angle;
    }

    public Quaternion GetQuaternion()
    {
        return Quaternion.Euler(0f, 0f, GetAngle() + 1);
    }
}
