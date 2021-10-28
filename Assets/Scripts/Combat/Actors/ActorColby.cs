using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorColby : Actor {

	public GameObject tearPrefab;

	void Start(){
		redDmg = 6;
		base.InitializeVariables ();
		//filters.Add (ActorFilterEnum.ActorFilter.Controllable);
	}

	public override List<Action> GetActionList ()
	{
		List<Action> actionList = new List<Action> ();
		actionList.Add (new ActionCry(tearPrefab));
		actionList.Add (new ActionHealCry(tearPrefab));
		return actionList;
	}

	// Update is called once per frame
	void Update () {

	}
}
