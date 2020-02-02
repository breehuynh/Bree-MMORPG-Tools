// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Collections;

// entities ////////////////////////////////////////////////////////////////////
public partial class Player
{
    [SyncVar, HideInInspector]
    public bool isTaming = false;

    [Header("Setup Taming Time")]
    public float tamingTime = 2.0f;

    public float randomizeTime = 0.5f;

    [Header("Setup Base Taming Success Rate")]
    public float baseTamingSuccessRate = 0.10f;

    [SyncVar, HideInInspector]
    public float addedTamingSuccessRate = 0.0f;

    [Command]
    public void CmdsetupTamingSuccessRate(float amount)
    {
        addedTamingSuccessRate = amount;
    }

    [Command]
    private void CmdToggleTaming(bool choice)
    {
        isTaming = choice;
        animator.SetBool("Taming", choice);
    }

    public IEnumerator tamingTask()
    {
        float randomIncrease = Random.Range((-1 * randomizeTime) * tamingTime, randomizeTime * tamingTime);
        if (randomIncrease + tamingTime <= 0)
        {
            randomIncrease = (0.9f * tamingTime) * -1;
        }
        Debug.Log("Started Taming - Time Frame: " + (tamingTime + randomIncrease));
        CmdToggleTaming(true);
        yield return new WaitForSeconds((tamingTime + randomIncrease) * 0.9f);
        CmdStartTaming();
        yield return new WaitForSeconds((tamingTime + randomIncrease) * 0.1f);
        CmdToggleTaming(false);
    }

    [Command]
    public void CmdStartTaming()
    {
        Monster monster = (Monster)target;
        if (monster == null) return;

        if (tameSuccessful())
        {
            if (InventoryAdd(new Item(monster.tamingReward.item), monster.tamingReward.amount))
            {
                monster.health = 0;
            }
        }
    }

    public bool tameSuccessful()
    {
        float random = UnityEngine.Random.Range(0.0f, 1.0f);
        if (random <= (baseTamingSuccessRate + addedTamingSuccessRate))
            return true;
        return false;
    }
}

public partial class Monster
{
    public ScriptableItemAndAmount tamingReward;
}
