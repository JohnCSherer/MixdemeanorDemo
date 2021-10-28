using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFlicker : MonoBehaviour {
	public float duration = 1.0f;
	public float interval = 0.15f;
	private float halfInterval;
	private float startTime;
	private SpriteRenderer spRenderer;
	// Use this for initialization
	void Start () {
		halfInterval = interval / 2;
		startTime = Time.time;
		spRenderer = GetComponent<SpriteRenderer> ();
		if (GetComponents<SpriteFlicker> ().Length > 1) {
			foreach(SpriteFlicker sp in GetComponents<SpriteFlicker>()){
				if (!sp.Equals (this)) {
					Destroy (sp);
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time % interval < halfInterval) {
			spRenderer.enabled = false;
		} else {
			spRenderer.enabled = true;
		}
		if (Time.time - startTime >= duration) {
			spRenderer.enabled = true;
			Destroy (this);
		}
	}
}
