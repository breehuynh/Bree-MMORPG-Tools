// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;

// ===================================================================================
// NPC
// ===================================================================================
public partial class Npc
{
    // Required to keep track of how many players are within this Npcs interaction range
    [SyncVar, HideInInspector] public int accessingPlayers = 0;
}
