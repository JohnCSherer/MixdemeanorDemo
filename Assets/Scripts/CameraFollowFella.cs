using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowFella : MonoBehaviour {
	public GameObject fella;
	private Bounds background;
	private Rect cameraBounds;
	private float camHalfHeight;
	private float camHalfWidth;

	// Use this for initialization
	void Start () {
		Camera cam = Camera.main;
		background = GameObject.FindGameObjectWithTag ("Background").GetComponent<SpriteRenderer> ().bounds;
		camHalfHeight = cam.ScreenToWorldPoint (new Vector3 (cam.pixelWidth/2, cam.pixelHeight, 0)).y - transform.position.y;
		camHalfWidth = cam.ScreenToWorldPoint (new Vector3 (cam.pixelWidth, cam.pixelHeight/2, 0)).x - transform.position.x;
		//Debug.Log (camHalfHeight);
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 targetPosition = new Vector3 (fella.transform.position.x, fella.transform.position.y, transform.position.z);
		if (targetPosition.y + camHalfHeight > background.max.y) {
			targetPosition.y = background.max.y - camHalfHeight;
		}else if (targetPosition.y - camHalfHeight < background.min.y) {
			targetPosition.y = background.min.y + camHalfHeight;
		}
		if (targetPosition.x + camHalfWidth > background.max.x) {
			targetPosition.x = background.max.x - camHalfWidth;
		}else if (targetPosition.x - camHalfWidth < background.min.x) {
			targetPosition.x = background.min.x + camHalfWidth;
		}
		transform.position = targetPosition;
	}
}
