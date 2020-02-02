// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// ConfigurationManager

public partial class ConfigurationManager : MonoBehaviour
{

    [Header("Configuration")]
    public UCE_TemplateConfiguration configTemplate;
	
	[Header("Defines")]
	public UCE_TemplateDefines addonTemplate;

    [Header("Game Rules")]
    public UCE_TemplateGameRules rulesTemplate;

    // -----------------------------------------------------------------------------------
    // 
    // -----------------------------------------------------------------------------------
    void OnValidate()
	{
	
		
	
	}
	
	
	// -----------------------------------------------------------------------------------
	// 
	// -----------------------------------------------------------------------------------




    // -----------------------------------------------------------------------------------
    
}
