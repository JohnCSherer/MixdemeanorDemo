using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HomingParticle : MonoBehaviour {
	public Vector3 velocity;
	public Vector3 target;
	private float initialVelocityMult = 1.5f; //Scalar for velocity
	private float maximumVelocity = 5.0f; //Cap on particle velocity magnitude
	private float velocityLerpRate = 8.0f; //How quickly the particle adjusts to target velocity
	private float initialOffsetFactor = 3.0f; //Tear moves x steps when initialized

	public void SetVelocity (Vector3 vel) {
		velocity = vel * initialVelocityMult;
		transform.position += velocity * Time.deltaTime * initialOffsetFactor;
	}
		
	void Update () {
		//Point particle in direction of travel
		transform.eulerAngles = new Vector3(0,0,Mathf.Rad2Deg * Mathf.Atan2(velocity.y,velocity.x)+90);

		//Simulate velocity
		transform.position += velocity*Time.deltaTime;

		//Calculate an ideal velocity to reach target, and change current velocity towards that value within the magnitude of velocityLerpRate
		Vector3 targetVelocity = Vector3.Normalize (target - transform.position) * maximumVelocity;
		Vector3 deltaVelocity = Vector3.Normalize(targetVelocity - velocity)*velocityLerpRate;

		velocity += deltaVelocity * Time.deltaTime; //Update velocity

		//Cap velocity
		if (velocity.magnitude > maximumVelocity) {
			velocity = velocity.normalized * maximumVelocity;
		}

		//Remove particle if it reaches target
		if (Vector3.Distance(transform.position + velocity*Time.deltaTime, target) < maximumVelocity*Time.smoothDeltaTime) {
			Destroy (this.gameObject);
		}
	}
}
