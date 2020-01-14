// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;

public class UCE_UI_SettingsVariables : MonoBehaviour
{
    public KeyCode[] keybindings = new KeyCode[] { KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D, KeyCode.LeftControl,
        KeyCode.Space, KeyCode.LeftAlt, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4,
        KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0, KeyCode.R,
        KeyCode.T, KeyCode.Y, KeyCode.M, KeyCode.C, KeyCode.B, KeyCode.F };

    [HideInInspector] public bool isShowOverhead = true;
    [HideInInspector] public bool isShowChat = true;

    [HideInInspector]
    public bool[] keybindUpdate = new bool[] { false, false, false, false, false, false, false,
     false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false};
}
