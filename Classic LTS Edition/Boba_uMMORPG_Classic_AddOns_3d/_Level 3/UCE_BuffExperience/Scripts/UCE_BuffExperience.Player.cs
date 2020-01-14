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

// PLAYER

public partial class Player
{
    // -----------------------------------------------------------------------------------
    // experience
    // -----------------------------------------------------------------------------------
    public long experience
    {
        get { return _experience; }
        set
        {
#if _iMMOBUFFEXPERIENCE
            float fExpFactor = buffs.Sum(x => x.boostExperience);

            if (fExpFactor != 0 && value > _experience)
            {
                long diff = Math.Max(value, _experience) - Math.Min(value, _experience);
                value = _experience + (long)Mathf.Round(diff * fExpFactor);
            }
#endif
            if (value <= _experience)
            {
                // decrease
                _experience = Math.Max(value, 0);
            }
            else
            {
                // increase with level ups
                // set the new value (which might be more than expMax)
                _experience = value;

                // now see if we leveled up (possibly more than once too)
                // (can't level up if already max level)
                while (_experience >= experienceMax && level < maxLevel)
                {
                    // subtract current level's required exp, then level up
                    _experience -= experienceMax;
                    ++level;

                    // addon system hooks
                    //this.InvokeInstanceDevExtMethods("OnLevelUp");
                    Utils.InvokeMany(typeof(Player), this, "OnLevelUp_");
                }

                // set to expMax if there is still too much exp remaining
                if (_experience > experienceMax) _experience = experienceMax;
            }
        }
    }

    // -----------------------------------------------------------------------------------
}
