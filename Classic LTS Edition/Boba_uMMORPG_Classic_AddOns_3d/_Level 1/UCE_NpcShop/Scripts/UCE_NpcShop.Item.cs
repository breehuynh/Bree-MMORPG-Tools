// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Text;

public partial struct Item
{
    public string itemCategory
    {
        get { return data.itemCategory; }
    }

    [DevExtMethods("ToolTip")]
    private void ToolTip_UCE_NpcShop(StringBuilder tip)
    {
        tip.Replace("{ITEMCATEGORY}", itemCategory);
    }
}
