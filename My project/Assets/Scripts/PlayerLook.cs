using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : NetworkBehaviour
{
    public float sensitivity = 0.1f;
    public Transform cameraTransform;

    private Vector2 lookInput;
    private float xRotation = 0f;

    private NetworkVariable<float> netXRotation = new NetworkVariable<float>(
        0f, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Owner
    );

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void OnLook(InputValue lookValue)
    {
        if (!IsOwner) return;
        lookInput = lookValue.Get<Vector2>();
    }

    void Update()
    {
        if (IsOwner)
        {
            float mouseX = lookInput.x * sensitivity;
            float mouseY = lookInput.y * sensitivity;

            // Links/rechts draaien
            transform.Rotate(Vector3.up * mouseX);

            // Boven/beneden draaien
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            // Update netwerkvariabele
            netXRotation.Value = xRotation;

            if (cameraTransform != null)
            {
                cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            }

            // BELANGRIJK: Reset de input zodat hij niet blijft doordraaien
            lookInput = Vector2.zero;
        }
        else
        {
            if (cameraTransform != null)
            {
                cameraTransform.localRotation = Quaternion.Euler(netXRotation.Value, 0f, 0f);
            }
        }
    }
}