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

    AudioSource arrowSwooshSFX;

    private Transform playerTransform;
    //[SerializeField] private Animator explodeAnim;
    [SerializeField] private Animator arrowAnim;
    [SerializeField] private GameObject recallExplosionPrefab;
    [SerializeField] private ParticleSystem pSystem;
    [SerializeField] private AudioSource thunkSFX;
    [SerializeField] private float thunkVol;
    [SerializeField] private float swishVol;
    [Space(5)]
    [SerializeField] private float lerpSpeed;
    [SerializeField] private float explodeSpeed;
    [SerializeField] private float distToPlayer;

    private bool inRange = false;

    private Rigidbody rb;

    private bool bufferedExplosion = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        arrowSwooshSFX = GetComponent<AudioSource>();

        playerTransform = PlayerController.Instance.transform;

        distToPlayer *= distToPlayer;

        swishVol = arrowSwooshSFX.volume;
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

            thunkSFX.volume = thunkVol * SaveData.CurrSaveData.SfxVol;
            thunkSFX.Play();
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
                ObjReferencer.Instance.FilterController.SetFilterStatus(true);
                inRange = true;
            }
            else
            {
                BowLightIndicator.Instance.SetColor(BowLightIndicator.ColorStateEnum.CanBoom);
                ObjReferencer.Instance.FilterController.SetFilterStatus(false);
                inRange = false;
            }
        }

        if (bufferedExplosion)
        {
            if (!PlayerController.Instance.IsWithinArrowReleaseBuffer())
            {
                bufferedExplosion = false;
                Explode();
                return;
            }
        }
        else
        {
            if (PlayerController.Instance.IsWithinArrowReleaseBuffer() && InputHandler.Instance.Explode.Down_NoBufferRemoval())
                bufferedExplosion = true;
        }

        if ((state == ArrowStateEnum.InGround || state == ArrowStateEnum.Fired) &&
            (InputHandler.Instance.Explode.Down || bufferedExplosion))
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
            ObjReferencer.Instance.FilterController.SetFilterStatus(false);

            //BOOM!!!!
            Explosion.Instance.PlaySFX();

            Instantiate(ObjReferencer.Instance.ExplodeEffectPrefab, transform.position, Quaternion.identity, ObjReferencer.Instance.ArrowFXParent);
        }
        else
        {
            //Prevent recall if arrow was just fired
            if (PlayerController.Instance.IsWithinArrowReleaseBuffer())
                return;

            Instantiate(recallExplosionPrefab, transform.position, Quaternion.identity);
        }

        bufferedExplosion = false;

        ObjReferencer.Instance.ArrowRecallSFX.Play(0.125f);
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
        bufferedExplosion = false;

        arrowAnim.ResetTrigger("FadeOut");
        arrowAnim.SetTrigger("FadeIn");

        state = ArrowStateEnum.Fired;
        gameObject.layer = 11;

        gameObject.SetActive(true);
        rb.useGravity = true;

        transform.SetPositionAndRotation(targArrowPos.position, targArrowPos.rotation);
        transform.gameObject.SetActive(true);
        rb.velocity = targArrowPos.forward * arrowPower; //+ new Vector3(0, playerRB.velocity.y, 0);

        arrowSwooshSFX.volume = swishVol * SaveData.CurrSaveData.SfxVol;
        arrowSwooshSFX.Play();
    }

    public void Pickup()
    {
        arrowAnim.ResetTrigger("FadeIn");
        arrowAnim.SetTrigger("FadeOut");
        // gameObject.SetActive(false);

        state = ArrowStateEnum.Idle;
    }
}
