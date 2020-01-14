// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using System.Collections.Generic;

public class DontDestroyOnLoad : MonoBehaviour
{
    public Dictionary<string, DontDestroyOnLoad> singletons = new Dictionary<string, DontDestroyOnLoad>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // if we load the initial scene again then the object will exists twice
        // so let's make sure to delete any duplicates
        // -> its important to keep the exact ones so that server/client ids are
        //    the same
        if (!singletons.ContainsKey(name))
            singletons[name] = this;
        else Destroy(gameObject);
    }
}
