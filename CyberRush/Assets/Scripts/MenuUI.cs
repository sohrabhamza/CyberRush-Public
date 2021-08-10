using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class MenuUI : MonoBehaviour
{
    string username;
    [SerializeField] bool isUserNameScene;
    [SerializeField] bool isMain;
    [SerializeField] TextMeshProUGUI usernameDisplayText;
    [SerializeField] TextMeshProUGUI highscoreDisplay;
    [SerializeField] float usernameHeight, nonUsernameHeight;
    [SerializeField] GameObject pauseScreen;
    // [SerializeField] AudioMixer mixer;


    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void UsernameEntry(string user)
    {
        username = user;
    }

    public void UsernameSubmit()
    {
        if (username == "")
        {
            return;
        }
        PlayerPrefs.SetString("Username", username);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void Update()
    {
        if (isUserNameScene)
        {
            if (PlayerPrefs.GetString("Username") != "")
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }

        if (usernameDisplayText != null)
        {
            Camera.main.gameObject.transform.position = new Vector3(Camera.main.gameObject.transform.position.x, nonUsernameHeight, Camera.main.gameObject.transform.position.z);
            if (PlayerPrefs.HasKey("Username"))
            {
                Camera.main.gameObject.transform.position = new Vector3(Camera.main.gameObject.transform.position.x, usernameHeight, Camera.main.gameObject.transform.position.z);

                usernameDisplayText.gameObject.SetActive(true);
                usernameDisplayText.text = "Welcome " + PlayerPrefs.GetString("Username");
                if (PlayerPrefs.HasKey("HighScore") && PlayerPrefs.GetInt("HighScore") != 0)
                {
                    highscoreDisplay.text = "Your Highscore is " + PlayerPrefs.GetInt("HighScore").ToString();
                }
                else
                {
                    highscoreDisplay.gameObject.SetActive(false);
                }
            }
            else
            {
                usernameDisplayText.gameObject.SetActive(false);
            }
        }

        if (isMain)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PauseResume();
            }
        }
    }

    public void ResetUsername()
    {
        usernameDisplayText.gameObject.SetActive(false);
        PlayerPrefs.DeleteKey("Username");
        PlayerPrefs.DeleteKey("HighScore");
        Camera.main.gameObject.transform.position = new Vector3(Camera.main.gameObject.transform.position.x, usernameHeight, Camera.main.gameObject.transform.position.z);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Application.Quit();
    }

    bool paused;
    public void PauseResume()
    {
        Time.timeScale = paused ? 1 : 0;
        pauseScreen.SetActive(paused ? false : true);
        paused = !paused;
    }
}
