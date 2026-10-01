using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkSetup : NetworkBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AudioListener audioListener;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // Eigen speler: camera en listener aan
            if (playerCamera != null) playerCamera.enabled = true;
            if (audioListener != null) audioListener.enabled = true;
        }
        else
        {
            // Andere speler: zet ALLEEN het camera-renderelement uit, 
            // het GameObject blijft actief zodat de Gun zichtbaar blijft!
            if (playerCamera != null) playerCamera.enabled = false;
            if (audioListener != null) audioListener.enabled = false;
        }
    }
}