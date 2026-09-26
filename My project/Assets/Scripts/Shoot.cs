using UnityEngine;
using UnityEngine.InputSystem;

public class Shoot : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject bulletHolePrefab;
    public float range = 100f;

    void OnFire(InputValue fireValue)
    {
        if (!fireValue.isPressed)
            return;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            Debug.Log("Ik raak: " + hit.collider.name);

            GameObject bulletHole = Instantiate(
                bulletHolePrefab,
                hit.point + hit.normal * 0.01f,
                Quaternion.LookRotation(hit.normal)
            );

            bulletHole.transform.SetParent(hit.transform);
        }
    }
}
