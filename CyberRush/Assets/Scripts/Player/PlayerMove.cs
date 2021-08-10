using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    //Serialized
    [SerializeField] float speed = 7;
    [SerializeField] Animator animator;
    //Private
    Vector3 direction;
    Rigidbody rb;
    public bool active = true;
    Vector3 currentPos = Vector3.zero;
    [SerializeField] Transform playerBody;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (!active)
        {
            animator.SetFloat("Horizontal", 0);
            animator.SetFloat("Vertical", 0);
            return;
        }
        direction = new Vector3(Input.GetAxisRaw("Horizontal"), transform.position.y, Input.GetAxisRaw("Vertical"));
    }
    private void FixedUpdate()
    {
        if (!active) return;
        Move();
        rb.velocity = Vector3.zero;

        Vector3 beforePos = transform.position;
        Vector3 velocity = currentPos - beforePos;
        Vector3 velo = playerBody.transform.InverseTransformDirection(velocity);

        currentPos = transform.position;

        animator.SetFloat("Horizontal", velo.x);
        animator.SetFloat("Vertical", velo.z);
    }
    void Move()
    {
        rb.MovePosition(transform.position + (direction * speed * Time.deltaTime));
        rb.velocity = Vector3.zero;
    }
}
