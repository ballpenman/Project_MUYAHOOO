using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerMove move;
    PlayerState state;
    void Start()
    {
        move = GetComponent<PlayerMove>();
        state = GetComponent<PlayerState>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveControl();
    }

    void MoveControl()
    {
        if (!state.IsAlive) return;
        move.Move();
        move.Jump();
    }
}
