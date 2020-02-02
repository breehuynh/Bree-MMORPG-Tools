// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;
using UnityEngine;

// ===================================================================================
// UCE INTERACTABLE CLASS
// ===================================================================================
[RequireComponent(typeof(NetworkIdentity))]
public abstract partial class UCE_Interactable : NetworkBehaviour
{
    [Header("[-=-=-=- UCE INTERACTABLE -=-=-=-]")]
    public SpriteRenderer interactionSpriteRenderer;
    public string interactionText = "Interact with this Object";
    public Sprite interactionIcon;
    public bool automaticActivation;

    public UCE_InteractionRequirements interactionRequirements;

    protected UCE_UI_InteractableAccessRequirement instance;

    [SyncVar, HideInInspector] public bool unlocked = false;

    // -----------------------------------------------------------------------------------
    // Start
    // -----------------------------------------------------------------------------------
    public virtual void Start()
    {
        if (interactionIcon != null && interactionSpriteRenderer != null)
            interactionSpriteRenderer.sprite = interactionIcon;
    }

    // -----------------------------------------------------------------------------------
    // OnInteractClient
    // -----------------------------------------------------------------------------------
    //[ClientCallback]
    public virtual void OnInteractClient(Player player) { }

    // -----------------------------------------------------------------------------------
    // OnInteractServer
    // -----------------------------------------------------------------------------------
    //[ServerCallback]
    public virtual void OnInteractServer(Player player) { }

    // -----------------------------------------------------------------------------------
    // IsUnlocked
    // -----------------------------------------------------------------------------------
    public virtual bool IsUnlocked() { return false; }

    // -----------------------------------------------------------------------------------
    // ConfirmAccess
    // @Client
    // -----------------------------------------------------------------------------------
    public virtual void ConfirmAccess()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        if (interactionRequirements.checkRequirements(player) || IsUnlocked())
        {
            OnInteractClient(player);
            player.Cmd_UCE_OnInteractServer(this.gameObject);
        }
        else
        {
            if (automaticActivation)
                interactionRequirements.UpdateRequirementChat();
        }
    }

    // -----------------------------------------------------------------------------------
    // ShowAccessRequirementsUI
    // @Client
    // -----------------------------------------------------------------------------------
    protected virtual void ShowAccessRequirementsUI()
    {
        if (instance == null)
            instance = FindObjectOfType<UCE_UI_InteractableAccessRequirement>();

        instance.Show(this);
    }

    // -----------------------------------------------------------------------------------
    // HideAccessRequirementsUI
    // @Client
    // -----------------------------------------------------------------------------------
    protected void HideAccessRequirementsUI()
    {
        if (instance == null)
            instance = FindObjectOfType<UCE_UI_InteractableAccessRequirement>();

        instance.Hide();
    }

    // -----------------------------------------------------------------------------------
    // IsWorthUpdating
    // -----------------------------------------------------------------------------------
    public virtual bool IsWorthUpdating()
    {
        return netIdentity.observers == null ||
               netIdentity.observers.Count > 0;
    }

    // -----------------------------------------------------------------------------------
}
