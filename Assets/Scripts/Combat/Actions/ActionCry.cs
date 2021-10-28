using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCry : Action {
	private float duration = 1.0f;
	private float interval = 0.2f;
	private int tears = 0;
	public GameObject particlePrefab;
	private Vector3 faceOffset;

	public ActionCry () {
		actionName = "Cry";
		startTime = Time.time;
		faceOffset = new Vector3 (0.03f, 0.25f, 0.0f);
	}

	public ActionCry (GameObject tear) {
		actionName = "Cry";
		startTime = Time.time;
		particlePrefab = tear;
		faceOffset = new Vector3 (0.03f, 0.25f, 0.0f);
	}

	public override bool RunAction(){
		float timer = Time.time - startTime;
		if (tears < Mathf.Floor(timer / interval)) {
			tears++;
			GameObject g = GameObject.Instantiate (particlePrefab, 
				controller.GetHiActor ().transform.position + faceOffset, Quaternion.identity);
			g.AddComponent<ParticleGravity> ();
			ParticleGravity p = g.GetComponent<ParticleGravity> ();
			float angleDeviation = Random.Range (-0.5f, 0.5f);
			float velocityMagnitude = 1.0f;
			p.SetVariables(1, new Vector3 (-Mathf.Sin (angleDeviation), Mathf.Cos (angleDeviation), 0) * velocityMagnitude);
		}
		if (timer >= duration) {
			timer = 0.0f;
			return true;
		}
		return false;
	}

	public override bool IsValidAction ()
	{
		return true;
	}
}
