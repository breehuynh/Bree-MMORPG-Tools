// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System.Text;
using UnityEngine;

// UCE DAMAGE SKILL

public abstract partial class UCE_DamageSkill : ScriptableSkill
{
    [Header("[-=-=- UCE DAMAGE SKILL -=-=-]")]
    [Tooltip("[Required] Base damage of the attack.")]
    public LinearInt damage = new LinearInt { baseValue = 1 };

    [Tooltip("[Optional] Add Damage stat of caster to modify attack damage?")]
    public bool addCasterDamage;

    [Tooltip("[Optional] Base stun chance, modified by targets Resistance.")]
    public LinearFloat stunChance;

    [Tooltip("[Optional] Add Accuracy stat of caster to modify buff chance?")]
    public bool stunAddAccuracy;

    [Tooltip("[Optional] Minimum duration of the stun (in seconds).")]
    public LinearFloat minStunTime;

    [Tooltip("[Optional] Maximum duration of the stun (in seconds).")]
    public LinearFloat maxStunTime;

#if _iMMOPROJECTILES

    [Header("[Projectile - required for Projectile Skill / Optional for Melee Skill]")]
    [Tooltip("[Optional] The main projectile that will be spawned.")]
    public UCE_Projectile projectile;

    [Tooltip("[Optional] Projectile movement speed (Arrow/Gunshot ca. 35).")]
    public LinearFloat projectileSpeed;

    [Tooltip("[Optional] Amount of additional projectiles to spawn.")]
    public LinearInt sidekickAmount;

    [Tooltip("[Optional] Chance for each sidekick to be spawned (0-1 where 1 = 100%)")]
    public LinearFloat sidekickSpawnChance;

    [Tooltip("[Optional] Delay in seconds allows to stagger sidekicks when spawned.")]
    public float sidekickSpawnDelay;

    [Tooltip("[Optional] Rotation of sidekicks will vary by a random degree up to this.")]
    [Range(0, 360)] public float sidekickSpreadAngle;
#endif

    [Header("[Ammunition - and/or use default Ammo system]")]
    [Tooltip("[Required] Having ANY of these weapons equipped is enough for the skill to be useable")]
    public ScriptableItem[] equippedWeapons;

    [Tooltip("[Required] The ammunition must be in the players inventory")]
    public ScriptableItem[] requiredAmmo;

    [Tooltip("[Required] How much ammo is deducted per skill use?")]
    public int ammoAmount;

    [Header("[Visual Effects]")]
    [Tooltip("[Optional] GameObject spawned at the effect mount of the caster")]
    public GameObject muzzleEffect;

    [Tooltip("[Optional] GameObject spawned on the impact target or wall")]
    public GameObject impactEffect;

    [Header("[Recoil Target or Caster]")]
    [Tooltip("[Optional] Amount of units the caster is recoiled (can be negative)")]
    public LinearFloat recoilCaster;

    [Tooltip("[Optional] Min Amount of units the target is recoiled (can be negative)")]
    public LinearFloat minRecoilTarget;

    [Tooltip("[Optional] Max Amount of units the target is recoiled (can be negative)")]
    public LinearFloat maxRecoilTarget;

    [Tooltip("[Optional] Chance to recoil the target (0-1 where 1 = 100%")]
    public LinearFloat recoilChance;

    [Tooltip("[Optional] Add Accuracy stat of caster to modify recoil chance?")]
    public bool recoilAddAccuracy;

    [Header("[Cooldown Target]")]
    [Tooltip("[Optional] Chance to cooldown each skill on the target (0-1 where 1 = 100%")]
    public LinearFloat cooldownChance;

    [Tooltip("[Optional] Duration in seconds the cooldown ins modified?")]
    public LinearInt cooldownDuration;

    [Tooltip("[Optional] Add Accuracy stat of caster to modify cooldown chance?")]
    public bool cooldownAddAccuracy;

    [Header("[Buff Target]")]
    [Tooltip("[Optional] Buff applied to the target(s) (skill level dependant)")]
    public BuffSkill[] applyBuff;

    [Tooltip("[Optional] Level of the buff that is applied")]
    public LinearInt buffLevel;

    [Tooltip("[Optional] Chance of applying a buff (0-1 where 1 = 100%")]
    public LinearFloat buffChance;

    [Tooltip("[Optional] Add Accuracy stat of caster to modify buff chance?")]
    public bool buffAddAccuracy;

    [Header("[Debuff Target]")]
    [Tooltip("Remove up to x Buffs (positive status effects) from the target")]
    public LinearInt removeRandomBuff;

    [Tooltip("[Optional] Chance of removing a buff (0-1 where 1 = 100%")]
    public LinearFloat removeChance;

    [Tooltip("[Optional] Add Accuracy stat of caster to modify buff removal chance?")]
    public bool removeAddAccuracy;

    [Header("[Buff Caster]")]
    [Tooltip("[Optional] Buff applied to the caster (skill level dependant)")]
    public BuffSkill[] applyCasterBuff;

    [Tooltip("[Optional] Level of the buff that is applied")]
    public LinearInt buffCasterLevel;

    [Tooltip("[Optional] Chance of applying a buff (0-1 where 1 = 100%")]
    public LinearFloat buffCasterChance;

    [Header("[Debuff Caster]")]
    [Tooltip("Remove up to x Buffs (positive status effects) from the caster")]
    public LinearInt removeRandomCasterBuff;

    [Tooltip("[Optional] Chance of removing a buff (0-1 where 1 = 100%")]
    public LinearFloat removeCasterBuffChance;

    [Header("[Target AOE Effect]")]
    [Tooltip("[Optional] Radius in meters, affects all Entities within")]
    public LinearFloat impactRadius;

    [Tooltip("[Optional] Chance to trigger Aggro when any other entity but the main target is hit")]
    public LinearFloat triggerAggroChance;

    [Tooltip("[Optional] Spawn the impact effect only on the main target or on all?")]
    public bool visualEffectOnMainTargetOnly;

    [Header("[AOE Targeting]")]
    [Tooltip("[Optional] Changes 'not' affect into 'affect only'")]
    public bool reverseTargeting;

    [Tooltip("[Optional] Does 'not' affect the caster")]
    public bool notAffectSelf;

    [Tooltip("[Optional] Does 'not' affect members of the own party")]
    public bool notAffectOwnParty;

    [Tooltip("[Optional] Does 'not' affect members of the own guild")]
    public bool notAffectOwnGuild;

    [Tooltip("[Optional] Does 'not' affect members of the own realm (requires UCE PVP ZONE AddOn")]
    public bool notAffectOwnRealm;

    public bool notAffectPlayers;
    public bool notAffectMonsters;
    public bool notAffectPets;

    [Header("[Create GameObject underneath Target]")]
    [Tooltip("[Optional] Spawn GameObject underneath target (one entry per skill level, so one object per level)")]
    public GameObject[] createOnTarget;

    [Tooltip("[Optional] Chance to create all gameobjects (0-1 where 1 = 100%)")]
    public LinearFloat createChance;

    [Header("[Projectile Sidekick Settings]")]
    [Tooltip("[Optional] Sidekick projectiles do not apply a stun on their targets.")]
    public bool sidekicksDontStun;

    [Tooltip("[Optional] Sidekick projectiles do not apply a buff on their targets.")]
    public bool sidekicksDontBuff;

    [Tooltip("[Optional] Sidekick projectiles do not apply a recoil on their targets.")]
    public bool sidekicksDontRecoil;

    [Tooltip("[Optional] Sidekick projectiles do not debuff their targets.")]
    public bool sidekicksDontDebuff;

    [Tooltip("[Optional] Sidekick projectiles do feature a AOE effect.")]
    public bool sidekicksDontAOE;

    [Tooltip("[Optional] Sidekick projectiles do not create a object underneath their targets.")]
    public bool sidekicksDontCreateObject;

    protected Entity _caster;
    protected int _skillLevel;

    // -----------------------------------------------------------------------------------
    // Apply
    // -----------------------------------------------------------------------------------
    public override void Apply(Entity caster, int skillLevel)
    {
        _caster = caster;
        _skillLevel = skillLevel;

        // ------ consume ammo if needed
        ConsumeRequiredWeaponsAmmo(caster);         // default ammo system
        ConsumeAmmunition(caster);                  // custom ammo system

        // ------ create muzzle effect if any
        if (muzzleEffect != null)
        {
            GameObject go = Instantiate(muzzleEffect.gameObject, caster.skills.effectMount.position, caster.skills.effectMount.rotation);
            NetworkServer.Spawn(go);
        }

#if _iMMOPROJECTILES
        // ------ spawn one or more projectiles if any
        if (projectile != null)
            SpawnProjectile();
#endif

        // ------ Apply Caster Buff
        if (applyCasterBuff.Length > 0 && applyCasterBuff.Length >= skillLevel && applyCasterBuff[skillLevel - 1] != null)
            caster.UCE_ApplyBuff(applyCasterBuff[skillLevel - 1], buffCasterLevel.Get(skillLevel), buffCasterChance.Get(skillLevel));

        // ------ Remove Caster Buff
        if (removeRandomCasterBuff.Get(skillLevel) > 0 && caster.skills.buffs.Count > 0)
            caster.UCE_CleanupStatusBuffs(removeCasterBuffChance.Get(skillLevel), 0, removeRandomCasterBuff.Get(skillLevel));

        // ------ Recoil Caster
        if (recoilCaster.Get(skillLevel) > 0)
            caster.UCE_Recoil(caster, recoilCaster.Get(skillLevel));
    }

    // -----------------------------------------------------------------------------------
    // SpawnProjectile
    // -----------------------------------------------------------------------------------
#if _iMMOPROJECTILES

    protected void SpawnProjectile()
    {
        GameObject go = Instantiate(projectile.gameObject, _caster.skills.effectMount.position, _caster.skills.effectMount.rotation);

        UCE_Projectile effect = go.GetComponent<UCE_Projectile>();

        effect.speed = projectileSpeed.Get(_skillLevel);

        effect.target = _caster.target;
        effect.caster = _caster;
        effect.data.speed = projectileSpeed.Get(_skillLevel);
        effect.data.distance = castRange.Get(_skillLevel);

        effect.data.skillLevel = _skillLevel;
        effect.data.damage = damage.Get(_skillLevel);
        effect.data.addCasterDamage = addCasterDamage;

        effect.data.stunChance = stunChance.Get(_skillLevel);
        effect.data.stunAddAccuracy = stunAddAccuracy;
        effect.data.minStunTime = minStunTime.Get(_skillLevel);
        effect.data.maxStunTime = maxStunTime.Get(_skillLevel);

        effect.data.sidekick = false;
        effect.data.sidekickSpawnChance = sidekickSpawnChance.Get(_skillLevel);
        effect.data.sidekickAmount = sidekickAmount.Get(_skillLevel);
        effect.data.sidekickSpawnDelay = sidekickSpawnDelay;
        effect.data.sidekickSpreadAngle = sidekickSpreadAngle;

        effect.data.recoilCaster = recoilCaster.Get(_skillLevel);
        effect.data.minRecoilTarget = minRecoilTarget.Get(_skillLevel);
        effect.data.maxRecoilTarget = maxRecoilTarget.Get(_skillLevel);
        effect.data.recoilChance = recoilChance.Get(_skillLevel);
        effect.data.recoilAddAccuracy = recoilAddAccuracy;

        effect.data.cooldownChance = cooldownChance.Get(_skillLevel);
        effect.data.cooldownDuration = cooldownDuration.Get(_skillLevel);
        effect.data.cooldownAddAccuracy = cooldownAddAccuracy;

        if (applyBuff.Length > 0 && applyBuff.Length >= _skillLevel - 1)
        {
            effect.data.applyBuff = applyBuff[_skillLevel - 1];
            effect.data.buffLevel = buffLevel.Get(_skillLevel);
            effect.data.buffChance = buffChance.Get(_skillLevel);
            effect.data.buffAddAccuracy = buffAddAccuracy;
        }

        effect.data.impactEffect = impactEffect;
        effect.data.impactRadius = impactRadius.Get(_skillLevel);
        effect.data.triggerAggroChance = triggerAggroChance.Get(_skillLevel);
        effect.data.visualEffectOnMainTargetOnly = visualEffectOnMainTargetOnly;
        effect.data.reverseTargeting = reverseTargeting;
        effect.data.notAffectSelf = notAffectSelf;
        effect.data.notAffectOwnParty = notAffectOwnParty;
        effect.data.notAffectOwnGuild = notAffectOwnGuild;
        effect.data.notAffectOwnRealm = notAffectOwnRealm;
		effect.data.notAffectPlayers = notAffectPlayers;
		effect.data.notAffectMonsters = notAffectMonsters;
		effect.data.notAffectPets = notAffectPets;

        effect.data.removeRandomBuff = removeRandomBuff.Get(_skillLevel);
        effect.data.removeChance = removeChance.Get(_skillLevel);
        effect.data.removeAddAccuracy = removeAddAccuracy;

        effect.data.createOnTarget = createOnTarget;
        effect.data.createChance = createChance.Get(_skillLevel);

        effect.data.sidekicksDontStun = sidekicksDontStun;
        effect.data.sidekicksDontBuff = sidekicksDontBuff;
        effect.data.sidekicksDontRecoil = sidekicksDontRecoil;
        effect.data.sidekicksDontDebuff = sidekicksDontDebuff;
        effect.data.sidekicksDontAOE = sidekicksDontAOE;
        effect.data.sidekicksDontCreateObject = sidekicksDontCreateObject;

        effect.Init();

        NetworkServer.Spawn(go);
    }

#endif

    // ============================= HELPER FUNCTIONS ====================================

    // -----------------------------------------------------------------------------------
    // CheckAmmunition
    // -----------------------------------------------------------------------------------
    public bool CheckAmmunition(Entity caster)
    {
        if (equippedWeapons.Length <= 0 && requiredAmmo == null) return true;

        bool valid = false;

        if (equippedWeapons.Length <= 0)
        {
            valid = true;
        }
        else
        {
            foreach (ScriptableItem equippedWeapon in equippedWeapons)
            {
                if (equippedWeapon && caster.UCE_checkHasEquipment(equippedWeapon))
                {
                    valid = true;
                    break;
                }
            }
        }

        if (!valid) return false;

        foreach (ScriptableItem ammo in requiredAmmo)
        {
            Player player = (Player) caster;
            player.inventory.Count(new Item(ammo));
            int count = player.inventory.Count(new Item(ammo));
            if (count <= 0 || count < ammoAmount)
                return false;
        }

        return true;
    }

    // -----------------------------------------------------------------------------------
    // ConsumeAmmunition
    // -----------------------------------------------------------------------------------
    public void ConsumeAmmunition(Entity caster)
    {
        Player player = (Player)caster;
        if (requiredAmmo.Length <= 0 || ammoAmount <= 0) return;

        foreach (ScriptableItem ammo in requiredAmmo)
        {
            if (ammoAmount > 0 && player.inventory.Count(new Item(ammo)) >= ammoAmount)
            {
                player.inventory.Remove(new Item(ammo), ammoAmount);
                return;
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // SpawnEffect
    // -----------------------------------------------------------------------------------
    public void SpawnEffect(Entity caster, Entity spawnTarget)
    {
        if (impactEffect != null && spawnTarget != null && caster != null)
        {
            GameObject go = Instantiate(impactEffect.gameObject, spawnTarget.transform.position, Quaternion.identity);
            go.GetComponent<OneTimeTargetSkillEffect>().caster = caster;
            go.GetComponent<OneTimeTargetSkillEffect>().target = spawnTarget;
            NetworkServer.Spawn(go);
        }
    }

    // -----------------------------------------------------------------------------------
    // GetEquipmentWeaponIndex
    // -----------------------------------------------------------------------------------
    private int GetEquipmentWeaponIndex(Entity caster)
    {
        return caster.equipment.slots.FindIndex(slot => slot.amount > 0 &&
                                          slot.item.data is WeaponItem);
    }

    // -----------------------------------------------------------------------------------
    // HasRequiredWeaponAndAmmo
    // -----------------------------------------------------------------------------------
    private bool HasRequiredWeaponAndAmmo(Entity caster)
    {
        int weaponIndex = caster.equipment.GetEquippedWeaponIndex();
        if (weaponIndex != -1)
        {
            // no ammo required, or has that ammo equipped?
            WeaponItem itemData = (WeaponItem)caster.equipment.slots[weaponIndex].item.data;
            return itemData.requiredAmmo == null ||
                   caster.equipment.GetItemIndexByName(itemData.requiredAmmo.name) != -1;
        }
        return false;
    }

    private void ConsumeRequiredWeaponsAmmo(Entity caster)
    {
        int weaponIndex = caster.equipment.GetEquippedWeaponIndex();
        if (weaponIndex != -1)
        {
            // no ammo required, or has that ammo equipped?
            WeaponItem itemData = (WeaponItem)caster.equipment.slots[weaponIndex].item.data;
            if (itemData.requiredAmmo != null)
            {
                int ammoIndex = caster.equipment.GetItemIndexByName(itemData.requiredAmmo.name);
                if (ammoIndex != 0)
                {
                    // reduce it
                    ItemSlot slot = caster.equipment.slots[ammoIndex];
                    --slot.amount;
                    caster.equipment.slots[ammoIndex] = slot;
                }
            }
        }
    }

    // ==================================== CHECKS =======================================

    // -----------------------------------------------------------------------------------
    // CheckSelf
    // -----------------------------------------------------------------------------------
    public override bool CheckSelf(Entity caster, int skillLevel)
    {
        return base.CheckSelf(caster, skillLevel) && HasRequiredWeaponAndAmmo(caster) && CheckAmmunition(caster);
    }

    // -----------------------------------------------------------------------------------
    // CheckTarget
    // -----------------------------------------------------------------------------------
    public override bool CheckTarget(Entity caster)
    {
        return caster.target != null && caster.target != caster && caster.CanAttack(caster.target);
    }

    // -----------------------------------------------------------------------------------
    // CheckDistance
    // -----------------------------------------------------------------------------------
    public override bool CheckDistance(Entity caster, int skillLevel, out Vector3 destination)
    {
        // target still around?
        if (caster.target != null)
        {
            destination = Utils.ClosestPoint(caster.target, caster.transform.position);
            return Utils.ClosestDistance(caster, caster.target) <= castRange.Get(skillLevel);
        }
        destination = caster.transform.position;
        return false;
    }

    // =================================== TOOLTIP =======================================

    // -----------------------------------------------------------------------------------
    // ToolTip
    // -----------------------------------------------------------------------------------
    public override string ToolTip(int skillLevel, bool showRequirements = false)
    {
        StringBuilder tip = new StringBuilder(base.ToolTip(skillLevel, showRequirements));

        tip.Replace("{DAMAGE}", damage.Get(skillLevel).ToString());
        tip.Replace("{STUNCHANCE}", Mathf.RoundToInt(stunChance.Get(skillLevel) * 100).ToString());
        tip.Replace("{STUNTIME}", minStunTime.Get(skillLevel).ToString("F1") + "-" + maxStunTime.Get(skillLevel).ToString("F1") + "s");

        string s = "";

        if (equippedWeapons.Length > 0)
        {
            s += "Allowed Weapon(s): \n";
            foreach (ScriptableItem equippedWeapon in equippedWeapons)
            {
                if (equippedWeapon)
                    s += "* " + equippedWeapon.name + "\n";
            }
        }

        tip.Replace("{EQUIPPEDWEAPON}", s);

        if (requiredAmmo.Length > 0)
        {
            s = "Required Ammunition (any): \n";

            foreach (ScriptableItem ammo in requiredAmmo)
                s += ammo.name + "[x" + ammoAmount + "]\n";

            tip.Replace("{REQUIREDAMMO}", s);
        }
        else
        {
            tip.Replace("{REQUIREDAMMO}", "");
        }

        return tip.ToString();
    }

    // -----------------------------------------------------------------------------------
}
