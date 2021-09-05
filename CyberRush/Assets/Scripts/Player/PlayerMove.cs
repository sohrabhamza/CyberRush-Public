using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    //Serialized
    [SerializeField] float speed = 7;
    [SerializeField] Animator animator;
    [SerializeField] FloatingJoystick moveJoystick;
    //Private
    Vector3 direction;
    // Rigidbody rb;
    CharacterController characterController;
    public bool active = true;
    Vector3 currentPos = Vector3.zero;
    [SerializeField] Transform playerBody;
    void Start()
    {
        // rb = GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();

    }
    void Update()
    {
        if (!active)
        {
            animator.SetFloat("Horizontal", 0);
            animator.SetFloat("Vertical", 0);
            return;
        }
        // direction = new Vector3(Input.GetAxisRaw("Horizontal"), transform.position.y, Input.GetAxisRaw("Vertical"));
        direction = new Vector3(moveJoystick.Horizontal, 0, moveJoystick.Vertical);
    }
    private void FixedUpdate()
    {
        if (!active) return;
        Move();

        // Vector3 beforePos = transform.position;
        // Vector3 velocity = currentPos - beforePos;
        // Vector3 velo = playerBody.transform.InverseTransformDirection(velocity);

        // currentPos = transform.position;

        // animator.SetFloat("Horizontal", velo.x);
        // animator.SetFloat("Vertical", velo.z);
        animator.SetFloat("Horizontal", characterController.velocity.x);
        animator.SetFloat("Vertical", characterController.velocity.z);
    }
    void Move()
    {
        characterController.Move(direction * speed * Time.deltaTime);
        // rb.MovePosition(transform.position + (direction * speed * Time.deltaTime));
        // rb.velocity = Vector3.zero;
    }
}
