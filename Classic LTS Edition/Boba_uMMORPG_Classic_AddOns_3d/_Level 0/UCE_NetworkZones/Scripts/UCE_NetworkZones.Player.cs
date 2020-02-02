// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

// PLAYER

public partial class Player
{
    [Header("[-=-=- UCE NETWORK ZONES -=-=-]")]
    public UnityScene startingScene;

    // -----------------------------------------------------------------------------------
    // UCE_OnPortal
    // @Server
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    public void UCE_OnPortal(SceneLocation targetScene)
    {
        this.transform.position = targetScene.position;
        Database.singleton.CharacterSave(this, false);
        Database.singleton.SaveCharacterScene(this.name, targetScene.mapScene.SceneName);

        // ask client to switch server
        this.connectionToClient.Send(
            new SwitchServerMsg
            {
                scenePath = targetScene.mapScene.SceneName,
                characterName = this.name
            }
        );

        // immediately destroy so nothing messes with the new
        // position and so it's not saved again etc.
        NetworkServer.Destroy(this.gameObject);
    }

#if _iMMOBINDPOINT

    // -----------------------------------------------------------------------------------
    // UCE_OnPortal
    // @Server
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    public void UCE_OnPortal(UCE_BindPoint bindpoint)
    {
        this.transform.position = bindpoint.position;
        Database.singleton.CharacterSave(this, false);
        Database.singleton.SaveCharacterScene(this.name, bindpoint.SceneName);

        // ask client to switch server
        this.connectionToClient.Send(
            new SwitchServerMsg
            {
                scenePath = bindpoint.SceneName,
                characterName = this.name
            }
        );

        // immediately destroy so nothing messes with the new
        // position and so it's not saved again etc.
        NetworkServer.Destroy(this.gameObject);
    }

#endif

    // -----------------------------------------------------------------------------------
}
