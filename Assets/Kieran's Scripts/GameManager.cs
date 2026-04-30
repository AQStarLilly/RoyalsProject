using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private PlayerControler playerControler;

    [Header("Scores")]
    public int team1Score = 0;
    public int team2Score = 0;

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

    //Copy p1 code for later players/bots
    [Header("Reset Positions")]
    public Transform puckTransform;
    public Transform player1Transform;

    public Vector3 puckStartPosition;
    public Vector3 player1StartPosition;

    private Rigidbody puckRigidbody;
    private PuckBehaviour puckBehaviour;


    void Start()
    {
        playerControler = player1Transform.GetComponent<PlayerControler>();


        puckStartPosition = puckTransform.position;
        player1StartPosition = player1Transform.position;

        puckRigidbody = puckTransform.GetComponent<Rigidbody>();
        puckBehaviour = puckTransform.GetComponent<PuckBehaviour>();

        scoredBannerObject.SetActive(false);
    }

    public void TeamScored(int teamNumber)
    {
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

        playerControler.ResetMovement();
    }

    public void ResetScores()
    {
        team1Score = 0;
        team2Score = 0;
        team1ScoreText.text = "0";
        team2ScoreText.text = "0";
    }
}
