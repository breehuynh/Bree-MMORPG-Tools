// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
#if _UMA

using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class NetworkManagerMMO : NetworkManager
{
    private void LoadPreview(GameObject prefab, Transform location, int selectionIndex, CharactersAvailableMsg.CharacterPreview character)
    {
        // instantiate the prefab
        GameObject preview = Instantiate(prefab.gameObject, location.position, location.rotation);
        preview.transform.parent = location;
        Player player = preview.GetComponent<Player>();

        // assign basic preview values like name and equipment
        player.name = character.name;
        string decompressor = CompressUMA.Compressor.DecompressDna(character.umaDna);
        player.umaDna = character.umaDna;
        preview.GetComponentInChildren<UMA.CharacterSystem.DynamicCharacterAvatar>().LoadFromRecipeString(decompressor);
        for (int i = 0; i < character.equipment.Length; ++i)
        {
            ItemSlot slot = character.equipment[i];
            player.equipment.Add(slot);
            if (slot.amount > 0)
            {
                // OnEquipmentChanged won't be called unless spawned, we
                // need to refresh manually
                player.RefreshLocation(i);
                player.UpdateUma();
            }
        }

        // add selection script
        preview.AddComponent<SelectableCharacter>();
        preview.GetComponent<SelectableCharacter>().index = selectionIndex;
    }

    private Player CreateCharacter(GameObject classPrefab, string characterName, string account, string dna)
    {
        // create new character based on the prefab.
        // -> we also assign default items and equipment for new characters
        // -> skills are handled in Database.CharacterLoad every time. if we
        //    add new ones to a prefab, all existing players should get them
        // (instantiate temporary player)
        //print("creating character: " + message.name + " " + message.classIndex);
        Player player = Instantiate(classPrefab).GetComponent<Player>();
        player.name = characterName;
        player.account = account;
        player.className = classPrefab.name;
        player.umaDna = dna;
        player.transform.position = GetStartPositionFor(player.className).position;
        for (int i = 0; i < player.inventorySize; ++i)
        {
            // add empty slot or default item if any
            player.inventory.Add(i < player.defaultItems.Length ? new ItemSlot(new Item(player.defaultItems[i].item), player.defaultItems[i].amount) : new ItemSlot());
        }
        for (int i = 0; i < player.equipmentInfo.Length; ++i)
        {
            // add empty slot or default item if any
            EquipmentInfo info = player.equipmentInfo[i];
            player.equipment.Add(info.defaultItem.item != null ? new ItemSlot(new Item(info.defaultItem.item), info.defaultItem.amount) : new ItemSlot());
        }
        player.health = player.healthMax; // after equipment in case of boni
        player.mana = player.manaMax; // after equipment in case of boni

        return player;
    }
}

#endif