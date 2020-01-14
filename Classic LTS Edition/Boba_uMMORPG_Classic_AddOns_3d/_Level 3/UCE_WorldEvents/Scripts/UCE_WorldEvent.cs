// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Text;
using Mirror;

// UCE WORLD EVENT

[System.Serializable]
public partial struct UCE_WorldEvent
{
    public string name;
    public bool participated;
    public int count;

    // -----------------------------------------------------------------------------------
    // Modify
    // -----------------------------------------------------------------------------------
    public void Modify(int value, bool showPopup = true)
    {
        bool bReset = false;

        foreach (UCE_WorldEventData data in template.thresholdData)
        {
            if (data.thresholdType == ThresholdType.Below &&
                count > data.thresholdValue &&
                count + value <= data.thresholdValue)
            {
                if (data.limitToThreshold)
                    count = data.thresholdValue;
                else if (data.resetOnThreshold)
                    bReset = true;

                if (!string.IsNullOrWhiteSpace(data.messageOnThreshold) && showPopup)
                    NetworkManagerMMO.UCE_BroadCastPopupToOnlinePlayers(template, data.messageParticipantsOnly, data.messageOnThreshold);

                if (data.stopFurtherThresholdChecks)
                    break;
            }
            else if (data.thresholdType == ThresholdType.Above &&
                count < data.thresholdValue &&
                count + value >= data.thresholdValue)
            {
                if (data.limitToThreshold)
                    count = data.thresholdValue;
                else if (data.resetOnThreshold)
                    bReset = true;

                if (!string.IsNullOrWhiteSpace(data.messageOnThreshold) && showPopup)
                    NetworkManagerMMO.UCE_BroadCastPopupToOnlinePlayers(template, data.messageParticipantsOnly, data.messageOnThreshold);

                if (data.stopFurtherThresholdChecks)
                    break;
            }
        }

        if (!bReset)
            count += value;
        else
            count = 0;

        participated = true;
    }

    // -----------------------------------------------------------------------------------
    // WorldEventTemplate (Getter)
    // -----------------------------------------------------------------------------------
    public UCE_WorldEventTemplate template
    {
        get { return UCE_WorldEventTemplate.dict[name.GetDeterministicHashCode()]; }
    }

    // -----------------------------------------------------------------------------------
}

public class SyncListUCE_WorldEvent : SyncList<UCE_WorldEvent> { }
