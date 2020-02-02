// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;

// UCE UI SKILLS

public partial class UCE_UI_Skills : MonoBehaviour
{
    public KeyCode hotKey = KeyCode.R;
    public GameObject panel;
    public UCE_UI_SkillSlot slotPrefab;
    public Transform content;
    public Text skillExperienceText;

    public string currentCategory;

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    private void Update()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        // hotkey (not while typing in chat, etc.)
        if (Input.GetKeyDown(hotKey) && !UIUtils.AnyInputActive())
            panel.SetActive(!panel.activeSelf);

        if (panel.activeSelf)
            Refresh();
    }

    // -----------------------------------------------------------------------------------
    // Refresh
    // -----------------------------------------------------------------------------------
    private void Refresh()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        // only update the panel if it's active
        if (panel.activeSelf)
        {
            // instantiate/destroy enough slots
            // (we only care about non status skills)
            UIUtils.BalancePrefabs(slotPrefab.gameObject, getSkillCount(), content);

            int t_index = -1;
            int s_index = 0;

            // refresh all
            for (int i = 0; i < player.skills.Count; ++i)
            {
                if (canShow(i))
                {
                    s_index = i;
                    t_index++;

                    UCE_UI_SkillSlot slot = content.GetChild(t_index).GetComponent<UCE_UI_SkillSlot>();
                    Skill skill = player.skills[s_index];

                    bool isPassive = skill.data is PassiveSkill;

                    // drag and drop name has to be the index in the real skill list,
                    // not in the filtered list, otherwise drag and drop may fail
                    int skillIndex = player.skills.FindIndex(s => s.name == skill.name);
                    slot.dragAndDropable.name = skillIndex.ToString();

                    // click event
                    slot.button.interactable = skill.level > 0 &&
                                               !isPassive &&
                                               player.CastCheckSelf(skill); // checks mana, cooldown etc.
                    slot.button.onClick.SetListener(() =>
                    {
                        player.CmdUseSkill(skillIndex);
                    });

                    // set state
                    slot.dragAndDropable.dragable = skill.level > 0 && !isPassive;

                    // image
                    if (skill.level > 0)
                    {
                        slot.image.color = Color.white;
                        slot.image.sprite = skill.image;
                    }

                    // description
                    slot.descriptionText.text = skill.ToolTip(showRequirements: skill.level == 0);

                    // learn / upgrade
                    if (skill.level < skill.maxLevel && player.CanUpgradeSkill(skill))
                    {
                        slot.upgradeButton.gameObject.SetActive(true);
                        slot.upgradeButton.GetComponentInChildren<Text>().text = skill.level == 0 ? "Learn" : "Upgrade";
                        slot.upgradeButton.interactable = true;
                        slot.upgradeButton.onClick.SetListener(() =>
                        {
                            player.CmdUpgradeSkill(skillIndex);
                            Refresh();
                        });
                    }
                    else slot.upgradeButton.gameObject.SetActive(false);

                    // cooldown overlay
                    float cooldown = skill.CooldownRemaining();
                    slot.cooldownOverlay.SetActive(skill.level > 0 && cooldown > 0);
                    slot.cooldownText.text = cooldown.ToString("F0");
                    slot.cooldownCircle.fillAmount = skill.cooldown > 0 ? cooldown / skill.cooldown : 0;
                }
            }

            // skill experience
            skillExperienceText.text = player.skillExperience.ToString();
        }
    }

    // -----------------------------------------------------------------------------------
    // OnEnable
    // -----------------------------------------------------------------------------------
    private void OnEnable()
    {
        if (panel.activeInHierarchy)
            changeCategory(currentCategory);
    }

    // -----------------------------------------------------------------------------------
    // changeCategory
    // -----------------------------------------------------------------------------------
    public void changeCategory(string newCategory)
    {
        currentCategory = newCategory;

        for (int i = 0; i < content.childCount; ++i)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        Invoke("Refresh", .1f);

        panel.SetActive(true);
    }

    // -----------------------------------------------------------------------------------
    // getSkillCount
    // -----------------------------------------------------------------------------------
    private int getSkillCount()
    {
        Player player = Player.localPlayer;
        if (!player) return 0;

        int count = 0;

        for (int i = 0; i < player.skills.Count; ++i)
        {
            if (canShow(i))
                count++;
        }

        return count;
    }

    // -----------------------------------------------------------------------------------
    // canShow
    // -----------------------------------------------------------------------------------
    private bool canShow(int index)
    {
        Player player = Player.localPlayer;
        if (!player) return false;

        bool valid = ((player.skills[index].category == currentCategory || currentCategory == "") &&
                        (!player.skills[index].unlearnable || (player.skills[index].unlearnable && player.skills[index].level > 0))
                    );

#if _iMMOPRESTIGECLASSES
        valid = (player.UCE_CheckPrestigeClass(player.skills[index].data.learnablePrestigeClasses)) ? valid : false;
#endif

        return valid;
    }

    // -----------------------------------------------------------------------------------
}
