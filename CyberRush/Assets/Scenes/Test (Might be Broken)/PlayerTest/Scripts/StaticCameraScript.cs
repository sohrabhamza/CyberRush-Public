using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticCameraScript : MonoBehaviour
{
    public Transform target;

    public Vector3 offset;
    public float smoothSpeed = 5;

    public bool isSmooth = false;

    // Update is called once per frame
    void LateUpdate()
    {
        if (isSmooth)
            SmoothCamera();
        else
            StaticCamera();
    }

    void StaticCamera()
    {
        transform.position = target.position + offset;
    }

    void SmoothCamera()
    {
        Vector3 targetPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }
}
