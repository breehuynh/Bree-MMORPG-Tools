// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using Mirror;

public partial class NetworkManagerMMO : NetworkManager
{
    // -----------------------------------------------------------------------------------
    // OnServerCharacterCreate_UCE_Traits
    // -----------------------------------------------------------------------------------
    [DevExtMethods("OnServerCharacterCreate")]
    private void OnServerCharacterCreate_UCE_Traits(CharacterCreateMsg netMsg, Player player)
    {
        if (netMsg == null || netMsg.traits == null || netMsg.traits.Length == 0) return;

        foreach (int traitId in netMsg.traits)
        {
            if (traitId != 0)
            {
                UCE_TraitTemplate trait;
                if (UCE_TraitTemplate.dict.TryGetValue(traitId, out trait))
                {
                    player.UCE_Traits.Add(new UCE_Trait(trait));
					
					
#if _iMMOATTRIBUTES
					foreach (UCE_AttributeTemplate template in player.playerAttributes.UCE_AttributeTypes)
					{
						if (template == null) continue;
						UCE_Attribute attr = new UCE_Attribute(template);
						player.UCE_Attributes.Add(attr);
					}
#endif
									
                    // ----------------------------------------------------------------------
                    foreach (UCE_SkillRequirement startSkill in trait.startingSkills)
                    {
                        for (int i = 0; i < player.skills.Count; ++i)
                        {
                            if (player.skills[i].data == startSkill.skill)
                            {
                                Skill skill = player.skills[i];
                                skill.level += startSkill.level;
                                player.skills[i] = skill;
                            }
                        }
                    }

                    // ----------------------------------------------------------------------
                    foreach (UCE_ItemModifier startItem in trait.startingItems)
                    {
                        player.InventoryAdd(new Item(startItem.item), startItem.amount);
                    }

#if _iMMOPRESTIGECLASSES
                    if (trait.startingPrestigeClass != null)
                        player.UCE_prestigeClass = trait.startingPrestigeClass;
#endif
#if _iMMOHONORSHOP
                    foreach (UCE_HonorShopCurrencyCost currency in trait.startingHonorCurrency)
                        player.UCE_AddHonorCurrency(currency.honorCurrency, currency.amount);
#endif
#if _iMMOFACTIONS
                    foreach (UCE_FactionRating faction in trait.startingFactions)
                        player.UCE_AddFactionRating(faction.faction, faction.startRating);
#endif
#if _iMMOCRAFTING
                    foreach (UCE_CraftingProfessionRequirement prof in trait.startingCraftingProfession)
                    {
                        if (player.UCE_HasCraftingProfession(prof.template))
                        {
                            var tmpProf = player.UCE_getCraftingProfession(prof.template);
                            tmpProf.experience += prof.level;
                            player.UCE_setCraftingProfession(tmpProf);
                        }
                        else
                        {
                            UCE_CraftingProfession tmpProf = new UCE_CraftingProfession(prof.template.name);
                            tmpProf.experience += prof.level;
                            player.UCE_Crafts.Add(tmpProf);
                        }
                    }
#endif
#if _iMMOHARVESTING
                    foreach (UCE_HarvestingProfessionRequirement prof in trait.startingHarvestingProfession)
                    {
                        if (player.HasHarvestingProfession(prof.template))
                        {
                            var tmpProf = player.getHarvestingProfession(prof.template);
                            tmpProf.experience += prof.level;
                            player.SetHarvestingProfession(tmpProf);
                        }
                        else
                        {
                            UCE_HarvestingProfession tmpProf = new UCE_HarvestingProfession(prof.template.name);
                            tmpProf.experience += prof.level;
                            player.UCE_Professions.Add(tmpProf);
                        }
                    }
#endif
#if _iMMOPVP
                    player.UCE_setRealm(trait.changeRealm, trait.changeAlliedRealm);
#endif
					
					// ------------ Recalculate all Maxes here again (in case of bonusses)
					
					player.health = player.healthMax;
					player.mana = player.manaMax;
#if _iMMOSTAMINA
					player.stamina = player.staminaMax;
#endif
					
					// ----------------------------------------------------------------------
					
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
}
