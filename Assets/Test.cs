using UnityEngine;

public class Test : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private bool updateVelocity;
    [SerializeField] private float hSpeed;
    [SerializeField] private float gravSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (updateVelocity)
        {
            rb.velocity = new Vector3(0, rb.velocity.y - gravSpeed, hSpeed);
        }
    }
}
