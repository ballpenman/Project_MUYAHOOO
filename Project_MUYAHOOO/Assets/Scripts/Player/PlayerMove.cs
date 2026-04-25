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
    bool isSlope = false;
    [SerializeField] int jumpCount;
    [SerializeField] int maxJumpCount;
    bool grounded;
    RaycastHit2D slopeHit;
    float platformAngle;

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
        grounded = Physics2D.Raycast(transform.position+Vector3.right * 0.45f, Vector2.down, 1.2f, LayerMask.GetMask("Platform"))
            || Physics2D.Raycast(transform.position + Vector3.left * 0.45f, Vector2.down, 1.2f, LayerMask.GetMask("Platform"));
        slopeHit = Physics2D.Raycast(transform.position + Vector3.down * 0.5f, Vector2.down, 1.75f, LayerMask.GetMask("Platform"));

        motion.SetBoolParameter("isGrounded", grounded);

        motion.SetBoolParameter("isFalling", rb.linearVelocityY <= 0 && !grounded);
    }
    public void Move()
    {
        Vector2 _vector = moveAction.ReadValue<Vector2>() * Vector2.right;
        int flipValue = motion.spriteRenderer.flipX ? -1 : 1;

        motion.SetBoolParameter("isMove", _vector.x != 0);
        if(_vector.x != 0)
            motion.SetFloatParameter("MoveVector", _vector.x);
        motion.Flip(_vector.x);
        //Debug.Log(Vector2.Angle(slopeHit.normal, Vector2.up) * Mathf.Sign(slopeHit.normal.x));
        isSlope = grounded && slopeHit.normal != Vector2.up;

        if (canPressJump && isSlope)
        {
            //경사로
            rb.gravityScale = 0;
            Vector2 slopeDir = Vector3.ProjectOnPlane(_vector, slopeHit.normal).normalized;
            rb.linearVelocity = slopeDir * moveForce;
        }
        else
        {
            //경사로를 벗어났을 때
            rb.gravityScale = initGravityScale;
            rb.linearVelocityX = _vector.x * moveForce;
        }

        motion.SetBoolParameter("isSlope", isSlope);
        platformAngle = Vector2.Angle(slopeHit.normal, Vector2.up) * Mathf.Sign(slopeHit.normal.x);

        motion.SetFloatParameter("PlatformAngle", platformAngle);
        if(_vector.x != 0)
            motion.RotateBody(-platformAngle);
        else
            motion.RotateBody(0);
    }

    public void Jump()
    {
        float jumpInput = jumpAction.ReadValue<float>();
        if (jumpCount>0 && canPressJump && jumpInput == 1) {
            jumpCount--;
            canPressJump = false;

            motion.SetTriggerParameter("Jump");

            rb.gravityScale = initGravityScale;
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);

        }else if(rb.linearVelocityY > 0 && jumpInput == 0)
        {
            rb.linearVelocityY = 0f;
        }

        if (rb.linearVelocityY <= 0 && jumpInput == 0)
        {
            canPressJump = true;
        }

        if (rb.linearVelocityY <= 0&& grounded)
        {
            jumpCount = maxJumpCount;
        }
        
        if(jumpCount == maxJumpCount && rb.linearVelocityY < 0 && !grounded)
        {
            StartCoroutine(CoyoteTime(0.1f));
        }

        motion.SetFloatParameter("JumpPower", rb.linearVelocityY * (isSlope ? 0 : 1));
    }

    IEnumerator CoyoteTime(float time)
    {
        yield return new WaitForSeconds(time);
        jumpCount= maxJumpCount-1;
    }
}
