using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementOverworld : MonoBehaviour {

	public float speed = 0.10f;

	private SpriteRenderer sprite;
	private Animator animController;
	private Rigidbody2D body;
	private CircleCollider2D interactZone;
	private float interactionDistance = 3.0f;
	private byte direction = 0;
	private float defaultSpeed;
	private float animationSpeed;

	private const byte still = 0;
	private const byte north = 1;
	private const byte south = 2;
	private const byte east = 3;
	private const byte west = 4;

	private KeyCode[] directionKey = new KeyCode[5];

	private bool left;
	private bool right;

	private float squrtTwo;

	// Use this for initialization
	void Start () {
		directionKey [1] = KeyCode.W;
		directionKey [2] = KeyCode.S;
		directionKey [3] = KeyCode.D;
		directionKey [4] = KeyCode.A;
		animController = GetComponent<Animator> ();
		body = GetComponent<Rigidbody2D> ();
		defaultSpeed = animController.speed;
		speed /= 20.0f;
		animationSpeed = speed * 5;
		interactZone = GetComponent<CircleCollider2D> ();
		sprite = GetComponent<SpriteRenderer> ();
		right = sprite.flipX;
		left = !right;

		squrtTwo = Mathf.Sqrt (2.0f);
	}

	void Update () {
		
		if (Input.GetKeyDown (directionKey[north])) {
			direction = north;
			animController.Play ("Walk North");
			animController.speed = animationSpeed;
		}else if (Input.GetKeyDown (directionKey[south])) {
			direction = south;
			animController.Play ("Walk South");
			animController.speed = animationSpeed;
		}
		if (Input.GetKeyDown (directionKey[east])) {
			direction = east;
			animController.Play ("Walk Left/Right");
			animController.speed = animationSpeed;
			sprite.flipX = right;
		}else if (Input.GetKeyDown (directionKey[west])) {
			direction = west;
			animController.Play ("Walk Left/Right");
			animController.speed = animationSpeed;
			sprite.flipX = left;
		}

		if (direction != still){
			if(!Input.GetKey (directionKey [direction])) {
				if (Input.GetKey (directionKey [north])) {
					direction = north;
					animController.Play ("Walk North");
					animController.speed = animationSpeed;
				} else if (Input.GetKey (directionKey [south])) {
					direction = south;
					animController.Play ("Walk South");
					animController.speed = animationSpeed;
				} else if (Input.GetKey (directionKey [east])) {
					direction = east;
					animController.Play ("Walk Left/Right");
					animController.speed = animationSpeed;
					sprite.flipX = right;
				} else if (Input.GetKey (directionKey [west])) {
					direction = west;
					animController.Play ("Walk Left/Right");
					animController.speed = animationSpeed;
					sprite.flipX = left;
				} else {
					direction = still;
					animController.Play ("Stand");
					animController.speed = defaultSpeed;
					body.velocity = Vector2.zero;
				}
			}
		} else {
			body.velocity = Vector2.zero;
		}

		switch (direction) {
		case north:
			if (Input.GetKey (directionKey[west])) {
				body.velocity = new Vector2 (-speed, speed)/squrtTwo;
			} else if (Input.GetKey (directionKey[east])) {
				body.velocity = new Vector2 (speed, speed)/squrtTwo;
			} else {
				body.velocity = new Vector2 (0, speed);
			}
			break;
		case south:
			if (Input.GetKey (directionKey [west])) {
				body.velocity = new Vector2 (-speed, -speed)/squrtTwo;
			} else if (Input.GetKey (directionKey [east])) {
				body.velocity = new Vector2 (speed, -speed)/squrtTwo;
			} else {
				body.velocity = new Vector2 (0, -speed);
			}
			break;
		case east:
			if (Input.GetKey (directionKey[north])) {
				body.velocity = new Vector2 (speed, speed)/squrtTwo;
			} else if (Input.GetKey (directionKey[south])) {
				body.velocity = new Vector2 (speed, -speed)/squrtTwo;
			} else {
				body.velocity = new Vector2 (speed, 0);
			}
			break;
		case west:
			if (Input.GetKey (directionKey[north])) {
				body.velocity = new Vector2 (-speed, speed)/squrtTwo;
			} else if (Input.GetKey (directionKey[south])) {
				body.velocity = new Vector2 (-speed, -speed)/squrtTwo;
			} else {
				body.velocity = new Vector2 (-speed, 0);
			}
			break;
		}
	}

	private GameObject checkForInteractable(){
		RaycastHit2D[] hits = Physics2D.CircleCastAll (((Vector2) (transform.position)) + interactZone.offset, 
														interactZone.radius*interactionDistance, Vector2.zero, 0.0f);
		foreach(RaycastHit2D h in hits){
			Component interact = h.transform.gameObject.GetComponent<Interaction> ();
			if(interact != null){
				return h.transform.gameObject;
			}
		}
		return null;
	}

	public void faceObject(GameObject obj){
		float xDiff = obj.transform.position.x - transform.position.x;
		float yDiff = obj.transform.position.y - transform.position.y;
		if (Mathf.Abs (xDiff) > Mathf.Abs (yDiff)) {
			animController.Play ("Stand");
			if (xDiff > 0) {
				sprite.flipX = right;
			} else {
				sprite.flipX = left;
			}
		} else {
			if (yDiff > 0) {
				animController.Play ("Face North");
			} else {
				animController.Play ("Face South");
			}
		}
	}

	public void freeze(){
		body.velocity = Vector3.zero;
	}
}
