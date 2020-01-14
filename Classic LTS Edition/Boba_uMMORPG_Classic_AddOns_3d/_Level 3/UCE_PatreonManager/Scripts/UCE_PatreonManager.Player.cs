// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// PLAYER

public partial class Player
{

    [Header("[UCE PATREON]")]
    public float patreonIntervalSeconds = 3600f;

    protected Patreon patreon;
    double dPatreonTimer = 0f;

    // -----------------------------------------------------------------------------------
    // OnStartLocalPlayer_UCE_PatreonManager
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnStartLocalPlayer")]
    private void OnStartLocalPlayer_UCE_PatreonManager()
    {
        
        patreon = GetComponent<Patreon>();

        if (patreon != null && !string.IsNullOrWhiteSpace(patreon.client_id))        
        {
            patreon.onConnect = UCE_RefreshPatreonToken;
            patreon.connect();
        }

    }

    // -----------------------------------------------------------------------------------
    // UCE_HasActivePatreonSubscription
    // -----------------------------------------------------------------------------------
    public bool UCE_HasActivePatreonSubscription(int minAmount)
    {
        return (patreon != null && patreon.pledge >= minAmount);
    }

    // -----------------------------------------------------------------------------------
    // Update_UCE_PatreonManager
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Update")]
    private void Update_UCE_PatreonManager()
    {
        if (patreon == null || string.IsNullOrWhiteSpace(patreon.client_id)) return;

        if (Time.time > dPatreonTimer)
        {
            dPatreonTimer = Time.time + patreonIntervalSeconds;
            UCE_RefreshPatreonToken();
        }
    }

    // -----------------------------------------------------------------------------------
    // Update_UCE_PatreonManager
    // -----------------------------------------------------------------------------------
    protected void UCE_RefreshPatreonToken(string statusText="")
    {
        patreon.refreshToken();
    }

    // -----------------------------------------------------------------------------------
}
