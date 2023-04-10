using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickupHitbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerController.Instance.PickupArrow();
    }
}
