using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHighscores : MonoBehaviour
{
    public TMPro.TextMeshProUGUI[] rNames;
    public TMPro.TextMeshProUGUI[] rScores;
    [SerializeField] bool wantRefresh = true;
    HighScores myScores;

    void Start() //Fetches the Data at the beginning
    {
        for (int i = 0; i < rNames.Length; i++)
        {
            rNames[i].text = "Please Wait";
            rScores[i].text = "Please Wait";
        }
        myScores = GetComponent<HighScores>();
        if (wantRefresh)
        { StartCoroutine("RefreshHighscores"); }
    }

    public void SetScoresToMenu(PlayerScore[] highscoreList) //Assigns proper name and score for each text value
    {
        for (int i = 0; i < rNames.Length; i++)
        {
            rNames[i].text = "NO DATA";
            rScores[i].text = "";
            if (highscoreList.Length > i)
            {
                rScores[i].text = highscoreList[i].score.ToString();
                rNames[i].text = highscoreList[i].username;
            }
        }
    }

    public void NoConnection()
    {
        for (int i = 0; i < rNames.Length; i++)
        {
            rNames[i].text = "No Connection";
            rScores[i].text = "";
        }
    }
    IEnumerator RefreshHighscores() //Refreshes the scores every 30 seconds
    {
        while (true)
        {
            myScores.DownloadScores();
            yield return new WaitForSeconds(30);
        }
    }
}
