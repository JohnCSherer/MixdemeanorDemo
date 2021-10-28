using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextWriter : MonoBehaviour {

	public GameObject templateSprite; //Placeholder GameObject with SpriteRenderer for instantiating other letters
	//Also serves as a cursor to determine where to place the new letter

	public List<Sprite> spriteList; //List containing all sprites in the alphabet. Does not need to be ordered.
	public Sprite whitespace;
	private Sprite[] spriteAlphabet; //Ordered in ascii, such that spriteAlphabet[ascii('letter')] gives the sprite for that letter

	private List<GameObject> letterArray; //Contains all the letters we've been asked to write

	private float spaceSize = 0.1f; //How big spaces are (ASCII 32)

	private float letterSpacing = 0.02f; //How far apart buffers between letters are

	private float startTime; //Used for timing letter speed
	private int letersRevealed; //Number of letters whose renderers have been enabled

	private string textToWrite; //Stores the text that we've been assigned to write

	private float lineLength = 20.0f; //How far to add letters before 

	private float lineSpacing = 0.5f; //How much space between lines

	private float fontScale = 1.2f;

	private bool writing = false;

	void Start () {
		//Assign every letter in spriteList to spriteAlphabet
		spriteAlphabet = new Sprite[127];
		foreach (Sprite sprite in spriteList) {
			spriteAlphabet [int.Parse(sprite.name)] = sprite;
		}
		spriteAlphabet [32] = whitespace;

		writeWord ("Bruh why is this spacing so wonky");
	}

	// Update is called once per frame
	void Update () {
		if (writing) {
			float timer = Time.time;
		}
	}

	public void WriteText(string text){
		Reset ();
		writing = true;
		//Break the text into words with ' ' being the delimiter
		List<string> wordList = new List<string> ();
		while (text.IndexOf(' ')!=-1) {
			wordList.Add(text.Substring(0,text.IndexOf(' ')));
			text = text.Substring (text.IndexOf (' ') + 1);
		}

		//For each word, determine if a new line is needed before writing the word
		foreach (string word in wordList) {
			if (templateSprite.transform.position.x + wordLength (word) - transform.position.x > lineLength) {
				Debug.Log ("Here");
				templateSprite.transform.position = new Vector3 (0, templateSprite.transform.position.y + lineSpacing, templateSprite.transform.position.z);
			}
			writeWord (word);
		}
	}

	//Write the word by advancing the cursor and instantiating right on it
	private void writeWord(string word){

		//For each character in the word...
		char[] charText = word.ToCharArray ();
		for (int i = 0; i < charText.Length; i++) {
			//Make the templatesprite match, then duplicate it
			GameObject newLetter = Instantiate (templateSprite);
			SpriteRenderer newLetterRenderer = newLetter.GetComponent<SpriteRenderer> ();
			newLetterRenderer.enabled = true;
			newLetterRenderer.sprite = GetSprite (charText [i]);
			newLetter.transform.localScale *= fontScale;
			newLetter.transform.parent = transform;
			templateSprite.transform.Translate (newLetterRenderer.bounds.size.x, 0, 0);
			letterArray.Add (newLetter);
		}
	}

	//Gives the length of the word in world units
	private float wordLength(string word){
		float totalLength = 0.0f;
		foreach (char c in word) {
			totalLength += GetSprite (c).bounds.size.x * fontScale;
		}
		return totalLength;
	}

	private Sprite GetSprite(char c){
		return spriteAlphabet [(int)c];
	}

	private void Reset(){
		letterArray = new List<GameObject> ();
		startTime = Time.time;
		foreach (GameObject g in transform) {
			if (!(g.Equals (gameObject) || g.Equals(templateSprite))) {
				Destroy (g);
			}
		}
	}
}
