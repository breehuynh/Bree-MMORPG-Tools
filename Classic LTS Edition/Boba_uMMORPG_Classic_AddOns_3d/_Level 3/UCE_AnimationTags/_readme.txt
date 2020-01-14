Introduction

By default all Animations in uMMORPG are played using the skills name. This requires you
to create a unique animation parameter and animation per skill in your game and you cannot
have multiple skills that use the same animation.

This addon solves that issue: instead of using the name, you assign a tag to each skill
and the skill animation is played using this tag (instead of the name). This allows you
to reduce the amont of animation parameters and animations in your project and makes it
possible to re-use the same animation for a number of skills.

Installation

1. Search your project code and replace this on Enity, Player, Monster, Pet and so on:

foreach (Skill skill in skills)
                animator.SetBool(skill.name, skill.CastTimeRemaining() > 0);
                
simply with this:

foreach (Skill skill in skills)
                animator.SetBool(skill.animationTag, skill.CastTimeRemaining() > 0);
                
2. Edit the new "animationTag" slot in each one of your skills.

3. The name of your animation parameter + animation must match exactly the name of the
animationTag now.

------------------------------------------------------------------------------------------

Alternatively you can also use this code snippet below as it solves a few other issues as
well (like adjusting duration of skill animations to cast time and checking for each
animator for its existence, this prevents a NIL error when a npc does not have a casting
animation state for example):

// -----------------------------------------------------------------------------------
// LateUpdate
// -----------------------------------------------------------------------------------
public virtual void LateUpdate()
{
	if (isClient)
	{
		
		foreach (Animator anim in GetComponentsInChildren<Animator>()) {
		
			if (animator.runtimeAnimatorController != null) {
			
				if (state != "CASTING")
					anim.speed = 1;
				
				if (animator.parameters.Any(x => x.name == "MOVING"))
					anim.SetBool("MOVING", IsMoving() && state != "CASTING" && !IsMounted());
				
				if (animator.parameters.Any(x => x.name == "CASTING"))
					anim.SetBool("CASTING", state == "CASTING");
				
				if (animator.parameters.Any(x => x.name == "STUNNED"))
					anim.SetBool("STUNNED", state == "STUNNED");
				
				if (animator.parameters.Any(x => x.name == "MOUNTED"))
					anim.SetBool("MOUNTED", IsMounted()); // for seated animation
				
				if (animator.parameters.Any(x => x.name == "DEAD"))
					anim.SetBool("DEAD", state == "DEAD");
			
				foreach (Skill skill in skills) {
					if (skill.level > 0 && !(skill.data is PassiveSkill) && animator.parameters.Any(x => x.name == skill.animationName)) {
						if (skill.CastTimeRemaining() > 0)
							anim.speed = anim.GetCurrentAnimatorStateInfo(0).length / skill.castTime;
						if (animator.parameters.Any(x => x.name == skill.animationTag))
							anim.SetBool(skill.animationTag, skill.CastTimeRemaining() > 0);
					}
				}
			
			}
			
		}
	}

	// addon system hooks
	this.InvokeInstanceDevExtMethods("LateUpdate_");
	Utils.InvokeMany(typeof(Entity), this, "LateUpdate_");

}