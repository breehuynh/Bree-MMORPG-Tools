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
using UMA;
using UMA.CharacterSystem;
using UnityEngine;

public partial class Player : Entity
{
    [SyncVar]
    public string umaDna = "";

    private void OnStartLocalPlayer_UmaIntegration()
    {
        StartCoroutine(WaitForDcs());
    }

    public void ProcessBones(Transform transform)
    {
        foreach (Transform t in transform)
        {
            RecursivelyRemoveChildBones(t);
            GameObject.Destroy(t.gameObject);
        }
    }

    private void RecursivelyRemoveChildBones(Transform transform)
    {
        GetComponentInChildren<DynamicCharacterAvatar>().umaData.skeleton.RemoveBone(UMAUtils.StringToHash(transform.name));
        foreach (Transform t in transform)
        {
            Debug.Log("RecursivelyRemoveChildBones(" + t + ".");
            RecursivelyRemoveChildBones(t);
        }
    }

    public void UpdateUma()
    {
        RefreshUma();
    }

    public void RefreshUma()
    {
        if (umaDna == "") return;

        DynamicCharacterAvatar avatar = GetComponentInChildren<DynamicCharacterAvatar>();

        if (GetComponentInChildren<DynamicCharacterAvatar>() == null) return;

        string decompressed = CompressUMA.Compressor.DecompressDna(umaDna);

        avatar.ClearSlots();
        avatar.LoadFromRecipeString(decompressed);

        for (int i = 0; i < equipment.Count; i++)
        {
            ItemSlot slot = equipment[i];
            EquipmentInfo info = equipmentInfo[i];

            //  valid item?
            if (slot.amount > 0)
            {
                EquipmentItem itemData = (EquipmentItem)slot.item.data;

                UMATextRecipe maleRecipe = itemData.maleUmaRecipe;
                UMATextRecipe femaleRecipe = itemData.femaleUmaRecipe;

                if (avatar == null) return;

                if (maleRecipe != null)
                    avatar.SetSlot(maleRecipe);

                if (femaleRecipe != null)
                    avatar.SetSlot(femaleRecipe);
            }
        }
    }

    private IEnumerator WaitForDcs()
    {
        yield return new WaitWhile(() => FindObjectOfType<DynamicCharacterSystem>() == null);
        yield return new WaitWhile(() => GetComponentInChildren<DynamicCharacterAvatar>() == null);
        StartCoroutine(WaitForDna());
    }

    private IEnumerator WaitForDna()
    {
        yield return new WaitWhile(() => umaDna == "");
        RefreshUma();
    }
}

#endif