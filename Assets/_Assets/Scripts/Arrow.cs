using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private ParticleSystem explosionEffect;
    [Space(5)]
    [SerializeField] private float lerpSpeed;
    [SerializeField] private float explodeSpeed;
    private Transform explosionTransform;

    private Rigidbody rb;

    private bool canExplode;
    private bool flyBack;

    void Awake()
    {
        explosionTransform = explosionEffect.gameObject.transform;
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (flyBack)
        {
            Vector3 targetForward = (player.position - transform.position).normalized;

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
        }
        else
        {
            if (rb.velocity.sqrMagnitude >= 12f)
                transform.forward = rb.velocity.normalized;
        }
    }

    void Update()
    {
        if (InputHandler.Instance.Explode.Down && canExplode)
            Explode();
    }

    public void Explode()
    {
        explosionTransform.position = transform.position;
        explosionEffect.Play();

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.up), 1f);

        canExplode = false;
        flyBack = true;
        rb.useGravity = false;
    }

    public void Fire(Transform targArrowPos, float arrowPower)
    {
        gameObject.SetActive(true);
        flyBack = false;
        rb.useGravity = true;

        transform.position = targArrowPos.position;
        transform.rotation = targArrowPos.rotation;
        transform.gameObject.SetActive(true);
        rb.velocity = targArrowPos.forward * arrowPower;

        canExplode = true;
    }

    public void Pickup()
    {
        gameObject.SetActive(false);

        canExplode = false;
    }
}
