using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActorFilterEnum;

public class Actor : MonoBehaviour {

	public List<ActorFilter> filters;
	public List<Sprite> healthbars;
	public SpriteRenderer healthIcon;

	public int numActions; //How many actions the Actor can take per turn (usually 1)
	protected int actionsTaken; //How many times this Actor has acted this turn.
	public float redDmg; //How much damage the actor can take before it's fully yellow
	private float yellowDmg; //Half of redHealth
	private float blackDmg; //The most damage an actor can have
	protected float damage; //How much of a beating this actor has taken. Chance of getting KO'd after redDmg

	public int row = 0;
	public int col = 0;

	void Start () {
		InitializeVariables ();
	}

	protected void InitializeVariables(){
		filters = new List<ActorFilter> ();
		actionsTaken = 0;
		if (healthIcon == null) {
			Debug.LogWarning ("Warning, no health icon attatched to this actor");
		}
		int parentNameIteger = int.Parse (transform.parent.name);
		row = (int)(parentNameIteger / 10);
		col = parentNameIteger - (row * 10);
		yellowDmg = redDmg / 2;
		blackDmg = redDmg + yellowDmg;
		UpdateHealthIcon ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public bool HasActionsRemaining() {
		return actionsTaken < numActions && !IsDead();
	}

	public void ApplyDamage (DamageInstance dmg){
		damage += dmg.damage;
		if (damage >= blackDmg) {
			damage = blackDmg;
		}
		UpdateHealthIcon ();
	}

	public void ApplyHeal(int healthRecovered){
		damage -= healthRecovered;
		if (damage < 0) {
			damage = 0;
		}
		UpdateHealthIcon ();
	}

	public virtual List<Action> GetActionList (){
		return new List<Action>();
	}

	public virtual bool IsDead(){
		return false;
	}

	public virtual bool CheckForDeath(){
		return IsDead ();
	}

	public void UpdateHealthIcon(){
		float r, g;
		if (damage <= yellowDmg) {
			r = 1 * (damage / yellowDmg);
			g = 1;
		} else if (damage <= redDmg) {
			r = 1;
			g = 1 * (1 - (damage - yellowDmg) / yellowDmg);
		} else {
			g = 0;
			r = 1 * (1 - (damage - redDmg) / yellowDmg);
		}
		Color newColor = new Color (r,g,0);
		healthIcon.color = newColor;
	}

	public void IncrementActionCount(){
		actionsTaken++;
	}

	public void RestoreActions(){
		actionsTaken = 0;
	}
}
