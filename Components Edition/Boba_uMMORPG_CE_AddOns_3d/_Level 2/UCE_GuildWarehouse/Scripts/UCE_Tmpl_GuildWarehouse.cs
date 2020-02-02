// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// GUILD WAREHOUSE CONFIG

[CreateAssetMenu(fileName = "UCE Guild Warehouse", menuName = "UCE Templates/New UCE Guild Warehouse", order = 999)]
public class UCE_Tmpl_GuildWarehouse : ScriptableObject
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
    public string warehouseUpgradeLabel = "Guild Warehouse upgraded!";

    //public string warehouseBusyLabel		= "Guild Warehouse busy!";
}
