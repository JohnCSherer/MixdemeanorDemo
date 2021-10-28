using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpAndDownEffect : EffectScript {
	public float amplitude;
	public float frequency;
	private float constant;
	private Vector3 original;
	// Use this for initialization
	void Start () {
		frequency = 1;
		amplitude = 0.002f;
		offset = -Time.time*2*Mathf.PI*frequency;
		constant = 2 * Mathf.PI * frequency;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (0, Mathf.Cos (Time.time * constant + offset) * amplitude,0);
	}
}
