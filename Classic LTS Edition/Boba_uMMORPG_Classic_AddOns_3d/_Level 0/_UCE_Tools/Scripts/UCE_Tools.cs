// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

// UCE TOOLS

public partial class UCE_Tools
{
    private const char CONST_DELIMITER = ';';

    public static int joy_tIdx = -1;
    public static bool pointerDown;

    // -----------------------------------------------------------------------------------
    // GetTouchDown
    // @Client
    // -----------------------------------------------------------------------------------
    public static bool GetTouchDown
    {
        get { return (joy_tIdx == -1 && Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began); }
    }

    // -----------------------------------------------------------------------------------
    // GetTouchUp
    // @Client
    // -----------------------------------------------------------------------------------
    public static bool GetTouchUp
    {
        get { return (joy_tIdx == -1 && Input.touchCount == 1 && (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled)); }
    }

    // -----------------------------------------------------------------------------------
    // ReachableRandomUnitCircleOnNavMesh
    // -----------------------------------------------------------------------------------
    public static Vector3 ReachableRandomUnitCircleOnNavMesh(Vector3 position, float radiusMultiplier, int solverAttempts = 3)
    {
        for (int i = 0; i < solverAttempts; ++i)
        {
            Vector3 candidate = RandomUnitCircleOnNavMesh(position, radiusMultiplier);

            NavMeshHit hit;
            if (!NavMesh.Raycast(position, candidate, out hit, NavMesh.AllAreas))
                return candidate;
        }

        return position;
    }

    // -----------------------------------------------------------------------------------
    // RandomUnitCircleOnNavMesh
    // -----------------------------------------------------------------------------------
    public static Vector3 RandomUnitCircleOnNavMesh(Vector3 position, float radiusMultiplier)
    {
        Vector2 r = UnityEngine.Random.insideUnitCircle * radiusMultiplier;

        Vector3 randomPosition = new Vector3(position.x + r.x, position.y, position.z + r.y);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPosition, out hit, radiusMultiplier * 2, NavMesh.AllAreas))
            return hit.position;
        return position;
    }

    // -----------------------------------------------------------------------------------
    // UCE_SelectionHandling
    // Checks if the client has clicked on any clickable object in the scene (not UI)
    // @Client
    // -----------------------------------------------------------------------------------
    public static bool UCE_SelectionHandling(GameObject target)
    {
        Player player = Player.localPlayer;
        if (!player) return false;

        if (player.isAlive &&
            Input.GetMouseButtonDown(0) && !Utils.IsCursorOverUserInterface() && Input.touchCount <= 1)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            bool cast = player.localPlayerClickThrough ? Utils.RaycastWithout(ray, out hit, player.gameObject) : Physics.Raycast(ray, out hit);

            if (cast && hit.transform.gameObject == target)
            {
                return true;
            }
        }

        return false;
    }

    // -----------------------------------------------------------------------------------
    // UCE_CheckSelectionHandling
    // Validates the interaction range, the player's state and if the player is alive or not
    // @Client OR @Server
    // -----------------------------------------------------------------------------------
    public static bool UCE_CheckSelectionHandling(GameObject target, Player localPlayer = null)
    {
        if (localPlayer == null)
            localPlayer = Player.localPlayer;

        if (!localPlayer || !target) return false;

        return localPlayer.isAlive &&
                (
                (target.GetComponent<Collider>() && OldClosestDistance(localPlayer.collider, target.GetComponent<Collider>()) <= localPlayer.interactionRange) ||
                (target.GetComponent<Entity>() && Utils.ClosestDistance(localPlayer, target.GetComponent<Entity>()) <= localPlayer.interactionRange)
                );
    }

    // OLD CLOSEST DISTANCE from before v1.84
    public static float OldClosestDistance(Collider a, Collider b)
    {
        // return 0 if both intersect or if one is inside another.
        // ClosestPoint distance wouldn't be > 0 in those cases otherwise.
        if (a.bounds.Intersects(b.bounds))
            return 0;

        // Unity offers ClosestPointOnBounds and ClosestPoint.
        // ClosestPoint is more accurate. OnBounds often doesn't get <1 because
        // it uses a point at the top of the player collider, not in the center.
        // (use Debug.DrawLine here to see the difference)
        return Vector3.Distance(a.ClosestPoint(b.transform.position),
                                b.ClosestPoint(a.transform.position));
    }

    // -----------------------------------------------------------------------------------
    // IntArrayToString
    // -----------------------------------------------------------------------------------
    public static string IntArrayToString(int[] array)
    {
        if (array == null || array.Length == 0) return null;
        string arrayString = "";
        for (int i = 0; i < array.Length; i++)
        {
            arrayString += array[i].ToString();
            if (i < array.Length - 1)
                arrayString += CONST_DELIMITER;
        }
        return arrayString;
    }

    // -----------------------------------------------------------------------------------
    // IntStringToArray
    // -----------------------------------------------------------------------------------
    public static int[] IntStringToArray(string array)
    {
        if (string.IsNullOrWhiteSpace(array)) return null;
        string[] tokens = array.Split(CONST_DELIMITER);
        int[] arrayInt = Array.ConvertAll<string, int>(tokens, int.Parse);
        return arrayInt;
    }

    // -----------------------------------------------------------------------------------
    // FindOnlinePlayerByName
    // -----------------------------------------------------------------------------------
    public static Player FindOnlinePlayerByName(string playerName)
    {
        if (!string.IsNullOrWhiteSpace(playerName))
        {
            if (Player.onlinePlayers.ContainsKey(playerName))
            {
                return Player.onlinePlayers[playerName].GetComponent<Player>();
            }
        }
        return null;
    }

    // -------------------------------------------------------------------------------
    // ArrayContains
    // -------------------------------------------------------------------------------
    public static bool ArrayContains(int[] defines, int define)
    {
        foreach (int def in defines)
        {
            if (def == define)
                return true;
        }
        return false;
    }

    // -------------------------------------------------------------------------------
    // ArrayContains
    // -------------------------------------------------------------------------------
    public static bool ArrayContains(string[] defines, string define)
    {
        foreach (string def in defines)
        {
            if (def == define)
                return true;
        }
        return false;
    }

    // -------------------------------------------------------------------------------
    // RemoveFromArray
    // -------------------------------------------------------------------------------
    public static string[] RemoveFromArray(string[] defines, string define)
    {
        return defines.Where(x => x != define).ToArray();
    }

    // -----------------------------------------------------------------------------------
}
