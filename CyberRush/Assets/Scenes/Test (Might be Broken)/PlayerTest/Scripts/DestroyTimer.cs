using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    public float destroyCountdown = 1;

    void Start() 
    {
        DestroyIn();
    }

    IEnumerator DestroyIn ()
    {
        yield return new WaitForSeconds(destroyCountdown);

        Destroy(gameObject);
    }
}
