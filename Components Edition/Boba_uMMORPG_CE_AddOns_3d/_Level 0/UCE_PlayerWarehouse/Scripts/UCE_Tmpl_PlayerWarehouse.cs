// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PLAYER WAREHOUSE CONFIG

[CreateAssetMenu(fileName = "UCE Player Warehouse", menuName = "UCE Templates/New UCE Player Warehouse", order = 999)]
public class UCE_Tmpl_PlayerWarehouse : ScriptableObject
{
    [Header("[STORAGE]")]
    public LinearInt warehouseStorageItems = new LinearInt { baseValue = 10 };

    public LinearLong warehouseStorageGold = new LinearLong { baseValue = 10000 };

    [Header("[UPRADING]")]
    public UCE_Cost[] warehouseUpgradeCost;

    [Header("[ALLOWANCES]")]
    public bool storeSellable = true;

    public bool storeTradable = true;
    public bool storeDestroyable = true;

    [Header("[MESSAGES]")]
    public string upgradeLabel = "Player Warehouse upgraded!";
}
