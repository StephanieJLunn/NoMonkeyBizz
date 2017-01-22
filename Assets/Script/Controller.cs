using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {

    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    public bool useGravity = true;
    private Vector3 moveDirection = Vector3.zero;

    public float hitdist = 10.0f;
    public float playerRotateSpeed = 1.0f;

    private Animator animator;
    CharacterController controller;

    public GameObject objectInFront;
    Vector3 lockPosition;
    private bool lockedPosition = false;

    public float lowObstacleHeight = 3.0f;
    public float HighObstacleHeight = 5.0f;

    public bool climbing = false;
    public float climbSpeed = 5.0f;

    public GameObject throwableObject = null;
    public Transform rightHand;

    public GameObject actionnableObject = null;

    public bool onTop = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Input.GetKeyDown("t"))
            ThrowObject();

        if (Input.GetKeyDown("e"))
            Action();

        if (Input.GetAxis("Vertical") > 0)
            animator.SetBool("moving_forward", true);
        else
            animator.SetBool("moving_forward", false);
        if (Input.GetAxis("Vertical") < 0)
            animator.SetBool("moving_backward", true);
        else
            animator.SetBool("moving_backward", false);
       

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Vector3 targetPoint = ray.GetPoint(hitdist);

        Vector3 lookPos = targetPoint - transform.position;
        lookPos.y = 0;
        
        CharacterController controller = GetComponent<CharacterController>();
        transform.Rotate(0, Input.GetAxis("Horizontal") * playerRotateSpeed, 0);
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        float curSpeed = speed * Input.GetAxis("Vertical");
        if (Input.GetAxis("Vertical") < 0)
            curSpeed /= 2;

        
        controller.SimpleMove(forward * curSpeed);


        detectObjectInFront();
        onTopOfObject();

        if (Input.GetButton("Jump") && controller.isGrounded)
        {
            animator.SetBool("jump", true);
            moveDirection = forward * curSpeed;
            moveDirection.y = jumpSpeed;
        }

        if (!controller.isGrounded)
        {
            animator.SetBool("jump", false);
        }

        if (animator.GetBehaviour<Climb_StateMachine>().finishClimbing)
        {
            animator.SetBool("climb", false);
            animator.GetBehaviour<Climb_StateMachine>().finishClimbing = false;
        }

        if (climbing)
        {
            transform.Translate(Vector3.up * climbSpeed * Time.deltaTime);
            transform.Translate(Vector3.forward * 0.5f * Time.deltaTime);
            if (onTop)
            {
                climbing = false;
                useGravity = true;
                controller.center = new Vector3(0, 1.5f, 0);
            }
        }

        if (lockedPosition)
            transform.position = new Vector3(lockPosition.x, transform.position.y, lockPosition.z);

        if (useGravity)
        { 
            moveDirection.y -= gravity * Time.deltaTime;
            controller.Move(moveDirection * Time.deltaTime);
        }
        if (!onTop && transform.position.y > (HighObstacleHeight + 1))
        {
            climbing = false;
            useGravity = true;
            controller.center = new Vector3(0, 1.5f, 0);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Throwable" && throwableObject == null)
        {
            throwableObject = other.gameObject;
            pickUpObject();
        }

        if (other.gameObject.tag == "Actionable")
        {
            actionnableObject = other.gameObject;
            other.gameObject.transform.GetComponent<Renderer>().material.color = new Color(0.0f, 0.0f, 255.0f);
        }

        if (other.gameObject.tag == "Moveable")
        {
            Debug.Log(other.gameObject.name + " detected !");
            other.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 200f + transform.up * 200f);
        }
    }


    void ThrowObject()
    {
        if (throwableObject != null)
        {
            animator.SetBool("throw",true);
            throwableObject.transform.parent = null;
            throwableObject.AddComponent<Rigidbody>();
            StartCoroutine(Throw(throwableObject));
            throwableObject = null;
        }
    }

    void Action()
    {
        if (actionnableObject != null)
        {
            actionnableObject.GetComponent<Actionnable>().action();
            actionnableObject = null;
        }
    }
        
    void pickUpObject()
    {
        Debug.Log("Throwable item "+ throwableObject.name  + " detected !!");
        Destroy(throwableObject.GetComponent<Rigidbody>());
        throwableObject.transform.position = rightHand.transform.position + transform.forward*0.2f + transform.up * -0.2f;
        throwableObject.transform.parent = rightHand;
    }

    public void detectObjectInFront()
    {
        RaycastHit hit = new RaycastHit();
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        if (Physics.Raycast(transform.position, fwd, out hit, 1))
        {
            objectInFront = hit.transform.gameObject;
            if (objectInFront.gameObject.tag == "HighObstacle" && Input.GetAxis("Vertical") > 0 && !climbing)
            {
                Debug.Log("CLIMB");
                climbing = true;
                animator.SetBool("climb", true);
                controller.center = new Vector3(0, 2.5f, 0);
                useGravity = false;
            }
        }
        else
            objectInFront = null;
    }

    public void onTopOfObject()
    {
        RaycastHit hit = new RaycastHit();
        Vector3 down = transform.TransformDirection(Vector3.down);
        if (Physics.Raycast(transform.position, down, out hit, 3))
        {
            if (hit.transform.gameObject.tag == "HighObstacle")
            {
                onTop = true;
            }
            else
                onTop = false;
        }
    }

    IEnumerator Throw(GameObject go)
    {
        for(int i = 0;i < 3;i++)
        {
            if (go.GetComponent<Rigidbody>() == null)
            {
                throwableObject = null;
                go.transform.parent = null;
                go.AddComponent<Rigidbody>();
            }
            go.GetComponent<Rigidbody>().AddForce(moveDirection +  transform.forward * 300.0f + transform.up * 300.0f);
            yield return new WaitForSeconds(0.1f);
        }
        animator.SetBool("throw", false);
    }
}
