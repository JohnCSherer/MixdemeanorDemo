using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicTextWriter : MonoBehaviour {
	public float fr;
	public LongScriptReader reader;

	private Text text;

	private int frameCounter = 0;
	private float timer = 0;
	public float waitTime = 0.0f;
	private float accelerator = 1.0f;
	public float accelerationFactor = 1.0f;
	public float defaultTimeBetweenLetters = 0.015f;
	private float timeBetweenLetters;
	public bool writing = false;
	private bool finish = false;
	public Camera cam;

	private List<string> queuedText;
	private List<float> queuedWaits;
	public bool autoPause = true;
	public float commaTime = 0.1f;
	public float newSentenceTime = 0.15f;
	public float ellipsesTime = 0.4f;
	private int ellipses = 0;

	private bool punctuationPaused = false;
	
	private string textToWrite;
	
	// Use this for initialization
	void Start () {
		queuedText = new List<string> ();
		queuedWaits = new List<float> ();

		text = GetComponent<Text> ();
		timeBetweenLetters = defaultTimeBetweenLetters;
		text.resizeTextMaxSize = cam.pixelWidth / 32;
	}
	
	// Update is called once per frame
	void Update () {
		fr = 1.0f / Time.deltaTime;
		if (waitTime > 0.0f) {
			waitTime -= Time.deltaTime;
			if (waitTime <= 0.0f) {
				waitTime = 0.0f;
				if (queuedText.Count != 0) {
					WriteText (queuedText [0]);
					queuedText.RemoveAt (0);
				} else if (punctuationPaused) {
					writing = true;
					punctuationPaused = false;
				}
			}
		}
		if (writing) {
			timer += Time.deltaTime*accelerator;
			int loopCounter = 0;
			while (timer >= timeBetweenLetters && loopCounter < 400||finish) {
				timer -= timeBetweenLetters;
				loopCounter++;
				char l = textToWrite [frameCounter];
				AddLetter (l);
				if(autoPause){
					if (l == ',') {
						writing = false;
						waitTime = commaTime;
						punctuationPaused = true;
					} else if ((l == '.' || l == '?' || l == '!')&&(frameCounter + 2 < textToWrite.Length)) {
						if (textToWrite.Substring (frameCounter,3).Equals ("...")) {
							ellipses = 2;
							waitTime = ellipsesTime;
						}
						if (ellipses > 0) {
							ellipses--;
						} else {
							waitTime = newSentenceTime;
							writing = false;
							punctuationPaused = true;
						}
					}
				}
				frameCounter++;
				if (frameCounter == textToWrite.Length) {
					punctuationPaused = false;
					waitTime = 0;
					frameCounter = 0;
					accelerator = 1.0f;
					timer = 0;
					textToWrite = "";
					if (queuedWaits.Count != 0) {
						if (finish) {
							if (queuedText.Count != 0) {
								textToWrite = queuedText [0];
								queuedText.RemoveAt (0);
							} else {
								writing = false;
								reader.doneWriting ();
								finish = false;
								queuedText.Clear ();
								queuedWaits.Clear ();
							}
						} else {
							waitTime = queuedWaits [0];
							queuedWaits.RemoveAt (0);
							writing = false;
						}
					} else {
						writing = false;
						reader.doneWriting ();
						finish = false;
						queuedText.Clear ();
						queuedWaits.Clear ();
					}
				}
			}
			if (loopCounter >= 400) {
				Debug.LogWarning ("Interpreter: Attempted to print more than 400 letters at once.");
			}
		}
	}

	
	public void WriteText(string text){
		text = text.Replace ("<name>", reader.playerName);
		textToWrite = text;
		writing = true;
	}

	public void WriteText(string text, bool enclose){
		text = text.Replace ("<name>", reader.playerName);
		if (enclose) {
			text = "\"" + text + "\"";
		}
		textToWrite = text;
		writing = true;
	}

	public void WriteText(List<string> txts, List<float> pauses){
		queuedText = txts;
		queuedWaits = pauses;
		textToWrite = queuedText [0];
		textToWrite = textToWrite.Replace ("<name>", reader.playerName);
		queuedText.RemoveAt (0);
		writing = true;
	}

	public void WriteText(List<string> txts, List<float> pauses, bool enclose){
		queuedText = txts;
		queuedWaits = pauses;
		textToWrite = queuedText [0];
		textToWrite = textToWrite.Replace ("<name>", reader.playerName);
		queuedText.RemoveAt (0);
		if (enclose) {
			textToWrite = "\"" + textToWrite;
			queuedText [queuedText.Count - 1] += "\"";
		}
		writing = true;
	}

	public void SetColor(Color c){
		text.color = c;
	}
	
	public void Clear(){
		text.text = "";
	}

	public void Style(FontStyle f){
		text.fontStyle = f;
	}

	public void Finish(){
		text.text += textToWrite.Substring (frameCounter);
		for (int i = 0; i < queuedText.Count; i++) {
			text.text += queuedText [i];
		}
		writing = false;
		punctuationPaused = false;
		queuedText.Clear ();
		queuedWaits.Clear ();
		textToWrite = "";
		frameCounter = 0;
		timer = 0;
		waitTime = 0;
		reader.doneWriting ();
	}

	public bool IsNotWriting(){
		return (!writing) && (queuedText.Count == 0) && (!punctuationPaused);
	}

	public void Accelerate(){
		accelerator = accelerationFactor;
	}
	
	/**
	 * Adds a letter to the current position of the templateSprite, and moves it over.
	 */
	private void AddLetter(char letter){
		text.text += letter;
	}

	public void HardReturn(){
		text.text += "\n";
	}
}
