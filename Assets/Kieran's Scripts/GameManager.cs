using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Scores")]
    public int team1Score = 0;
    public int team2Score = 0;

    [Header("UI")]
    public TMP_Text team1ScoreText;
    public TMP_Text team2ScoreText;
    public GameObject scoredBannerObject;
    public TMP_Text scoredBannerText;
    public float bannerDuration = 2f;

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
            scoredBannerText.text = "Amherst Royals Scored!";
        }
        else
        {
            team2Score++;
            team2ScoreText.text = team2Score.ToString();
            scoredBannerText.text = "Opposing Team Scored!";
        }

        StartCoroutine(ShowBannerThenReset());    
    }

    private IEnumerator ShowBannerThenReset()
    {
        scoredBannerObject.SetActive(true);

        yield return new WaitForSeconds(bannerDuration);

        scoredBannerObject.SetActive(false);
        ResetPositions();
    }

    private void ResetPositions()
    {
        // Reset puck : detach first if it's being carried
        puckBehaviour.ForceDetach();
        puckRigidbody.linearVelocity = Vector3.zero;
        puckRigidbody.angularVelocity = Vector3.zero;
        puckTransform.position = puckStartPosition;

        // Reset players
        player1Transform.position = player1StartPosition;
    }

    public void ResetScores()
    {
        team1Score = 0;
        team2Score = 0;
        team1ScoreText.text = "0";
        team2ScoreText.text = "0";
    }
}
