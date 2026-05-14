using System.Reflection;
using UnityEngine;
using StarterAssets;

public class PlayerRespawn : MonoBehaviour
{
    [Tooltip("Y position below which the player is considered fallen off")]
    public float killPlaneY = -15f;

    [Tooltip("Health lost on each fall (0 = no penalty)")]
    public float fallDamage = 25f;

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private CharacterController cc;
    private PlayerHealth health;

    // Access the private vertical velocity so it resets cleanly on teleport
    private static readonly FieldInfo VertVelField =
        typeof(ThirdPersonController).GetField("_verticalVelocity",
            BindingFlags.NonPublic | BindingFlags.Instance);

    private void Start()
    {
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
        cc     = GetComponent<CharacterController>();
        health = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (transform.position.y < killPlaneY)
            Respawn();
    }

    private void Respawn()
    {
        // Disable the CharacterController so setting position doesn't fight with collision
        cc.enabled = false;
        transform.SetPositionAndRotation(spawnPosition, spawnRotation);
        cc.enabled = true;

        // Zero out the stored vertical velocity so the controller doesn't keep falling
        var tpc = GetComponent<ThirdPersonController>();
        if (tpc != null) VertVelField?.SetValue(tpc, 0f);

        if (fallDamage > 0f)
            health?.TakeDamage(fallDamage);
    }
}
