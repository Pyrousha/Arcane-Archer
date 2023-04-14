using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickupHitbox : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        PlayerController.Instance.TryPickupArrow();
    }
}
