using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonFireAnim : Singleton<SummonFireAnim>
{
    private Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void SummonFire()
    {
        anim.SetTrigger("SummonFire");
    }
}
