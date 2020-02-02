// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

public enum BuffType { Both, Buff, Nerf, None }

public enum ThresholdType { None, Below, Above }

public enum GroupType { None, Party, Guild, Realm }

// SCRIPTABLE OBJECT BY FOLDER

[System.Serializable]
public partial class UCE_ScripableObjectEntry
{
    public ScriptableObject scriptableObject;
    public string folderName;
#if _iMMOASSETBUNDLEMANAGER
    public string bundleName;
    public bool loadFromAssetBundle;
#endif
}

// MUTABLE WRAPPER

[System.Serializable]
public sealed class MutableWrapper<T>
{
    public T Value;

    public MutableWrapper(T value)
    {
        this.Value = value;
    }
}

// UCE POPUP - CLASS

[System.Serializable]
public partial class UCE_PopupClass
{
    public string message = "Default Message";
    [Range(0, 255)] public byte iconId;
    [Range(0, 255)] public byte soundId;
}

// UCE WEIGHTED CHANCE - CLASS

[System.Serializable]
public partial class UCE_WeightedChance : MonoBehaviour
{
    [Range(0, 1)] public float chance;
}

// UCE ITEM REWARD - CLASS

[System.Serializable]
public partial class UCE_ItemReward
{
    [Range(0, 1)] public float probability;
    public ScriptableItem item;
    public int amount;
}

// UCE ITEM REQUIREMENT - CLASS

[System.Serializable]
public partial class UCE_ItemRequirement
{
    public ScriptableItem item;
    public int amount = 1;
}

// UCE ITEM MODIFIER - CLASS

[System.Serializable]
public partial class UCE_ItemModifier
{
    public ScriptableItem item;
    public int amount = 1;
}

// UCE SKILL REQUIREMENT - CLASS

[System.Serializable]
public partial class UCE_SkillRequirement
{
    public ScriptableSkill skill;
    public int level = 1;
}

// UCE PROFESSION REQUIREMENT - CLASS

#if _iMMOHARVESTING

[System.Serializable]
public partial class UCE_HarvestingProfessionRequirement
{
    public UCE_HarvestingProfessionTemplate template;

    [Tooltip("Minimum required profession level?")]
    public int level = 1;
}

#endif

// UCE CRAFT REQUIREMENT - CLASS

#if _iMMOCRAFTING

[System.Serializable]
public partial class UCE_CraftingProfessionRequirement
{
    public UCE_CraftingProfessionTemplate template;

    [Tooltip("Minimum required craft level?")]
    public int level = 1;
}

#endif

// UCE ATTRIBUTE MODIFIER

#if _iMMOATTRIBUTES

[System.Serializable]
public partial class UCE_AttributeModifier
{
    public UCE_AttributeTemplate template;

    [Range(-1f, 1f)]
    public float percentBonus = 0f;
    public int flatBonus = 0;
}

#else
public partial class UCE_AttributeModifier { }
#endif

// UCE ATTRIBUTE REQUIREMENT

#if _iMMOATTRIBUTES

[System.Serializable]
public partial class UCE_AttributeRequirement
{
    public UCE_AttributeTemplate template;
    public int minValue = 0;
    public int maxValue = 0;
}

#else
public partial class UCE_AttributeRequirement { }
#endif
