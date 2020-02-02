// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System.Linq;
using UnityEngine;

// PLAYER

public partial class Player
{
    [Header("[-=-=- UCE WEIGHT SYSTEM -=-=-]")]
    public UCE_WeightSystem weightSystem;

    protected int totalWeight;
    protected int maxWeight;
    protected float _updateTimer;

    // -----------------------------------------------------------------------------------
    // Update_UCE_WeightSystem
    // @Server
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    [DevExtMethods("Update")]
    private void Update_UCE_WeightSystem()
    {
        if (weightSystem.burdenedBuff == null || !isServer) return;

        // -- Delayed Update (once per second instead of once per frame)

        if (Time.time > _updateTimer)
        {
            // -- calculate weight
            UCE_CalculateWeight();

            if (totalWeight <= 0) return;

            // -- check burdened
            int burdenLevel = UCE_IsBurdened();

            // -- apply or remove burdened
            if (burdenLevel > 0)
            {
                skills.AddOrRefreshBuff(new Buff(weightSystem.burdenedBuff, burdenLevel));
            }
            else
            {
                UCE_RemoveBuff(weightSystem.burdenedBuff);
            }

            _updateTimer = Time.time + cacheTimerInterval;
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_CalculateWeight
    // -----------------------------------------------------------------------------------
    protected void UCE_CalculateWeight()
    {
        totalWeight = 0;

#if _iMMOATTRIBUTES
        if (weightSystem.weightAttribute != null)
        {
            UCE_Attribute attrib = playerAttributes.UCE_Attributes.FirstOrDefault(x => x.template == weightSystem.weightAttribute);
            maxWeight = weightSystem.carryPerPoint + ((attrib.points + playerAttributes.UCE_calculateBonusAttribute(attrib)) * weightSystem.carryPerPoint);
        }
#endif

        for (int i = 0; i < inventory.slots.Count; ++i)
        {
            ItemSlot slot = inventory.slots[i];
            if (slot.amount > 0)
                totalWeight += slot.item.data.weight * slot.amount;
        }

        for (int i = 0; i < equipment.slots.Count; ++i)
        {
            ItemSlot slot = equipment.slots[i];
            if (slot.amount > 0)
                totalWeight += slot.item.data.weight * slot.amount;
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_IsBurdened
    // -----------------------------------------------------------------------------------
    protected int UCE_IsBurdened()
    {
        if (totalWeight <= maxWeight)
        {
            return 0;
        }
        else
        {
            return Mathf.Min((int)totalWeight / maxWeight, weightSystem.maxBurdenLevel);
        }
    }

    // -----------------------------------------------------------------------------------
}
