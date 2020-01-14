// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;

[Serializable]
public class PayPalGetAccessTokenJsonResponse
{
    public string scope;
    public string access_token;
    public string token_type;
    public string app_id;
    public string expires_in;
}
