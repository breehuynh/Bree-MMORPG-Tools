using Mirror;

// InfoBox

public partial class UCE_InfoBox : NetworkBehaviour
{
    public bool forceUseChat;

    protected UCE_UI_InfoBox instance;

    // -----------------------------------------------------------------------------------
    // TargetAddMessage
    // @Server -> @Client
    // -----------------------------------------------------------------------------------
    [TargetRpc]
    public void TargetAddMessage(NetworkConnection target, string message, byte color, bool show)
    {
        if (target != null || message != "")
            AddMessage(message, color, show);
    }

    // -----------------------------------------------------------------------------------
    // AddMessage
    // @Client
    // -----------------------------------------------------------------------------------
    [Client]
    public void AddMessage(string message, byte color, bool show = true)
    {
        if (forceUseChat)
        {
            GetComponent<Player>().chat.AddMsgInfo(message);
        }
        else
        {
            if (instance == null)
                instance = FindObjectOfType<UCE_UI_InfoBox>();

            instance.AddMsg(new InfoText(message, color), show);
        }
    }
}

// InfoText

[System.Serializable]
public class InfoText
{
    public string content;
    public byte color;

    public InfoText(string _info, byte _color)
    {
        content = _info;
        color = _color;
    }
}