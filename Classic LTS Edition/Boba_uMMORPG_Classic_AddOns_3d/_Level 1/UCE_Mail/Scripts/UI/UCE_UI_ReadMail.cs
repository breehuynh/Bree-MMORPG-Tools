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
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// UCE UI READ MAIL

public partial class UCE_UI_ReadMail : MonoBehaviour
{
    public RectTransform messagesContent;
    public RectTransform readContent;
    public GameObject messageSlot;
    public GameObject readMailPanel;
    public UCE_UI_SendMail sendMailPanel;
    public Button newMailButton;
    public Button takeItemButton;
    public Button deleteButton;
    public Text receivedText;
    public Text expiresText;
    public Text fromText;
    public Text subjectText;
    public Text bodyText;

    private int readingIndex = -1;
    private int cnt = 0;

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    private void Update()
    {
        Player player = Player.localPlayer;
        if (player == null) return;

        long current = Epoch.Current();

        //-- setup send mail button
        newMailButton.gameObject.SetActive(player.mailSettings.mailSendFromAnywhere);
        newMailButton.onClick.SetListener(() =>
        {
            sendMailPanel.Show();
            readMailPanel.SetActive(false);
        });

        // -- setup delete button

        deleteButton.onClick.SetListener(() =>
        {
            if (NetworkTime.time >= player.nextRiskyActionTime)
            {
                readingIndex = -1;

                for (int i = 0; i < messagesContent.childCount; ++i)
                {
                    int idx = i;
                    UIMailMessageSlot slot = messagesContent.GetChild(idx).GetComponent<UIMailMessageSlot>();
                    if (slot.toggle.isOn)
                        player.CmdMail_DeleteMessage(slot.mailIndex);
                }
            }
        });

        //-- setup take item button
        takeItemButton.gameObject.SetActive(readingIndex > -1);

        //count messages that haven't expired yet
        int mailCount = player.mailMessages.Count((m) => current <= m.expires);
        UIUtils.BalancePrefabs(messageSlot, mailCount, messagesContent);

        if (mailCount != cnt)
        {
            cnt = mailCount;
        }

        int slotIndex = -1;

        //loop over messages backwards because we add to the synclist so newer messages appear at end, we want to display newer on top
        for (int mailIndex = player.mailMessages.Count - 1; mailIndex >= 0; mailIndex--)
        {
            MailMessage message = player.mailMessages[mailIndex];

            //if message has expired, skip it
            if (current > message.expires) continue;

            int tmpIndex = mailIndex;

            slotIndex++;
            UIMailMessageSlot slot = messagesContent.GetChild(slotIndex).GetComponent<UIMailMessageSlot>();
            slot.textReceived.text = message.sentAt;
            slot.textFrom.text = message.from;
            slot.textSubject.text = message.subject;
            slot.mailIndex = tmpIndex;

            if (message.item != null)
            {
                ItemSlot item = new ItemSlot(new Item(message.item));
                slot.itemSlot.interactable = false;
                slot.itemSlot.GetComponent<Image>().color = Color.white;
                slot.itemSlot.GetComponent<Image>().sprite = message.item.image;
                slot.itemSlot.GetComponent<UIShowToolTip>().enabled = true;
                slot.itemSlot.GetComponent<UIShowToolTip>().text = item.ToolTip();
            }
            else
            {
                slot.itemSlot.interactable = false;
                slot.itemSlot.GetComponent<Image>().color = Color.clear;
                slot.itemSlot.GetComponent<Image>().sprite = null;
                slot.itemSlot.GetComponent<UIShowToolTip>().enabled = false;
            }

            //if the message has been read, show normal font, else bold text
            if (message.read > 0)
            {
                slot.textReceived.fontStyle = FontStyle.Normal;
                slot.textFrom.fontStyle = FontStyle.Normal;
                slot.textSubject.fontStyle = FontStyle.Normal;
            }
            else
            {
                slot.textReceived.fontStyle = FontStyle.Bold;
                slot.textFrom.fontStyle = FontStyle.Bold;
                slot.textSubject.fontStyle = FontStyle.Bold;
            }

            //click on slot, read message
            slot.readButton.onClick.SetListener(() =>
            {
                readingIndex = tmpIndex;
                if (message.read == 0)
                {
                    player.CmdMail_ReadMessage(tmpIndex);
                }
            });
        }

        // currently selected message
        if (readingIndex > -1)
        {
            MailMessage reading = player.mailMessages[readingIndex];

            readContent.gameObject.SetActive(true);
            receivedText.text = reading.sentAt;
            expiresText.text = reading.expiresAt;
            fromText.text = reading.from;
            subjectText.text = reading.subject;
            bodyText.text = reading.body;

            if (reading.item != null)
            {
                ItemSlot item = new ItemSlot(new Item(reading.item));
                takeItemButton.onClick.SetListener(() =>
                {
                    player.CmdMail_TakeItem(readingIndex);
                    takeItemButton.interactable = false;
                });
                takeItemButton.interactable = true;
                takeItemButton.GetComponent<Image>().color = Color.white;
                takeItemButton.GetComponent<Image>().sprite = reading.item.image;
                takeItemButton.GetComponent<UIShowToolTip>().enabled = true;
                takeItemButton.GetComponent<UIShowToolTip>().text = item.ToolTip();
            }
            else
            {
                takeItemButton.onClick.RemoveAllListeners();
                takeItemButton.interactable = false;
                takeItemButton.GetComponent<Image>().color = Color.clear;
                takeItemButton.GetComponent<Image>().sprite = null;
                takeItemButton.GetComponent<UIShowToolTip>().enabled = false;
            }
        }
        else
        {
            readContent.gameObject.SetActive(false);
        }
    }

    // -----------------------------------------------------------------------------------
}
