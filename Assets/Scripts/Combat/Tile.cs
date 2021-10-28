using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour{
	public int[] location;
	//private Actor[] occupant;
	//private SpriteRenderer sprite;

	void Awake(){
		location = new int[2];
		if(!int.TryParse(name.Substring (0, 1), out location[0])){
			Debug.LogError ("Could not parse name");
		}
		if(!int.TryParse(name.Substring (1, 1), out location[1])){
			Debug.LogError ("Could not parse name");
		}
		//occupant = transform.GetComponentsInChildren<Actor> ();
	}
}