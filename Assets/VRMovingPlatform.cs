using UnityEngine;

public class VRMovingPlatform : MonoBehaviour
{
    private Vector3 lastPosition;  // Position précédente de la plateforme
    private Vector3 deltaMove;     // Déplacement depuis le dernier frame

    private void Start()
    {
        lastPosition = transform.position; // Initialisation
    }

    private void LateUpdate()
    {
        // Calcul du delta de déplacement
        deltaMove = transform.position - lastPosition;
        lastPosition = transform.position;
    }

    // Fonction publique pour que le joueur PC récupère le delta
    public Vector3 GetDeltaMove()
    {
        return deltaMove;
    }

    // Optional : pour détecter si le joueur PC est dessus via trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerPC"))
        {
            // Tu peux gérer ici un parentage si tu veux, ou juste détecter
            // Debug.Log("PlayerPC est sur la plateforme");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerPC"))
        {
            // Debug.Log("PlayerPC a quitté la plateforme");
        }
    }
}
