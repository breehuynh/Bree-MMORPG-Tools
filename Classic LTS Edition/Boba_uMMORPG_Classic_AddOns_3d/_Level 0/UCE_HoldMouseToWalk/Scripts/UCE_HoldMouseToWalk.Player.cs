// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// ===================================================================================
// HOLD MOUSE TO WALK - PLAYER
// ===================================================================================
public partial class Player : Entity
{
    //change this if you need to use different timing
    private float secondsBetweenMovementRequests = 0.5f;

    //shouldn't have to change anything below this
    private bool isMouseDown = false;

    private float nextMovement = 0f;

    // -----------------------------------------------------------------------------------
    // UpdateClient_HoldMouseToWalk
    // -----------------------------------------------------------------------------------
    [DevExtMethods("UpdateClient")]
    private void UpdateClient_HoldMouseToWalk()
    {
        //if mouse up, reset everything
        if (Input.GetMouseButtonUp(0))
        {
            //reset
            isMouseDown = false;
        }

        if (isLocalPlayer)
        {
            if (state == "IDLE" || state == "MOVING" || state == "CASTING")
            {
                //if mouse is down
                if (isMouseDown || (Input.GetMouseButtonDown(0) && !Utils.IsCursorOverUserInterface() && Input.touchCount <= 1))
                {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    //if hit something
                    if (Physics.Raycast(ray, out hit))
                    {
                        var entity = hit.transform.GetComponent<Entity>();

                        //if mouse is held down
                        if (isMouseDown)
                        {
                            //subtract the delta time
                            nextMovement -= Time.deltaTime;
                            //if it is time to move, initiate move and reset timer
                            if (nextMovement <= 0f)
                            {
                                nextMovement = secondsBetweenMovementRequests;

                                // set indicator and navigate to the nearest walkable
                                // destination. this prevents twitching when destination is
                                // accidentally in a room without a door etc.
                                var bestDestination = agent.NearestValidDestination(hit.point);
                                SetIndicatorViaPosition(bestDestination);

                                agent.destination = bestDestination;
                            }
                        }
                        else if (!entity)
                        {
                            //it's a movement target
                            //set mouseDown to true
                            isMouseDown = true;
                            //set the timer for next movement
                            nextMovement = secondsBetweenMovementRequests;
                        }
                    }
                }
            }
        }
    }
}
