using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganizeSprites : MonoBehaviour {

	private List<Transform> sprites = new List<Transform>();
	private IComparer<GameObject> sorter;

	// Use this for initialization
	void Start () {
		Object[] tempSprites = GameObject.FindGameObjectsWithTag("Character");
		foreach (GameObject g in tempSprites) {
			if (g.GetComponent<SpriteRenderer>() != null && !g.CompareTag("Background")) {
				sprites.Add (g.transform);
			}
		}
	}
	
	 //Update is called once per frame
	 void Update () {
		for (int i = 1; i < sprites.Count; i++) {
			int j = i;
			while (j >= 1 && sprites [j].position.y > sprites [j - 1].position.y) {
				Transform temp = sprites [j - 1];
				sprites [j - 1] = sprites [j];
				sprites [j] = temp;
				j--;
			}
		}
   		for (int i = 0; i < sprites.Count; i++) {
			sprites [i].GetComponent<SpriteRenderer> ().sortingOrder = i;
		}
	}
}