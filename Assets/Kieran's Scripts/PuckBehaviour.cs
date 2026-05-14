using UnityEngine;

public class PuckBehaviour : MonoBehaviour
{
    [Header("Attachment Settings")]
    public float attachDistance = 1.0f;
    public float attachHeight = 0.1f;

    private Rigidbody rb;
    private Transform currentHolder = null;

    private int wallMask;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezePositionY
                       | RigidbodyConstraints.FreezeRotationX
                       | RigidbodyConstraints.FreezeRotationZ;

        wallMask = LayerMask.GetMask("Walls");
    }

    void Update()
    {
        if (currentHolder != null)
        {
            transform.position = GetClampedAttachPosition();
        }
    }

    private Vector3 GetClampedAttachPosition()
    {
        Vector3 origin = currentHolder.position;
        origin.y = attachHeight;

        Vector3 direction = currentHolder.forward;
        direction.y = 0f;
        direction.Normalize();

        if (Physics.SphereCast(origin, 0.3f, direction, out RaycastHit hit, attachDistance, wallMask))
        {
            float clampedDistance = Mathf.Max(0f, hit.distance - 0.15f);
            Vector3 clampedPos = origin + direction * clampedDistance;
            clampedPos.y = attachHeight;
            return clampedPos;
        }

        Vector3 targetPosition = origin + direction * attachDistance;
        targetPosition.y = attachHeight;
        return targetPosition;
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
