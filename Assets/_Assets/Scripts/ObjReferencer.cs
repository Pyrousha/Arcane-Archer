using UnityEngine;

public class ObjReferencer : Singleton<ObjReferencer>
{
    [field: SerializeField] public Transform ArrowFire_Bow;
    [field: SerializeField] public GameObject ExplodeIndicator;
    [field: SerializeField] public ParticleSystem ExplodeEffect;
}