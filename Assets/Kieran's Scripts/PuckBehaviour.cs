using UnityEngine;

public class PuckBehaviour : MonoBehaviour
{
    [Header("Attachment Settings")]
    public float attachDistance = 1.0f;
    public float attachHeight = 0.1f;

    private Transform playerTransform;
    private Rigidbody rb;
    private bool isAttached = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void Update()
    {
        if (isAttached && playerTransform  != null)
        {
            Vector3 targetPosition = playerTransform.position + playerTransform.forward * attachDistance;

            targetPosition.y = attachHeight;
            transform.position = targetPosition;
        }
    }

    public void AttachToPlayer(Transform player)
    {
        if (isAttached) return;

        isAttached = true;
        playerTransform = player;
        rb.isKinematic = true;
    }

    // Shoot Mechanic
    public void DetachFromPlayer(Vector3 shootDirection, float force)
    {
        isAttached = false;
        playerTransform = null;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezePositionY 
            | RigidbodyConstraints.FreezeRotationX 
            | RigidbodyConstraints.FreezeRotationZ;
        rb.AddForce(shootDirection * force, ForceMode.Impulse);
    }

    public bool IsAttached()
    {
        return isAttached;
    }

    public void ForceDetach()
    {
        isAttached = false;
        playerTransform = null;
        rb.isKinematic = false;
    }
}
