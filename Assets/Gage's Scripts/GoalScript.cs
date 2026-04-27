using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoalScript : MonoBehaviour
{
    public TMP_Text currentScoreText;
    int currentScore = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider puck)
    {
        if (puck.CompareTag("Puck"))
        {
            Debug.Log("hi");
            currentScore += 1;

            currentScoreText.text = currentScore.ToString();
        }
        

    }
}
