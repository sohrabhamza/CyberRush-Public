using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScores : MonoBehaviour
{
    string privateCode;
    string publicCode;
    public LeaderBoardKeys keys;    //I can't give you my leaderboard keys as they can easily be misused. So you can go to dreamlo.com and make your own if you would like to fork this project. 
    //I am planning on replacing this entire implementation with a firebase based one so this is going to be obselete anyway
    const string webURL = "http://dreamlo.com/lb/"; //  Website the keys are for

    public PlayerScore[] scoreList;
    DisplayHighscores myDisplay;

    static HighScores instance; //Required for STATIC usability
    void Start()
    {
        if (keys == null)
        {
            Debug.Log("Please provide your dreamlo public and private keys.");
        }
        privateCode = keys.privateCode;
        publicCode = keys.publicCode;
        instance = this; //Sets Static Instance
        myDisplay = GetComponent<DisplayHighscores>();
    }

    public static void UploadScore(string username, int score)  //CALLED when Uploading new Score to WEBSITE
    {//STATIC to call from other scripts easily
        instance.StartCoroutine(instance.DatabaseUpload(username, score)); //Calls Instance
    }

    IEnumerator DatabaseUpload(string userame, int score) //Called when sending new score to Website
    {
        WWW www = new WWW(webURL + privateCode + "/add/" + WWW.EscapeURL(userame) + "/" + score);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            print("Upload Successful");
            DownloadScores();
        }
        else
        {
            print("Error uploading" + www.error);
            myDisplay.NoConnection();
        }
    }

    public void DownloadScores()
    {
        StartCoroutine("DatabaseDownload");
    }
    IEnumerator DatabaseDownload()
    {
        //WWW www = new WWW(webURL + publicCode + "/pipe/"); //Gets the whole list
        WWW www = new WWW(webURL + publicCode + "/pipe/0/10"); //Gets top 10
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            OrganizeInfo(www.text);
            myDisplay.SetScoresToMenu(scoreList);
        }
        else
        {
            print("Error Downloading" + www.error);
            myDisplay.NoConnection();
        }
    }

    void OrganizeInfo(string rawData) //Divides Scoreboard info by new lines
    {
        string[] entries = rawData.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        scoreList = new PlayerScore[entries.Length];
        for (int i = 0; i < entries.Length; i++) //For each entry in the string array
        {
            string[] entryInfo = entries[i].Split(new char[] { '|' });
            string username = entryInfo[0];
            int score = int.Parse(entryInfo[1]);
            scoreList[i] = new PlayerScore(username, score);
            print(scoreList[i].username + ": " + scoreList[i].score);
        }
    }
}

public struct PlayerScore //Creates place to store the variables for the name and score of each player
{
    public string username;
    public int score;

    public PlayerScore(string _username, int _score)
    {
        username = _username;
        score = _score;
    }
}