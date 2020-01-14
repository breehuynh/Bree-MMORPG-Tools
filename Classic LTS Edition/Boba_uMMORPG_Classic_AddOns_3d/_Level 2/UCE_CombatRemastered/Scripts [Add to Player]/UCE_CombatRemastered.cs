// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UCE_CombatRemastered : MonoBehaviour
{
    // Our combat types that we have avaliable to use.
    public enum CombatType { Unarmed, Slash1Hand, Pierce1Hand, Bludgeon1Hand, Slash2Hand, Pierce2Hand, Bludgeon2Hand, RangedThrown, RangedBow, RangedGun, Shield, Dual }

    [Header("UCE Combat Remastered")]
    public ScriptableSkill[] skillDefaults;

    [HideInInspector] public bool changingEquipment = false;

    private CombatType currentCombatType = CombatType.Unarmed;
    private Skill[] usableSkills;
    private Animator animator;
    private Player player;
    private ItemSlot mainSlot, offSlot;
    private Item mainItem, offItem;

    private float waitTimer = 0;
    private float resetTimer = 0.05f;

    // Grab our animator and player then populate our scriptable skills into actual skills for later use.
    private void Start()
    {
        animator = GetComponent<Animator>();
        player = Player.localPlayer;

        usableSkills = new Skill[] { new Skill(skillDefaults[0]), new Skill(skillDefaults[1]), new Skill(skillDefaults[2]), new Skill(skillDefaults[3]),
        new Skill(skillDefaults[4]), new Skill(skillDefaults[5]), new Skill(skillDefaults[6]), new Skill(skillDefaults[7]), new Skill(skillDefaults[8]),
        new Skill(skillDefaults[9]), new Skill(skillDefaults[10]), new Skill(skillDefaults[11])};

        if (player != null)
        {
            if (player.equipment[0].amount == 0 && player.equipment[4].amount == 0)
            {
                SetupCombatType();
                return;
            }

            if (player.equipment[0].amount > 0)
                AutoRemoveEquipment(player.equipment[0]);
            else
                AutoRemoveEquipment(player.equipment[4]);
        }
    }

    // Update our auto attack so we actually perform the animation.
    private void Update()
    {
        if (player != null)
            if (player.isClient && player.state == "CASTING")
                if (player.skills[0].IsCasting())
                    animator.SetBool(player.skills[player.autoAttack].name, player.skills[0].IsCasting());
    }

    // Checks our equipment for unwanted equipped items, sets our combat type, then sends command to remove items if need.
    public void AutoRemoveEquipment(ItemSlot slot)
    {
        if (waitTimer <= Time.time)
        {
            waitTimer = Time.time + resetTimer;
            changingEquipment = true;

            // Set our item slot, item, and scriptable item early to not use un-needed resources.
            mainSlot = player.equipment[0]; offSlot = player.equipment[4]; mainItem = mainSlot.item; offItem = offSlot.item;

            if (mainSlot.amount == 0 && offSlot.amount == 0) { currentCombatType = CombatType.Unarmed; SetupCombatType(); return; }

            if (slot.amount == 0) return;

            // Simplify our equipped item slots to scriptable items and our current slot changed.
            EquipmentItem changedScriptItem = (EquipmentItem)slot.item.data;
            print(changedScriptItem.equipType);
            switch (changedScriptItem.equipType)
            {
                case EquipmentItem.EquipType.Slash1Hand:
                    if (offSlot.amount > 0 && player.InventorySlotsFree() >= offSlot.amount)
                    {
                        EquipmentItem offScriptItem = (EquipmentItem)offSlot.item.data;
                        if ((offScriptItem.equipType == EquipmentItem.EquipType.RangedBow || offScriptItem.equipType == EquipmentItem.EquipType.RangedGun) && mainSlot.amount > 0)
                        {
                            player.CmdAutoRemoveEquipment(player.equipment.IndexOf(offSlot));
                            player.InventoryRemove(mainItem, 1);
                        }
                    }
                    currentCombatType = CombatType.Slash1Hand;
                    break;

                case EquipmentItem.EquipType.Pierce1Hand:
                    if (offSlot.amount > 0 && player.InventorySlotsFree() >= offSlot.amount)
                    {
                        EquipmentItem offScriptItem = (EquipmentItem)offSlot.item.data;
                        if (offScriptItem.equipType != EquipmentItem.EquipType.Shield || offScriptItem.equipType != EquipmentItem.EquipType.Dual)
                            player.CmdAutoRemoveEquipment(player.equipment.IndexOf(offSlot));
                    }
                    currentCombatType = CombatType.Pierce1Hand;
                    break;

                case EquipmentItem.EquipType.Bludgeon1Hand:
                    if (offSlot.amount > 0 && player.InventorySlotsFree() >= offSlot.amount)
                    {
                        EquipmentItem offScriptItem = (EquipmentItem)offSlot.item.data;
                        if (offScriptItem.equipType != EquipmentItem.EquipType.Shield || offScriptItem.equipType != EquipmentItem.EquipType.Dual)
                            player.CmdAutoRemoveEquipment(player.equipment.IndexOf(offSlot));
                    }
                    currentCombatType = CombatType.Bludgeon1Hand;
                    break;

                case EquipmentItem.EquipType.Slash2Hand:
                    if (offSlot.amount > 0 && player.InventorySlotsFree() >= offSlot.amount)
                        player.CmdAutoRemoveEquipment(player.equipment.IndexOf(offSlot));
                    currentCombatType = CombatType.Slash2Hand;
                    break;

                case EquipmentItem.EquipType.Pierce2Hand:
                    if (offSlot.amount > 0 && player.InventorySlotsFree() >= offSlot.amount)
                        player.CmdAutoRemoveEquipment(player.equipment.IndexOf(offSlot));
                    currentCombatType = CombatType.Pierce2Hand;
                    break;

                case EquipmentItem.EquipType.Bludgeon2Hand:
                    if (offSlot.amount > 0 && player.InventorySlotsFree() >= offSlot.amount)
                        player.CmdAutoRemoveEquipment(player.equipment.IndexOf(offSlot));
                    currentCombatType = CombatType.Bludgeon2Hand;
                    break;

                case EquipmentItem.EquipType.RangedThrown:
                    if (offSlot.amount > 0 && player.InventorySlotsFree() >= offSlot.amount)
                    {
                        EquipmentItem offScriptItem = (EquipmentItem)offSlot.item.data;
                        if (offScriptItem.equipType != EquipmentItem.EquipType.Shield)
                            player.CmdAutoRemoveEquipment(player.equipment.IndexOf(offSlot));
                    }
                    currentCombatType = CombatType.RangedThrown;
                    break;

                case EquipmentItem.EquipType.RangedBow:
                    if ((mainSlot.amount > 0 && player.InventorySlotsFree() >= mainSlot.amount) && offSlot.amount > 0)
                    {
                        player.CmdAutoRemoveEquipment(player.equipment.IndexOf(mainSlot));
                        player.InventoryRemove(offItem, 1);
                    }
                    currentCombatType = CombatType.RangedBow;
                    break;

                case EquipmentItem.EquipType.RangedGun:
                    if ((mainSlot.amount > 0 && player.InventorySlotsFree() >= mainSlot.amount) && offSlot.amount > 0)
                    {
                        player.CmdAutoRemoveEquipment(player.equipment.IndexOf(mainSlot));
                        player.InventoryRemove(offItem, 1);
                    }
                    currentCombatType = CombatType.RangedGun;
                    break;

                case EquipmentItem.EquipType.Shield:
                    if (mainSlot.amount > 0 && player.InventorySlotsFree() >= mainSlot.amount)
                    {
                        EquipmentItem mainScriptItem = (EquipmentItem)mainSlot.item.data;
                        if (mainScriptItem.equipType == EquipmentItem.EquipType.Slash2Hand || mainScriptItem.equipType == EquipmentItem.EquipType.Pierce2Hand || mainScriptItem.equipType == EquipmentItem.EquipType.Bludgeon2Hand)
                        {
                            player.CmdAutoRemoveEquipment(player.equipment.IndexOf(mainSlot));
                            player.InventoryRemove(offItem, 1);
                        }

                        if (offSlot.amount > 0)
                            player.InventoryRemove(offItem, 1);
                    }
                    currentCombatType = CombatType.Shield;
                    break;

                case EquipmentItem.EquipType.Dual:
                    if (mainSlot.amount > 0 && player.InventorySlotsFree() >= mainSlot.amount)
                    {
                        EquipmentItem mainScriptItem = (EquipmentItem)mainSlot.item.data;
                        if (mainScriptItem.equipType == EquipmentItem.EquipType.Slash2Hand || mainScriptItem.equipType == EquipmentItem.EquipType.Pierce2Hand || mainScriptItem.equipType == EquipmentItem.EquipType.Bludgeon2Hand || mainScriptItem.equipType == EquipmentItem.EquipType.RangedThrown)
                        {
                            player.CmdAutoRemoveEquipment(player.equipment.IndexOf(mainSlot));
                            player.InventoryRemove(offItem, 1);
                        }

                        if (offSlot.amount > 0)
                            player.InventoryRemove(offItem, 1);
                    }
                    currentCombatType = CombatType.Dual;
                    break;
            }

            if (mainSlot.amount > 0 && offSlot.amount > 0)
                if (((EquipmentItem)player.equipment[4].item.data).equipType == EquipmentItem.EquipType.Shield && ((EquipmentItem)player.equipment[0].item.data).equipType != EquipmentItem.EquipType.RangedThrown)
                    currentCombatType = CombatType.Shield;

            if (mainSlot.amount > 0 && offSlot.amount > 0)
                if (((EquipmentItem)player.equipment[4].item.data).equipType == EquipmentItem.EquipType.Dual)
                    currentCombatType = CombatType.Dual;

            SetupCombatType();
        }
    }

    // Setup our animators so we use the correct animations and our default auto attack skill.
    private void SetupCombatType()
    {
        // Set all of our animator bools to false so we don't have overlaping issues.
        animator.SetBool("UNARMED", false); animator.SetBool("SLASH1H", false); animator.SetBool("PIERCE1H", false); animator.SetBool("BLUDGEON1H", false);
        animator.SetBool("SLASH2H", false); animator.SetBool("PIERCE2H", false); animator.SetBool("BLUDGEON2H", false); animator.SetBool("RANGEDTHROWN", false);
        animator.SetBool("RANGEDBOW", false); animator.SetBool("RANGEDGUN", false); animator.SetBool("SHIELD", false); animator.SetBool("DUAL", false);

        //Set our current animator bool based on weapon type.
        switch (currentCombatType)
        {
            case CombatType.Unarmed:
                animator.SetBool("UNARMED", true);
                player.skills[0] = usableSkills[0];
                player.autoAttack = 0;
                break;

            case CombatType.Slash1Hand:
                animator.SetBool("SLASH1H", true);
                player.skills[0] = usableSkills[1];
                player.autoAttack = 1;
                break;

            case CombatType.Pierce1Hand:
                animator.SetBool("PIERCE1H", true);
                player.skills[0] = usableSkills[2];
                player.autoAttack = 2;
                break;

            case CombatType.Bludgeon1Hand:
                animator.SetBool("BLUDGEON1H", true);
                player.skills[0] = usableSkills[3];
                player.autoAttack = 3;
                break;

            case CombatType.Slash2Hand:
                animator.SetBool("SLASH2H", true);
                player.skills[0] = usableSkills[4];
                player.autoAttack = 4;
                break;

            case CombatType.Pierce2Hand:
                animator.SetBool("PIERCE2H", true);
                player.skills[0] = usableSkills[5];
                player.autoAttack = 5;
                break;

            case CombatType.Bludgeon2Hand:
                animator.SetBool("BLUDGEON2H", true);
                player.skills[0] = usableSkills[6];
                player.autoAttack = 6;
                break;

            case CombatType.RangedThrown:
                animator.SetBool("RANGEDTHROWN", true);
                player.skills[0] = usableSkills[7];
                player.autoAttack = 7;
                break;

            case CombatType.RangedBow:
                animator.SetBool("RANGEDBOW", true);
                player.skills[0] = usableSkills[8];
                player.autoAttack = 8;
                break;

            case CombatType.RangedGun:
                animator.SetBool("RANGEDGUN", true);
                player.skills[0] = usableSkills[9];
                player.autoAttack = 9;
                break;

            case CombatType.Shield:
                animator.SetBool("SHIELD", true);
                player.skills[0] = usableSkills[10];
                player.autoAttack = 10;
                break;

            case CombatType.Dual:
                animator.SetBool("DUAL", true);
                player.skills[0] = usableSkills[11];
                player.autoAttack = 11;
                break;
        }

        changingEquipment = false;
    }
}
