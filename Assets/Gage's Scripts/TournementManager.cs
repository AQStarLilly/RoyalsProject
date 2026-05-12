using UnityEngine;

public class TournementManager : MonoBehaviour
{
    private GameManager gameManager;
    
    public int CurrentWins = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //if(gameManager.team1Score >= 5)
        //{
        //    DisplayVictory();
        //    //gameManager.ResetScores();
        //    CurrentWins += 1;
        //}
        //else if(gameManager.team2Score >= 5)
        //{
        //    DisplayLoss();
        //    //gameManager.ResetScores();
        //}

        //if(CurrentWins == 2)
        //{
        //    DisplayChampions();
        //    //gameManager.ResetScores();
        //}
    }

    
    void DisplayChampions()
    {
        //Debug.Log("Champs");
    }
    void DisplayVictory()
    {
        //Debug.Log("Winner");
    }
    void DisplayLoss()
    {
        //Debug.Log("Loss");
    }
}
