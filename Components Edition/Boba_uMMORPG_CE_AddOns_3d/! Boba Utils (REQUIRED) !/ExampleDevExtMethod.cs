using Mirror;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public partial class Player
{
    // ===================================== README =================================

    // This is an example of the DevExtMethod system hooks
    // Some addons use DevExtMethods. These are necessary in order to add additional functionality to the default core.
    // You will know if you've successfully installed these DevExtMethod examples if you get Debug.Logs in your console.
    // Once you've learned how to install DevExtMethods, you can remove this file if you wish.

    // -----------------------------------------------------------------------------------
    // Steps
    // -----------------------------------------------------------------------------------
    // 1. Open up Player.cs
    // 2. Look for the method called LateUpdate()
    // 3. On the last line of this method, add the following code:
    //          this.InvokeInstanceDevExtMethods(nameof(LateUpdate));
    // 4. Look for the method called OnDamageDealtTo()
    // 5. On the last line of this method, add the following code:
    //          this.InvokeInstanceDevExtMethods(nameof(OnDamageDealtTo), victim);
    // 6. Start up your game.
    // 7. Congrats! If you see Debug.Logs in your console then you've successfully installed the DevExtMethod system example correctly.






    // -----------------------------------------------------------------------------------
    // LateUpdate_Example
    // @Client
    // -----------------------------------------------------------------------------------
    [ClientCallback]
    [DevExtMethods("LateUpdate")]
    private void LateUpdate_Example()
    {
        Debug.Log("You've SUCCESSFULLY installed the DevExtMethod LateUpdate_Example correctly.");
        Debug.Log("CLICK this and comment out this method now.");
    }

    // -----------------------------------------------------------------------------------
    // OnDamageDealt_Example
    // Custom Hook
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnDamageDealtTo")]
    private void OnDamageDealtTo_Example(Entity victim)
    {
        Debug.Log("You've SUCCESSFULLY installed the DevExtMethod OnDamageDealtTo_Example correctly.");
        Debug.Log("CLICK this and comment out this method now.");
    }
}
