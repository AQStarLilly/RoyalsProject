using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerControler : MonoBehaviour
{
    float gravity = -9.15f;
    [SerializeField] Vector2 playerMovementInput = new Vector2 (0, 0);
    [SerializeField] float speed;
    Vector3 movement;

    [SerializeField] CharacterController controller;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        controller.Move(movement * Time.deltaTime * speed);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        playerMovementInput = context.action.ReadValue<Vector2>();
        movement = new(playerMovementInput.x,0f,playerMovementInput.y);
        
    }
}
