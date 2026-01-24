using UnityEngine;
using System.Collections;

public class GravityManager : MonoBehaviour
{
    [Header("Références")]
    public Transform player;
    public PlayerMovementCustomKeys movement;

    [Header("Paramètres Smooth")]
    public float transitionDuration = 0.4f; // Temps pour finir la rotation (en secondes)
    public AnimationCurve rotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Pour un effet d'accélération/freinage

    [HideInInspector]
    public Vector3 gravityDirection = Vector3.down;

    private Coroutine rotationCoroutine;

    // Fonctions appelées par la roue
    public void SetGravityUp() => StartGravityTransition(Vector3.up);
    public void SetGravityDown() => StartGravityTransition(Vector3.down);
    public void SetGravityLeft() => StartGravityTransition(Vector3.left);
    public void SetGravityRight() => StartGravityTransition(Vector3.right);
    public void SetGravityForward() => StartGravityTransition(Vector3.forward);
    public void SetGravityBack() => StartGravityTransition(Vector3.back);

    void StartGravityTransition(Vector3 newGravity)
    {
        // On met à jour la direction de gravité pour le script de mouvement
        gravityDirection = newGravity.normalized;

        // Si une rotation est déjà en cours, on l'arrête pour lancer la nouvelle
        if (rotationCoroutine != null) StopCoroutine(rotationCoroutine);
        rotationCoroutine = StartCoroutine(SmoothRotationRoutine(newGravity));

        // On reset la vitesse pour éviter que le joueur s'envole pendant le virage
        if (movement != null) movement.velocity = Vector3.zero;
    }

    IEnumerator SmoothRotationRoutine(Vector3 newGravity)
    {
        Vector3 targetUp = -newGravity.normalized;
        Quaternion startRotation = player.rotation;

        // On calcule la rotation finale cible
        // On projette le forward actuel sur le nouveau plan pour garder le cap
        Vector3 forward = Vector3.ProjectOnPlane(player.forward, targetUp);
        if (forward.sqrMagnitude < 0.01f) forward = Vector3.ProjectOnPlane(player.right, targetUp);

        Quaternion targetRotation = Quaternion.LookRotation(forward, targetUp);

        float elapsed = 0;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.unscaledDeltaTime; // On utilise unscaled pour que ça marche pendant le ralenti de la roue
            float percent = elapsed / transitionDuration;

            // On utilise la courbe pour rendre le mouvement plus organique
            float curvePercent = rotationCurve.Evaluate(percent);

            player.rotation = Quaternion.Slerp(startRotation, targetRotation, curvePercent);
            yield return null;
        }

        player.rotation = targetRotation; // On s'assure d'être parfaitement aligné à la fin
    }
}