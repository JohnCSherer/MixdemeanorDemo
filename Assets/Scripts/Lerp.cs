using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Lerp : MonoBehaviour {
	private Vector3 start;
	private Vector3 diff;
	private float duration;
	private float startTime;

	public void Initialize(){
		start = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		duration = 0.0f;
		startTime = Time.time;
	}

	public void Initialize(Vector3 st, Vector3 e, float t){
		start = new Vector3(st.x,st.y,st.z);
		diff = new Vector3(e.x,e.y,e.z) - start;
		duration = t;
		startTime = Time.time;
	}

	void Start () {
		
	}

	void Update () {
		float per = (Time.time - startTime) / duration;
		transform.position = start + (diff * per);
		if (per >= 1) {
			transform.position = start + diff;
			OnLerpComplete (EventArgs.Empty);
			Destroy (GetComponent<Lerp> ());
		}
	}

	protected virtual void OnLerpComplete(EventArgs e){
		EventHandler handler = LerpComplete;
		if (handler != null) {
			handler (this, e);
		}
	}

	public event EventHandler LerpComplete;
}
