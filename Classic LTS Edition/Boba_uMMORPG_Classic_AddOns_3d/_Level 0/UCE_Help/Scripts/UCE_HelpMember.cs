// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;

[System.Serializable]
public partial struct UCE_HelpMember
{
    #region Variables

    public bool readBefore;         //Was the message already read?
    public string senderAcc;        //Who was the sender?
    public string senderCharacter;  //What character was the sender on?
    public string title;            //What is the report about?
    public string message;          //What are the details about this report?
    public bool solved;             //Was this reports ticket resolved?
    public string time;             //What was the time and date the report was made?
    public string position;         //Where was the player when the report was made?

    #endregion Variables

    #region Functions

    //Sets the following variables for use in-game.

    public UCE_HelpMember(bool _readBefore, string _senderAcc, string _senderCharacter, string _title, string _message, bool _solved, string _time, string _position)
    {
        readBefore = _readBefore;
        senderAcc = _senderAcc;
        senderCharacter = _senderCharacter;
        title = _title;
        message = _message;
        solved = _solved;
        time = _time;
        position = _position;
    }

    #endregion Functions
}

public class SyncListReport : SyncList<UCE_HelpMember> { }
