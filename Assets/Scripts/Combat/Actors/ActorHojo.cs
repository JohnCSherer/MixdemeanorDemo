using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ActorHojo : Actor {

	public GameObject tearPrefab;

	void Start(){
		redDmg = 8;
		base.InitializeVariables ();
		filters.Add (ActorFilterEnum.ActorFilter.Attackable);
	}

	public override List<Action> GetActionList ()
	{
		List<Action> actionList = new List<Action> ();
		actionList.Add (new ActionHojoPunch());
		actionList.Add (new ActionHojoPunch());
		actionList.Add (new ActionHojoPunch());
		actionList.Add (new ActionHojoPunch());
		actionList.Add (new ActionHojoPunch());
		actionList.Add (new ActionCry(tearPrefab));
		return actionList;
	}

	// Update is called once per frame
	void Update () {

	}
}
