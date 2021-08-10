using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] float aimHieght = 1;
    public bool active = true;
    private void Update()
    {
        if (!active) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, new Vector3(0, aimHieght, 0));
        float dis;
        plane.Raycast(ray, out dis);
        Vector3 pointToLook = new Vector3(ray.GetPoint(dis).x, transform.position.y, ray.GetPoint(dis).z) - transform.position;
        Debug.DrawLine(ray.origin, pointToLook, Color.green);
        transform.rotation = Quaternion.LookRotation(pointToLook, Vector3.up);
    }
}
