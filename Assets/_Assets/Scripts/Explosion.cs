using UnityEngine;

public class Explosion : Singleton<Explosion>
{
    [SerializeField] private Renderer rangeObj;
    public Renderer RangeObj => rangeObj;

    [SerializeField] private float explosionPower;

    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip[] clips;

    public void BoomPlayer(float _sqrExplosionRadius)
    {
        PlayerController.Instance.CanSpaceRelease = false;

        Vector3 playerPos = PlayerController.Instance.BottomOfModel.position;

        Vector3 boomDir = playerPos - transform.position;
        boomDir.y = Mathf.Max(0f, boomDir.y);
        float dist = boomDir.magnitude;
        float maxDist = Mathf.Sqrt(_sqrExplosionRadius);

        float distPercent = Mathf.Min(dist * 2 / maxDist, 1);
        float invDistPercent = 1 - distPercent;

        //If the player is close to the arrow, prioritize going up, if they're farther away use the direction from the arrow to the player
        Vector3 newDir = Vector3.up * invDistPercent + boomDir.normalized * distPercent;
        newDir.Normalize();
        newDir.y = 1;
        //newDir += Vector3.up;

        Vector3 boomVelocity = explosionPower * PlayerController.Instance.BowDrawPercent * newDir;

        Vector3 currVelocity = PlayerController.Instance.RB.velocity;
        currVelocity.y = Mathf.Max((currVelocity.y * 0.35f) + boomVelocity.y, boomVelocity.y);
        currVelocity += new Vector3(boomVelocity.x, 0, boomVelocity.z);
        PlayerController.Instance.RB.velocity = currVelocity;
        PlayerController.Instance.DoScreenshake();

        //Vector3 horizontalVelocity = (_player.position - transform.position).normalized;
        //horizontalVelocity.y = 0;
        //horizontalVelocity *= explosionPower * PlayerController.Instance.BowDrawPercent * 0.5f;
        //Rigidbody rb = _player.gameObject.GetComponent<Rigidbody>();
        //rb.velocity = new Vector3(rb.velocity.x, explosionPower * PlayerController.Instance.BowDrawPercent, rb.velocity.z);
        //rb.velocity += horizontalVelocity;
    }

    public void PlaySFX()
    {
        int rand = Random.Range(0, clips.Length);
        if (rand == clips.Length)
            rand = 0;
        source.clip = clips[rand];

        source.volume = SaveData.CurrSaveData.SfxVol;
        source.Play();
    }
}
