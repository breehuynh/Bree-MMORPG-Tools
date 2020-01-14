// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

// UCE UI Character Info Attributes

public partial class UCE_UI_CharacterInfoAttributes : MonoBehaviour
{
    public KeyCode hotKey = KeyCode.C;
    public GameObject panel;
    public UCE_UI_AttributeSlot slotPrefab;
    public Transform attributeContent;
#if _iMMOELEMENTS
    public Transform elementsContent;
    public UCE_UI_ElementSlot slotElementPrefab;
#endif
    public Text damageText;
    public Text defenseText;
    public Text healthText;
    public Text manaText;
    public Text criticalChanceText;
    public Text blockChanceText;
    public Text speedText;
    public Text levelText;
    public Text currentExperienceText;
    public Text maximumExperienceText;
    public Text skillExperienceText;
    public Text attrPointsText;
    public Text accuracyText;
    public Text resistanceText;
    public Text blockFactorText;
    public Text blockBreakText;
    public Text reflectText;
    public Text defBreakText;
    public Text critFactorText;
    public Text drainHealthText;
    public Text drainManaText;
    public Text critEvasionText;

    public Text reserved1Text;
    public Text reserved2Text;

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    private void Update()
    {
#if _iMMOATTRIBUTES

        Player player = Player.localPlayer;
        if (!player) return;

        if (Input.GetKeyDown(hotKey) && !UIUtils.AnyInputActive())
            panel.SetActive(!panel.activeSelf);

        if (panel.activeSelf)
        {
            // -- Update Main Stats

            damageText.text = player.damage.ToString();
            defenseText.text = player.defense.ToString();
            healthText.text = player.healthMax.ToString();
            manaText.text = player.manaMax.ToString();
            criticalChanceText.text = (player.criticalChance * 100).ToString("F0") + "%";
            blockChanceText.text = (player.blockChance * 100).ToString("F0") + "%";
            speedText.text = player.speed.ToString();
            levelText.text = player.level.ToString();
            currentExperienceText.text = player.experience.ToString();
            maximumExperienceText.text = player.experienceMax.ToString();
            skillExperienceText.text = player.skillExperience.ToString();
            attrPointsText.text = player.UCE_AttributesSpendable().ToString();

            // -- Update Secondary Stats

            accuracyText.text = (player.accuracy * 100).ToString("F0") + "%";
            resistanceText.text = (player.resistance * 100).ToString("F0") + "%";
            blockFactorText.text = (player.blockFactor * 100).ToString("F0") + "%";
            blockBreakText.text = (player.blockBreakFactor * 100).ToString("F0") + "%";
            reflectText.text = (player.reflectDamageFactor * 100).ToString("F0") + "%";
            defBreakText.text = (player.defenseBreakFactor * 100).ToString("F0") + "%";
            critFactorText.text = (player.criticalFactor * 100).ToString("F0") + "%";
            drainHealthText.text = (player.drainHealthFactor * 100).ToString("F0") + "%";
            drainManaText.text = (player.drainManaFactor * 100).ToString("F0") + "%";
            critEvasionText.text = (player.criticalEvasion * 100).ToString("F0") + "%";

#if _iMMOATTRIBUTES
            UpdateAttributes();
#endif

#if _iMMOELEMENTS
            UpdateElements();
#endif
        }

#endif
    }

    // -----------------------------------------------------------------------------------
    // UpdateAttributes
    // -----------------------------------------------------------------------------------
#if _iMMOATTRIBUTES

    protected void UpdateAttributes()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        UIUtils.BalancePrefabs(slotPrefab.gameObject, player.UCE_Attributes.Count, attributeContent);

        for (int i = 0; i < player.UCE_Attributes.Count; ++i)
        {
            UCE_UI_AttributeSlot slot = attributeContent.GetChild(i).GetComponent<UCE_UI_AttributeSlot>();
            var attr = player.UCE_Attributes[i];
            int bonus = player.UCE_calculateBonusAttribute(attr);

            int slotIndex = i;

            slot.tooltip.enabled = true;
            slot.tooltip.text = attr.ToolTip();
            slot.image.sprite = attr.template.image;
            slot.label.text = attr.template.name + ":";
            slot.points.text = attr.points.ToString() + " +" + bonus.ToString();
            slot.button.interactable = player.UCE_AttributesSpendable() > 0;

            slot.button.onClick.SetListener(() =>
            {
                player.Cmd_UCE_IncreaseAttribute(slotIndex);
            });
        }
    }

#endif

    // -----------------------------------------------------------------------------------
    // UpdateElements
    // -----------------------------------------------------------------------------------
#if _iMMOELEMENTS

    protected void UpdateElements()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        UIUtils.BalancePrefabs(slotElementPrefab.gameObject, UCE_ElementTemplate.dict.Count, elementsContent);

        for (int i = 0; i < UCE_ElementTemplate.dict.Count; ++i)
        {
            UCE_UI_ElementSlot slot2 = elementsContent.GetChild(i).GetComponent<UCE_UI_ElementSlot>();
            UCE_ElementTemplate ele = UCE_ElementTemplate.dict.Values.ElementAt(i);
            float points = 1.0f - player.UCE_CalculateElementalResistance(ele);

            slot2.tooltip.enabled = true;
            slot2.tooltip.text = ele.toolTip;
            slot2.image.sprite = ele.image;
            slot2.label.text = ele.name + ":";
            slot2.points.text = (points * 100).ToString("F0") + "%";
        }
    }

#endif

    // -----------------------------------------------------------------------------------
}
