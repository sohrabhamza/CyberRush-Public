using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] float FireRate;
    [SerializeField] Transform shootPos;

    //private
    float nextTimeToFire;

    private void Update()
    {
        if (Time.time > nextTimeToFire && Input.GetKey(KeyCode.Mouse0))
        {
            GameObject shotBullet = Instantiate(bullet, shootPos.transform.position, shootPos.transform.rotation);

            nextTimeToFire = Time.time + 1 / FireRate;
        }
    }
}
