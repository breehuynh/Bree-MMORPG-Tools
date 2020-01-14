using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public partial class Entity
{
    // Sets new target
    public void PartialOnAggro(Entity entity)
    {
        target = entity;
        OnAggro(target);
    }
}
