// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    private UCE_CombatRemastered combatRemastered;
    [HideInInspector] public int autoAttack = 0;

    private void Start_CombatRemastered()
    {
        combatRemastered = GetComponent<UCE_CombatRemastered>();
    }

    // Checks if we have the required equipment to cast that skill.
    public bool CheckEquipSkills(ScriptableSkill skill)
    {
        for (int i = 0; i < equipment.slots.Count; i++)
        {
            if (equipment.slots[i].amount > 0)
            {
                EquipmentItem equipItem = equipment.slots[i].item.data as EquipmentItem;

                if ((int)equipItem.equipType == (int)skill.skillType || skill.skillType == ScriptableSkill.SkillType.None)
                {
                    return true;
                }
            }
        }

        return false;
    }

    // Removes unwanted equipment when switching to different weapon types.
    [Command]
    public void CmdAutoRemoveEquipment(int index)
    {
        // validate
        if (0 <= index && index < equipment.slots.Count && equipment.slots[index].amount > 0)
        {
            // check inventory for free slot and pass it to swapinventoryequip()
            ItemSlot item = equipment.slots[index];

            if (inventory.SlotsFree() >= item.amount)
            {
                if (item.amount > 0)
                {
                    inventory.Add(item.item, 1);
                    item.DecreaseAmount(1);
                    equipment.slots[index] = item;
                }
            }
        }
    }
}
