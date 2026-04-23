using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerControler : MonoBehaviour
{
    float gravity = -9.15f;
    [SerializeField] Vector2 playerMovementInput = new Vector2 (0, 0);
    [SerializeField] float speed;
    float maxSpeed = 15f;
    float minSpeed = 5f;
    [SerializeField] float currentTime;
    bool timerActive = false;
    bool isSprintHeld = false;
    bool isMoveHeld = false;
    Vector3 movement;
    
    [SerializeField] Vector3 slideMovement;

    [SerializeField] CharacterController controller;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSprintHeld)
        {
            speed = Mathf.MoveTowards(speed, minSpeed, 2f * Time.deltaTime);
        }
        else if (isSprintHeld)
        {
            speed = Mathf.MoveTowards(speed, maxSpeed, 2f * Time.deltaTime);
        }
        if(timerActive)
        {
            currentTime -= Time.deltaTime;
            controller.Move(slideMovement * Time.deltaTime * speed);
            if (currentTime <= 0f)
            {
                slideMovement = new(0, 0, 0);
                timerActive = false;
                currentTime = 3f;

            }
        }

        
        controller.Move(movement * Time.deltaTime * speed);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        playerMovementInput = context.action.ReadValue<Vector2>();
        movement = new(playerMovementInput.x,0f,playerMovementInput.y);
        if(playerMovementInput.x != 0f || playerMovementInput.y != 0f)
        {
            slideMovement.x = playerMovementInput.x;
            slideMovement.z = playerMovementInput.y;
        }
        
        if (context.canceled)
        {
            
            timerActive = true;
        }
        

    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isSprintHeld = true;
        }
        Debug.Log(context.phase);
        if (context.canceled)
        {
            isSprintHeld = false;
        }
    }

    // Player interaction with puck
    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Puck"))
        {
            PuckBehaviour puck = hit.gameObject.GetComponent<PuckBehaviour>();
            if (puck != null)
            {
                puck.AttachToPlayer(transform);
            }
        }
    }
}
