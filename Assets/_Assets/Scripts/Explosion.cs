using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : Singleton<Explosion>
{
    [SerializeField] private float explosionPower;

    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip[] clips;

    void OnTriggerEnter(Collider _col)
    {
        PlayerController.Instance.CanSpaceRelease = false;

        //Debug.Log(PlayerController.Instance.BowDrawPercent);
        Vector3 horizontalVelocity = (_col.transform.position - transform.position).normalized;
        horizontalVelocity.y = 0;
        horizontalVelocity *= explosionPower * PlayerController.Instance.BowDrawPercent * 0.5f;
        //Vector3 velocityToAdd = ((_col.transform.position + new Vector3(0, 3, 0)) - transform.position).normalized * explosionPower * PlayerController.Instance.BowDrawPercent;
        // Vector3 velocityToAdd = (_col.transform.position - transform.position);
        // velocityToAdd = (explosionPower / velocityToAdd.magnitude) * velocityToAdd.normalized;
        // Debug.Log(velocityToAdd);
        // Debug.Log(velocityToAdd.magnitude);
        Rigidbody rb = _col.gameObject.GetComponent<Rigidbody>();
        rb.velocity = new Vector3(rb.velocity.x, explosionPower * PlayerController.Instance.BowDrawPercent, rb.velocity.z);
        rb.velocity += horizontalVelocity;

        //Debug.DrawRay(_col.gameObject.transform.position, velocityToAdd, Color.red, 2f);
        //Debug.DrawRay(_col.gameObject.transform.position, velocityToAdd * 0.5f, Color.blue, 2f);
    }

    public void PlaySFX()
    {
        int rand = Random.Range(0, clips.Length);
        if (rand == clips.Length)
            rand = 0;
        source.clip = clips[rand];
        source.Play();
    }
}
