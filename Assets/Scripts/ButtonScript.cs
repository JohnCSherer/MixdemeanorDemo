using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour {
	public int number = 0;

	public Sprite normal;
	public Sprite lightGrey;
	public Sprite darkGrey;

	private Collider2D col;
	public PlayerInteractionHandler interactionScript;

	private bool down = false;
	private bool over = false;

	private float downAmmount = 0.05f;

	private SpriteRenderer sprite;
	// Use this for initialization
	void Start () {
		sprite = GetComponent<SpriteRenderer> ();
		normal = sprite.sprite;
		col = GetComponent<Collider2D> ();
		if (interactionScript == null) {
			Debug.LogError ("Button has no reference to PlayerInteractionHandler script");
		}
		if (number == 0) {
			Debug.LogError ("Button has not been assigned a number");
		}
	}

	// Update is called once per frame
	public void CheckMouse (Vector3 p) {
		if (down) {
			p = new Vector3 (p.x, p.y - downAmmount, p.z);
		}
		if (col.OverlapPoint (p)) {
			over = true;
		} else {
			over = false;
		}
		if (down) {
			if (!over) {
				down = false;
				transform.Translate (0.0f, downAmmount, 0.0f);
				sprite.sprite = normal;
			} else if (Input.GetMouseButtonUp (0)) {
				interactionScript.RecieveAnswer (number);
			}
		} else {
			if (over) {
				if (Input.GetMouseButtonDown (0)) {
					down = true;
					transform.Translate (0.0f, -downAmmount, 0.0f);
					sprite.sprite = darkGrey;
				} else {
					sprite.sprite = lightGrey;
				}
			} else {
				sprite.sprite = normal;
			}
		}

	}

}
