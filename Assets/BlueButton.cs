using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueButton : MonoBehaviour {
	public LongScriptReader reader;
	public string linkedTextName;
	public Camera cam;
	
	public Sprite normal;
	public Sprite lightGrey;
	public Sprite darkGrey;
	
	private Collider2D col;
	
	private bool down = false;
	private bool over = false;
	
	private SpriteRenderer sprite;
	// Use this for initialization
	void Start () {
		sprite = GetComponent<SpriteRenderer> ();
		normal = sprite.sprite;
		col = GetComponent<Collider2D> ();
		if (reader == null) {
			Debug.LogError ("Button has no reference to reader script");
		}
	}
	
	// Update is called once per frame
	public void Update () {
		Vector3 p = cam.ScreenToWorldPoint (new Vector3 (cam.pixelWidth * Input.mousePosition.x / Screen.width, cam.pixelHeight * Input.mousePosition.y / Screen.height, 10.0f));
		if (col.OverlapPoint (p)) {
			over = true;
		} else {
			over = false;
		}
		if (down) {
			if (!over) {
				down = false;
				sprite.sprite = normal;
			} else if (Input.GetMouseButtonUp (0)) {
				reader.RecieveAnswer (linkedTextName);
			}
		} else {
			if (over) {
				if (Input.GetMouseButtonDown (0)) {
					down = true;
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
