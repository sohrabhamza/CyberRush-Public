using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] GameObject[] configs1;
    [SerializeField] GameObject[] configs2;
    [SerializeField] GameObject[] configs3;
    [SerializeField] GameObject[] configs4;

    public List<GameObject> enemies;

    //private
    // GameObject nextLevel = null;
    LevelManager manager;

    public bool spawnedEnemies;

    private void OnEnable()
    {
        //Enable one configration in 4 corners of the map randomly. Lol this was a last minute change and that is why its so shit
        foreach (GameObject conf in configs1) { conf.SetActive(false); }
        int ranConf1 = Random.Range(0, configs1.Length);
        configs1[ranConf1].SetActive(true);

        foreach (GameObject conf in configs2) { conf.SetActive(false); }
        int ranConf2 = Random.Range(0, configs2.Length);
        configs2[ranConf2].SetActive(true);

        foreach (GameObject conf in configs3) { conf.SetActive(false); }
        int ranConf3 = Random.Range(0, configs3.Length);
        configs3[ranConf3].SetActive(true);

        foreach (GameObject conf in configs4) { conf.SetActive(false); }
        int ranConf4 = Random.Range(0, configs4.Length);
        configs4[ranConf4].SetActive(true);

        manager = GameObject.FindObjectOfType<LevelManager>();
    }

    private void Update()
    {
        //If there is an enemy to remove, remove them
        GameObject enemToRemove = null;
        foreach (GameObject enem in enemies)
        {
            if (enem.GetComponent<EnemyStateMachine>().isDead)
            {
                enemToRemove = enem;
            }
        }
        if (enemToRemove != null)
        { enemies.Remove(enemToRemove); Destroy(enemToRemove); }

        //If no enemies, tell the level manager this
        if (enemies.Count <= 0 && spawnedEnemies)
        {
            manager.NextLevel(gameObject);
        }
    }
}
