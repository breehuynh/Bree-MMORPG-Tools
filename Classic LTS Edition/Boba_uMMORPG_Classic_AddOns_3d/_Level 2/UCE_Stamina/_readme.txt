Introduction

Adds a new stat "Stamina" besides Health and Mana to all Players. Stamina can be depleted
and recovered by various means. Stamina is similar to a energy system found in mobile
games. Stamina in general is setup to slowly deplete and it makes the players life harder
once it is depleted. But players are never incapaciated or die due to the loss of stamina.

- Every action except IDLE depletes stamina
- Skills can consume stamina as well
- But, players can still cast skills even when they have insufficient stamina

When stamina is depleted:

- The character is affected by a Buff as long as stamina == 0
- This buff can be defined in any way like a typical Buff

Stamina can be recovered:

- By using a "UCE Potion Item"
- By spending offline time in a "UCE Sanctuary"

Stamina can be modified:

- By "UCE Attributes"
- By passive skills
- By buffs
- By equipment

- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

Installation

IMPORTANT: This AddOn requires "UCE SmartInvoke (VIP3)" and "UCE Attributes" in order to function properly.

1. UI
1.1 Go to your Canvas, locate "healthMana"
1.2 Add "UIStamina" script as new component to it
1.3 Add the new "StaminaBar" prefab to the "HealthManaPanel"
1.4 Link all inspector slots in "UIStamina" component to the new "StaminaBar"

2. CORE CHANGES

2.1 NETWORKMANAGERMMO.cs
About line 374, add this line after
"player.mana = player.manaMax"
in CreateCharacter function:

player.stamina = player.staminaMax; // after equipment in case of boni

2.2 ENTITY.cs
About line 693, add this line after
"mana -= skill.manaCosts"
in FinishCastSkill function:

#if _iMMOSTAMINA
stamina -= skill.staminaCosts;
#endif

------------------------------------------------------------------------------------------
---- if you are using mySQL, skip the database changes below, we did already for you! ----
------------------------------------------------------------------------------------------

2.3 DATABASE.cs

2.3.1 Connect
insert below
"mana INTEGER NOT NULL,"
this:
stamina INTEGER NOT NULL,

2.3.2 CharacterLoad
insert below
"int mana = "
this:
int stamina				  = Convert.ToInt32((long)mainrow[8]);

2.3.3 CharacterLoad
insert below
"player.mana = mana;"
this:
player.stamina = stamina;

2.3.4 CharacterLoad
at the same spot, move all 3 lines:
"player.health = row.health;"
"player.mana = row.mana;"
"player.stamina = row.stamina;"

below this line:

Utils.InvokeMany(typeof(Database), this, "CharacterLoad_", player);

2.3.5 CharacterSave
insert after @mana in the ExecuteNonQuery
, @stamina
and further below after @mana insert new line:
new SqliteParameter("@stamina", player.stamina),

------------------------------------------------------------------------------------------
3. ITEMS
3.1 Create a new "UCE Potion Item" using the context menu
3.2 See the new section where it allows you to recover Stamina on usage

4. PLAYER PREFABS
4.1 See new section "Stamina" on player prefabs