using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideToSideEffect: EffectScript {
	public float amplitude;
	public float frequency;
	private float constant;
	private float differential;
	// Use this for initialization
	void Start () {
		frequency = 1;
		amplitude = 0.002f;
		differential = -Time.time*2*Mathf.PI*frequency;
		constant = 2 * Mathf.PI * frequency;
	}

	// Update is called once per frame
	void Update () {
		transform.Translate (Mathf.Sin (Time.time * constant + differential) * amplitude,0,0);
	}
}
