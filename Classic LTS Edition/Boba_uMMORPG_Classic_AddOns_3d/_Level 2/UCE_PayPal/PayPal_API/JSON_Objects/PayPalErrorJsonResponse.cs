// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;
using System.Collections.Generic;

[Serializable]
public class PayPalErrorJsonResponse
{
    public string name;
    public string message;
    public string information_link;
    public string debug_id;

    public List<Detail> details;

    [Serializable]
    public class Detail
    {
        public string field;
        public string issue;
    }
}
