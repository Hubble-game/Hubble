using UnityEngine;
using PLAYERTWO.PlatformerProject;

[RequireComponent(typeof(EntityController))]
public class InheritParentMovement : MonoBehaviour
{
    private EntityController entityController;

    private Rigidbody parentRigidbody = null;
    private bool isOnParent = false;

    private void Awake()
    {
        entityController = GetComponent<EntityController>();
    }

    private void Update()
    {
        if (isOnParent && parentRigidbody != null)
        {
            // Applique uniquement la vitesse horizontale (X/Z)
            Vector3 horizontalVelocity = parentRigidbody.velocity;
            horizontalVelocity.y = 0f;

            entityController.Move(horizontalVelocity * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            parentRigidbody = rb;
            isOnParent = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody == parentRigidbody)
        {
            isOnParent = false;
            parentRigidbody = null;
        }
    }
}
