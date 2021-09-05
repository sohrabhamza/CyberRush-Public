using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMobileAim : MonoBehaviour
{
    [SerializeField] FloatingJoystick joystick;
    [SerializeField] float touchSensitivity = 0.15f;

    void FixedUpdate()
    {
        Vector3 lookRot = new Vector3(joystick.Horizontal, 0, joystick.Vertical);
        transform.LookAt(lookRot + transform.position);
    }
}
