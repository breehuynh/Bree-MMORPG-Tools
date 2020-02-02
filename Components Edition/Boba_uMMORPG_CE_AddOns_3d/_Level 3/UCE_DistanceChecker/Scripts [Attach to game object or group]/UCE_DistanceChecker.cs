// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: 
 
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;

// UCE DISTANCE CHECKER

public partial class UCE_DistanceChecker : MonoBehaviour
{
    [Header("[-=-=- UCE DISTANCE CHECKER -=-=-]")]
    [Tooltip("How often is it updated?")]
    [Range(0.01f, 3f)] public float updateInterval = 0.25f;

    [Tooltip("Maximum distance from object position to player position?")]
    [Range(1, 999)] public int maxDistance = 100;

    public GameObject centerObject;

    protected float fInterval;
    protected Player player;
    protected Vector3 tilePosition;
    private bool init;

    // -----------------------------------------------------------------------------------
    // OnEnable
    // -----------------------------------------------------------------------------------
    private void OnEnable()
    {
        if (centerObject)
            tilePosition = centerObject.transform.position;
        else
            tilePosition = this.gameObject.transform.position;

        Activate(false);
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
        if (player == null)
            player = Player.localPlayer;

        if (player == null) return;

        if (Vector3.Distance(tilePosition, player.transform.position) >= maxDistance)
        {
            Activate(false);
        }
        else
        {
            Activate(true);
        }
    }

    // -----------------------------------------------------------------------------------
    // Activate
    // -----------------------------------------------------------------------------------
    private void Activate(bool setActive = true)
    {
        int childs = this.gameObject.transform.childCount;

        if (childs < 1) return;

        for (int i = childs - 1; i >= 0; i--)
            transform.GetChild(i).gameObject.SetActive(setActive);
    }

    // -----------------------------------------------------------------------------------
}
