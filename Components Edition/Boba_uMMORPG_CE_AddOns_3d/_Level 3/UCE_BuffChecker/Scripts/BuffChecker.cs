// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BuffChecker

public class BuffChecker : MonoBehaviour
{
    [Header("[UCE BUFF CHECKER]")]
    public BuffCheckerEntry[] buffEntry;

    protected Entity entity;
    protected Animator animator;
    protected float cacheTimerInterval = 1.0f;
    protected float _cacheTimer;

    // -----------------------------------------------------------------------------------
    // Start
    // -----------------------------------------------------------------------------------
    private void Start()
    {
        entity = GetComponent<Entity>();
        animator = entity.animator;
    }

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    private void Update()
    {
        if (Time.time == 0 || Time.time > _cacheTimer)
        {
            if (entity == null) return;

            foreach (BuffCheckerEntry entry in buffEntry)
            {
                if (entry.isActive(entity))
                {
                    entry.ToggleGameObject(true);

                    if (entry.animationIndex != -1)
                        animator.SetInteger("IndexBuff", entry.animationIndex);
                }
                else
                {
                    entry.ToggleGameObject(false);

                    if (entry.animationIndex != -1)
                        animator.SetInteger("IndexBuff", 0);
                }
            }

            _cacheTimer = Time.time + cacheTimerInterval;
        }
    }

    // -----------------------------------------------------------------------------------
}
