// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections.Generic;
using UnityEngine;
using Action = System.Action;

// UCE LOOM MANAGER
// The LoomManager will moves things to another thread to process, then bring them back to main thread.
// Very useful for SQLite so you can send a save/whatever to another thread without blocking the main
// Unity thread.

public class UCE_LoomManager : MonoBehaviour
{
    public interface ILoom
    {
        void QueueOnMainThread(Action action);
    }

    private static NullLoom _nullLoom = new NullLoom();
    private static LoomDispatcher _loom;

    public static ILoom Loom
    {
        get
        {
            if (_loom != null)
            {
                return _loom as ILoom;
            }
            return _nullLoom as ILoom;
        }
    }

    private void Awake()
    {
        _loom = new LoomDispatcher();
    }

    private void OnDestroy()
    {
        _loom = null;
    }

    private void Update()
    {
        if (Application.isPlaying && _loom != null)
        {
            _loom.Update();
        }
    }

    private class NullLoom : ILoom
    {
        public void QueueOnMainThread(Action action)
        {
        }
    }

    private class LoomDispatcher : ILoom
    {
        private readonly List<Action> actions = new List<Action>();

        public void QueueOnMainThread(Action action)
        {
            lock (actions)
            {
                actions.Add(action);
            }
        }

        public void Update()
        {
            // Pop the actions from the synchronized list
            Action[] actionsToRun = null;
            lock (actions)
            {
                actionsToRun = actions.ToArray();
                actions.Clear();
            }
            // Run each action
            foreach (Action action in actionsToRun)
            {
                action();
            }
        }
    }
}
