using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shoot : NetworkBehaviour
{
    public Camera playerCamera;
    public GameObject bulletHolePrefab;
    public float range = 100f;
    public int damageAmount = 20;

    void OnFire(InputValue fireValue)
    {
        // Alleen de eigenaar van de speler mag schieten
        if (!IsOwner || !fireValue.isPressed)
            return;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            Debug.Log("Ik raak: " + hit.collider.name);

            // Controleer of het object dat we raken een PlayerHealth component heeft
            PlayerHealth targetHealth = hit.collider.GetComponentInParent<PlayerHealth>();

            if (targetHealth != null)
            {
                // Roep de server aan om de schade te verwerken
                RequestDamageServerRpc(targetHealth.NetworkObjectId, damageAmount);
            }

            // Kogelgat visueel spawnen
            if (bulletHolePrefab != null)
            {
                GameObject bulletHole = Instantiate(
                    bulletHolePrefab,
                    hit.point + hit.normal * 0.01f,
                    Quaternion.LookRotation(hit.normal)
                );
                bulletHole.transform.SetParent(hit.transform);
            }
        }
    }

    // ServerRpc zorgt ervoor dat deze logica op de Server/Host draait
    [ServerRpc]
    private void RequestDamageServerRpc(ulong targetNetworkObjectId, int damage)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetNetworkObjectId, out NetworkObject targetObject))
        {
            PlayerHealth health = targetObject.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }
}