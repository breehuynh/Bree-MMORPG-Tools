// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using Mirror;
using System.Linq;

// ACTIVATEABLE OBJECT

public partial class UCE_ActivateableObject : NetworkBehaviour
{
    [Tooltip("[Required] Link the activated game object here (its usually a child of this)")]
    public GameObject activateableObject;

    [Tooltip("[Required] Is the linked object visible by default or not?")]
    [SyncVar] public bool _visible = true;

    [Tooltip("[Optional] Automatic reset to default visibility after x seconds (0 to disable)")]
    public float resetVisibility = 0;

#if _iMMOWORLDEVENTS

    [Header("[UCE WORLD EVENT (object visibility is based on event status)]")]
    [Tooltip("[Optional] This world event will be checked")]
    public UCE_WorldEventTemplate worldEvent;

    [Tooltip("[Optional] Min count the world event has been completed (0 to disable)")]
    public int minEventCount;

    [Tooltip("[Optional] Max count the world event has been completed (0 to disable)")]
    public int maxEventCount;

#endif

    [Header("[UPDATE THROTTLE]")]
    [SerializeField] [Range(0.01f, 3f)] private float updateInterval = 0.25f;

    protected bool _defaultVisible = true;
    protected float fInterval;

    // -----------------------------------------------------------------------------------
    // Start
    // -----------------------------------------------------------------------------------
    private void Start()
    {
#if _iMMOWORLDEVENTS
        if (worldEvent != null)
            _defaultVisible = NetworkManagerMMO.UCE_CheckWorldEvent(worldEvent, minEventCount, maxEventCount);
        else
            _defaultVisible = _visible;
#else
		_defaultVisible = _visible;
#endif
    }

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    private void Update()
    {
        if (Time.time > fInterval)
        {
            UCE_SlowUpdate();
            fInterval = Time.time + updateInterval;
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_SlowUpdate
    // -----------------------------------------------------------------------------------
    private void UCE_SlowUpdate()
    {
        activateableObject.SetActive(_visible);
    }

    // -----------------------------------------------------------------------------------
    // Toggle
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    public void Toggle(bool visible)
    {
        if (visible != _visible)
        {
            _visible = visible;
            if (resetVisibility > 0)
                Invoke("Reset", resetVisibility);
        }
    }

    // -----------------------------------------------------------------------------------
    // Reset
    // -----------------------------------------------------------------------------------
    public void Reset()
    {
        Toggle(_defaultVisible);
    }

    // -----------------------------------------------------------------------------------
}
