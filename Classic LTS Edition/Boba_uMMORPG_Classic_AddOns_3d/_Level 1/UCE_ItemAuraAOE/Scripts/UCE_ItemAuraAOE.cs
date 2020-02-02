// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System.Collections.Generic;
using UnityEngine;

// ITEM TARGETTED AOE

[CreateAssetMenu(menuName = "uMMORPG Item/UCE Item Aura AOE", order = 999)]
public class UCE_ItemAuraAOE : UsableItem
{
    [Header("-=-=-=- UCE Target AOE Item -=-=-=-")]
    public OneTimeTargetSkillEffect effect;

    public LinearInt damage = new LinearInt { baseValue = 1 };

    [Tooltip("[Optional] Add caster damage to total damage or not?")]
    public bool useCasterDamage;

#if _iMMOATTRIBUTES

    [Tooltip("[Optional] Add caster accuracy to the buff chance?")]
    public bool useCasterAccuracy;

#endif
    public LinearFloat stunChance; // range [0,1]
    public LinearFloat stunTime; // in seconds
    public int skillLevel;
    public float radius;
    [Range(0, 1)] public float triggerAggroChance;
    public string successMessage = "You damaged: ";

    [Header("-=-=-=- Apply Buff on Target -=-=-=-")]
    public BuffSkill applyBuff;

    public int buffLevel;
    [Range(0, 1)] public float buffChance;

    [Header("-=-=-=- Targeting -=-=-=-")]
    [Tooltip("[Optional] Changes 'affect' affect into 'not affect' and vice-versa")]
    public bool reverseTargeting;

    [Tooltip("[Optional] Does affect the caster")]
    public bool affectSelf;

    [Tooltip("[Optional] Does affect members of the own party")]
    public bool affectOwnParty;

    [Tooltip("[Optional] Does affect members of the own guild")]
    public bool affectOwnGuild;

    [Tooltip("[Optional] Does affect members of the own realm (requires UCE PVP ZONE AddOn")]
    public bool affectOwnRealm;

    public bool affectPlayers;
    public bool affectMonsters;
    public bool affectPets;

    [Tooltip("Decrease amount by how many each use (can be 0)?")]
    public int decreaseAmount = 1;

    // -----------------------------------------------------------------------------------
    // SpawnEffect
    // -----------------------------------------------------------------------------------
    public void SpawnEffect(Entity caster, Entity spawnTarget)
    {
        if (effect != null)
        {
            GameObject go = Instantiate(effect.gameObject, spawnTarget.transform.position, Quaternion.identity);
            go.GetComponent<OneTimeTargetSkillEffect>().caster = caster;
            go.GetComponent<OneTimeTargetSkillEffect>().target = spawnTarget;
            NetworkServer.Spawn(go);
        }
    }

    // -----------------------------------------------------------------------------------
    // CheckTarget
    // -----------------------------------------------------------------------------------
    public bool CheckTarget(Entity caster)
    {
        return true;
    }

    // -----------------------------------------------------------------------------------
    // CheckDistance
    // -----------------------------------------------------------------------------------
    public bool CheckDistance(Entity caster, int skillLevel, out Vector3 destination)
    {
        destination = caster.transform.position;
        return true;
    }

    // -----------------------------------------------------------------------------------
    // Use
    // -----------------------------------------------------------------------------------
    public override void Use(Player player, int inventoryIndex)
    {
        List<Entity> targets = new List<Entity>();
        ItemSlot slot = player.inventory[inventoryIndex];

        // -- Only activate if enough charges left
        if (decreaseAmount == 0 || slot.amount >= decreaseAmount)
        {
            // always call base function too
            base.Use(player, inventoryIndex);

            targets = player.UCE_GetCorrectedTargetsInSphere(player.transform, radius, false, affectSelf, affectOwnParty, affectOwnGuild, affectOwnRealm, reverseTargeting, affectPlayers, affectMonsters, affectPets);

            foreach (Entity target in targets)
            {
                int dmg = damage.Get(skillLevel);

                if (useCasterDamage)
                    dmg += player.damage;

                float buffModifier = 0;
#if _iMMOATTRIBUTES
                if (useCasterAccuracy) buffModifier = target.UCE_HarmonizeChance(buffModifier, player.accuracy);
#endif

                // deal damage directly with base damage + skill damage
                player.DealDamageAt(target,
                            dmg,
                            stunChance.Get(skillLevel),
                            stunTime.Get(skillLevel));

                target.UCE_ApplyBuff(applyBuff, buffLevel, buffChance, buffModifier);
                player.UCE_TargetAddMessage(successMessage + target.name);

                SpawnEffect(player, player.target);

                if (UnityEngine.Random.value <= triggerAggroChance)
                    target.target = player;
            }

            // decrease amount
            slot.DecreaseAmount(decreaseAmount);
            player.inventory[inventoryIndex] = slot;
        }

        targets.Clear();
    }

    // -----------------------------------------------------------------------------------
}
