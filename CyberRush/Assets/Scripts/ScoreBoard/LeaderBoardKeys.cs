using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Keys", menuName = "LeaderBoardKeys", order = 1)]
public class LeaderBoardKeys : ScriptableObject
{
    public string privateCode;  //Key to Upload New Info
    public string publicCode;   //Key to download
}
