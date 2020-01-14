// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
// Created and maintained by Davide Moriello

// * Discord Support: DaviDeMo#8519
// * E-mail support: davide@factorycreative.it

using Mirror;
using UnityEngine;
using UnityEngine.AI;

public partial class Player
{
    private Vector3 startJumpPosition;
    private Vector3 endJumpPosition;
    protected bool isJumping;
    private float jumpDelta;
    private float jumptimer;

    [Header("Player Jump Setup")]
    public float jumpHeight = 3f;
    public float maxDistanceJump = 5f;

    /**
     *
     * We calc the jump on client side so there server has less load. Server side we check if he can really jump with a
     * a lightweight code.
     *
     * **/

    [Client]
    public void calcJump(float horizontal, float vertical)
    {
        Vector3 startJumpPositiontmp = transform.position;
        Vector3 endJumpPositiontmp = transform.position + (agent.velocity.normalized * maxDistanceJump);
        jumpDelta = 0;

        //Player is still and not requesting to move with wasd movement
        if (agent.velocity == Vector3.zero && horizontal == 0 && vertical == 0)
        {
            JumpInPosition(startJumpPositiontmp);
            return;
        }

        //Player is still and wants to move and jump in a direction
        if ((horizontal != 0 || vertical != 0))
        {
            Vector3 input = new Vector3(horizontal, 0, vertical);
            if (input.magnitude > 1) input = input.normalized;

            // get camera rotation without up/down angle, only left/right
            Vector3 angles = Camera.main.transform.rotation.eulerAngles;
            angles.x = 0;
            Quaternion rotation = Quaternion.Euler(angles); // back to quaternion

            // calculate input direction relative to camera rotation
            Vector3 direction = rotation * input;
            endJumpPositiontmp = transform.position + (direction * maxDistanceJump);
        }

        float tParabola = 1;
        Vector3 parabola = Vector3.zero;
        Vector3 oldParabola = transform.position;
        bool foundCollider = false;
        for (float i = 0.1f; i < 1f; i += 0.1f)
        {
            parabola = MathParabola.Parabola(startJumpPositiontmp, endJumpPositiontmp, jumpHeight, i);

            Vector3 endCapsule = new Vector3(parabola.x, parabola.y + 2.667f, parabola.z);

            Debug.DrawLine(oldParabola, parabola, Color.white, 3f);
            oldParabola = parabola;

            Collider[] colliders = Physics.OverlapCapsule(parabola, endCapsule, 0.5f);
            foreach (Collider co in colliders)
            {
                Entity candidate = co.GetComponentInParent<Entity>();
                if (candidate == null)
                {
                    tParabola = i;
                    foundCollider = true;
                    break;
                }
            }
            if (foundCollider) break;
        }

        NavMeshHit hit;
        if (NavMesh.SamplePosition(parabola, out hit, 1f, NavMesh.AllAreas))
        {
            jumpDelta = 0;
            endJumpPosition = hit.position;
            startJumpPosition = startJumpPositiontmp;
            isJumping = true;
        }
        else
        {
            Vector3 endLinecast = parabola + (Vector3.down * 50);
            RaycastHit hit2;
            Debug.DrawLine(parabola + (Vector3.up), endLinecast, Color.red, 3f);

            if (Physics.Linecast(parabola + (Vector3.up), endLinecast, out hit2))
            {
                Debug.DrawLine(parabola + (Vector3.up), hit2.point, Color.green, 3f);

                if (NavMesh.SamplePosition(hit2.point, out hit, 1, NavMesh.AllAreas))
                {
                    jumpDelta = 0;
                    endJumpPosition = hit.position;
                    startJumpPosition = startJumpPositiontmp;
                    isJumping = true;
                }
            }
        }

        //If there is no place anywhere to land we jump in position
        if (!isJumping)
        {
            JumpInPosition(startJumpPositiontmp);
        }
    }

    private void JumpHandling(float horizontal, float vertical)
    {
        if (Input.GetKeyDown("space") && !Utils.IsCursorOverUserInterface() && !UIUtils.AnyInputActive())
        {
            if (!isJumping && (state == "IDLE" || state == "MOVING" || state == "CASTING"))
            {
                calcJump(horizontal, vertical);
                if (isJumping)
                {
                    CmdRequestJump(endJumpPosition);
                }
            }
        }
    }

    private void JumpInPosition(Vector3 startJumpPositiontmp)
    {
        //If you are not moving you just jump in position. No need to calc the jump.
        rubberbanding.ResetMovement();
        agent.ResetMovement();
        endJumpPosition = startJumpPositiontmp;
        startJumpPosition = startJumpPositiontmp;
        isJumping = true;
    }

    [Command]
    private void CmdRequestJump(Vector3 endjump)
    {
        if ((transform.position - endjump).magnitude > 10)
        {
            rubberbanding.ResetMovement();
        }
        else
        {
            if (Time.time - jumptimer < 1.0)
            {
                rubberbanding.ResetMovement();
                jumptimer = Time.time;
            }
            else
            {
                endJumpPosition = endjump;
                startJumpPosition = transform.position;
                isJumping = true;
                jumpDelta = 0;
                RpcSendJumpToAll(endjump);
            }
        }
    }

    private void LateUpdate_Jump()
    {
        animator.SetBool("JUMPING", isJumping);
    }

    private bool EventJumpRequest()
    {
        return isJumping;
    }

    private string UpdateServer_JUMPING()
    {
        // events sorted by priority (e.g. target doesn't matter if we died)
        if (EventDied())
        {
            OnDeath();
            return "DEAD";
        }
        if (EventStunned())
        {
            rubberbanding.ResetMovement();
            return "STUNNED";
        }
        if (EventJumpFinished())
        {
            return "IDLE";
        }

        ChangeJumpPosition();
        return "JUMPING";
    }

    private bool EventJumpFinished()
    {
        return !isJumping;
    }

    private void UpdateClient_Jump()
    {
        if (isJumping && isClientOnly)
        {
            ChangeJumpPosition();
            return;
        }
    }

    [ClientRpc]
    private void RpcSendJumpToAll(Vector3 endjump)
    {
        endJumpPosition = endjump;
        jumpDelta = 0;
        startJumpPosition = transform.position;
        isJumping = true;
    }

    private void ChangeJumpPosition()
    {
        jumpDelta += Time.deltaTime;

        transform.position = MathParabola.Parabola(startJumpPosition, endJumpPosition, 3f, Mathf.Clamp01(jumpDelta / 0.75f));

        if (jumpDelta / 0.75f >= 1.0f)
        {
            isJumping = false;
            agent.Warp(transform.position);
        }
    }
}
