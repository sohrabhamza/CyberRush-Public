using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    //Sorry for not organizing this. Did not have time.
    [System.Serializable]
    public class DifficutyScaling
    {
        public List<GameObject> enemiesToSpawn;
    }
    [Header("Enemy Spawning")]
    [SerializeField] DifficutyScaling[] difficutyScaling;
    [SerializeField] float noSpawnSquareRadius = 7;
    [SerializeField] float maxSpawn = 20;
    [Space]


    public bool spawnNextLevel;
    public bool consolePressed;
    [SerializeField] GameObject room;
    [SerializeField] GameObject currentRoom;
    [SerializeField] float heightOnNext = 15;

    //private 
    GameObject nextLevel = null;
    CameraController cameraController;
    Transform camTarget;
    bool scoreUpdated;
    RoomManager roomManager;

    //Score
    int localScore = 0;
    int localHighScore = 0;
    string username;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI HighScoreText;
    [SerializeField] TextMeshProUGUI runningScore;
    [SerializeField] TextMeshProUGUI runningHighScore;
    [SerializeField] GameObject deathPanel;
    [SerializeField] GameObject joysticks;

    [Header("Audio")]
    [SerializeField] AudioClip[] music;
    List<AudioClip> nowPlaying = new List<AudioClip>();
    AudioSource source;


    private void Start()
    {
        //Get required components and build navmesh at start
        roomManager = FindObjectOfType<RoomManager>();
        cameraController = FindObjectOfType<CameraController>();
        GetComponent<NavMeshSurface>().BuildNavMesh();
        source = GetComponent<AudioSource>();

        username = PlayerPrefs.GetString("Username");
        localHighScore = PlayerPrefs.GetInt("HighScore");

        HighScoreText.text = "Highscore: " + localHighScore.ToString();
        runningHighScore.text = "Highscore: " + localHighScore.ToString();

        scoreText.text = "Score: " + localScore.ToString();
        runningScore.text = "Score: " + localScore.ToString();

        //Spawn enemies
        SpawnEnemies();

        //Get audio clips and transfer them to local list
        foreach (AudioClip clip in music)
        {
            nowPlaying.Add(clip);
        }
        for (int i = 0; i < nowPlaying.Count; i++)     //Shuffle the list
        {
            AudioClip temp = nowPlaying[i];
            int randomIndex = Random.Range(i, nowPlaying.Count);
            nowPlaying[i] = nowPlaying[randomIndex];
            nowPlaying[randomIndex] = temp;
        }

        //Play first song
        source.clip = nowPlaying[0];
        source.Play();
    }
    private void Update()
    {
        if (spawnNextLevel && consolePressed)   //If next level is unlocked and user presses button on console
        {
            UpdateLocalScore();     //Update our score

            MoveLevelUpward();      //Spawn and move level upward
        }
        else if (spawnNextLevel)        //If next level is unlocked, stop music. We do not care if the player has pressed console button or not
        {
            MuteAudio();
        }
    }
    void MuteAudio()
    {
        source.volume = Mathf.Lerp(source.volume, 0, Time.deltaTime * .2f);
        if (source.volume <= 0.05f)
        {
            source.Stop();
        }
    }

    void MoveLevelUpward()
    {
        cameraController.focusOnFace = true;    //Tell the camera to focus on face
        Destroy(currentRoom);   //Destroy current room
        if (nextLevel == null)  //If next levl does not exist, spawn one
        {
            nextLevel = Instantiate(room, new Vector3(0, heightOnNext, 0), Quaternion.identity);
        }
        else    //If it does move it upward
        {
            nextLevel.transform.position = Vector3.Lerp(nextLevel.transform.position, Vector3.zero, Time.deltaTime * 3);
        }
        if (Mathf.Abs(nextLevel.transform.position.y) <= 0.01f)   //If it has reached the desired position, stop this method, stop focusing on face, reset camera angle, build navmesh, remove reference to next level, reset console pressed variable, reset score updated variable
        {
            cameraController.focusOnFace = false;
            cameraController.transform.eulerAngles = new Vector3(90, 0, 0);
            GetComponent<NavMeshSurface>().BuildNavMesh();
            SpawnEnemies();

            FindObjectOfType<PlayerMove>().active = true;
            FindObjectOfType<PlayerAim>().active = true;

            nextLevel = null;

            spawnNextLevel = false;
            consolePressed = false;
            scoreUpdated = false;

            //Remove the music track currently playing (was playing) from list 
            nowPlaying.RemoveAt(0);
            if (nowPlaying.Count == 0)      //If there are no more music tracks left do the same thing we did in start
            {
                foreach (AudioClip clip in music)
                {
                    nowPlaying.Add(clip);
                }
                for (int i = 0; i < nowPlaying.Count; i++)     //Shuffle the list
                {
                    AudioClip temp = nowPlaying[i];
                    int randomIndex = Random.Range(i, nowPlaying.Count);
                    nowPlaying[i] = nowPlaying[randomIndex];
                    nowPlaying[randomIndex] = temp;
                }
            }
            source.volume = .15f;       //Volume
            source.clip = nowPlaying[0];    //Play the new first track
            source.Play();
        }
    }
    void UpdateLocalScore()
    {
        if (scoreUpdated) return;
        localScore++;
        scoreText.text = "Score: " + localScore.ToString();
        runningScore.text = "Score: " + localScore.ToString();
        scoreUpdated = true;
    }

    void SpawnEnemies()
    {
        //Works perfectly fine. Only problem with this method is that the enemies may spawn clustered in only one quadrant
        // foreach (GameObject enem in difficutyScaling[localScore].enemiesToSpawn)
        // {
        //     Instantiate(enem, enemySpawnLocation(Random.Range(0, 4)), Quaternion.identity);
        // }    


        //This is the weird algorithm i wrote to spawn enemies. I think it might be overly complicated but works perfectly
        roomManager = FindObjectOfType<RoomManager>();  //Get reference to the room manager
        int currentScaling = localScore;    //Set the index of difficult that we are on currently to the score
        if (localScore >= difficutyScaling.Length)      //If our score is more than the number of defineed difficulty scales then use the last difficulty scale
        {
            currentScaling = difficutyScaling.Length - 1;
        }

        int numPerQuadrant = difficutyScaling[currentScaling].enemiesToSpawn.Count / 4;     //Get number of enemies per quadrant 
        int remainingNumOfEnemies = difficutyScaling[currentScaling].enemiesToSpawn.Count % 4;      //Get number of enemies that cannot be spawned evenly

        for (int i = 0; i < difficutyScaling[currentScaling].enemiesToSpawn.Count; i++)     //Shuffle the list
        {
            GameObject temp = difficutyScaling[currentScaling].enemiesToSpawn[i];
            int randomIndex = Random.Range(i, difficutyScaling[currentScaling].enemiesToSpawn.Count);
            difficutyScaling[currentScaling].enemiesToSpawn[i] = difficutyScaling[currentScaling].enemiesToSpawn[randomIndex];
            difficutyScaling[currentScaling].enemiesToSpawn[randomIndex] = temp;
        }

        int enemyToSpawn = 0;
        for (int i = 0; i < 4; i++)     //Loop through all quadrants
        {
            for (int j = 0; j < numPerQuadrant; j++)    //Loop through all enemies that can be spawned in a quadrant
            {
                roomManager.enemies.Add(Instantiate(difficutyScaling[currentScaling].enemiesToSpawn[enemyToSpawn], enemySpawnLocation(i), Quaternion.identity));    //Spawn enemy in quadrant
                enemyToSpawn++;
            }
        }
        if (remainingNumOfEnemies != 0)     //If there are remaining enemies
        {
            for (int i = 0; i < remainingNumOfEnemies; i++)     //Loop through them
            {
                roomManager.enemies.Add(Instantiate(difficutyScaling[currentScaling].enemiesToSpawn[enemyToSpawn], enemySpawnLocation(Random.Range(0, 4)), Quaternion.identity));       //Spawn them in a random quadrant
                enemyToSpawn++;
            }
        }
        roomManager.spawnedEnemies = true;
    }

    Vector3 enemySpawnLocation(int quadrant)        //Get a random spawn location based on requested quadrant. This algorithm will only work for square arenas or maybe slightly rectangular ones
    {
        //Not gonna bother commenting as its pretty self explanatory and kinda shit
        float sideOffsetPositive = Random.Range(noSpawnSquareRadius, maxSpawn);
        float sideOffsetNegetive = Random.Range(-noSpawnSquareRadius, -maxSpawn);

        float topOffsetPositive = Random.Range(noSpawnSquareRadius, maxSpawn);
        float topOffsetNegetive = Random.Range(-noSpawnSquareRadius, -maxSpawn);

        if (quadrant == 0)
        {
            return new Vector3(sideOffsetPositive, 0, topOffsetPositive);
        }
        else if (quadrant == 1)
        {
            return new Vector3(sideOffsetNegetive, 0, topOffsetPositive);
        }
        else if (quadrant == 2)
        {
            return new Vector3(sideOffsetNegetive, 0, topOffsetNegetive);
        }
        else if (quadrant == 3)
        {
            return new Vector3(sideOffsetPositive, 0, topOffsetNegetive);
        }
        else
        {
            return Vector3.zero;
        }
    }

    public void PlayerDead()
    {
        //stuff to do when player dies
        deathPanel.SetActive(true);
        runningScore.transform.parent.gameObject.SetActive(false);
        runningHighScore.transform.parent.gameObject.SetActive(false);
        joysticks.SetActive(false);
        if (localScore > localHighScore)
        {
            localHighScore = localScore;
            PlayerPrefs.SetInt("HighScore", localHighScore);
            scoreText.text = "NEW HIGHSCORE";
            HighScoreText.text = localHighScore.ToString();
        }
        if (localScore != 0)
        {
            HighScores.UploadScore(username, localHighScore);
        }
        else
        {
            FindObjectOfType<HighScores>().DownloadScores();
        }
    }

    public void NextLevel(GameObject me)        //Called from the room manager when everyone is dead
    {
        currentRoom = me;
        spawnNextLevel = true;
        FindObjectOfType<ConsoleNextLevel>().activateTerminal();
    }
}
