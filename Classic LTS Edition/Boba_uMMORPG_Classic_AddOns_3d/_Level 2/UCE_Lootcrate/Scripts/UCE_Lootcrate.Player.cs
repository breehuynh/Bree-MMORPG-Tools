// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;

#if _iMMOCHEST

// PLAYER

public partial class Player
{
    protected UCE_UI_Lootcrate UCE_lootcrateUIInstance;
    [HideInInspector] public UCE_Lootcrate UCE_selectedLootcrate;

    // -----------------------------------------------------------------------------------
    // UCE_OnSelect_Lootcrate
    // @Client
    // -----------------------------------------------------------------------------------
    [Client]
    public void UCE_OnSelect_Lootcrate(UCE_Lootcrate _UCE_selectedLootcrate)
    {
        if (UCE_lootcrateUIInstance)
            UCE_lootcrateUIInstance.Hide(false);

        UCE_selectedLootcrate = _UCE_selectedLootcrate;
        LookAtY(UCE_selectedLootcrate.gameObject.transform.position);
        Cmd_UCE_checkLootcrateAccess(UCE_selectedLootcrate.gameObject);
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_LootcrateAccess
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    protected void Cmd_UCE_checkLootcrateAccess(GameObject _UCE_selectedLootcrate)
    {
        UCE_selectedLootcrate = _UCE_selectedLootcrate.GetComponent<UCE_Lootcrate>();

        if (UCE_LootcrateValidation())
        {
            Target_UCE_startLootcrateAccess(connectionToClient);
        }
        else
        {
            if (UCE_selectedLootcrate != null && UCE_selectedLootcrate.checkInteractionRange(this) && UCE_selectedLootcrate.lockedMessage != "")
            {
                UCE_ShowPrompt(UCE_selectedLootcrate.lockedMessage);
            }
            else
            {
                agent.destination = this.collider.ClosestPointOnBounds(transform.position);
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // Target_UCE_startLootcrateAccess
    // @Server -> @Client
    // -----------------------------------------------------------------------------------
    [TargetRpc]
    protected void Target_UCE_startLootcrateAccess(NetworkConnection target)
    {
        if (UCE_LootcrateValidation())
        {
            UCE_addTask();
            UCE_setTimer(UCE_selectedLootcrate.accessDuration);
            UCE_CastbarShow(UCE_selectedLootcrate.accessLabel, UCE_selectedLootcrate.accessDuration);
            StartAnimation(UCE_selectedLootcrate.playerAnimation);
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_LootcrateValidation
    // -----------------------------------------------------------------------------------
    public bool UCE_LootcrateValidation()
    {
        bool bValid = (UCE_selectedLootcrate != null &&
            UCE_selectedLootcrate.checkInteractionRange(this) &&
            UCE_selectedLootcrate.interactionRequirements.checkState(this));

        if (!bValid)
            UCE_cancelLootcrate();

        return bValid;
    }

    // -----------------------------------------------------------------------------------
    // LateUpdate_UCE_Lootcrate
    // @Client
    // -----------------------------------------------------------------------------------
    [ClientCallback]
    [DevExtMethods("LateUpdate")]
    private void LateUpdate_UCE_Lootcrate()
    {
        if (UCE_LootcrateValidation() && UCE_checkTimer())
        {
            UCE_removeTask();
            UCE_stopTimer();
            UCE_CastbarHide();

            Cmd_UCE_finishLootcrateAccess();

            if (!UCE_lootcrateUIInstance)
                UCE_lootcrateUIInstance = FindObjectOfType<UCE_UI_Lootcrate>();

            UCE_lootcrateUIInstance.Show();

            StopAnimation(UCE_selectedLootcrate.playerAnimation);

            UCE_AddMessage(UCE_selectedLootcrate.successMessage);
        }
    }

    // -----------------------------------------------------------------------------------
    // OnDamageDealt_UCE_cancelLootcrate
    // Custom Hook
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnDamageDealt")]
    private void OnDamageDealt_UCE_cancelLootcrate()
    {
        UCE_cancelLootcrate();
    }

    // -----------------------------------------------------------------------------------
    // UCE_cancelLootcrate
    // -----------------------------------------------------------------------------------
    public void UCE_cancelLootcrate()
    {
        if (UCE_selectedLootcrate != null)
        {
            UCE_stopTimer();
            UCE_removeTask();
            UCE_CastbarHide();

            StopAnimation(UCE_selectedLootcrate.playerAnimation);

            UCE_selectedLootcrate = null;
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_finishLootcrate
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_finishLootcrateAccess()
    {
        UCE_removeTask();
        UCE_stopTimer();

#if _iMMOCHEST && _iMMOQUESTS
        UCE_IncreaseQuestLootCounterFor(UCE_selectedLootcrate.name);
#endif

        UCE_selectedLootcrate.OnLoot();
        UCE_selectedLootcrate.OnAccessed();
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_TakeLootcrateGold
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_TakeLootcrateGold()
    {
        if (
            UCE_LootcrateValidation()
            )
        {
            gold += UCE_selectedLootcrate.gold;
            UCE_selectedLootcrate.gold = 0;
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_TakeLootcrateCoins
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_TakeLootcrateCoins()
    {
        if (
            UCE_LootcrateValidation()
            )
        {
            coins += UCE_selectedLootcrate.coins;
            UCE_selectedLootcrate.coins = 0;
        }
    }

    // -----------------------------------------------------------------------------------
    // Cmd_UCE_TakeLootcrateItem
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    public void Cmd_UCE_TakeLootcrateItem(int index)
    {
        if (
            UCE_LootcrateValidation() &&
            0 <= index && index < UCE_selectedLootcrate.inventory.Count &&
            UCE_selectedLootcrate.inventory[index].amount > 0)
        {
            ItemSlot slot = UCE_selectedLootcrate.inventory[index];

            if (InventoryAdd(slot.item, slot.amount))
            {
                slot.amount = 0;
                UCE_selectedLootcrate.inventory[index] = slot;
            }
        }
    }

    // -----------------------------------------------------------------------------------
}

#endif
