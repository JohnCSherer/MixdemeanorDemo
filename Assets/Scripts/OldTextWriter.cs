using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldTextWriter : MonoBehaviour {
	public PlayerInteractionHandler parentScript;

	private Vector3 originalPosition;
	private SpriteRenderer textBoxRenderer;
	private int frameCounter = 0;
	private float timer = 0;

	private float accelerator = 1.0f;
	public float accelerationFactor = 3.0f;

	private float defaultTimeBetweenLetters = 0.015f;
	private float timeBetweenLetters;
	private float xMargin;
	private float lineSpacing = -0.09f;
	private float letterSpacing = 0.01f;
	private bool writing = false;

	private List<GameObject> currentWord; //A list containing all the letter clones in the most recent word
	private List<GameObject> currentBlock; //Contains all the letters currently being printed

	private string textToWrite;

	private GameObject templateSprite; 
	/*Used as a template to create letters from. Think wooden 
	 *cubes in a printing press, except this cube is used over 
	 *and over to make all kinds of letters.
	 */

	private SpriteRenderer templateRenderer;
	private Sprite[] letterArray = new Sprite[127];

	public Sprite[] otherLetterSprites;

	private Transform[] answerButtons;
	private ButtonScript[] buttonScripts;
	private Camera cam;
	private bool writingAnswers = false;

	public bool visible = false;
	// Use this for initialization
	void Start () {
		cam = Camera.main;

		originalPosition = new Vector3 (-1.082f, 0.09f, 0.0f);
		xMargin = originalPosition.x*-1;
		textBoxRenderer = GetComponent<SpriteRenderer> ();

		createTemplateSprite ();

		otherLetterSprites = new Sprite[127];
		Sprite[] tempOtherSprites = Resources.LoadAll<Sprite> ("Fonts/BasicFont");
		foreach (Sprite s in tempOtherSprites) {
			otherLetterSprites [int.Parse (s.name)] = Sprite.Instantiate(s);
			Resources.UnloadAsset (s);
		}
		for (int i = 1; i <= 31; i++) {
			otherLetterSprites [i] = otherLetterSprites [0];
		}
		for(int i = 0; i < 127; i++){
			letterArray [i] = otherLetterSprites [i];
		}
		answerButtons = new Transform[0];
		timeBetweenLetters = defaultTimeBetweenLetters;
		textBoxRenderer.enabled = false;
		currentWord = new List<GameObject> ();
		currentBlock = new List<GameObject> ();
	}

	private void createTemplateSprite(){
		templateSprite = new GameObject ("templateSprite");
		templateSprite.AddComponent (typeof(SpriteRenderer));
		templateRenderer = templateSprite.GetComponent<SpriteRenderer> ();
		templateRenderer.sortingLayerName = "Textbox Text";
		templateSprite.transform.position = originalPosition;
		templateSprite.SetActive (false);
	}

	// Update is called once per frame
	void Update () {
		if (writing) {
			timer += Time.deltaTime*accelerator;
			int loopCounter = 0;
			while (timer >= timeBetweenLetters && loopCounter < 100) {
				timer -= timeBetweenLetters;
				loopCounter++;
				AddLetter ((int)textToWrite [frameCounter]);
				frameCounter++;
				if (frameCounter == textToWrite.Length) {
					frameCounter = 0;
					writing = false;
					currentWord.Clear();
					parentScript.FinishedWriting ();
					accelerator = 1.0f;
					timer = 0;
				}
			}
			if (loopCounter >= 100) {
				Debug.LogWarning ("Interpreter: Attempted to print more than 100 letters. Something probably went wrong with the speed setting. Error code 19");
			}
		}
		if (writingAnswers) {

		} else if (answerButtons.Length != 0) {
			foreach (ButtonScript b in buttonScripts) {
				Vector3 p = cam.ScreenToWorldPoint (new Vector3 (cam.pixelWidth * Input.mousePosition.x / Screen.width, cam.pixelHeight * Input.mousePosition.y / Screen.height, 10.0f));
				b.CheckMouse(p);
			}
		}
	}

	public void SetSpeed(float s){
		timeBetweenLetters = defaultTimeBetweenLetters/s;
	}

	public void SetSpeed(){
		timeBetweenLetters = defaultTimeBetweenLetters;
	}

	public void AddEffect(string s, List<string> paramList){
		switch (s) {
		case "up and down":
			templateSprite.AddComponent (typeof(UpAndDownEffect));
			break;
		case "side to side":
			templateSprite.AddComponent (typeof(SideToSideEffect));
			break;
		case "round and round":
			templateSprite.AddComponent (typeof(UpAndDownEffect));
			templateSprite.AddComponent (typeof(SideToSideEffect));
			break;
		/*case "jitter":
			templateSprite.AddComponent<JitterEffect>();
			if(paramList.Count > 0){
				templateSprite.GetComponent<JitterEffect>().timeBetweenJitters = float.Parse(paramList[0]);
				if(paramList.Count == 2){
					templateSprite.GetComponent<JitterEffect>().jitterAmount = float.Parse (paramList[1]);
				}
			}
			if(paramList.Count > 2){
				Debug.LogError ("Inerpreter: The /'Jitter/' effect can only take zero, one or two parameters. Found " + paramList.Count + ". Error code one billion");
			}
			break;*/
		case "clear":
			createTemplateSprite ();
			break;
		default:
			Debug.LogError ("Interpreter: Invalid effect \"" + s + "\", see API for list of valid effects. Command ignored. Error code 21"); 
			break;
		}
	}

	public void Show(){
		textBoxRenderer.enabled = true;
		visible = true;
	}

	public void Hide(){
		textBoxRenderer.enabled = false;
		visible = false;
	}

	public void WriteText(string text){
		textToWrite = text;
		writing = true;
		xMargin = originalPosition.x*-1;
	}

	public void Clear(){
		foreach (Transform c in transform) {
			GameObject.Destroy (c.gameObject);
		}
		templateSprite.transform.position = originalPosition;
		currentBlock.Clear ();
	}

	public void ClearAnswers(){
		foreach (Transform t in answerButtons) {
			foreach (Transform child in t) {
				GameObject.Destroy (child.gameObject);
			}
			t.GetComponent<SpriteRenderer> ().enabled = false;
			visible = false;
		}
		answerButtons = new Transform[0];
	}

	public void Accelerate(){
		accelerator = accelerationFactor;
	}

	/**
	 * Populate the buttons with the given answers. Creates only text and parents it to the buttons.
	 */
	public void WriteAnswers (List<string> answers, List<GameObject> buttons){
		//writingAnswers = true;
		answerButtons = new Transform[answers.Count - 1];
		buttonScripts = new ButtonScript[answers.Count - 1];
		for (int i = 0; i < answers.Count - 1; i++) {
			buttons [i].GetComponent<SpriteRenderer> ().enabled = true;
			answerButtons [i] = buttons [i].transform;
			buttonScripts [i] = buttons [i].GetComponent<ButtonScript> ();
		}
		for (int i = 1; i < answers.Count; i++) {
			currentBlock.Clear ();
			templateSprite.transform.position = answerButtons [i-1].position;
			templateSprite.transform.Translate (0.0f, -0.0466666666f, 0.0f);
			Vector3 orig = templateSprite.transform.position;
			while (answers [i].Length != 0) {
				AddParentedLetter (answers[i][0], buttons[i-1].transform);
				answers [i] = answers [i].Substring (1);
			}
			float dist = Vector3.Distance(orig,templateSprite.transform.position);
			foreach(GameObject g in currentBlock){
				g.transform.Translate(-dist/2,0,0);
			}
		}
	}

	/**
	 * Adds a letter to the current position of the templateSprite, and moves it over.
	 */
	private void AddLetter(int asciiCode){
		templateRenderer.sprite = letterArray [asciiCode]; //set the template to be the right letter
		GameObject obj = GameObject.Instantiate (templateSprite,transform);  //create the letter
		obj.SetActive (true);
		currentWord.Add (obj); //add the object to the current "word"
		currentBlock.Add (obj);
		if (asciiCode == 32) {
			currentWord.Clear(); //clear the word after a space
		}
		if (obj.transform.localPosition.x + templateRenderer.sprite.bounds.max.x > xMargin) { //if the letter passes over the xMargin limit...
			//move the entire word to the next line
			templateSprite.transform.localPosition = new Vector3 (originalPosition.x, templateSprite.transform.localPosition.y, originalPosition.z);
			templateSprite.transform.Translate (0.0f, lineSpacing, 0.0f);
			foreach (GameObject letter in currentWord) {
				letter.transform.localPosition = templateSprite.transform.localPosition;
				templateSprite.transform.Translate (letter.GetComponent<SpriteRenderer> ().sprite.bounds.max.x + letterSpacing, 0.0f, 0.0f);
			}
		} else {
			templateSprite.transform.Translate (templateRenderer.sprite.bounds.max.x + letterSpacing, 0.0f, 0.0f);
		}
	}

	private void AddParentedLetter(int asciiCode, Transform parent){
		templateRenderer.sprite = letterArray [asciiCode];
		GameObject obj = GameObject.Instantiate (templateSprite);
		obj.transform.SetParent (parent, true);
		obj.SetActive (true);
		currentWord.Add (obj);
		currentBlock.Add (obj);
		if (asciiCode == 32) {
			currentWord.Clear();
		}
		templateSprite.transform.Translate (templateRenderer.sprite.bounds.max.x + letterSpacing, 0.0f, 0.0f);
	}
}
