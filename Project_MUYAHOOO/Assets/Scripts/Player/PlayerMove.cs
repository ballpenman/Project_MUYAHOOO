using System.Collections;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float moveForce = 1f;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] InputActionAsset inputAsset;
    InputAction moveAction, jumpAction;

    float initGravityScale = 2f;

    //조건들
    bool canPressJump = false;
    int jumpCount;
    [SerializeField] int maxJumpCount;
    bool grounded => groundHit;
    RaycastHit2D groundHit;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        var playerMap = inputAsset.FindActionMap("Player");
        moveAction = playerMap.FindAction("Move");
        jumpAction = playerMap.FindAction("Jump");
    }
    private void FixedUpdate()
    {
        groundHit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, LayerMask.GetMask("Platform"));
    }
    public void Move()
    {
        Vector2 _vector = moveAction.ReadValue<Vector2>() * Vector2.right;
        
        if (jumpAction.ReadValue<float>() == 0 && groundHit && groundHit.normal != Vector2.up)
        {
            //경사로
            if (_vector.x == 0)
                rb.gravityScale = 0;
                _vector = Vector3.ProjectOnPlane(_vector, groundHit.normal).normalized;
            rb.linearVelocityX = _vector.x * moveForce;
            rb.linearVelocityY = _vector.y * moveForce;
        }
        else
        {
            //경사로를 벗어났을 때
            rb.gravityScale = initGravityScale;
            rb.linearVelocityX = _vector.x * moveForce;
        }
    }

    public void Jump()
    {
        if (jumpCount>0 && canPressJump && jumpAction.ReadValue<float>() == 1) {
            jumpCount--;
            rb.linearVelocityY = 0f;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            canPressJump = false;
        }else if(rb.linearVelocityY > 0 && jumpAction.ReadValue<float>() == 0)
        {
            rb.linearVelocityY = 0f;
        }

        if (jumpAction.ReadValue<float>() == 0)
        {
            canPressJump = true;
        }

        if (rb.linearVelocityY == 0&& grounded)
        {
            jumpCount = maxJumpCount;
        }
        
        if(jumpCount == maxJumpCount && rb.linearVelocityY < 0 && !grounded)
        {
            StartCoroutine(CoyoteTime(0.1f));
        }
    }

    IEnumerator CoyoteTime(float time)
    {
        yield return new WaitForSeconds(time);
        jumpCount= maxJumpCount-1;
    }
}
