using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : Singleton<Music>
{
    public static Music music = null;

    private void Start()
    {
        if (music == null)
            music = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}
