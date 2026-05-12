using UnityEngine;

public class TEstadvance : MonoBehaviour
{
    int currentRound = 1;
    bool win = false;
    public GameObject roundOne;
    public GameObject roundTwo;
    public GameObject roundThree;
    public GameObject Champions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnAdvance()
    {
        win = true;

        if (win && currentRound == 1)
        {
            roundOne.SetActive(false); roundTwo.SetActive(true);
            currentRound += 1;
            win = false;
        }
        else if (win && currentRound == 2)
        {
            roundTwo.SetActive(false); roundThree.SetActive(true);
            currentRound += 1;
            win = false;
        }
        else if (win && currentRound == 3)
        {
            roundThree.SetActive(false); Champions.SetActive(true);
            currentRound += 1;
            win = false;
        }

    }
    public void Reset()
    {
        currentRound = 1;
        roundOne.SetActive(true); Champions.SetActive(false);
    }

}
