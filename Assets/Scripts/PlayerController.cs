using Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private LayerMask hitMask;
    
    public static Vector3 WorldMousePos => Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    private Rigidbody2D PlayerRb => gameObject.GetOrAddComponent<Rigidbody2D>();
    
    private void Update()
    {
        Move(moveInput);
        
    }
    
    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
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
        if (context.action.WasPressedThisFrame())
        {
            Attack(WorldMousePos);
        }
    }
    
    private void Move(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.01f)
        {
            PlayerRb.linearVelocity = direction * moveSpeed;
        }
        else
        {
            PlayerRb.linearVelocity = Vector2.zero;
        }
    }

    private void CheckInteraction()
    {
        var hit = Physics2D.OverlapCircle(transform.position, 1.5f, interactableMask);
        if (hit != null && hit.TryGetComponent<IInteractable>(out var component))
        {
            component.Interact(gameObject);
        }
    }
    
    private void Attack(Vector3 mousePos)
    {
        mousePos.z = 0;
        var direction = mousePos - transform.position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0)
        {
            angle += 360f;
        }
        gameObject.GetComponent<AttackHandler>().PerformAttack(angle);
    }
}
