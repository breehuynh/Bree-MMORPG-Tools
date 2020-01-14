// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;
using System.Collections.Generic;

[Serializable]
public class PayPalCreatePaymentJsonResponse
{
    public string id;
    public string intent;
    public string state;

    public Payer payer;

    public List<Transaction> transactions;

    public string create_time;

    public List<Link> links;

    [Serializable]
    public class Payer
    {
        public string payment_method;
    }

    [Serializable]
    public class Transaction
    {
        public Amount amount;
        public string description;
    }

    [Serializable]
    public class Amount
    {
        public string total;
        public string currency;
    }

    [Serializable]
    public class Link
    {
        public string href;
        public string rel;
        public string method;
    }
}
