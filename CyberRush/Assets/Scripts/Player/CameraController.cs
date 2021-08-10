using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Serialized
    [SerializeField] float height = 10;
    [SerializeField] float distance = 0;
    [SerializeField] Transform target;
    [Header("The below option makes it so that the camera follows the object tagged as player")]
    [SerializeField] bool usePlayerPositionWithTag = true;

    public bool focusOnFace;
    [SerializeField] Transform focusPosition;

    private void Start()
    {
        if (usePlayerPositionWithTag)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
    private void FixedUpdate()
    {
        if (!focusOnFace)
        { transform.position = target.TransformPoint(0, height, -distance); }
        else
        {
            transform.position = focusPosition.localPosition;
            transform.rotation = focusPosition.localRotation;
        }
    }
}
