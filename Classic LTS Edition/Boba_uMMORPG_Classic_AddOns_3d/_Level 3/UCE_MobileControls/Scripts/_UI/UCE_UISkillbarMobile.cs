// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// UCE_UISkillbarMobile

public partial class UCE_UISkillbarMobile : MonoBehaviour
{
    public GameObject panel;
    public UISkillbarSlot slotPrefab;
    public Transform content;

    private bool initialized, error;
    private const int itemStart = 6;

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    private void Update()
    {
        if (panel.name == "SkillbarPanel")
            return; //if regular Skillbar then don't do anything

        Player player = Player.localPlayer;
        if (player == null) return;

        if (player.skillbar.Length < 8)
        {
            panel.SetActive(false);
            if (!initialized)
            {
                initialized = true;
            }
            return;
        }

        if (!initialized)
        {
            GameObject skillbar = GameObject.Find("Skillbar");
            skillbar.SetActive(false);

            //Destroy(skillbar);

            // set the rest to nothing & invisible, if skillbar.Length > 8
            for (int i = itemStart + 2; i < player.skillbar.Length; ++i)
            {
                player.skillbar[i].reference = "";
                content.GetChild(i).GetComponent<UISkillbarSlot>().gameObject.SetActive(false);
            }

            initialized = true;
        }

        // Attack Button
        if (player.skillbar[0].reference == "")
            player.skillbar[0].reference = player.skills[0].name;
    }
}
