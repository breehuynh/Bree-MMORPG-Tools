// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// UCE DAMAGE DATA

public partial class UCE_DamageData
{
    // ---------- Base Settings
    public int skillLevel = 1;
    public int damage = 0;
    public bool addCasterDamage = false;
    public float speed = 0;
    public float distance = 0;
    public float stunChance = 0;
    public bool stunAddAccuracy = false;
    public float minStunTime = 0;
    public float maxStunTime = 0;

    // ---------- Cooldown Target
    public float cooldownChance = 0;
    public int cooldownDuration = 0;
    public bool cooldownAddAccuracy = false;

    // ---------- Sidekicks
    public bool sidekick = false;
    public float sidekickSpawnChance = 0;
    public int sidekickAmount = 0;
    public float sidekickSpawnDelay = 0;
    public float sidekickSpreadAngle = 0;

    // ---------- Displace Target or Caster
    public float recoilCaster = 0;
    public float minRecoilTarget = 0;
    public float maxRecoilTarget = 0;
    public float recoilChance = 0;
    public bool recoilAddAccuracy = false;

    // ---------- Apply Buff on Target
    public BuffSkill applyBuff = null;
    public int buffLevel = 0;
    public float buffChance = 0;
    public bool buffAddAccuracy = false;

    // ---------- AOE Effects
    public GameObject impactEffect = null;
    public float impactRadius = 0;
    public float triggerAggroChance = 0;
    public bool visualEffectOnMainTargetOnly = false;

    // ---------- Targeting
    public bool reverseTargeting = false;
    public bool notAffectSelf = false;
    public bool notAffectOwnParty = false;
    public bool notAffectOwnGuild = false;
    public bool notAffectOwnRealm = false;
    public bool notAffectPlayers = false;
    public bool notAffectMonsters = false;
    public bool notAffectPets = false;

    // ---------- Remove Buffs from Target
    public int removeRandomBuff = 0;
    public float removeChance = 0;
    public bool removeAddAccuracy = false;

    // ---------- Create GameObject on Target
    public GameObject[] createOnTarget;
    public float createChance = 0;

    // ---------- Sidekick Settings
    public bool sidekicksDontStun = false;
    public bool sidekicksDontBuff = false;
    public bool sidekicksDontRecoil = false;
    public bool sidekicksDontDebuff = false;
    public bool sidekicksDontAOE = false;
    public bool sidekicksDontCreateObject = false;

    // -----------------------------------------------------------------------------------
}
