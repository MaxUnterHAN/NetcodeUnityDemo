using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkSetup : NetworkBehaviour
{
    [SerializeField] private GameObject cameraHolder; // Of de Camera direct

    public override void OnNetworkSpawn()
    {
        // Is dit het karakter van de speler op DEZE computer?
        if (IsOwner)
        {
            // Zet de camera AAN voor jezelf
            if (cameraHolder != null) cameraHolder.SetActive(true);
        }
        else
        {
            // Zet de camera UIT voor andere spelers op het netwerk
            if (cameraHolder != null) cameraHolder.SetActive(false);
        }
    }
}