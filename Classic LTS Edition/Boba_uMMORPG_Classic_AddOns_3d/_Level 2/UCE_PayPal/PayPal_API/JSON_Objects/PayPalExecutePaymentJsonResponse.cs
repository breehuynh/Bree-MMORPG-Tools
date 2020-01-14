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
public class PayPalExecutePaymentJsonResponse
{
    public string id;
    public string intent;
    public string state;
    public string cart;
    public string create_time;
    public Payer payer;
    public List<Transaction> transactions;
    public RedirectURLs redirect_urls;
    public List<Link> links;

    [Serializable]
    public class Link
    {
        public string href;
        public string rel;
        public string method;
    }

    [Serializable]
    public class RedirectURLs
    {
        public string return_url;
        public string cancel_url;
    }

    [Serializable]
    public class Transaction
    {
        public Amount amount;
        public Payee payee;
        public string description;
        public string invoice_number;
        public ItemList item_list;
        public List<RelatedResources> related_resources;
    }

    [Serializable]
    public class Item
    {
        public string name;
        public string description;
        public string price;
        public string currency;
        public string quantity;
    }

    [Serializable]
    public class ShippingAddress
    {
        public string recipient_name;
        public string line1;
        public string city;
        public string state;
        public string postal_code;
        public string country_code;
    }

    [Serializable]
    public class ItemList
    {
        public List<Item> items;
        public ShippingAddress shipping_address;
    }

    [Serializable]
    public class Amount
    {
        public string total;
        public string currency;
        public Details details;
    }

    [Serializable]
    public class Payee
    {
        public string merchant_id;
        public string email;
    }

    [Serializable]
    public class Payer
    {
        public string payment_method;
        public string status;
        public PayerInfo payer_info;
    }

    [Serializable]
    public class PayerInfo
    {
        public string email;
        public string first_name;
        public string payer_id;
        public string country_code;
        public ShippingAddress shipping_address;
    }

    [Serializable]
    public class RelatedResources
    {
        public Sale sale;
    }

    [Serializable]
    public class Sale
    {
        public string id;
        public string state;
        public Amount amount;
        public string payment_mode;
        public string protection_eligibility;
        public string protection_eligibility_type;
        public TransactionFee transaction_fee;
        public string parent_payment;
        public string create_time;
        public string update_time;
        public List<Link> links;
    }

    [Serializable]
    public class Details
    {
        public string subtotal;
    }

    [Serializable]
    public class TransactionFee
    {
        public string value;
        public string currency;
    }
}
