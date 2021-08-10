using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootScript : MonoBehaviour
{
    public GameObject bulletObject;

    public GameObject shootParticles;

    public float bulletSpeed;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            GameObject bulletInstance = Instantiate(bulletObject, transform.position, transform.rotation) as GameObject;

            bulletInstance.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed * Time.deltaTime);

            GameObject particleInstance = Instantiate(shootParticles, transform.position, transform.rotation);

            particleInstance.GetComponent<ParticleSystem>().Play();
        }
    }
}
