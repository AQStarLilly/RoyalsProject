using UnityEngine;
using UnityEngine.InputSystem;

public class GoalScript : MonoBehaviour
{
    [Header("Which net scored on")]
    public int teamScoredOn = 1;

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Puck"))
        {
            int scoringTeam = teamScoredOn == 1 ? 2 : 1;
            gameManager.TeamScored(scoringTeam);          
        }
    }

    public void OnResetTest(InputAction.CallbackContext context)
    {
        gameManager.ResetScores();
    }
}
