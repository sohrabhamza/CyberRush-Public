using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleNextLevel : MonoBehaviour
{
    [SerializeField] GameObject textPrompt;
    [SerializeField] Material active, inactive;
    [SerializeField] GameObject hintText;
    public bool consoleActive;
    bool playerNear;
    GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && consoleActive)
        {
            textPrompt.SetActive(true);     //If player is within trigger, prompt them to press e but only if console is activated
            playerNear = true;
        }
    }

    private void Update()       //I wanted to use ontriggerstay but i had some issues. I forgot what they were though
    {
        if (playerNear)     //If player is near...
        {
            if (Input.GetKeyDown(KeyCode.E))    //And they press e...
            {
                GameObject.FindObjectOfType<LevelManager>().consolePressed = true;      //Tell the level manager to make a new level

                player.transform.position = new Vector3(0, 0, 0);       //Send player to middle with rotation and stop them from moving and aiming
                player.transform.GetChild(0).eulerAngles = new Vector3(0, 0, 0);
                FindObjectOfType<PlayerMove>().active = false;
                FindObjectOfType<PlayerAim>().active = false;

                consoleActive = false;
                textPrompt.SetActive(false);
                hintText.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            textPrompt.SetActive(false);
            playerNear = false;     //This was what i was missing that made the player bug out when pressing e in between a level. (If you know you know)
        }
    }

    public void activateTerminal()      //Method for the room manager to call when every enemy is dead
    {
        consoleActive = true;
        hintText.SetActive(true);
    }
}
