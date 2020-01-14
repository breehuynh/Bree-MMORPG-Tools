// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;
using System.Text;
using UnityEngine;
using System.Collections.Generic;

// SWITCHABLE MESH

[System.Serializable]
public partial class SwitchableMesh
{
    public GameObject mesh;
    [HideInInspector] public Material defaultMaterial;
}

// SWITCHABLE COLOR

[System.Serializable]
public partial class SwitchableColor
{
    public string propertyName;
    public Color color;
}
