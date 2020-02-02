// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
// Note: this script has to be on an always-active UI parent, so that we can
// always find it from other code. (GameObject.Find doesn't find inactive ones)
using Mirror;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

// UCE UI SEND MAIL

public partial class UCE_UI_SendMail : MonoBehaviour
{
    public static UCE_UI_SendMail singleton;

    protected NetworkManagerMMO manager;
    protected NetworkAuthenticatorMMO auth;

    public GameObject panel;

    public RectTransform searchPanel;
    public InputField search;
    public RectTransform searchContent;
    public UIMailSearchSlot searchSlot;
    public Button searchButton;

    public RectTransform messagePanel;
    public Text recipientText;
    public InputField subject;
    public InputField body;

    public Text dialogMessage;
    public GameObject dialog;
    public Button dialogButton;

    public Button acceptButton;
    public Button cancelButton;

    public UCE_UI_MailItemSlot slotPrefab;
    public UIDragAndDropable itemSlot;

    private bool sending = false;
    [HideInInspector] public string recipient;
    [HideInInspector] public int itemIndex = -1;

    // -----------------------------------------------------------------------------------
    // UCE_UI_SendMail
    // -----------------------------------------------------------------------------------
    public UCE_UI_SendMail()
    {
        // assign singleton only once (to work with DontDestroyOnLoad when
        // using Zones / switching scenes)
        if (singleton == null) singleton = this;
    }

    // -----------------------------------------------------------------------------------
    // Show
    // -----------------------------------------------------------------------------------
    public void Show()
    {
        itemIndex = -1;
        panel.SetActive(true);
    }

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    private void Update()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        if (manager == null)
            manager = FindObjectOfType<NetworkManagerMMO>();

        if (auth == null)
            auth = manager.GetComponent<NetworkAuthenticatorMMO>();

        if (manager != null && panel.activeSelf)
        {
            // item slot
            if (itemIndex != -1)
            {
                ScriptableItem item = player.inventory.slots[itemIndex].item.data;
                itemSlot.GetComponent<Image>().color = Color.white;
                itemSlot.GetComponent<Image>().sprite = item.image;
                itemSlot.GetComponent<UIShowToolTip>().enabled = true;
                itemSlot.GetComponent<UIShowToolTip>().text = item.ToolTip();
            }
            else
            {
                itemSlot.GetComponent<Image>().color = Color.clear;
                itemSlot.GetComponent<Image>().sprite = null;
                itemSlot.GetComponent<UIShowToolTip>().enabled = false;
            }

            // no one selected yet, show search box
            if (string.IsNullOrEmpty(recipient))
            {
                searchPanel.gameObject.SetActive(true);
                messagePanel.gameObject.SetActive(false);
                itemIndex = -1;

                if (search.text.Length > 0 &&
                    search.text.Length <= auth.accountMaxLength &&
                    Regex.IsMatch(search.text, @"^[a-zA-Z0-9_]+$"))
                {
                    searchButton.interactable = true;
                }
                else
                {
                    searchButton.interactable = false;
                }
                searchButton.onClick.SetListener(() =>
                {
                    if (NetworkTime.time >= player.nextRiskyActionTime)
                    {
                        //prepare and send search request, get response
                        sending = true;
                        search.interactable = false;
                        searchButton.interactable = false;
                        player.CmdMail_Search(search.text);
                        dialogMessage.text = "Searching...";
                        dialog.SetActive(true);
                    }
                });
            }
            else
            {
                searchPanel.gameObject.SetActive(false);
                messagePanel.gameObject.SetActive(true);
                recipientText.text = recipient;

                acceptButton.interactable = !string.IsNullOrEmpty(subject.text) && player.mailSettings.costPerMail.checkCost(player);
                acceptButton.onClick.SetListener(() =>
                {
                    if (NetworkTime.time >= player.nextRiskyActionTime)
                    {
                        sending = true;
                        dialogMessage.text = "Sending Mail...";
                        dialog.SetActive(true);
                        player.CmdMail_Send(recipient, subject.text, body.text, itemIndex);
                        itemIndex = -1;
                    }
                });

                dialogButton.onClick.SetListener(() =>
                {
                    cancelButton.onClick.Invoke();
                });
            }

            //show the dialog button if we are not sending
            dialogButton.gameObject.SetActive(!sending);

            // cancel
            cancelButton.interactable = !sending;
            cancelButton.onClick.SetListener(() =>
            {
                recipient = "";
                search.text = "";
                subject.text = "";
                body.text = "";
                sending = false;
                itemIndex = -1;
                UIUtils.BalancePrefabs(searchSlot.gameObject, 0, searchContent);
                dialog.SetActive(false);
                panel.SetActive(false);
            });
        }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public void UpdateSearchResults(List<MailSearch> results)
    {
        UIUtils.BalancePrefabs(searchSlot.gameObject, results.Count, searchContent);

        for (int i = 0; i < results.Count; ++i)
        {
            UIMailSearchSlot slot = searchContent.GetChild(i).GetComponent<UIMailSearchSlot>();
            slot.nameText.text = results[i].name;
            slot.levelText.text = results[i].level.ToString();
            slot.guildText.text = results[i].guild == null ? "" : results[i].guild;
            slot.actionButton.onClick.SetListener(() =>
            {
                recipient = slot.nameText.text;
            });
        }

        searchButton.interactable = true;
        search.interactable = true;
        sending = false;
        dialog.SetActive(false);
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public void MailMessageSent(string status)
    {
        dialogMessage.text = status;
        sending = false;
        itemIndex = -1;
        dialog.SetActive(true);
    }

    // -----------------------------------------------------------------------------------
}
