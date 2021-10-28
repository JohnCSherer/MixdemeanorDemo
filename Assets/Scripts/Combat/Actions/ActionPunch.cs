using UnityEngine;
using System.Collections;
using SubmenuEnums;
using System;
using ActorFilterEnum;
//Bro what if i was a girl
public class ActionPunch : Action {
	protected Actor punched; //Target to punch

	protected float duration = 200.0f; //To be used to time animations. Initial value is arbitrary
	protected float finalDuration = 200.0f; //As above
	protected float momentOfDamage = 0.11f; //When in step 2 the damage is applied
	protected int animationStep = 0; //State machine for animation
	public Vector3 punchDistance; //Establishes how far away puncherstands from target
	protected Animator anim; //Reference to puncher's animation controller. Assumes they have appropriate triggerss

	private float firstLerpDuration = 0.5f; //How long to lerp towards target
	private float secondLerpDuration = 0.5f; //How long to lerp back into position

	public ActionPunch(){
		actionName = "Punch"; 
		startTime = Time.time; 
		punchDistance = new Vector3 (-0.6f, 0.0f, 0.0f);
	}

	public override bool RunAction (){
		//Submenu 0: either 'roll' a random target or player manually selects
		if (currentSubmenu == 0) {
			if (automate) {
				RollTarget ();
			} else {
				ChooseTarget ();
			}
		} else {
		//Submenu 1: play animation and eventually return true
			return Animate ();
		}
		return false;
	}
		
	public virtual void ChooseTarget(){
		enemyGrid.UpdateHighlightedTile ();
		if (enemyGrid.PlayerSelectActor (ActorFilter.Attackable)) { //Keep executing from the grid until we get an actor with the specifications
			punched = enemyGrid.HiGetActor ();
			currentSubmenu++; //Advance submenu
			startTime = Time.time; //Start timer
			punchDistance *= Mathf.Sign(hiActor.transform.lossyScale.x); //Scale punch distance based on direction target is facing
			enemyGrid.Clear ();
			friendlyGrid.Clear ();
			controller.actionList.Clear ();
		}
	}

	public virtual void RollTarget(){
		Debug.LogError ("Unimplemented");
	}

	//Lerp HiActor to position in front of secondaryHiActor[0], then trigger punch animation on HiActor, then Slerp back to local 0,0,0 (tile)
	public bool Animate(){
		float timer = Time.time - startTime;
		if (animationStep == 0) {
			anim = hiActor.transform.GetChild(0).GetComponent<Animator> ();
			anim.SetTrigger ("Walk");
			Lerp lerp = hiActor.gameObject.AddComponent<Lerp> ();
			lerp.Initialize(hiActor.transform.parent.position,
				punched.transform.parent.position + punchDistance, firstLerpDuration);
			lerp.LerpComplete += c_LerpComplete;
			animationStep = 1;
		}

		if (animationStep == 2 && timer >= momentOfDamage) {
			animationStep = 3;

			DamageInstance dmg = new DamageInstance (1, controller.GetHiActor ());
			punched.ApplyDamage (dmg);

			punched.transform.Find("Sprite").gameObject.AddComponent<SpriteFlicker>();
		}

		//Re-use addedLerp as a trigger
		if (timer >= duration && animationStep == 3) {
			Lerp lerp = hiActor.gameObject.AddComponent<Lerp> ();
			lerp.Initialize(hiActor.transform.position,
				hiActor.transform.parent.position, secondLerpDuration);
			animationStep = 4;
			startTime = Time.time;
			timer = 0.0f;
			finalDuration = secondLerpDuration;
			anim.SetTrigger ("Walk");
		}
		if (timer >= finalDuration) {
			anim.SetTrigger ("Idle");
			return true;
		}
		return false;
	}

	private void c_LerpComplete(object sender, EventArgs e){
		anim.SetTrigger ("Punch");
		Debug.Log ("PUNCHING NOW");
		startTime = Time.time;
		duration = anim.GetCurrentAnimatorClipInfo (0) [0].clip.length;
		animationStep = 2;
	}

	public override bool IsValidAction ()
	{
		Debug.LogWarning ("IsValidAction for ActionPunch is not implemented");
		return true;
	}
}
