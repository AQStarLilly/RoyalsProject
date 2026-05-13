using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // the tournment manager
    public GameObject tournementManagerObject;
    private TournementManager tournementManager;

    private PlayerControler playerControler;

    [Header("Scores")]
    public int team1Score = 0;
    public int team2Score = 0;

    //keeps the total wins
    public int totalWins = 0;

    [Header("UI")]
    public TMP_Text team1ScoreText;
    public TMP_Text team2ScoreText;
    public GameObject scoredBannerObject;
    public TMP_Text scoredBannerText;
    public float bannerDuration = 2f;

    [Header("Team Names")]
    public string team1Name = "Amherst Royals";
    public string team2Name = "Opposing Team";

    [Header("Win Cons")]
    public int scoreToWin = 5;
    public GameObject winBannerObject;
    public TMP_Text winBannerText;
    public float winBannerDuration = 3f;

    [Header("Reset Positions")]
    public Transform puckTransform;
    public Transform player1Transform;

    public Vector3 puckStartPosition;
    public Vector3 player1StartPosition;

    private Rigidbody puckRigidbody;
    private PuckBehaviour puckBehaviour;

    public List<Transform> allCPUs = new();
    private List<Vector3> cpuStartPositions = new();

    private bool scoringLocked = false;


    void Start()
    {
        // get the manager this was to see if thats why it's not working
        tournementManager = tournementManagerObject.GetComponent<TournementManager>();

        playerControler = player1Transform.GetComponent<PlayerControler>();

        puckStartPosition = puckTransform.position;
        player1StartPosition = player1Transform.position;
        foreach (Transform cpu in allCPUs)
        {
            cpuStartPositions.Add(cpu.position);
        }          

        puckRigidbody = puckTransform.GetComponent<Rigidbody>();
        puckBehaviour = puckTransform.GetComponent<PuckBehaviour>();

        scoredBannerObject.SetActive(false);
    }

    public void TeamScored(int teamNumber)
    {
        if (scoringLocked) return;
        scoringLocked = true;

        if (teamNumber == 1)
        {
            team1Score++;
            team1ScoreText.text = team1Score.ToString();
            scoredBannerText.text = team1Name + " Scored!";
        }
        else
        {
            team2Score++;
            team2ScoreText.text = team2Score.ToString();
            scoredBannerText.text = team2Name + " Scored!";
        }

        StartCoroutine(ShowBannerThenReset(teamNumber));    
    }

    private IEnumerator ShowBannerThenReset(int teamNumber)
    {
        scoredBannerObject.SetActive(true);
        yield return new WaitForSeconds(bannerDuration);
        scoredBannerObject.SetActive(false);

        if (team1Score >= scoreToWin || team2Score >= scoreToWin)
        {
            winBannerText.text = (teamNumber == 1 ? team1Name : team2Name) + " Win!";
            winBannerObject.SetActive(true);
            yield return new WaitForSeconds(winBannerDuration);
            winBannerObject.SetActive(false);


            //total wins and display right now it shows all messages so i need to fix it
            //TotalWins();
            
            if (team2Score >= scoreToWin)
            {
                winBannerText.text = team1Name + " is Eelinmated";
                winBannerObject.SetActive(true);
                yield return new WaitForSeconds(winBannerDuration);
                winBannerObject.SetActive(false);
                Debug.Log("team 2 wins");
            }
            else if (team1Score >= scoreToWin)
            {
                totalWins += 1;
                winBannerText.text = team1Name + " moves on to the next round  Wins: " + totalWins;
                winBannerObject.SetActive(true);
                yield return new WaitForSeconds(winBannerDuration);
                winBannerObject.SetActive(false);
                Debug.Log("team 1 wins");
            }

            if (totalWins == 3)
            {
                winBannerText.text = team1Name + " Wins the cup";
                winBannerObject.SetActive(true);
                yield return new WaitForSeconds(winBannerDuration);
                winBannerObject.SetActive(false);
                Debug.Log("next round");
            }
            //

            ResetScores();
        }

        ResetPositions();
    }

    private void ResetPositions()
    {
        // Reset puck 
        puckBehaviour.ForceDetach();
        puckRigidbody.linearVelocity = Vector3.zero;
        puckRigidbody.angularVelocity = Vector3.zero;
        puckTransform.position = puckStartPosition;

        // Reset players
        CharacterController charcon = player1Transform.GetComponent<CharacterController>();
        charcon.enabled = false;
        player1Transform.position = player1StartPosition;
        charcon.enabled = true;

        for (int i = 0; i < allCPUs.Count; i++)
        {
            CharacterController cpuCC = allCPUs[i].GetComponent<CharacterController>();
            cpuCC.enabled = false;
            allCPUs[i].position = cpuStartPositions[i];
            cpuCC.enabled = true;

            CPUController cpuCtrl = allCPUs[i].GetComponent<CPUController>();
            if (cpuCtrl != null) cpuCtrl.ResetMovement();
        }


        playerControler.ResetMovement();
        scoringLocked = false;
    }

    public void ResetScores()
    {
        team1Score = 0;
        team2Score = 0;
        team1ScoreText.text = "0";
        team2ScoreText.text = "0";
    }


    // the code to keep track of total wins

    //public void TotalWins()
    //{
    //    if (team2Score >= scoreToWin)
    //    {
    //        winBannerText.text = team1Name + " is Eelinmated";
    //        winBannerObject.SetActive(true);
    //        yield return new WaitForSeconds(winBannerDuration);
    //        winBannerObject.SetActive(false);
    //    }
    //    else if (team1Score >= scoreToWin)
    //    {
    //        totalWins++;
    //        winBannerText.text = team1Name + " moves on to the next round  Wins: " + tournementManager.CurrentWins;
    //        winBannerObject.SetActive(true);
    //        yield return new WaitForSeconds(winBannerDuration);
    //        winBannerObject.SetActive(false);
    //    }

    //    if(totalWins == 3)
    //    {
    //        winBannerText.text = team1Name + " Wins the cup";
    //        winBannerObject.SetActive(true);
    //        yield return new WaitForSeconds(winBannerDuration);
    //        winBannerObject.SetActive(false);
    //    }
        
    //}
}
