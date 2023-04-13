using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    private Rigidbody rb;
    [Header("Self-References")]
    [SerializeField] private Transform raycastParent;
    [SerializeField] private Animator bowAnim;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Transform modelOrientation;

    [Header("External References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform arrowTransform;
    [SerializeField] private Transform targArrowPos;
    [SerializeField] private Rigidbody arrowRB;

    [Header("Parameters")]
    [SerializeField] private float arrowPower;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float accelSpeed_ground;
    [SerializeField] private float frictionSpeed_ground;
    [SerializeField] private float accelSpeed_air;
    [SerializeField] private float frictionSpeed_air;

    [SerializeField] private float gravUp;
    [SerializeField] private float gravDown;
    [SerializeField] private float spaceReleaseGravMult;
    [Space(5)]
    [Space(10)]
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float raycastHeight;

    [Header("Settings")]
    [SerializeField] private float turnSpeedX;
    [SerializeField] private float turnSpeedY;

    float targHorizontalRotation;
    float targVerticalRotation;

    bool grounded = false;

    private List<Transform> raycastPoints = new List<Transform>();

    private float shootPickupDuration = 0.3f;
    private float nextShootPickupTime;

    private enum BowStateEnum
    {
        Ready,
        DrawBack,
        Fired
    }
    private BowStateEnum bowState;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

        rb = GetComponent<Rigidbody>();

        for (int i = 0; i < raycastParent.childCount; i++)
        {
            raycastPoints.Add(raycastParent.GetChild(i));
        }
    }

    void Update()
    {
        //Jump
        if (InputHandler.Instance.Jump.Down)
        {
            if (grounded)
                rb.velocity += transform.up * jumpPower;
        }


        switch (bowState)
        {
            case BowStateEnum.Ready:
                if (InputHandler.Instance.Shoot.Down)
                {
                    bowState = BowStateEnum.DrawBack;
                    bowAnim.SetTrigger("DrawBack");
                }
                break;
            case BowStateEnum.DrawBack:
                if (InputHandler.Instance.Shoot.Up)
                {
                    FireArrow();
                }
                break;
            case BowStateEnum.Fired:
                break;
        }

        //Camera Spin horizontal
        float mouseX = turnSpeedX * InputHandler.Instance.Look.x * Time.deltaTime;
        float mouseY = turnSpeedY * InputHandler.Instance.Look.y * Time.deltaTime;

        targHorizontalRotation += mouseX;

        targVerticalRotation -= mouseY;
        targVerticalRotation = Mathf.Clamp(targVerticalRotation, -89.5f, 89.5f);

        //Rotate camera and player model
        cameraTransform.rotation = Quaternion.Euler(targVerticalRotation, targHorizontalRotation, 0);
        modelOrientation.rotation = Quaternion.Euler(0, targHorizontalRotation, 0);

        //Place camera
        cameraTransform.position = cameraTarget.position;

        //Jump
        if (InputHandler.Instance.Jump.Down)
        {
            if (grounded)
                rb.velocity += transform.up * jumpPower;
        }

        //Space release gravity
        if (InputHandler.Instance.Jump.Up && rb.velocity.y > 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * spaceReleaseGravMult, rb.velocity.z);
        }
    }

    public void PickupArrow()
    {
        if (Time.time < nextShootPickupTime)
            return;

        arrowTransform.gameObject.SetActive(false);
        bowState = BowStateEnum.Ready;
        bowAnim.SetTrigger("Pickup");
    }

    private void FireArrow()
    {
        nextShootPickupTime = Time.time + shootPickupDuration;

        bowState = BowStateEnum.Fired;
        bowAnim.SetTrigger("Fire");

        arrowRB.velocity = Vector3.zero;
        arrowTransform.position = targArrowPos.position;
        arrowTransform.rotation = targArrowPos.rotation;
        arrowTransform.gameObject.SetActive(true);
        arrowRB.velocity = targArrowPos.forward * arrowPower;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Debug.DrawRay(transform.position, transform.forward * 1.6f / 2f, new Color(1, 0, 0), 5f);
        // Debug.DrawRay(cameraTransform.position, cameraTransform.forward * 1.5f / 2f, new Color(0, 1, 0), 5f);

        #region determine if player is grounded or not
        grounded = false; //number of raycasts that hit the ground 

        foreach (Transform point in raycastPoints)
        {
            if (Physics.Raycast(point.position, -transform.up, out RaycastHit hit, raycastHeight, groundLayer))
            {
                grounded = true;
                break;
            }
        }
        #endregion

        #region Apply Gravity
        if (InputHandler.Instance.Jump.Holding && rb.velocity.y > 0)
            rb.velocity -= new Vector3(0, gravUp, 0);
        else
            rb.velocity -= new Vector3(0, gravDown, 0);
        #endregion


        #region Acceleration
        //Get gravityless velocity
        Vector3 noGravVelocity = rb.velocity;
        noGravVelocity.y = 0;

        //Convert global velocity to local velocity
        Vector3 velocity_local = modelOrientation.InverseTransformDirection(noGravVelocity);


        //XZ Friction + acceleration
        Vector3 currInput = new Vector3(InputHandler.Instance.MoveXZ.x, 0, InputHandler.Instance.MoveXZ.y);
        if (grounded)
        {
            //Apply ground fricion
            Vector3 velocity_local_friction = velocity_local.normalized * Mathf.Max(0, velocity_local.magnitude - frictionSpeed_ground);

            Vector3 updatedVelocity = velocity_local_friction;

            if (currInput.magnitude > 0.05f) //Pressing something, try to accelerate
            {
                Vector3 velocity_local_input = velocity_local_friction + currInput * accelSpeed_ground;

                if (velocity_local_friction.magnitude <= maxSpeed)
                {
                    //under max speed, accelerate towards max speed
                    updatedVelocity = velocity_local_input.normalized * Mathf.Min(maxSpeed, velocity_local_input.magnitude);
                }
                else
                {
                    //over max speed
                    if (velocity_local_input.magnitude <= maxSpeed) //Use new direction, would go less than max speed
                    {
                        updatedVelocity = velocity_local_input;
                    }
                    else //Would stay over max speed, use vector with smaller magnitude
                    {
                        //Would accelerate more, so don't user player input
                        if (velocity_local_input.magnitude > velocity_local_friction.magnitude)
                            updatedVelocity = velocity_local_friction;
                        else
                            //Would accelerate less, user player input (input moves velocity more to 0,0 than just friciton)
                            updatedVelocity = velocity_local_input;
                    }
                }
            }

            //Convert local velocity to global velocity
            rb.velocity = new Vector3(0, rb.velocity.y, 0) + modelOrientation.TransformDirection(updatedVelocity);
        }
        else
        {
            //Apply air fricion
            Vector3 velocity_local_friction = velocity_local.normalized * Mathf.Max(0, velocity_local.magnitude - frictionSpeed_air);

            Vector3 updatedVelocity = velocity_local_friction;

            if (currInput.magnitude > 0.05f) //Pressing something, try to accelerate
            {
                Vector3 velocity_local_input = velocity_local_friction + currInput * accelSpeed_air;

                if (velocity_local_friction.magnitude <= maxSpeed)
                {
                    //under max speed, accelerate towards max speed
                    updatedVelocity = velocity_local_input.normalized * Mathf.Min(maxSpeed, velocity_local_input.magnitude);
                }
                else
                {
                    //over max speed
                    if (velocity_local_input.magnitude <= maxSpeed) //Use new direction, would go less than max speed
                    {
                        updatedVelocity = velocity_local_input;
                    }
                    else //Would stay over max speed, use vector with smaller magnitude
                    {
                        //Would accelerate more, so don't user player input
                        if (velocity_local_input.magnitude > velocity_local_friction.magnitude)
                            updatedVelocity = velocity_local_friction;
                        else
                            //Would accelerate less, user player input (input moves velocity more to 0,0 than just friciton)
                            updatedVelocity = velocity_local_input;
                    }
                }
            }

            //Convert local velocity to global velocity
            rb.velocity = new Vector3(0, rb.velocity.y, 0) + modelOrientation.TransformDirection(updatedVelocity);
        }
        #endregion
    }
}