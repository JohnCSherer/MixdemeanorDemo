using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHealCry : Action {
	private float duration = 1.4f; //How long to cry
	private float interval = 0.2f; //How long between tears
	private int tears; //Used to track 
	public GameObject particlePrefab;
	private Vector3 faceOffset;
	private Transform target;
	private Vector3 targetOffset;

	public ActionHealCry (GameObject tear) {
		actionName = "Tearful Heal";
		tears = 0;
		startTime = Time.time;
		particlePrefab = tear;
		faceOffset = new Vector3 (0.03f, 0.25f, 0.0f);
		targetOffset = new Vector3 (0.03f, 0.25f, 0.0f);
	}

	public override bool RunAction(){
		if (currentSubmenu == 0) {
			if (automate) {
				RollTarget ();
			} else {
				ChooseTarget ();
			}
			return false;
		} else {
			float timer = Time.time - startTime;
			if (tears < Mathf.Floor(timer / interval)) {
				tears++;
				GameObject g = GameObject.Instantiate (particlePrefab, 
					hiActor.transform.position + faceOffset, Quaternion.identity);
				g.AddComponent<HomingParticle> ();
				HomingParticle p = g.GetComponent<HomingParticle> ();
				p.target = target.position + targetOffset;
				float angleDeviation = Random.Range (0.0f, Mathf.PI*2);
				float velocityMagnitude = 1.0f;
				p.SetVelocity (new Vector3 (-Mathf.Sin (angleDeviation), Mathf.Cos (angleDeviation), 0) * velocityMagnitude);
			}
			if (timer >= duration) {
				target.GetComponent<Actor> ().ApplyHeal (1);
				return true;
			}
			return false;
		}
	}

	private void ChooseTarget(){
		friendlyGrid.UpdateHighlightedTile ();
		if (friendlyGrid.PlayerSelectActor ()) {
			target = friendlyGrid.HiGetActor ().transform;
			currentSubmenu++;
			startTime = Time.time;
			enemyGrid.Clear ();
			friendlyGrid.Clear ();
			controller.actionList.Clear ();
		}
	}

	private void RollTarget(){
		List<Actor> validHealTargets = new List<Actor> ();
		foreach (Actor a in friendlyGrid.actors) {
			if(!a.Equals(hiActor)){
				validHealTargets.Add (a);
			}
		}
		int randomIndex = Random.Range(0,validHealTargets.Count);
		Debug.Log ("NUMBER: " + validHealTargets[0].name);
		target = validHealTargets [randomIndex].transform;
		currentSubmenu++;
		startTime = Time.time;
	}

	public override bool IsValidAction ()
	{
		Debug.LogWarning ("Not implemented");
		return true;
	}
}
