using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CPUController : MonoBehaviour
{
    [Header("Team Setup")]
    public int teamID;
    public List<Transform> teammates = new();
    public List<Transform> opponents = new();

    [Header("References")]
    public Transform puckTransform;
    public Transform ownGoal;
    public Transform opposingGoal;

    [Header("Movement")]
    public float moveSpeed = 4.5f;
    public float rotationSpeed = 8f;

    [Header("Shooting")]
    public float shootRange = 6f;
    public float shootForce = 15f;

    [Header("Passing")]
    public float passRange = 8f;
    public float blockCheckRadius = 0.5f;

    [Header("Defending")]
    public float defendOffset = 2f;

    [Header("Seperation")]
    public float seperationRadius = 2f;
    public float seperationStrength = 2f;

    [Header("Positioning")]
    public int teamIndex = 0;

    private CharacterController controller;
    private PuckBehaviour puckBehaviour;
    private float gravity = -9.15f;
    private float verticalVelocity;

    private enum CPUState { Chasing, Attacking, Defending, Supporting };
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
        if (IsMyPuck())
        {
            currentState = CPUState.Attacking;
        }
        else if (TeammateHasPuck())
        {
            currentState = CPUState.Supporting;
        }
        else if (OpponentHasPuck())
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
                {
                    // Check if the path to goal is clear before shooting
                    if (IsPathClear(puckTransform.position, opposingGoal.position))
                        Shoot();
                    else
                        TryPass();
                }
                else
                {
                    MoveToward(opposingGoal.position);
                }
                break;

            case CPUState.Defending:
                Transform puckCarrier = GetOpponentWithPuck();
                if (puckCarrier != null)
                {
                    Vector3 interceptPoint = Vector3.Lerp(
                        puckCarrier.position,
                        ownGoal.position,
                        defendOffset
                    );
                    MoveToward(interceptPoint);
                }
                break;

            case CPUState.Supporting:
                // Move to an open position ahead of the puck for a potential pass
                MoveToward(GetSupportPosition());
                break;
        }
    }

    private void Shoot()
    {
        Vector3 shootDirection = (opposingGoal.position - puckTransform.position);
        shootDirection.y = 0f;
        shootDirection.Normalize();
        puckBehaviour.DetachFromCPU(shootDirection, shootForce);
    }

    private void TryPass()
    {
        Transform bestTeammate = GetBestTeammateToPassTo();

        if (bestTeammate != null)
        {
            Vector3 passDirection = (bestTeammate.position - puckTransform.position);
            passDirection.y = 0f;
            passDirection.Normalize();
            puckBehaviour.DetachFromCPU(passDirection, shootForce * 0.75f); // Slightly softer than a shot
        }
        else
        {
            // No good pass available, push forward anyway
            MoveToward(opposingGoal.position);
        }
    }

    // Find the teammate closest to the opposing goal who has a clear path
    private Transform GetBestTeammateToPassTo()
    {
        Transform best = null;
        float bestScore = float.MaxValue;

        foreach (Transform teammate in teammates)
        {
            float distToGoal = Vector3.Distance(teammate.position, opposingGoal.position);
            float distFromMe = Vector3.Distance(transform.position, teammate.position);

            // Only consider teammates within pass range with a clear path
            if (distFromMe <= passRange && IsPathClear(puckTransform.position, teammate.position))
            {
                // Lower score = closer to goal = better pass target
                if (distToGoal < bestScore)
                {
                    bestScore = distToGoal;
                    best = teammate;
                }
            }
        }

        return best;
    }

    // SphereCast to check if path between two points is clear of opponents
    private bool IsPathClear(Vector3 from, Vector3 to)
    {
        Vector3 direction = (to - from).normalized;
        float distance = Vector3.Distance(from, to);

        // Build a layermask that only hits opponents
        int opponentLayer = LayerMask.GetMask("Opponent");

        if (Physics.SphereCast(from, blockCheckRadius, direction, out RaycastHit hit, distance, opponentLayer))
        {
            return false; // Something is in the way
        }
        return true;
    }

    // Where a supporting player should move — ahead and to the side of the puck
    private Vector3 GetSupportPosition()
    {
        Vector3 toGoal = (opposingGoal.position - puckTransform.position).normalized;

        // Offset to the side so two teammates aren't stacked together
        Vector3 sideOffset = Vector3.Cross(toGoal, Vector3.up) * 3f;

        int myIndex = teamIndex;
        float[] offsets = { -1f, 0f, 1f };
        float side = offsets[Mathf.Clamp(myIndex, 0, offsets.Length - 1)];

        return puckTransform.position + toGoal * 3f + sideOffset * side;
    }

    private Vector3 GetSeperationForce()
    {
        Vector3 seperationForce = Vector3.zero;

        foreach (Transform teammate in teammates)
        {
            if (teammate == null) continue;

            float dist = Vector3.Distance(transform.position, teammate.position);

            if (dist < seperationRadius && dist > 0.01f)
            {
                Vector3 pushDirection = (transform.position - teammate.position).normalized;
                seperationForce += pushDirection * (seperationRadius - dist);
            }
        }

        return seperationForce;
    }

    // --- Movement ---
    private void MoveToward(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position);
        direction.y = 0f;

        Vector3 seperation = GetSeperationForce();
        Vector3 blendedDirection = (direction.normalized * moveSpeed) + (seperation * seperationStrength);
        blendedDirection.y = 0f;

        verticalVelocity += gravity * Time.deltaTime;
        Vector3 movement = direction.normalized * moveSpeed;
        movement.y = verticalVelocity;

        controller.Move(movement * Time.deltaTime);

        if (blendedDirection.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(blendedDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    // --- State Helpers ---

    private bool IsMyPuck() => puckBehaviour.IsAttachedTo(transform);

    private bool TeammateHasPuck() =>
        teammates.Any(t => puckBehaviour.IsAttachedTo(t));

    private bool OpponentHasPuck() =>
        opponents.Any(o => puckBehaviour.IsAttachedTo(o));

    private Transform GetOpponentWithPuck() =>
        opponents.FirstOrDefault(o => puckBehaviour.IsAttachedTo(o));

    // --- Pickup ---

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Puck"))
        {
            PuckBehaviour hitPuck = hit.gameObject.GetComponent<PuckBehaviour>();
            if (hitPuck != null)
                hitPuck.AttachToCPU(transform);
        }
    }

    public void ResetMovement()
    {
        verticalVelocity = 0f;
    }
}
