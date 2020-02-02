// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UCE_UI_ImprovedTarget : MonoBehaviour
{
    [Header("Target")]
    public GameObject panel;

    public Slider healthSlider;
    public Text nameText;
    public Transform buffsPanel;
    public UIBuffSlot buffSlotPrefab;
    public Button tradeButton, guildInviteButton, partyInviteButton;

    [Header("=-=-= UCE Improved Target =-=-=")]
    public Text healthText;

    public Text levelText;
    public GameObject challengeObject;
    public GameObject eliteObject;
    public GameObject bossObject;
    public bool nameColoring = false;

    private void Update()
    {
        Player player = Player.localPlayer;
        if (player != null)
        {
            // show nextTarget > target
            Entity target = player.nextTarget ?? player.target;
            if (target != null && target != player)
            {
                float distance = Utils.ClosestDistance(player, target);

                if (!(target is Player) && target.health.current > 0 && distance < 50) SetupTarget(player, target, distance);
                else if (target is Player && distance < 50) SetupTarget(player, target, distance);
                else panel.SetActive(false);
            }
            else panel.SetActive(false);
        }
        else panel.SetActive(false);
    }

    // Performs all required setup for our target.
    private void SetupTarget(Player player, Entity target, float distance)
    {
        // name and health
        panel.SetActive(true);
        nameText.text = target.name;
        healthSlider.value = target.health.Percent();
        healthText.text = target.health.ToString();
        levelText.text = target.level.ToString();

        BuffControl(target);
        ButtonControl(player, target, distance);
        TargetControl(player, target);
    }

    // Controls all functionality for buffs on target.
    private void BuffControl(Entity target)
    {
        // target buffs
        UIUtils.BalancePrefabs(buffSlotPrefab.gameObject, target.skills.buffs.Count, buffsPanel);
        for (int i = 0; i < target.skills.buffs.Count; ++i)
        {
            UIBuffSlot slot = buffsPanel.GetChild(i).GetComponent<UIBuffSlot>();

            // refresh
            slot.image.color = Color.white;
            slot.image.sprite = target.skills.buffs[i].image;
            slot.tooltip.text = target.skills.buffs[i].ToolTip();
            slot.slider.maxValue = target.skills.buffs[i].buffTime;
            slot.slider.value = target.skills.buffs[i].BuffTimeRemaining();
        }
    }

    // Controls all functionality for buttons on target.
    private void ButtonControl(Player player, Entity target, float distance)
    {
        // trade button
        if (target is Player)
        {
            tradeButton.gameObject.SetActive(true);
            tradeButton.interactable = player.trading.CanStartTradeWith(target);
            tradeButton.onClick.SetListener(() =>
            {
                player.trading.CmdSendRequest();
            });
        }
        else tradeButton.gameObject.SetActive(false);

        // guild invite button
        if (target is Player && player.guild.InGuild())
        {
            guildInviteButton.gameObject.SetActive(true);
            guildInviteButton.interactable = !((Player)target).guild.InGuild() &&
                                             player.guild.guild.CanInvite(player.name, target.name) &&
                                             NetworkTime.time >= player.nextRiskyActionTime &&
                                             distance <= player.interactionRange;
            guildInviteButton.onClick.SetListener(() =>
            {
                player.guild.CmdInviteTarget();
            });
        }
        else guildInviteButton.gameObject.SetActive(false);

        // party invite button
        if (target is Player)
        {
            partyInviteButton.gameObject.SetActive(true);
            partyInviteButton.interactable = (!player.party.InParty() || !player.party.party.IsFull()) &&
                                             !((Player)target).party.InParty() &&
                                             NetworkTime.time >= player.nextRiskyActionTime &&
                                             distance <= player.interactionRange;
            partyInviteButton.onClick.SetListener(() =>
            {
                player.party.CmdInvite(target.name);
            });
        }
        else partyInviteButton.gameObject.SetActive(false);
    }

    // Controls all functionality for improved target.
    private void TargetControl(Player player, Entity target)
    {
        // Setup Elite
        if (target.isElite) eliteObject.SetActive(true);
        else eliteObject.SetActive(false);

        // Setup Boss
        if (target.isBoss) bossObject.SetActive(true);
        else bossObject.SetActive(false);

        // Setup Level Info
        levelText.gameObject.SetActive(true);
        challengeObject.SetActive(false);
        if (target.level.current <= (player.level.current + 1) && target.level.current >= (player.level.current - 1)) { levelText.color = Color.white; nameText.color = Color.white; }
        else if (target.level.current <= (player.level.current + 3) && target.level.current >= player.level.current + 1) { levelText.color = Color.yellow; nameText.color = Color.yellow; }
        else if (target.level.current <= (player.level.current + 4) && target.level.current >= player.level.current + 3) { levelText.color = Color.red; nameText.color = Color.red; }
        else if (target.level.current >= (player.level.current - 3) && target.level.current <= player.level.current - 1) { levelText.color = Color.green; nameText.color = Color.green; }
        else if (target.level.current >= (player.level.current - 4) && target.level.current <= player.level.current - 3) { levelText.color = Color.grey; nameText.color = Color.grey; }
        else
        {
            levelText.gameObject.SetActive(false);
            challengeObject.SetActive(true);
            nameText.color = Color.white;
        }
    }
}
