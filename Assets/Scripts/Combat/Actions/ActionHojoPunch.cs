using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActorFilterEnum;

public class ActionHojoPunch : ActionPunch {
	
	public override void RollTarget(){
		List<Actor> gridActorList = enemyGrid.actors;
		List<Actor> validTargets = new List<Actor> ();
		foreach (Actor a in gridActorList) {
			if (a.filters.Contains (ActorFilter.Attackable) && a.name != "Colby") {
				validTargets.Add (a);
			}
		}
		Debug.Log (validTargets.Count);
		int randomIndex = Random.Range (0, validTargets.Count);
		punched = validTargets [randomIndex];
		currentSubmenu++;
		startTime = Time.time;
		punchDistance = new Vector3 (-0.6f, 0.0f, 0.0f	) * Mathf.Sign(hiActor.transform.lossyScale.x);
		enemyGrid.Clear ();
		friendlyGrid.Clear ();
		controller.actionList.Clear ();
	}

	public override bool IsValidAction ()
	{
		Debug.LogWarning ("IsValidAction for ActionHealCry is not implemented");
		return true;
	}
}
