using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkSetup : NetworkBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AudioListener audioListener;

    private void Awake()
    {
        // Zoek de Camera automatisch op als hij niet handmatig is toegewezen
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>(true);
        }

        // Zoek de AudioListener automatisch op
        if (audioListener == null && playerCamera != null)
        {
            audioListener = playerCamera.GetComponent<AudioListener>();
        }
    }

    public override void OnNetworkSpawn()
    {
        if (playerCamera == null)
        {
            Debug.LogError("Geen Camera gevonden op het Player object!");
            return;
        }

        if (IsOwner)
        {
            // Zorg dat het GameObject van de camera AAN staat
            playerCamera.gameObject.SetActive(true);

            // Zet het Camera-component zelf AAN voor de eigenaar
            playerCamera.enabled = true;

            if (audioListener != null)
            {
                audioListener.enabled = true;
            }
        }
        else
        {
            // Voor andere spelers op het netwerk:
            // Laat het GameObject AAN staan (zodat Gun/visuele childs werken),
            // maar zet de Camera rendering en AudioListener UIT.
            playerCamera.enabled = false;

            if (audioListener != null)
            {
                audioListener.enabled = false;
            }
        }
    }
}