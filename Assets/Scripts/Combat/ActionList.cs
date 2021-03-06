using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionList : MonoBehaviour {

	private Text[] textbox;
	public GameObject highBox; //Box for highlighting options
	private Vector3 hiBoxRootPos; //What position to start the highlight box in
	private float vertSpacing; //How far to move the highlight box vertically
	private float horizSpacing; 

	private int hiIndex;
	private int actionListLength;

	// Use this for initialization
	void Start () {
		hiBoxRootPos = new Vector3 (-0.827f, 0.656f, transform.position.z);
		vertSpacing = 0.16f;
		horizSpacing = 1.487f;
		highBox.transform.position = hiBoxRootPos;
		highBox.SetActive(false);
		hiIndex = 0;
		actionListLength = 0;

		Text[] allTxtboxes = GetComponentsInChildren<Text> ();
		textbox = new Text[6];
		for (int i = 1; i <= 6; i ++) {
			textbox [i - 1] = allTxtboxes [i];
		}
		Clear ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AddActions(List<Action> list){
		if (list.Count > 5) {
			Debug.Log ("Too many actions");
		}
		int i = 0;
		while (i < list.Count && i < 5) {
			textbox [i].text = list [i].actionName;
			i++;
		}
		textbox [i].text = "Back";
		highBox.SetActive(true);
		highBox.transform.position = hiBoxRootPos;
		hiIndex = 0;
		actionListLength = list.Count;
	}

	public void Clear(){
		for (int i = 0; i < 6; i++) {
			textbox [i].text = "";
		}
		highBox.SetActive(false);
		hiIndex = 0;
		actionListLength = 0;
	}

	public int SelectAction(){
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			if (InRange (hiIndex + 3)) {
				hiIndex += 3;
				UpdateHiBox ();
			} else {
				hiIndex = actionListLength;
				UpdateHiBox ();
			}
		}
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			if (InRange (hiIndex - 3)) {
				hiIndex -= 3;
				UpdateHiBox ();
			} else {
				hiIndex = 0;
				UpdateHiBox ();
			}
		}
		if (Input.GetKeyDown (KeyCode.DownArrow) && InRange(hiIndex+1)) {
			hiIndex += 1;
			UpdateHiBox ();
		}
		if (Input.GetKeyDown (KeyCode.UpArrow) && InRange(hiIndex-1)) {
			hiIndex -= 1;
			UpdateHiBox ();
		}
		if (Input.GetKeyDown (KeyCode.Space)) {
			return hiIndex;
		}
		return -1;
	}

	private void UpdateHiBox(){
		highBox.transform.position = hiBoxRootPos;
		highBox.transform.Translate (horizSpacing * ((int)(hiIndex / 3)), -vertSpacing * (hiIndex % 3), 0.0f);
	}

	private bool InRange(int index){
		return (index >= 0 && index < actionListLength + 1);
	}
}
