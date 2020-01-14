// =======================================================================================
// Created and maintained by Boba
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using System.Collections;
using UnityEditor;

#if UNITY_EDITOR

[CustomEditor(typeof(Monster))]
public class MonsterEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Monster entity = (Monster)target;
        if (GUILayout.Button("Apply Blueprints"))
        {
            entity.ApplyBlueprints();
        }
    }
}

#endif
