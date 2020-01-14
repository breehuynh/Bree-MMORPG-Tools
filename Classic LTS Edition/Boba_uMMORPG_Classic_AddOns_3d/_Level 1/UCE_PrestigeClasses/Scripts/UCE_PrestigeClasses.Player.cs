// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using Mirror;

// PLAYER

public partial class Player : Entity
{
    protected UCE_PrestigeClassTemplate _UCE_prestigeClass;
    [SyncVar] protected int UCE_hashPrestigeClass;

    // -----------------------------------------------------------------------------------
    // UCE_PrestigeClassTemplate
    // -----------------------------------------------------------------------------------
    public UCE_PrestigeClassTemplate UCE_prestigeClass
    {
        set { UCE_setPrestigeClass(value); }
        get { return UCE_getPrestigeClass(); }
    }

    // -----------------------------------------------------------------------------------
    // UCE_setPrestigeClass
    // -----------------------------------------------------------------------------------
    private void UCE_setPrestigeClass(UCE_PrestigeClassTemplate prestigeClass)
    {
        UCE_hashPrestigeClass = prestigeClass.name.GetDeterministicHashCode();
        _UCE_prestigeClass = prestigeClass;
    }

    // -----------------------------------------------------------------------------------
    // UCE_getPrestigeClass
    // -----------------------------------------------------------------------------------
    private UCE_PrestigeClassTemplate UCE_getPrestigeClass()
    {
        if (_UCE_prestigeClass != null)
            return _UCE_prestigeClass;

        UCE_PrestigeClassTemplate prestigeClassData;

        if (UCE_hashPrestigeClass != 0 && UCE_PrestigeClassTemplate.dict.TryGetValue(UCE_hashPrestigeClass, out prestigeClassData))
            _UCE_prestigeClass = prestigeClassData;

        return _UCE_prestigeClass;
    }

    // -----------------------------------------------------------------------------------
    // OnStartClient_UCE_PrestigeClasses
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnStartClient")]
    private void OnStartClient_UCE_PrestigeClasses()
    {
        bool bValid = false;

        for (int index = 0; index < equipmentInfo.Length; ++index)
        {
            bValid = UCE_RefreshPrestigeClass(index);
        }

        //if (!bValid) UCE_prestigeClass = null;
    }

    // -----------------------------------------------------------------------------------
    // OnRefreshLocation_UCE_PrestigeClasses
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnRefreshLocation")]
    private void OnRefreshLocation_UCE_PrestigeClasses(int index)
    {
        bool bValid = UCE_RefreshPrestigeClass(index);

        //if (!bValid) UCE_prestigeClass = null;
    }

    // -----------------------------------------------------------------------------------
    // UCE_RefreshPrestigeClass
    // -----------------------------------------------------------------------------------
    public bool UCE_RefreshPrestigeClass(int index)
    {
        ItemSlot slot = equipment[index];
        EquipmentInfo info = equipmentInfo[index];

        if (slot.amount > 0 && ((EquipmentItem)slot.item.data).prestigeClass != null)
        {
            UCE_prestigeClass = ((EquipmentItem)slot.item.data).prestigeClass;
            return true;
        }

        return false;
    }

    // -----------------------------------------------------------------------------------
    // UCE_CheckPrestigeClass
    // -----------------------------------------------------------------------------------
    public bool UCE_CheckPrestigeClass(UCE_PrestigeClassTemplate[] requiredPrestigeClasses)
    {
#if _iMMOPRESTIGECLASSES
        return UCE_checkHasPrestigeClass(requiredPrestigeClasses);
#else
		return false;
#endif
    }

    // -----------------------------------------------------------------------------------
    // UCE_CanUpgradeSkill
    // -----------------------------------------------------------------------------------
    public bool UCE_CanUpgradeSkill(Skill skill)
    {
#if _iMMOPRESTIGECLASSES
        return UCE_CheckPrestigeClass(skill.data.learnablePrestigeClasses);
#else
		return false;
#endif
    }

    // -----------------------------------------------------------------------------------
}
