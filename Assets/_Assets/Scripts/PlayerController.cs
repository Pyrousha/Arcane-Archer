using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    private Rigidbody rb;
    public Rigidbody RB => rb;
    [Header("Self-References")]
    [SerializeField] private Transform raycastParent;
    [SerializeField] private Animator bowAnim;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Transform shadowTransform;
    [field: SerializeField] public Transform BottomOfModel { get; private set; }

    [Header("External References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform targArrowPos;
    [SerializeField] private Arrow arrow;
    [SerializeField] private Transform arrowFire1;
    [SerializeField] private Transform arrowFire2;

    [Header("Parameters")]
    [SerializeField] private float arrowPower;
    [SerializeField] private float maxSpeed_Normal;
    [SerializeField] private float maxSpeed_Charging;
    [SerializeField] private float accelSpeed_ground;
    [SerializeField] private float frictionSpeed_ground;
    [SerializeField] private float accelSpeed_air;
    [SerializeField] private float frictionSpeed_air;

    [SerializeField] private float gravUp;
    [SerializeField] private float gravDown;
    [SerializeField] private float slamGrav;
    [SerializeField] private float spaceReleaseGravMult;
    [Space(5)]
    [Space(10)]
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float raycastHeight;

    [Header("Settings")]
    public static float turnSpeedX = 25;
    // public static float turnSpeedY = 50;

    private float bowDrawPercent = 0;
    public float BowDrawPercent => bowDrawPercent;

    float targHorizontalSpin;
    float targVerticalSpin;

    bool grounded = false;

    private List<Transform> raycastPoints = new List<Transform>();

    private float shootPickupDuration = 0.3f;
    private float nextShootPickupTime;

    public bool CanSpaceRelease = false;

    private bool levelOver = false;

    private bool releasedArrow = false;

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
        if (levelOver)
            return;

        //if (InputHandler.Instance.Restart.Down)
        //{
        //    SceneManager.LoadScene(1);
        //}

        //Jump
        if (InputHandler.Instance.Jump.Down)
        {
            if (grounded)
                rb.velocity += transform.up * jumpPower;
        }


        switch (bowState)
        {
            case BowStateEnum.Ready:
                if (InputHandler.Instance.Shoot.Down || InputHandler.Instance.Shoot.Holding)
                {
                    releasedArrow = false;
                    bowState = BowStateEnum.DrawBack;
                    bowAnim.SetTrigger("DrawBack");
                }
                break;
            case BowStateEnum.DrawBack:
                bowDrawPercent = Mathf.Min(1, bowAnim.GetCurrentAnimatorStateInfo(0).normalizedTime);
                SetFireSize(bowDrawPercent);

                if (InputHandler.Instance.Shoot.Up)
                {
                    releasedArrow = true;
                }

                if (bowDrawPercent >= 0.5f && releasedArrow)
                {
                    FireArrow();
                }

                break;
            case BowStateEnum.Fired:
                break;
        }

        //Camera Spin horizontal
        float amountToTurn = turnSpeedX * InputHandler.Instance.Look.x * Time.deltaTime;
        transform.rotation = Quaternion.AngleAxis(amountToTurn, transform.up) * transform.rotation;

        //Camera Spin vertical
        //targVerticalSpin -= turnSpeedY * InputHandler.Instance.Look.y * Time.deltaTime;
        targVerticalSpin -= turnSpeedX * InputHandler.Instance.Look.y * Time.deltaTime;
        targVerticalSpin = Mathf.Clamp(targVerticalSpin, -90f, 90f);
        cameraTarget.localRotation = Quaternion.Euler(targVerticalSpin, 0, 0);

        // //Make actual camera be facing in same direction as target
        cameraTransform.position = cameraTarget.position;
        cameraTransform.rotation = cameraTarget.rotation;

        //Jump
        if (InputHandler.Instance.Jump.Down)
        {
            if (grounded)
            {
                rb.velocity += transform.up * jumpPower;
                CanSpaceRelease = true;
            }
        }

        //Space release gravity
        if (InputHandler.Instance.Jump.Up && rb.velocity.y > 0 && CanSpaceRelease)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * spaceReleaseGravMult, rb.velocity.z);
        }
    }

    public void TryPickupArrow()
    {
        if (Time.time < nextShootPickupTime)
            return;

        if (InputHandler.Instance.Shoot.Down || arrow.State == Arrow.ArrowStateEnum.FlyingBack)
        {
            bowState = BowStateEnum.Ready;
            bowAnim.SetTrigger("Pickup");
            SummonFireAnim.Instance.SummonFire();

            arrow.Pickup();
        }
    }

    public void PickupArrow()
    {
        bowState = BowStateEnum.Ready;
        bowAnim.SetTrigger("Pickup");
    }

    private void FireArrow()
    {
        nextShootPickupTime = Time.time + shootPickupDuration;

        bowState = BowStateEnum.Fired;

        float firePower = arrowPower * Mathf.Clamp(bowAnim.GetCurrentAnimatorStateInfo(0).normalizedTime, 0.1f, 1.0f);
        firePower *= Mathf.Max(1, Vector3.Dot(targArrowPos.forward, rb.velocity * 0.1f));
        arrow.Fire(targArrowPos, firePower);

        bowAnim.SetTrigger("Fire");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (levelOver)
            return;

        // Debug.DrawRay(transform.position, transform.forward * 1.6f / 2f, new Color(1, 0, 0), 5f);
        // Debug.DrawRay(cameraTransform.position, cameraTransform.forward * 1.5f / 2f, new Color(0, 1, 0), 5f);

        #region Set position and scale of "shadow" object
        float newScale = 0;
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit _hit, 50, groundLayer))
        {
            shadowTransform.position = _hit.point;
            float distToGround = _hit.distance;

            //Shadow should be smaller the further away the character is from the ground
            newScale = Mathf.Max(1.5f - 0.1f * distToGround, 0.5f);
        }
        shadowTransform.localScale = new Vector3(newScale, shadowTransform.localScale.y, newScale);
        #endregion

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
        if (InputHandler.Instance.Slam.Holding)
        {
            rb.velocity -= new Vector3(0, slamGrav, 0);
        }
        else
        {
            if (InputHandler.Instance.Jump.Holding && rb.velocity.y > 0 && CanSpaceRelease)
                rb.velocity -= new Vector3(0, gravUp, 0);
            else
                rb.velocity -= new Vector3(0, gravDown, 0);
        }
        #endregion


        #region Acceleration
        //Get gravityless velocity
        Vector3 noGravVelocity = rb.velocity;
        noGravVelocity.y = 0;

        //Convert global velocity to local velocity
        Vector3 velocity_local = transform.InverseTransformDirection(noGravVelocity);

        float maxSpeed = maxSpeed_Normal;
        if (bowState == BowStateEnum.DrawBack)
            maxSpeed = maxSpeed_Charging;

        //XZ Friction + acceleration
        Vector3 currInput = new Vector3(InputHandler.Instance.MoveXZ.x, 0, InputHandler.Instance.MoveXZ.y);

        // if (useSlamGrav)
        //     currInput = Vector3.zero;

        if (InputHandler.Instance.Restart.Down)
            Debug.Log("a?");

        if (currInput.magnitude > 1f)
            currInput.Normalize();
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
            rb.velocity = new Vector3(0, rb.velocity.y, 0) + transform.TransformDirection(updatedVelocity);
        }
        else
        {
            //Apply air fricion
            Vector3 velocity_local_friction = velocity_local.normalized * Mathf.Max(0, velocity_local.magnitude - frictionSpeed_air);

            Vector3 updatedVelocity = velocity_local_friction;

            if (currInput.magnitude > 0.05f) //Pressing something, try to accelerate
            {
                Vector3 inputVect = currInput * accelSpeed_air;


                float angle = Mathf.Atan2(currInput.normalized.z, currInput.normalized.x);

                if (angle > Mathf.PI / 2.0f && angle < Mathf.PI)
                    Debug.Log("a");

                Vector3 velocity_local_with_input = velocity_local_friction + inputVect;

                if (velocity_local_friction.magnitude <= maxSpeed)
                {
                    //under max speed, accelerate towards max speed
                    updatedVelocity = velocity_local_with_input.normalized * Mathf.Min(maxSpeed, velocity_local_with_input.magnitude);
                }
                else
                {
                    float velocityOntoInput = Vector3.Project(velocity_local_with_input, inputVect).magnitude;
                    if (Vector3.Dot(velocity_local_with_input, inputVect) < 0)
                        velocityOntoInput *= -1;

                    Debug.Log(velocityOntoInput);
                    if (velocityOntoInput <= maxSpeed)
                    {
                        //Speed in direction of input lower than maxSpeed
                        updatedVelocity = velocity_local_with_input;
                    }
                    else
                    {
                        //Would accelerate more, so don't user player input directly

                        Vector3 velocityOntoFriction = Vector3.Project(velocity_local_friction, inputVect);

                        Vector3 perp = velocity_local_friction - velocityOntoFriction;

                        //Accelerate towards max speed
                        float amountToAdd = Mathf.Max(0, Mathf.Min(maxSpeed - velocityOntoFriction.magnitude, inputVect.magnitude));
                        float perpAmountToSubtract = Mathf.Max(0, Mathf.Min(accelSpeed_air - amountToAdd, perp.magnitude));

                        perp = perp.normalized * perpAmountToSubtract;

                        updatedVelocity = velocity_local_friction + amountToAdd * inputVect.normalized - perp;
                    }
                    ////over max speed
                    //if (velocity_local_with_input.magnitude <= maxSpeed) //Use new direction, would go less than max speed
                    //{

                    //}
                    //else //Would stay over max speed, use vector with smaller magnitude
                    //{
                    //    if (velocity_local_with_input.magnitude <= velocity_local_friction.magnitude)
                    //        //Would accelerate less, user player input (input moves velocity more to 0,0 than just friciton)
                    //        updatedVelocity = velocity_local_with_input;
                    //    else
                    //    {

                    //    }
                    //}
                }
            }

            //Debug.Log(updatedVelocity);

            //Convert local velocity to global velocity
            rb.velocity = new Vector3(0, rb.velocity.y, 0) + transform.TransformDirection(updatedVelocity);
        }
        #endregion
    }

    public void SetFireSize(float _percentage)
    {
        arrowFire1.localScale = Vector3.one * _percentage;
        arrowFire2.localScale = Vector3.one * _percentage;
    }

    public void OnLevelEnd()
    {
        levelOver = true;
        rb.velocity = Vector3.zero;
        rb.useGravity = false;

        Cursor.lockState = CursorLockMode.None;
    }
}
