## Target AoE Skill

*Made by bobatea#9400 on Discord!*

### DESCRIPTION:

- Targets all enemies in the specified radius
- Has an optional AoE taunting effect if taunting chance is greater than 0
- Spawns an AoE effect

### INSTRUCTIONS: 

1. Install via the .unitypackage or drag and drop the files to your project
2. Add a new "AoE Attack" animation state to your Player's animator controller
3. Add a new animation bool parameter called "AoE Attack" to your Player's animator controller
4. Create a transition from the "Entry" state to the new "AoE Attack" state with the conditions that "AoE Attack" == True and "CASTING" == True
5. Create a transition back to the "Grounded" state with the conditions that "AoE Attack" == False and "CASTING" == False
6. Add the AoE Attack prefab to the Network Manager's Spawnable Prefabs list
