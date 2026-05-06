using Unity.VisualScripting;
using UnityEngine;

public class CPUController : MonoBehaviour
{
    [Header("References")]
    public Transform puckTransform;
    public Transform playerTransform;
    public Transform ownGoal;
    public Transform opposingGoal;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 8f;

    [Header("Shooting")]
    public float shootRange = 6f;
    public float shootForce = 15f;

    [Header("Defending")]
    public float defendOffset = 2f;

    private CharacterController controller;
    private PuckBehaviour puckBehaviour;
    private float gravity = -9.15f;
    private float verticalVelocity;

    private enum CPUState { Chasing, Attacking, Defending };
    private CPUState currentState;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        puckBehaviour = puckTransform.GetComponent<PuckBehaviour>();
    }

    void Update()
    {
        UpdateState();
        ExecuteState();
    }

    // Used to decide which state the cpu is in
    private void UpdateState()
    {
        if (puckBehaviour.IsAttachedToCPU())
        {
            currentState = CPUState.Attacking;
        }
        else if (puckBehaviour.IsAttached())
        {
            currentState = CPUState.Defending;
        }
        else
        {
            currentState = CPUState.Chasing;
        }
    }

    // Runs Logic for current state
    private void ExecuteState()
    {
        switch (currentState)
        {
            case CPUState.Chasing:
                MoveToward(puckTransform.position);
                break;

            case CPUState.Attacking:
                float distToGoal = Vector3.Distance(transform.position, opposingGoal.position);
                if (distToGoal <= shootRange)
                    Shoot();
                else
                    MoveToward(opposingGoal.position);
                break;

            case CPUState.Defending:
                // Move to a point between the player and the CPU's own goal
                Vector3 interceptPoint = Vector3.Lerp(
                    playerTransform.position,
                    ownGoal.position,
                    0.4f    // 0 = on top of player, 1 = at own goal
                );
                MoveToward(interceptPoint);
                break;
        }
    }

    private void MoveToward(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position);
        direction.y = 0f;

        verticalVelocity += gravity * Time.deltaTime;
        Vector3 movement = direction.normalized * moveSpeed;
        movement.y = verticalVelocity;

        controller.Move(movement * Time.deltaTime);

        if (direction.sqrMagnitude  > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
                );
        }
    }

    private void Shoot()
    {
        Vector3 shootDirection = (opposingGoal.position - puckTransform.position);
        shootDirection.y = 0f;
        shootDirection.Normalize();
        puckBehaviour.DetachFromCPU(shootDirection, shootForce);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Puck"))
        {
            PuckBehaviour hitPuck = hit.gameObject.GetComponent<PuckBehaviour>();
            if (hitPuck != null)
            {
                hitPuck.AttachToCPU(transform);
            }
        }
    }

    public void ResetMovement()
    {
        verticalVelocity = 0f;
    }
}
