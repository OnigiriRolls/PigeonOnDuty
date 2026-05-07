using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public float VerticalInput { get; private set; }

    void Update()
    {
        Vector2 move = new(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );
        MoveInput = move.normalized;

        VerticalInput = 0f;

        if (Input.GetKey(KeyCode.Space))
            VerticalInput = 1f;
        else if (Input.GetKey(KeyCode.LeftControl))
            VerticalInput = -1f;
    }
}
