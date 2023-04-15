using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float explosionPower;

    void OnTriggerEnter(Collider _col)
    {
        //Debug.Log(PlayerController.Instance.BowDrawPercent);
        Vector3 velocityToAdd = ((_col.transform.position + new Vector3(0, 3, 0)) - transform.position).normalized * explosionPower * PlayerController.Instance.BowDrawPercent;
        // Vector3 velocityToAdd = (_col.transform.position - transform.position);
        // velocityToAdd = (explosionPower / velocityToAdd.magnitude) * velocityToAdd.normalized;
        // Debug.Log(velocityToAdd);
        // Debug.Log(velocityToAdd.magnitude);
        _col.gameObject.GetComponent<Rigidbody>().velocity += velocityToAdd;

        Debug.DrawRay(_col.gameObject.transform.position, velocityToAdd, Color.red, 2f);
        Debug.DrawRay(_col.gameObject.transform.position, velocityToAdd * 0.5f, Color.blue, 2f);
    }
}
