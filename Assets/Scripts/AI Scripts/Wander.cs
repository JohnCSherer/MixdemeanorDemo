using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : MonoBehaviour {

	public bool wander;
	public GameObject wanderBox; //domain to wander in
	//private CircleCollider2D interactZone;
	public float patience; //how often to move or face the other way (flip)
	public float speed; //how fast they move

	private Rigidbody2D physicsBody; //reference to the physics object
	private Animator animator; //reference to the animator
	private SpriteRenderer sprite;

	private float timer; //track how long till next action
	private float distance; //used to check for lack of progress towards target
	private Bounds box; //reference to wanderbox's bounds
	private Vector3 target; //location to walk towards
	private bool moving = false; //track if we're moving towards a location
	private float walkChance = 0.0f; //likelyhood to walk instead of flip. Increases with successive flips
	private int giveUp; //keep track of how many times we've failed to move towards our destination
	private float lastCheckedDistance;

	private bool left;
	private bool right;

	private int frameCounter;

	private int framesPerTick = 10;
	private int halfFramesPerTick;

	// Use this for initialization
	void Start () {
		//initialize variables
		//interactZone = GetComponent<CircleCollider2D> ();
		animator = GetComponent<Animator> ();
		sprite = GetComponent<SpriteRenderer> ();

		timer = patience;
		box = wanderBox.GetComponent<Collider2D> ().bounds;
		physicsBody = GetComponent<Rigidbody2D> (); 
		animator.Play ("Stand");
		right = sprite.flipX;
		left = !right;
		halfFramesPerTick = Mathf.RoundToInt (framesPerTick / 2.0f);
		if (patience == 0) {
			Debug.LogWarning ("Patience on " + gameObject.name + "'s wander script was 0. Assuming this was a mistake, setting it to 3");
		}
	}

	// Update is called once per frame
	void Update () {
		if(timer <=0.0f){ //if it's time to take an action
			moving = false;
			if(!wander || Random.Range(0.0f,1.0f) > walkChance ){ //roll to see if we walk or flip
				//flip, reset timer, and increase likelyhood to walk next time
				sprite.flipX = !sprite.flipX;
				timer = patience;
				walkChance += (1.0f - walkChance) / 2.0f;
				animator.Play ("Stand");
				physicsBody.velocity = Vector3.zero;
			}else{
				//pick a random location to walk towards
				target = box.center;
				target.x += box.extents.x * Random.Range (-0.9f, 0.9f);
				target.y += box.extents.y * Random.Range (-0.9f, 0.9f);
				lastCheckedDistance = Vector3.Distance (target, transform.position);
				if (lastCheckedDistance < speed * Time.fixedDeltaTime) {
					//target was very close, don't bother moving
					walkChance += (1.0f - walkChance) / 2.0f;
				} else {
			
					moving = true;

					//choose appropriate animation
					setWalkDirection ();
					physicsBody.velocity = Vector3.Normalize (target - transform.position) * speed * Time.fixedDeltaTime;

					//if we don't get there in 10 seconds, we give up
					timer = 10.0f;
				}
			}
		}
		if (moving) {
			frameCounter++;
			//POSSIBLE OPTIMIZATION???
			distance = Vector3.Distance (target, transform.position);
			if (frameCounter == halfFramesPerTick) {
				lastCheckedDistance = distance;
			} else if (frameCounter == framesPerTick) {
				giveUpTick ();
				frameCounter = 0;
			}
			//dynamically update movement
			physicsBody.velocity = Vector3.Normalize (target - transform.position) * speed * Time.fixedDeltaTime;
			if (distance < speed * Time.fixedDeltaTime || timer <= 0) { 
				//if we run out of time, or we reach our target
				//reset,
				freezeAndReset ();
				//and maybe flip
				if (Random.Range (0.0f, 1.0f) < 0.5f) {
					sprite.flipX = !sprite.flipX;
				}
			}
		}

		//decrement timer
		timer -= Time.fixedDeltaTime;
	}

	//reset most variables and stops motion
	public void freezeAndReset(){
		moving = false;
		animator.Play ("Stand");
		walkChance = 0.0f;
		timer = patience;
		giveUp = 0;
		physicsBody.velocity = Vector3.zero;
	}

	//called infrequently to see if we are not reaching our target destination
	public void giveUpTick(){
		if (lastCheckedDistance - distance < 0.0001f) {
			giveUp++;
			if (giveUp == 3) {
				//eventually give up and restart cycle 
				freezeAndReset ();
			}
		} else {
			giveUp = 0;
		}
	}

	private void setWalkDirection(){
		float xDiff = target.x - transform.position.x;
		float yDiff = target.y - transform.position.y;
		if (Mathf.Abs (xDiff) > Mathf.Abs (yDiff)) {
			animator.Play ("Walk Left/Right");
			if (xDiff < 0) {
				sprite.flipX = left;
			} else {
				sprite.flipX = right;
			}
		} else {
			sprite.flipX = right;
			if (yDiff > 0) {
				animator.Play ("Walk North");
			} else {
				animator.Play ("Walk South");
			}
		}
	}
}
