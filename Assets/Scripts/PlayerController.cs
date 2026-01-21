using Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float moveSpeed;
    private Vector2 _moveInput;
    [SerializeField] private LayerMask interactableMask;
    
    public static Vector2 WorldMousePos => Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    private Rigidbody2D PlayerRb => gameObject.GetOrAddComponent<Rigidbody2D>();
    
    private void Update()
    {
        Move(_moveInput);
    }
    
    public void Move(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.action.WasPressedThisFrame())
        {
            Dash();
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
            PlayerRb.linearVelocity = Vector3.zero;
        }
    }

    private void CheckInteraction()
    {
        var hit = Physics2D.OverlapCircle(transform.position, 1.5f, interactableMask);
        if (hit != null)
        {
            if (hit.TryGetComponent<IInteractable>(out var component))
            {
                component.Interact(gameObject);
            }
            if (hit.TryGetComponent<IPickable>(out var pickable))
            {
                Debug.Log("Fire");
                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().PickUp(pickable);
            }
        }
    }

    private void Dash()
    {
        //Dash logic here
    }
    
    private void Attack(Vector3 mousePos)
    {
        var direction = mousePos - transform.position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0)
        {
            angle += 360f;
        }
        GameObject.FindGameObjectWithTag("Player").gameObject.GetComponent<Player>().Attack(angle);
    }
}
