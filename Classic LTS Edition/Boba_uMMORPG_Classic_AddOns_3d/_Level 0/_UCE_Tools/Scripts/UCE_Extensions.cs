
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using System.Linq;

public static class UCE_Extensions
{

    // -----------------------------------------------------------------------------------
    // GetDeterministicHashCode
    // -----------------------------------------------------------------------------------
    public static int GetDeterministicHashCode(this string value)
    {
        unchecked {

            int hash1 = (5381 << 16) + 5381;
            int hash2 = hash1;

            for (int i = 0; i < value.Length; i += 2)
            {
                hash1 = ((hash1 << 5) + hash1) ^ value[i];
                if (i == value.Length - 1)
                    break;
                hash2 = ((hash2 << 5) + hash2) ^ value[i + 1];
            }

            return hash1 + (hash2 * 1566083941);

        }
    }

    // -----------------------------------------------------------------------------------

}
