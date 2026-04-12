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

    PlayerMotion motion;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        motion = GetComponent<PlayerMotion>();
        var playerMap = inputAsset.FindActionMap("Player");
        moveAction = playerMap.FindAction("Move");
        jumpAction = playerMap.FindAction("Jump");
    }
    private void FixedUpdate()
    {
        groundHit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, LayerMask.GetMask("Platform"));
        motion.SetBoolParameter("isGrounded", grounded);

        motion.SetBoolParameter("isFalling", rb.linearVelocityY < 0 && !grounded);
    }
    public void Move()
    {
        Vector2 _vector = moveAction.ReadValue<Vector2>() * Vector2.right;
        int flipValue = motion.spriteRenderer.flipX ? -1 : 1;

        motion.SetBoolParameter("isMove", _vector.x != 0);
        motion.Flip(_vector.x);
        Debug.Log(Vector2.Angle(groundHit.normal, Vector2.up) * flipValue);
        if (jumpAction.ReadValue<float>() == 0 && groundHit && groundHit.normal != Vector2.up)
        {
            //경사로
            motion.SetBoolParameter("isSlope", true);
            if (groundHit && _vector.x == 0)
                rb.gravityScale = 0;
            
            _vector = Vector3.ProjectOnPlane(_vector, groundHit.normal).normalized;

            motion.SetFloatParameter("PlatformAngle", Vector2.Angle(groundHit.normal, Vector2.up) * flipValue);
            rb.linearVelocityX = _vector.x * moveForce;
            rb.linearVelocityY = _vector.y * moveForce;
        }
        else
        {
            //경사로를 벗어났을 때a
            motion.SetBoolParameter("isSlope", false);
            motion.SetFloatParameter("PlatformAngle", 0);
            rb.gravityScale = initGravityScale;
            rb.linearVelocityX = _vector.x * moveForce;
        }
    }

    public void Jump()
    {
        if (jumpCount>0 && canPressJump && jumpAction.ReadValue<float>() == 1) {
            jumpCount--;
            motion.SetTriggerParameter("Jump");
            motion.PlayAnim("PlayerJump", -1, 0f);
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
