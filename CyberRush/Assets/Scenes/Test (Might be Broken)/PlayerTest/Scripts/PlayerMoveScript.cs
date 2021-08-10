using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveScript : MonoBehaviour
{
    public CharacterController controller;

    public float movementSpeed = 12f;
    public float rollSpeed = 12f;
    public float smoothRotSpeed = 5f;

    public bool smoothRot = false;

    Transform playerBody;

    float h;
    float v;

    Vector3 currentPos = Vector3.zero;

    Vector3 lastMove;
    Quaternion lastDirection;

    Animator animator;

    bool isRolling;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        playerBody = transform.GetChild(0);
        animator = playerBody.GetComponent<Animator>();
    }

    void FixedUpdate() 
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        //Velocity
        Vector3 beforePos = transform.position;
        Vector3 velocity = currentPos - beforePos;
        Vector3 velo = playerBody.transform.InverseTransformDirection(velocity);

        velo = new Vector3(Mathf.Round(velo.x * 10f) / 10f, velo.y,  Mathf.Round(velo.z * 10f) / 10f);

        float horizontal = velo.x;
        float vertical = velo.z;

        if ((velo.x < 0 && velo.z > 0) || (velo.x > 0 && velo.z < 0))
        {
            horizontal = -1;
            vertical = 0;
        }else if ((velo.x > 0 && velo.z > 0) || (velo.x < -0 && velo.z < 0))
        {
            horizontal = 1;
            vertical = 0;
        }

        animator.SetFloat("Vertical", -vertical);
        animator.SetFloat("Horizontal", horizontal);

        Debug.Log(velo.x + ", " + velo.z + "; horizontal: " + animator.GetFloat("Horizontal") + "; vertical: " + animator.GetFloat("Vertical"));

        currentPos = transform.position;
    }

    void Update()
    {
        if (!Input.GetKeyDown("e") && !isRolling)
        {
            float speed = movementSpeed;

            if (h != 0 && v != 0)
                speed /= 1.5f;

            
            Vector3 move = Vector3.right * h + Vector3.forward * v;

            controller.Move(move * speed * Time.deltaTime);

            lastMove = move;
        
            RotatePlayerWithRay();

        }else if (isRolling)
        {

            if (h == 0 && v == 0)
                lastMove = transform.forward;
                
            controller.Move(lastMove * rollSpeed * Time.deltaTime);
        }else
        {
            animator.SetFloat("rollSpeed", 1f);

            if (v < 0)
                animator.SetFloat("rollSpeed", -1f);

            animator.SetTrigger("Roll");
        }
    }

    IEnumerator RollAnim()
    {
        isRolling = true;

        yield return new WaitForSeconds(2.22f);

        controller.center = new Vector3(0, 0.955f, 0);

        isRolling = false;
    }

    public void SetControllerCenterZ(float z)
    {
        controller.center = new Vector3(controller.center.x, controller.center.y, z);
    }

    public void SetControllerCenterY(float y)
    {
        controller.center = new Vector3(controller.center.x, y, controller.center.z);
    }

    public void SetControllerHeight(float height)
    {
        controller.height = height;
    }

    void RotatePlayerWithRay() //Rotates player to cursor
    {
        RaycastHit hit;
        Ray ray = FindObjectOfType<Camera>().ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.layer != 6)
            {
                Vector3 targetPostition = new Vector3( hit.point.x, //x
                                    transform.position.y, //y
                                    hit.point.z ) ; //z
                if (smoothRot)
                {
                    Vector3 direction = targetPostition - transform.position;
                    Quaternion rotation = Quaternion.LookRotation(direction);

                    transform.rotation = Quaternion.Lerp(transform.rotation, rotation, smoothRotSpeed * Time.deltaTime);
                }else
                {
                    transform.LookAt(targetPostition);
                }

                lastDirection = transform.rotation;
            }
        }
    }

    void RotatePlayerToMoveDir(float h, float v)
    {
        Vector3 targetPostition = new Vector3( transform.position.x + h, //x
                                    transform.position.y, //y
                                    transform.position.z + v ) ; //z
        if (smoothRot)
        {
            Vector3 direction = targetPostition - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, smoothRotSpeed * Time.deltaTime);
        }else
        {
            transform.LookAt(targetPostition);
        }

        lastDirection = transform.rotation;
    }
}
