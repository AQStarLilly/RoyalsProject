using UnityEngine;

public class PuckBehaviour : MonoBehaviour
{
    [Header("Attachment Settings")]
    public float attachDistance = 1.0f;
    public float attachHeight = 0.1f;

    private Rigidbody rb;
    private Transform currentHolder = null;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void Update()
    {
        if (currentHolder != null)
        {
            Vector3 targetPosition = currentHolder.position
                + currentHolder.forward * attachDistance;
            targetPosition.y = attachHeight;
            transform.position = targetPosition;
        }
    }

    public void AttachToPlayer(Transform player)
    {
        if (currentHolder != null) return;
        currentHolder = player;
        rb.isKinematic = true;
    }

    // Shoot Mechanic
    public void DetachFromPlayer(Vector3 shootDirection, float force)
    {
        currentHolder = null;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezePositionY 
                       | RigidbodyConstraints.FreezeRotationX 
                       | RigidbodyConstraints.FreezeRotationZ;
        rb.AddForce(shootDirection * force, ForceMode.Impulse);
    }

    public void AttachToCPU(Transform cpu)
    {
        if (currentHolder != null) return;
        currentHolder = cpu;
        rb.isKinematic = true;
    }

    public bool IsAttached() => currentHolder != null;
    public bool IsAttachedToCPU() => currentHolder != null;

    public void DetachFromCPU(Vector3 shootDirection, float force)
    {
        currentHolder = null;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezePositionY
                       | RigidbodyConstraints.FreezeRotationX
                       | RigidbodyConstraints.FreezeRotationZ;

        rb.AddForce(shootDirection * force, ForceMode.Impulse);
    }

    public bool IsAttachedTo(Transform t) => currentHolder == t;

    public void ForceDetach()
    {
        currentHolder = null;
        rb.isKinematic = false;
    }
}
