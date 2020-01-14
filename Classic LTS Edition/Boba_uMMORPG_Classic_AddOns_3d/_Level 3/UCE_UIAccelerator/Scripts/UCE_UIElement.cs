// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// UCE_UIElement

public abstract class UCE_UIElement : MonoBehaviour
{
    [Header("[-=-=- UCE UI Element -=-=-]")]
    [SerializeField] private bool throttleUpdate = true;

    [SerializeField] [Range(0.01f, 3f)] private float updateInterval = 0.25f;

    protected float fInterval;

    // -----------------------------------------------------------------------------------
    // Start
    // -----------------------------------------------------------------------------------
    private void Update()
    {
        if (!throttleUpdate || (throttleUpdate && Time.time > fInterval))
        {
            UCE_SlowUpdate();
            fInterval = Time.time + updateInterval;
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_SlowUpdate
    // -----------------------------------------------------------------------------------
    protected virtual void UCE_SlowUpdate() { }

    // -----------------------------------------------------------------------------------
}
