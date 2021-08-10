using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    [SerializeField] bool isEnemy;
    [SerializeField] int totalHealth = 5;
    [SerializeField] Slider healthSlider;
    [HideInInspector]
    public GameObject lineRenderer;

    //Private
    int currentHealth;
    private void Start()
    {
        currentHealth = totalHealth;
        if (!isEnemy)
        {
            healthSlider.maxValue = totalHealth;
            healthSlider.value = totalHealth;
        }
    }
    public void TakeDamage()        //This is a weird script as I didn't put too much thought into it
    {
        currentHealth--;
        if (!isEnemy)
        {
            healthSlider.value = currentHealth;
        }
        if (currentHealth <= 0)
        {
            if (isEnemy)
            {
                if (lineRenderer != null) { Destroy(lineRenderer.gameObject); }
                GetComponent<EnemyStateMachine>().isDead = true;
            }
            else    //This means that this is the player
            {
                gameObject.SetActive(false);
                FindObjectOfType<LevelManager>().PlayerDead();
                healthSlider.gameObject.SetActive(false);
            }
        }
    }
}
