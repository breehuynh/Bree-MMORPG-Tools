## Target Taunt Skill

*Made by bobatea#9400 on Discord!*

### DESCRIPTION:

- Taunts targetted enemy if player is within enemy's range
- Has taunting if taunting chance is greater than 0
- Spawns a Taunt effect if successfully taunted

### INSTRUCTIONS: 

1. Drag and drop the files to your project
2. Add a new "Taunt Attack" animation state to your Player's animator controller
3. Add a new animation bool parameter called "Taunt Attack" to your Player's animator controller
4. Create a transition from the "Entry" state to the new "Taunt Attack" state with the conditions that "Taunt Attack" == True and "CASTING" == True
5. Create a transition back to the "Grounded" state with the conditions that "Taunt Attack" == False and "CASTING" == False
6. Add the Taunt Attack prefab to the Network Manager's Spawnable Prefabs list
