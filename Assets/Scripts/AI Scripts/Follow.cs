using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {
	public Transform target;
	private float speed = 0.25f;
	private float acceptableDistance = 0.2f;
	private Animator animator; //reference to the animator
	private string state;
	private bool left, right;
	private SpriteRenderer sprite;

	private Rigidbody2D body;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody2D> ();
		animator = GetComponent<Animator> ();
		sprite = GetComponent<SpriteRenderer> ();
		
		right = sprite.flipX;
		left = !right;
		this.enabled = false;
		state = "Stand";
	}
	
	// Update is called once per frame
	void Update () {
		float dist = Vector3.Distance (transform.position, target.position);
		if (dist >= acceptableDistance && body.velocity.magnitude < speed) {
			body.AddForce( (target.position-transform.position).normalized*speed);
		}
		if (body.velocity.magnitude < 0.01f) {
			if (!state.Equals ("Stand")) {
				animator.Play ("Stand");
				state = "Stand";
			}
		} else if (Mathf.Abs (body.velocity.x) > Mathf.Abs (body.velocity.y)) {
			if (body.velocity.x > 0) {
				if (!state.Equals ("Walk Left/Right") || sprite.flipX != right) {
					sprite.flipX = right;
					state = "Walk Left/Right";
					animator.Play ("Walk Left/Right");
				}
			} else if (!state.Equals ("Walk Left/Right") || sprite.flipX != left) {
				sprite.flipX = left;
				state = "Walk Left/Right";
				animator.Play ("Walk Left/Right");
			}
		} else {
			if (body.velocity.y > 0) {
				if (!state.Equals ("Walk North")) {
					state = "Walk North";
					animator.Play ("Walk North");
				}
			} else if (!state.Equals ("Walk South")) {
				state = "Walk South";
				animator.Play("Walk South");
			}
		}
	}
}
