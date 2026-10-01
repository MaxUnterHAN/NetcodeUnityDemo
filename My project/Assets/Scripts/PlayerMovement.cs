using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;

    private Vector2 moveInput;
    private Rigidbody rb;
    private PlayerInput playerInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
    }

    public override void OnNetworkSpawn()
    {
        // Alleen de eigenaar van dit karakter mag input verwerken
        if (!IsOwner)
        {
            if (playerInput != null) playerInput.enabled = false;
        }
        else
        {
            if (playerInput != null) playerInput.enabled = true;
        }
    }

    void OnMove(InputValue movementValue)
    {
        if (!IsOwner) return;
        moveInput = movementValue.Get<Vector2>();
    }

    public void OnJump(InputValue jumpValue)
    {
        if (!IsOwner) return;

        if (jumpValue.isPressed)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        // Alleen de eigenaar verwerkt de fysieke beweging
        if (!IsOwner) return;

        Vector3 movement = transform.forward * moveInput.y + transform.right * moveInput.x;

        rb.MovePosition(
            rb.position + movement * speed * Time.fixedDeltaTime
        );
    }
}