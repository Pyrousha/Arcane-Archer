using UnityEngine;

public class ObjReferencer : Singleton<ObjReferencer>
{
    [field: SerializeField] public Transform ArrowFire_Bow;
    [field: SerializeField] public GameObject ExplodeIndicator;
    //[field: SerializeField] public ParticleSystem ExplodeEffect;
    [field: SerializeField] public Transform ArrowFXParent;
    [field: SerializeField] public GameObject ExplodeEffectPrefab;
    [field: SerializeField] public Camera MainCamera;
    [field: SerializeField] public Transform SkyboxTransform { get; private set; }
    [field: SerializeField] public ParticleSystemModifier SpeedLines { get; private set; }

    private void Start()
    {
        MainCamera.fieldOfView = SaveData.CurrSaveData.Fov;
    }
}
