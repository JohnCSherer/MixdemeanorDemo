using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActorFilterEnum;

public class Grid : MonoBehaviour{

	public List<Actor> actors; //List of all Actors in the grid, in no particular order (Actors track their own position)

	private Transform [,] tiles; //Array of SpriteRenderers for tiles, to highlight certain tiles
	public Transform[] tileArray;
	private int hiRow, hiCol; //Highlighted row, column

	private void InitializeTiles(){
		tiles = new Transform[3,3];
		for (int i = 0; i < 9; i++) {
			tiles [(int)(i / 3), i % 3] = transform.Find ("" + (int)(i / 3) + (i % 3)).transform;
		}
	}

	void Awake(){
		InitializeTiles ();
		foreach (Transform r in tiles) {
			r.GetComponent<SpriteRenderer>().enabled = false;
		}
		//highlightedTile = new int[2];
		hiRow = 0;
		hiCol = 0;
	}

	//Returns true if this grid contains Actors with actions remaining.
	public bool hasRemainingActions(){
		foreach (Actor actor in actors) {
			if (actor.HasActionsRemaining ()) {
				return true;
			}
		}
		return false;
	}

	//Returns true if this grid contains controllable Actors with actions remaining.
	public bool hasRemainingControllableActions(){
		foreach (Actor actor in actors) {
			if (actor.filters.Contains(ActorFilter.Controllable) && actor.HasActionsRemaining ()) {
				return true;
			}
		}
		return false;
	}

	//Returns true once an Actor has been selected
	public bool PlayerSelectActor(List<ActorFilter> filters){
		if (Input.GetKeyDown (KeyCode.RightArrow) && hiCol < 2) {
			hiCol++;
			UpdateHighlightedTile (0, -1);
		}
		if (Input.GetKeyDown (KeyCode.LeftArrow) && hiCol > 0) {
			hiCol--;
			UpdateHighlightedTile (0, 1);
		}
		if (Input.GetKeyDown (KeyCode.DownArrow) && hiRow < 2) {
			hiRow++;
			UpdateHighlightedTile (-1, 0);
		}
		if (Input.GetKeyDown (KeyCode.UpArrow) && hiRow > 0) {
			hiRow--;
			UpdateHighlightedTile (1, 0);
		}
		//This next set of checks could be a little more efficient.
		if (Input.GetKeyDown (KeyCode.Space) && HiTileContainsActor()) {
			Actor a = HiGetActor ();
			foreach (ActorFilter f in filters) {
				if (!a.filters.Contains (f)) {
					Debug.Log ("Failed: " + f.ToString());
					return false;
				}
			}
			return true;
		}
		return false;
	}

	public bool PlayerSelectActor(ActorFilter filter){
		List<ActorFilter> filters = new List<ActorFilter> ();
		filters.Add (filter);
		return PlayerSelectActor (filters);
	}

	public bool PlayerSelectActor(){
		List<ActorFilter> filters = new List<ActorFilter> ();
		return PlayerSelectActor (filters);
	}

	private bool HiTileContainsActor(){
		return (TileContainsActor(hiRow, hiCol));
	}

	private bool TileContainsActor(int row, int col){
		foreach (Actor actor in actors) {
			if (actor.row == row && actor.col == col) {
				return true;
			}
		}
		return false;
	}

	public Actor GetActor(int row, int col){
		foreach (Actor actor in actors) {
			if (actor.row == row && actor.col == col) {
				return actor;
			}
		}
		Debug.LogError ("Error, used GetActor on an empty cell (" + row + ", " + col + ")");
		return null;
	}

	public Actor HiGetActor(){
		return GetActor (hiRow, hiCol);
	}

	public void UpdateHighlightedTile(){
		tiles [hiRow, hiCol].GetComponent<SpriteRenderer> ().enabled = true;
	}

	public void Clear(){
		for (int i = 0; i < 9; i++) {
			tiles [(int)(i / 3), i % 3].GetComponent<SpriteRenderer> ().enabled = false;
		}
	}

	public void RestoreActions(){
		foreach (Actor a in actors) {
			a.RestoreActions ();
		}
	}

	private void UpdateHighlightedTile(int oldTileOffsetRow, int oldTileOffsetCol){
		tiles [hiRow, hiCol].GetComponent<SpriteRenderer> ().enabled = true;
		tiles [hiRow + oldTileOffsetRow, hiCol + oldTileOffsetCol].GetComponent<SpriteRenderer> ().enabled = false;
	}
}