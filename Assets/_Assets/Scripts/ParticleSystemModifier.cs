using UnityEngine;

public class ParticleSystemModifier : MonoBehaviour
{
    [SerializeField] private bool modifySpeed = true;
    [SerializeField] private float minStartSpeed;
    [SerializeField] private float maxStartSpeed;

    [SerializeField] private bool modifyRadius = true;
    [SerializeField] private float minRadius;
    [SerializeField] private float maxRadius;

    [SerializeField] private bool modifyEmission = true;
    [SerializeField] private float minEmission;
    [SerializeField] private float maxEmission;

    private ParticleSystem pSystem;

    // Start is called before the first frame update
    void Start()
    {
        pSystem = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    public void UpdateParticleSystem(float percent)
    {
        if (modifySpeed)
            pSystem.startSpeed = Mathf.Lerp(minStartSpeed, maxStartSpeed, percent);

        if (modifyRadius)
        {
            var shape = pSystem.shape;
            shape.radius = Mathf.Lerp(minRadius, maxRadius, percent);
        }

        if (modifyEmission)
            pSystem.emissionRate = Mathf.Lerp(minEmission, maxEmission, percent);
    }
}
