using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "Boba Addons", menuName = "Boba Addons/string and Factions/string and Factions", order = 0)]
public class Progression : ScriptableObject
{
    [SerializeField] RaceFactionsSystem[] characterClasses = null;

    Dictionary<EntityClassification, Dictionary<string, float[]>> lookupTable = null;

    public float GetStat(string race, EntityClassification entityClassification, int faction)
    {
        BuildLookup();

        float[] factions = lookupTable[entityClassification][race];

        if (factions.Length < faction)
        {
            return 0;
        }

        return factions[faction - 1];
    }

    public int GetLevels(string race, EntityClassification entityClassification)
    {
        BuildLookup();

        float[] factions = lookupTable[entityClassification][race];
        return factions.Length;
    }

    private void BuildLookup()
    {
        if (lookupTable != null) return;

        lookupTable = new Dictionary<EntityClassification, Dictionary<string, float[]>>();

        foreach (RaceFactionsSystem progressionClass in characterClasses)
        {
            var raceLookupTable = new Dictionary<string, float[]>();

            foreach (RaceSystem progressionStat in progressionClass.races)
            {
                raceLookupTable[progressionStat.race] = progressionStat.factions;
            }

            lookupTable[progressionClass.entityClassification] = raceLookupTable;
        }
    }

    [System.Serializable]
    class RaceFactionsSystem
    {
        public EntityClassification entityClassification;
        public RaceSystem[] races;
    }

    [System.Serializable]
    class RaceSystem
    {
        public string race;
        public float[] factions;
    }
}
