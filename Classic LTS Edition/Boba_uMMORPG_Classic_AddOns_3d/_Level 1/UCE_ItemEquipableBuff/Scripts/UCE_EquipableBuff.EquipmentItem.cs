// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// EQUIPMENT ITEM

public partial class EquipmentItem
{
    [Header("-=-=- UCE EQUIPMENT BUFFS -=-=-")]
    public TargetBuffSkill onEquipBuffSelf;

    public int onEquipBuffSelfLevel;

    [Header("--- On getting Hit Buff: Self ---")]
    public TargetBuffSkill onHitBuffSelf;

    public int onHitBuffSelfLevel;
    [Range(0, 1)] public float onHitBuffSelfApplyChance;

    [Header("--- On Attack Buff: Target ---")]
    public TargetBuffSkill onAttackBuff;

    public int onAttackBuffLevel;
    [Range(0, 1)] public float onAttackBuffApplyChance;
    public float maxApplyRange;

    // -----------------------------------------------------------------------------------
    // ApplyOnAttackBuffTarget
    // -----------------------------------------------------------------------------------
    public void ApplyOnAttackBuffTarget(Player player, Entity entity)
    {
        if (onAttackBuff == null || onAttackBuffApplyChance <= 0) return;

        if (UnityEngine.Random.value <= onAttackBuffApplyChance)
        {
            if (Utils.ClosestDistance(player, entity) <= maxApplyRange)
            {
                entity.AddOrRefreshBuff(new Buff(onAttackBuff, onAttackBuffLevel));
                entity.UCE_SpawnEffect(player, onAttackBuff);
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // ApplyOnHitBuffSelf
    // -----------------------------------------------------------------------------------
    public void ApplyOnHitBuffSelf(Player player)
    {
        if (onHitBuffSelf == null || onHitBuffSelfApplyChance <= 0) return;

        Entity oldTarget = null;

        // set target to player before applying buff
        if (player.target != null)
            oldTarget = player.target;

        player.target = player;

        if (UnityEngine.Random.value <= onHitBuffSelfApplyChance)
            onHitBuffSelf.Apply(player, onHitBuffSelfLevel);

        // restore target
        if (oldTarget != null)
            player.target = oldTarget;
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public void ApplyEquipableBuff(Player player)
    {
        if (onEquipBuffSelf == null) return;

        Entity oldTarget = null;

        // set target to player before applying buff
        if (player.target != null)
            oldTarget = player.target;

        player.target = player;

        onEquipBuffSelf.Apply(player, onEquipBuffSelfLevel);

        // restore target
        if (oldTarget != null)
            player.target = oldTarget;
    }

    // -----------------------------------------------------------------------------------
}
