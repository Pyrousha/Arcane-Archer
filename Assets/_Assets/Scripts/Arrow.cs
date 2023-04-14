using System;
using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private Transform player;
    private Rigidbody playerRB;
    [SerializeField] private ParticleSystem explosionEffect;
    [SerializeField] private Animator explodeAnim;
    [Space(5)]
    [SerializeField] private float lerpSpeed;
    [SerializeField] private float explodeSpeed;
    private Transform explosionTransform;

    private Rigidbody rb;

    void Awake()
    {
        explosionTransform = explosionEffect.gameObject.transform;
        rb = GetComponent<Rigidbody>();
        playerRB = player.GetComponent<Rigidbody>();
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
                Vector3 targetForward = (player.position - transform.position).normalized;

                //transform.forward = Vector3.RotateTowards(transform.forward, targetForward, lerpSpeed)


                //Calculate new up-direction and rotate player
                Quaternion targRotation = Quaternion.LookRotation(targetForward);

                //Lerp up-direction (or jump if close enough)
                float angleDiff = Quaternion.Angle(transform.rotation, targRotation);
                if (angleDiff > lerpSpeed)
                {
                    //Angle between current rotation and target rotation big enough to lerp
                    transform.rotation = Quaternion.Lerp(transform.rotation, targRotation, lerpSpeed * (1f / angleDiff));
                }
                else
                {
                    //current and target rotation are close enough, jump value to stop lerp from going forever
                    transform.rotation = targRotation;
                }

                rb.angularVelocity = Vector3.zero;
                rb.velocity = transform.forward * explodeSpeed;
                rb.angularVelocity = Vector3.zero;
                break;
        }
    }

    private void OnCollisionEnter(Collision _col)
    {
        if (state == ArrowStateEnum.Fired)
        {
            state = ArrowStateEnum.InGround;

            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
            rb.useGravity = false;
        }
    }

    void Update()
    {
        if (InputHandler.Instance.Explode.Down && (state == ArrowStateEnum.InGround || state == ArrowStateEnum.Fired))
            Explode();
    }

    public void Explode()
    {
        state = ArrowStateEnum.FlyingBack;

        explosionTransform.position = transform.position;
        explosionEffect.Play();

        explodeAnim.SetTrigger("Explode");

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.up), 1f);

        rb.useGravity = false;
    }

    public void Fire(Transform targArrowPos, float arrowPower)
    {
        state = ArrowStateEnum.Fired;

        gameObject.SetActive(true);
        rb.useGravity = true;

        transform.position = targArrowPos.position;
        transform.rotation = targArrowPos.rotation;
        transform.gameObject.SetActive(true);
        rb.velocity = targArrowPos.forward * arrowPower; //+ new Vector3(0, playerRB.velocity.y, 0);
    }

    public void Pickup()
    {
        gameObject.SetActive(false);

        state = ArrowStateEnum.Idle;
    }
}
