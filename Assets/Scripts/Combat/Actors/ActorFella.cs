using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorFella : Actor {

	public GameObject tearPrefab;

	void Start(){
		redDmg = 8;
		base.InitializeVariables ();
		filters.Add (ActorFilterEnum.ActorFilter.Controllable);
		filters.Add (ActorFilterEnum.ActorFilter.Attackable);
	}

	public override List<Action> GetActionList ()
	{
		List<Action> actionList = new List<Action> ();
		actionList.Add (new ActionPunch());
		actionList.Add (new ActionCry(tearPrefab));
		actionList.Add (new ActionCry(tearPrefab));
		actionList.Add (new ActionHealCry(tearPrefab));
		return actionList;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
