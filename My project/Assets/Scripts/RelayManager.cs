using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class NetworkRelayManager : MonoBehaviour
{
    [Header("Relay Instellingen")]
    [Tooltip("Typ hier de Join Code in wanneer je als Client wilt verbinden")]
    public string joinCodeInput = "";

    [Header("Status (Read-Only)")]
    public string generatedJoinCode = "";

    private async void Start()
    {
        // Initialiseer Unity Gaming Services en meld anoniem aan bij het opstarten
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Aangemeld bij Unity Services als Player ID: {AuthenticationService.Instance.PlayerId}");
        }
    }

    // ContextMenu zonder parameters (deze verschijnt nu wél in het menu!)
    [ContextMenu("Start Relay Host")]
    public void ContextMenuStartHost()
    {
        _ = StartHostRelay(4);
    }

    // ContextMenu voor Client
    [ContextMenu("Start Relay Client")]
    public void ContextMenuStartClient()
    {
        _ = StartClientRelay();
    }

    public async Task StartHostRelay(int maxPlayers = 4)
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            generatedJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            var relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
            transport.SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();
            Debug.Log($"Relay Host Gestart! De Join Code is: {generatedJoinCode}");
        }
        catch (RelayServiceException e)
        {
            Debug.LogError($"Fout bij het maken van Relay Host: {e}");
        }
    }

    public async Task StartClientRelay()
    {
        if (string.IsNullOrEmpty(joinCodeInput))
        {
            Debug.LogWarning("Vul eerst een Join Code in bij 'Join Code Input' in de Inspector!");
            return;
        }

        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCodeInput.Trim());

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            var relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
            transport.SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
            Debug.Log($"Succesvol verbonden met Relay Host via code: {joinCodeInput}");
        }
        catch (RelayServiceException e)
        {
            Debug.LogError($"Fout bij verbinden met Relay Client: {e}");
        }
    }
}