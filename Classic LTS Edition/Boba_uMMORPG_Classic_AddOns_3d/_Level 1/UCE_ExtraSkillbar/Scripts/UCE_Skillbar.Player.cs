// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;

// PLAYER

public partial class Player
{
    [Header("-=-=- UCE Skillbar -=-=-")]
    public SkillbarEntry[] UCE_skillbar = {
        new SkillbarEntry{reference="", hotKey=KeyCode.Alpha1},
        new SkillbarEntry{reference="", hotKey=KeyCode.Alpha2},
        new SkillbarEntry{reference="", hotKey=KeyCode.Alpha3},
        new SkillbarEntry{reference="", hotKey=KeyCode.Alpha4},
        new SkillbarEntry{reference="", hotKey=KeyCode.Alpha5},
        new SkillbarEntry{reference="", hotKey=KeyCode.Alpha6},
        new SkillbarEntry{reference="", hotKey=KeyCode.Alpha7},
        new SkillbarEntry{reference="", hotKey=KeyCode.Alpha8},
        new SkillbarEntry{reference="", hotKey=KeyCode.Alpha9},
        new SkillbarEntry{reference="", hotKey=KeyCode.Alpha0},
    };

    [HideInInspector] public int UCE_skillbarIndex = 0;

    // -----------------------------------------------------------------------------------
    // OnEquipmentChanged_UCE_Skillbar
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnEquipmentChanged")]
    private void OnEquipmentChanged_UCE_Skillbar()
    {
        UCE_SaveSkillbar();

        for (int i = 0; i < equipment.Count; ++i)
        {
            ItemSlot slot = equipment[i];

            if (slot.amount > 0)
            {
                EquipmentItem itemData = (EquipmentItem)slot.item.data;

                if (itemData != null && itemData.UCE_skillbarIndex > 0)
                {
                    if (itemData.UCE_skillbarIndex != UCE_skillbarIndex)
                    {
                        //UCE_SaveSkillbar();
                        UCE_skillbarIndex = itemData.UCE_skillbarIndex;
                        UCE_LoadSkillbar();

                        break;
                    }

#if _iMMOUMA
                    return; //Vanishing Bar Fix
#endif
                }
                else
                {
                    UCE_skillbarIndex = 0;
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // OnStartLocalPlayer_UCE_Skillbar
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnStartLocalPlayer")]
    private void OnStartLocalPlayer_UCE_Skillbar()
    {
        OnEquipmentChanged_UCE_Skillbar();
    }

    // -----------------------------------------------------------------------------------
    // OnDestroy_UCE_Skillbar
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnDestroy")]
    private void OnDestroy_UCE_Skillbar()
    {
        if (isLocalPlayer)
            UCE_SaveSkillbar();
    }

    // -----------------------------------------------------------------------------------
    // UCE_SaveSkillbar
    // -----------------------------------------------------------------------------------
    //[Client] <- disabled while UNET OnDestroy isLocalPlayer bug exists
    private void UCE_SaveSkillbar()
    {
        if (UCE_skillbarIndex <= 0) return;

        for (int i = 0; i < UCE_skillbar.Length; ++i)
            PlayerPrefs.SetString(name + "_uce_skillbar_" + UCE_skillbarIndex + "_" + i, UCE_skillbar[i].reference);

        PlayerPrefs.Save();
    }

    // -----------------------------------------------------------------------------------
    // UCE_LoadSkillbar
    // -----------------------------------------------------------------------------------
    [Client]
    private void UCE_LoadSkillbar()
    {
        if (UCE_skillbarIndex <= 0) return;

        for (int i = 0; i < UCE_skillbar.Length; ++i)
        {
            if (PlayerPrefs.HasKey(name + "_uce_skillbar_" + UCE_skillbarIndex + "_" + i))
            {
                string entry = PlayerPrefs.GetString(name + "_uce_skillbar_" + UCE_skillbarIndex + "_" + i, "");
                if (string.IsNullOrWhiteSpace(entry))
                {
                    UCE_skillbar[i].reference = "";
                }
                else if (HasLearnedSkill(entry) ||
                  GetInventoryIndexByName(entry) != -1 ||
                  GetEquipmentIndexByName(entry) != -1)
                {
                    UCE_skillbar[i].reference = entry;
                }
                else
                {
                    UCE_skillbar[i].reference = "";
                }
            }
            else
            {
                UCE_skillbar[i].reference = "";
            }
        }
    }

    // ===================================================================================
    [DevExtMethods("OnDragAndDrop")]
    private void OnDragAndDrop_InventorySlot_UCESkillbarSlot(int[] slotIndices)
    {
        UCE_SaveSkillbar();
        UCE_skillbar[slotIndices[1]].reference = inventory[slotIndices[0]].item.name;
    }

    [DevExtMethods("OnDragAndDrop")]
    private void OnDragAndDrop_EquipmentSlot_UCESkillbarSlot(int[] slotIndices)
    {
        UCE_SaveSkillbar();
        UCE_skillbar[slotIndices[1]].reference = equipment[slotIndices[0]].item.name;
    }

    [DevExtMethods("OnDragAndDrop")]
    private void OnDragAndDrop_SkillsSlot_UCESkillbarSlot(int[] slotIndices)
    {
        UCE_SaveSkillbar();
        UCE_skillbar[slotIndices[1]].reference = skills[slotIndices[0]].name;
    }

    [DevExtMethods("OnDragAndDrop")]
    private void OnDragAndDrop_UCESkillbarSlot_UCESkillbarSlot(int[] slotIndices)
    {
        UCE_SaveSkillbar();
        string temp = UCE_skillbar[slotIndices[0]].reference;
        UCE_skillbar[slotIndices[0]].reference = UCE_skillbar[slotIndices[1]].reference;
        UCE_skillbar[slotIndices[1]].reference = temp;
    }

    [DevExtMethods("OnDragAndClear")]
    private void OnDragAndClear_UCESkillbarSlot(int slotIndex)
    {
        UCE_SaveSkillbar();
        UCE_skillbar[slotIndex].reference = "";
    }

    // -----------------------------------------------------------------------------------
}