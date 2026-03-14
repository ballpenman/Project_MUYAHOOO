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

    //Á¶°Çµé
    bool canPressJump = false;
    bool grounded = false;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        var playerMap = inputAsset.FindActionMap("Player");
        moveAction = playerMap.FindAction("Move");
        jumpAction = playerMap.FindAction("Jump");
    }
    private void FixedUpdate()
    {
        grounded = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, LayerMask.GetMask("Platform"));
    }
    public void Move()
    { 
        Vector2 _vector = moveAction.ReadValue<Vector2>() * Vector2.right;
        transform.Translate(_vector * moveForce * Time.deltaTime);
    }

    public void Jump()
    {
        if (grounded && canPressJump && jumpAction.ReadValue<float>() == 1) {
            rb.linearVelocityY = 0f;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            canPressJump = false;
        }else if(rb.linearVelocityY > 0 && jumpAction.ReadValue<float>() == 0)
        {
            rb.linearVelocityY = 0f;
            canPressJump = true;
        }
        else if (grounded && jumpAction.ReadValue<float>() == 0)
        {
            canPressJump = true;
        }
    }
}
