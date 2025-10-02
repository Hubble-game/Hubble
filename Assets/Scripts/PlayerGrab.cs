using UnityEngine;

public class PlayerGrab : MonoBehaviour
{
    [Header("Grab Settings")]
    public float grabDistance = 2f;
    public KeyCode grabKey = KeyCode.E;
    public Transform grabPoint; // Un empty GameObject devant le perso
    public LayerMask grabbableLayer;

    private FixedJoint grabJoint;
    private Rigidbody grabbedRb;

    void Update()
    {
        if (Input.GetKeyDown(grabKey))
        {
            if (grabbedRb == null)
            {
                TryGrab();
            }
            else
            {
                Release();
            }
        }
    }

    void TryGrab()
    {
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * grabDistance, Color.red, 1f);
        if (Physics.Raycast(ray, out RaycastHit hit, grabDistance, grabbableLayer))
        {
            Rigidbody rb = hit.collider.attachedRigidbody;
            if (rb != null)
            {
                grabbedRb = rb;

                grabJoint = gameObject.AddComponent<FixedJoint>();
                grabJoint.connectedBody = grabbedRb;
                grabJoint.anchor = grabPoint.localPosition;
                grabJoint.autoConfigureConnectedAnchor = false;
                grabJoint.connectedAnchor = Vector3.zero;
                grabJoint.breakForce = 500f;
                grabJoint.breakTorque = 500f;

                Debug.Log("Grabbed " + rb.name);
            }
        }
    }

    void Release()
    {
        if (grabJoint != null)
        {
            Destroy(grabJoint);
        }

        grabbedRb = null;

        Debug.Log("Released object");
    }
}
