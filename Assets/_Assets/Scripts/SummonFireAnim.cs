using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonFireAnim : Singleton<SummonFireAnim>
{
    private Animator anim;
    [SerializeField] private ParticleSystem summonParticles;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void SummonFire()
    {
        summonParticles.Play();
        //anim.SetTrigger("SummonFire");
    }
}
