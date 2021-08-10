using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinn : MonoBehaviour
{
    void FixedUpdate()
    {
        transform.localEulerAngles += new Vector3(0, 0, 50);
    }
}
