using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    [Header("Self-References")]
    [SerializeField] private Transform raycastParent;
    [SerializeField] private Animator bowAnim;
    [SerializeField] private Transform cameraTarget;

    [Header("External References")]
    [SerializeField] private Transform cameraTransform;

    [Header("Parameters")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float accelSpeed_ground;
    [SerializeField] private float frictionSpeed_ground;
    [SerializeField] private float accelSpeed_air;
    [SerializeField] private float frictionSpeed_air;
    [Space(10)]
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float raycastHeight;

    [Header("Settings")]
    [SerializeField] private float turnSpeedX;
    [SerializeField] private float turnSpeedY;

    float targHorizontalSpin;
    float targVerticalSpin;

    bool grounded = false;

    private Vector3 currNetGrav_Global = Vector3.down;

    private List<Transform> raycastPoints = new List<Transform>();

    private enum BowStateEnum
    {
        Ready,
        DrawBack,
        Fired
    }
    private BowStateEnum bowState;

    void Awake()
    {
        //Cursor.lockState = CursorLockMode.Locked;

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
                    bowState = BowStateEnum.Fired;
                    bowAnim.SetTrigger("Fire");
                }
                break;
            case BowStateEnum.Fired:
                break;
        }

        //Camera Spin horizontal
        float amountToTurn = turnSpeedX * InputHandler.Instance.Look.x * Time.deltaTime;
        transform.rotation = Quaternion.AngleAxis(amountToTurn, transform.up) * transform.rotation;

        //Camera Spin vertical
        targVerticalSpin -= turnSpeedY * InputHandler.Instance.Look.y * Time.deltaTime;
        targVerticalSpin = Mathf.Clamp(targVerticalSpin, -80.5f, 80.5f);
        cameraTarget.localRotation = Quaternion.Euler(targVerticalSpin, 0, 0);

        // //Make actual camera be facing in same direction as target
        cameraTransform.position = cameraTarget.position;
        cameraTransform.rotation = cameraTarget.rotation;

        //Jump
        if (InputHandler.Instance.Jump.Down)
        {
            if (grounded)
                rb.velocity += transform.up * jumpPower;
        }
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

        #region Calculate and apply Gravity
        //Apply gravity
        rb.velocity += currNetGrav_Global;
        #endregion


        #region Acceleration
        //Get gravityless velocity
        Vector3 onlyGravVelocity = Vector3.Project(rb.velocity, currNetGrav_Global.normalized);
        Vector3 noGravVelocity = rb.velocity - onlyGravVelocity;

        //Convert global velocity to local velocity
        Vector3 velocity_local = transform.InverseTransformDirection(noGravVelocity);


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
            rb.velocity = onlyGravVelocity + transform.TransformDirection(updatedVelocity);
        }
        #endregion
    }
}
