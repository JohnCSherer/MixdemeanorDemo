using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleGravity : MonoBehaviour {
	public Vector3 velocity;
	public float grav;

	private float gravMult = (1.0f/8.0f);
	private float velocityMult = 3;

	//Initialize velocity and gravity
	public void SetVariables(int gravity, Vector3 vel){
		grav = gravity * gravMult;
		velocity = vel * velocityMult;
	}

	void Update () {
		//Point particle in direction of travel
		transform.eulerAngles = new Vector3(0,0,Mathf.Rad2Deg * Mathf.Atan2(velocity.y,velocity.x)+90);

		//Apply gravity and velocity
		transform.position += velocity*Time.deltaTime;
		velocity = new Vector2 (velocity.x, velocity.y - grav);

		//Delete particle if it falls below visible area
		if (transform.position.y <= -1.2) {
			Destroy (this.gameObject);
		}
	}
}
