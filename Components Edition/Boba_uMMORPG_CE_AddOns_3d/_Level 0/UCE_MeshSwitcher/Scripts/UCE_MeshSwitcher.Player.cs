// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;
using UnityEngine;

// PLAYER

partial class Player
{
    protected Renderer[] cachedRenderers;

    // -----------------------------------------------------------------------------------
    // OnStartClient_UCE_MeshSwitcher
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnStartClient")]
    private void OnStartClient_UCE_MeshSwitcher()
    {
        // -- Cache the default Material
        EquipmentInfo[] infoList = ((PlayerEquipment)equipment).slotInfo;
        for (int index = 0; index < infoList.Length; ++index)
        {
            EquipmentInfo info = infoList[index];
            for (int i = 0; i < info.mesh.Length; ++i)
            {
                if (info.mesh[i].mesh != null)
                {
                    info.mesh[i].defaultMaterial = info.mesh[i].mesh.GetComponent<Renderer>().material;
                }
            }
            UCE_RefreshMesh(index);
        }
    }

    // -----------------------------------------------------------------------------------
    // OnRefreshLocation_UCE_MeshSwitcher
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnRefreshLocation")]
    private void OnRefreshLocation_UCE_MeshSwitcher(int index)
    {
        UCE_RefreshMesh(index);
    }

    // -----------------------------------------------------------------------------------
    // UCE_RefreshMesh
    // -----------------------------------------------------------------------------------
    private void UCE_RefreshMesh(int index)
    {
        ItemSlot slot = equipment.slots[index];
        EquipmentInfo info = ((PlayerEquipment)equipment).slotInfo[index];
        cachedRenderers = new Renderer[info.mesh.Length];

        for (int i = 0; i < info.mesh.Length; ++i)
        {
            cachedRenderers[i] = info.mesh[i].mesh.GetComponent<Renderer>();
        }

        if (info.requiredCategory != "" && info.mesh.Length > 0)
        {
            if (slot.amount > 0)
            {
                EquipmentItem itemData = (EquipmentItem)slot.item.data;
                UCE_SwitchToMesh(info, itemData.meshIndex, itemData.meshMaterial, itemData.switchableColors);
            }
            else
            {
                UCE_SwitchToMesh(info, 0, null, null);
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_SwitchToMesh
    // -----------------------------------------------------------------------------------
    private void UCE_SwitchToMesh(EquipmentInfo info, int[] meshIndex, Material mat, SwitchableColor[] colors)
    {
        for (int i = 0; i < info.mesh.Length; ++i)
        {
            if (info.mesh[i].mesh != null)
            {
                if (Array.Exists(meshIndex, e => e == i))
                {
                    info.mesh[i].mesh.SetActive(true);

                    if (mat != null)
                    {
                        cachedRenderers[i].material = mat;
                    }
                    else if (info.mesh[i].defaultMaterial != null)
                    {
                        cachedRenderers[i].material = info.mesh[i].defaultMaterial;
                    }

                    UCE_SwitchToColor(cachedRenderers[i].material, colors);
                }
                else
                {
                    info.mesh[i].mesh.SetActive(false);
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_SwitchToMesh
    // -----------------------------------------------------------------------------------
    private void UCE_SwitchToMesh(EquipmentInfo info, int meshIndex, Material mat, SwitchableColor[] colors)
    {
        for (int i = 0; i < info.mesh.Length; ++i)
        {
            if (info.mesh[i].mesh != null)
            {
                if (meshIndex == i)
                {
                    info.mesh[i].mesh.SetActive(true);

                    if (mat != null)
                    {
                        info.mesh[i].mesh.GetComponent<Renderer>().material = mat;
                    }
                    else if (info.mesh[i].defaultMaterial != null)
                    {
                        info.mesh[i].mesh.GetComponent<Renderer>().material = info.mesh[i].defaultMaterial;
                    }

                    UCE_SwitchToColor(cachedRenderers[i].material, colors);
                }
                else
                {
                    info.mesh[i].mesh.SetActive(false);
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_SwitchToColor
    // -----------------------------------------------------------------------------------
    private void UCE_SwitchToColor(Material material, SwitchableColor[] colors)
    {
        if (material == null || colors == null || colors.Length == 0) return;
        foreach (SwitchableColor color in colors)
            material.SetColor(color.propertyName, color.color);
    }

    // -----------------------------------------------------------------------------------
}
