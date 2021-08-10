using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerScript : MonoBehaviour
{
    public Transform target;

    public Vector3 offset;
    public Vector3 aimOffset;
    public float cameraXRot = 45;
    public float speed = 0.125f;

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetButton("Fire2"))
            Aim();
        else
        {
            Move();
            Rotate();
        }
    }

    void Move()
    {
        Vector3 targetPos = target.TransformPoint(offset);
        transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
    }

    void Rotate()
    {
        Vector3 direction = target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, speed * Time.deltaTime);

        transform.localEulerAngles = new Vector3(cameraXRot, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }
    
    void Aim()
    {
        //Move
        Vector3 targetPos = target.TransformPoint(aimOffset);
        transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);

        //Rotate
        RaycastHit hit;
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 direction = hit.point - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, speed * Time.deltaTime);
        }
    }
}
