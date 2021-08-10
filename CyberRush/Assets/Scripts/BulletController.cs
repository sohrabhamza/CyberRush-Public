using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    //Serialized
    [SerializeField] float bulletSpeed = 5;
    private void FixedUpdate()
    {
        transform.Translate(transform.forward * Time.deltaTime * bulletSpeed, Space.World);     //Simple way of moving it forward
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;
        if (other.TryGetComponent<DamageSystem>(out DamageSystem dsm))      //If the object that was hit has a damage system component, call the take damage function.
        {
            dsm.TakeDamage();
        }
        Destroy(gameObject);
    }
}
