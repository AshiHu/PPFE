using UnityEngine;

public class GravityManager : MonoBehaviour
{
    [Header("Références")]
    public Transform player;
    public Transform feetAnchor;
    public PlayerMovementCustomKeys movement;

    public Vector3 gravityDirection = Vector3.down;

    public void SetGravityUp() => ApplyGravity(Vector3.up);
    public void SetGravityDown() => ApplyGravity(Vector3.down);
    public void SetGravityLeft() => ApplyGravity(Vector3.left);
    public void SetGravityRight() => ApplyGravity(Vector3.right);

    void ApplyGravity(Vector3 newGravity)
    {
        gravityDirection = newGravity.normalized;
        Vector3 newUp = -gravityDirection;

        // Calcul forward stable
        Vector3 projectedForward = Vector3.ProjectOnPlane(player.forward, newUp);
        if (projectedForward.sqrMagnitude < 0.001f)
            projectedForward = Vector3.ProjectOnPlane(player.right, newUp);

        Quaternion targetRotation = Quaternion.LookRotation(projectedForward, newUp);

        // Rotation lissée (optionnel)
        player.rotation = Quaternion.Slerp(player.rotation, targetRotation, 1f);

        // Repositionner player pour que les pieds restent au FeetAnchor
        Vector3 feetPosition = feetAnchor.position;
        float halfHeight = player.GetComponent<CharacterController>().height / 2f;
        player.position = feetPosition + newUp * halfHeight;

        // Reset vitesse pour éviter bugs
        if (movement != null)
            movement.velocity = Vector3.zero;

        Debug.Log("Gravité changée : " + gravityDirection);
    }
}
