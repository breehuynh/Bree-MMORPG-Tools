// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using System;
using UnityEngine;

//

[System.Serializable]
public partial struct MailMessage
{
    public override bool Equals(System.Object obj)
    {
        return obj is MailMessage && this == (MailMessage)obj;
    }

    public override int GetHashCode()
    {
        return id.GetHashCode() ^ read.GetHashCode() ^ deleted.GetHashCode();
    }

    public static bool operator ==(MailMessage x, MailMessage y)
    {
        //only need to compare id and the 2 fields that are updated after initial insert.
        return x.id == y.id && x.read == y.read && x.deleted == y.deleted;
    }

    public static bool operator !=(MailMessage x, MailMessage y)
    {
        return !(x == y);
    }

    public string itemName;

    public ScriptableItem item
    {
        set
        {
            if (value != null)
                itemName = value.name;
            else
                itemName = "";
        }
        get
        {
            if (string.IsNullOrEmpty(itemName)) return null;
            ScriptableItem itemData;
            if (ScriptableItem.dict.TryGetValue(itemName.GetStableHashCode(), out itemData))
                return itemData;
            return null;
        }
    }

    public long id;

    public string from;
    public string to;

    public string subject;
    public string body;

    public long sent;

    public string sentAt
    {
        get { return timeSince(Epoch.Current(), sent) + " ago"; }
    }

    public long expires;

    public string expiresAt
    {
        get { return timeSince(Epoch.Current(), expires); }
    }

    public long read;
    public long deleted;

    public string timeSince(long a, long b)
    {
        const int SECOND = 1;
        const int MINUTE = 60 * SECOND;
        const int HOUR = 60 * MINUTE;
        const int DAY = 24 * HOUR;
        const int MONTH = 30 * DAY;

        //correct for change to milliseconds
        int A = Mathf.RoundToInt(a / 1000);
        int B = Mathf.RoundToInt(b / 1000);

        int delta = Mathf.Abs(A - B);
        TimeSpan ts = new TimeSpan(0, 0, delta);

        if (delta < 1 * MINUTE)
            return ts.Seconds + " seconds";

        if (delta < 45 * MINUTE)
            return ts.Minutes + " minutes";

        if (delta < 90 * MINUTE)
            return "an hour";

        if (delta < 24 * HOUR)
            return ts.Hours + " hours";

        if (delta < 30 * DAY)
            return ts.Days + " days";

        if (delta < 12 * MONTH)
        {
            int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
            return months + " months";
        }
        else
        {
            int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years + " years";
        }
    }
}

public class SyncListMailMessage : SyncList<MailMessage> { }
