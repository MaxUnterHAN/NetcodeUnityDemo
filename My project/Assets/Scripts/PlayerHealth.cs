using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{
    public int maxHealth = 100;

    [Header("UI Instellingen")]
    public Slider healthSlider;

    public NetworkVariable<int> currentHealth = new NetworkVariable<int>(
        100,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        // Sla de beginpositie op
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;

        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth.Value;
        }

        currentHealth.OnValueChanged += OnHealthChanged;
    }

    public override void OnNetworkDespawn()
    {
        currentHealth.OnValueChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int previousValue, int newValue)
    {
        if (healthSlider != null)
        {
            healthSlider.value = newValue;
        }

        if (newValue <= 0 && IsServer)
        {
            Respawn();
        }
    }

    public void TakeDamage(int damage)
    {
        if (!IsServer) return;

        currentHealth.Value -= damage;

        if (currentHealth.Value <= 0)
        {
            currentHealth.Value = 0;
            Respawn();
        }
    }

    private void Respawn()
    {
        if (!IsServer) return;

        // 1. Reset HP op de server
        currentHealth.Value = maxHealth;

        // 2. Geef aan álle clients (inclusief de eigenaar) de opdracht om de positie te resetten
        RespawnClientRpc(spawnPosition, spawnRotation);
    }

    // Deze functie wordt door de server aangeroepen, maar uitgevoerd op de specifieke client
    [ClientRpc]
    private void RespawnClientRpc(Vector3 targetPosition, Quaternion targetRotation)
    {
        // Zet de fysica stil
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Verplaats het object op de client
        transform.position = targetPosition;
        transform.rotation = targetRotation;

        Debug.Log($"{gameObject.name} is lokaal gerespawned op: {targetPosition}");
    }
}