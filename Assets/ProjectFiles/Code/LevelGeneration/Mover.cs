using Cysharp.Threading.Tasks;
using ProjectFiles.Code.LevelGeneration;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private GameObject death;
    [SerializeField] private Controller contoller;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        contoller.ReferencePlayer(this.gameObject);
    }
    
    void Update()
    {
        moveInput.x = 0f;
        moveInput.y = 0f;
        
        if (Keyboard.current.wKey.isPressed) moveInput.y = 1f;
        if (Keyboard.current.sKey.isPressed) moveInput.y = -1f;
        if (Keyboard.current.aKey.isPressed) moveInput.x = -1f;
        if (Keyboard.current.dKey.isPressed) moveInput.x = 1f;

        if (Keyboard.current.spaceKey.isPressed)
        {
            death.SetActive(true);
            Flip().Forget();
        }
    }

    private async UniTask Flip()
    {
        await UniTask.Delay(50);
        death.SetActive(false);
    }
    
    void FixedUpdate()
    {
        rb.linearVelocity = moveInput.normalized * moveSpeed;
    }
}
