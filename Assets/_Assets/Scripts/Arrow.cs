using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public enum ArrowStateEnum
    {
        Idle,
        Fired,
        InGround,
        FlyingBack
    }
    private ArrowStateEnum state;
    public ArrowStateEnum State => state;

    AudioSource audioSourceIDK;

    private Transform playerTransform;
    //[SerializeField] private Animator explodeAnim;
    [SerializeField] private Animator arrowAnim;
    [SerializeField] private GameObject recallExplosionPrefab;
    [SerializeField] private ParticleSystem pSystem;
    [Space(5)]
    [SerializeField] private float lerpSpeed;
    [SerializeField] private float explodeSpeed;
    [SerializeField] private float distToPlayer;

    private bool inRange = false;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSourceIDK = GetComponent<AudioSource>();

        playerTransform = PlayerController.Instance.transform;

        distToPlayer *= distToPlayer;
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case ArrowStateEnum.Idle:
                break;

            case ArrowStateEnum.Fired:
                if (rb.velocity.sqrMagnitude >= 12f)
                    transform.forward = rb.velocity.normalized;
                break;

            case ArrowStateEnum.InGround:
                rb.angularVelocity = Vector3.zero;
                rb.velocity = Vector3.zero;
                break;

            case ArrowStateEnum.FlyingBack:
                Vector3 targetForward = (playerTransform.position - transform.position).normalized;

                //Debug.Log(targetForward);

                //transform.forward = Vector3.RotateTowards(transform.forward, targetForward, lerpSpeed)

                transform.forward = Vector3.RotateTowards(transform.forward, targetForward, lerpSpeed * Mathf.Deg2Rad, 0);


                //Calculate new up-direction and rotate player
                Quaternion targRotation = Quaternion.LookRotation(targetForward);

                // //Lerp up-direction (or jump if close enough)
                // float angleDiff = Quaternion.Angle(transform.rotation, targRotation);
                // if (angleDiff > lerpSpeed)
                // {
                //     //Angle between current rotation and target rotation big enough to lerp
                //     transform.rotation = Quaternion.Lerp(transform.rotation, targRotation, lerpSpeed * (1f / angleDiff));
                // }
                // else
                // {
                //     //current and target rotation are close enough, jump value to stop lerp from going forever
                //     transform.rotation = targRotation;
                // }

                //rb.angularVelocity = Vector3.zero;
                rb.velocity = transform.forward * explodeSpeed;
                //rb.angularVelocity = Vector3.zero;
                break;
        }
    }

    private void OnCollisionEnter(Collision _col)
    {
        if (state == ArrowStateEnum.Fired)
        {
            state = ArrowStateEnum.InGround;

            //ObjReferencer.Instance.ExplodeEffect.transform.position = transform.position;
            ObjReferencer.Instance.ExplodeIndicator.transform.position = transform.position;

            ObjReferencer.Instance.ExplodeIndicator.SetActive(true);

            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
            rb.useGravity = false;
        }
    }

    void Update()
    {
        if (state == ArrowStateEnum.InGround)
        {
            float currDist = Vector3.SqrMagnitude(transform.position - playerTransform.position);
            if (currDist <= distToPlayer)
            {
                BowLightIndicator.Instance.SetColor(BowLightIndicator.ColorStateEnum.InRange);
                inRange = true;
            }
            else
            {
                BowLightIndicator.Instance.SetColor(BowLightIndicator.ColorStateEnum.CanBoom);
                inRange = false;
            }
        }

        if ((state == ArrowStateEnum.InGround || state == ArrowStateEnum.Fired) && InputHandler.Instance.Explode.Down)
            Explode();
    }

    public void Explode()
    {
        if (state == ArrowStateEnum.InGround)
        {
            if (inRange)
            {
                Explosion.Instance.BoomPlayer(distToPlayer);
                inRange = false;
            }

            BowLightIndicator.Instance.SetColor(BowLightIndicator.ColorStateEnum.Unlit);

            //BOOM!!!!
            Explosion.Instance.PlaySFX();

            Instantiate(ObjReferencer.Instance.ExplodeEffectPrefab, transform.position, Quaternion.identity, ObjReferencer.Instance.ArrowFXParent);
        }
        else
        {
            Instantiate(recallExplosionPrefab, transform.position, Quaternion.identity);
        }

        ObjReferencer.Instance.ExplodeIndicator.SetActive(false);

        state = ArrowStateEnum.FlyingBack;
        gameObject.layer = 12;

        pSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        StartCoroutine(DestroyRoutine());

        PlayerController.Instance.PickupArrow();
        Pickup();
    }

    private IEnumerator DestroyRoutine()
    {
        rb.useGravity = false;
        rb.velocity = Vector3.zero;

        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }

    public void Fire(Transform targArrowPos, float arrowPower)
    {
        arrowAnim.ResetTrigger("FadeOut");
        arrowAnim.SetTrigger("FadeIn");

        state = ArrowStateEnum.Fired;
        gameObject.layer = 11;

        gameObject.SetActive(true);
        rb.useGravity = true;

        transform.position = targArrowPos.position;
        transform.rotation = targArrowPos.rotation;
        transform.gameObject.SetActive(true);
        rb.velocity = targArrowPos.forward * arrowPower; //+ new Vector3(0, playerRB.velocity.y, 0);

        audioSourceIDK.volume = SaveData.CurrSaveData.SfxVol;
        audioSourceIDK.Play();
    }

    public void Pickup()
    {
        arrowAnim.ResetTrigger("FadeIn");
        arrowAnim.SetTrigger("FadeOut");
        // gameObject.SetActive(false);

        state = ArrowStateEnum.Idle;
    }
}
