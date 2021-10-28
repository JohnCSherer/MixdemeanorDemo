using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LongScriptReader : MonoBehaviour {
	public string playerName = "Miles";

	public BasicTextWriter writer;
	public List<TextAsset> scripts;
	public GameObject clarkBase;
	public SpriteRenderer currentExpression;
	public SpriteRenderer nametag;
	public Sprite surprised;
	public Sprite impressed;
	public Sprite normal;
	public Sprite normalSide;
	public Sprite embarrassed;
	public Sprite pleased;
	public Sprite pleasedSide;
	public Sprite lookingDown;
	public Sprite nervous;
	public Sprite nervousSide;
	private bool wearingTag = true;

	private List<Flag> flags;

	public SpriteRenderer backdrop;
	public Sprite pharmacy;
	public Sprite blurryPharm;
	public Sprite blurryCafe;
	public Sprite coffeeshop;
	public GameObject nightBackground;

	public BasicTextWriter dayEndText;

	public GameObject boyFace;
	public GameObject boyChart;

	public float fadeSpeed;
	private char[] trimChars = new char[3];

	private Color green, black, blue;

	private float waitTime = 0.0f;

	public bool paused = true;
	private bool ended = false;
	private float endedTimer = 0.0f;
	private int nightStage = 0;
	private Vector3 initialBoyPos;
	private Vector3 boyTargetPos;
	private float lerpTimer = 0.0f;
	private float tempScoreX;
	private float tempScoreY;

	private TextAsset nextScript;

	public GameObject textbox;
	public Text speakerText;
	public GameObject textboxAddon;
	public Text[] buttonText = new Text[4];
	public GameObject[] buttons = new GameObject[4];
	private int activeButtons = 0;
	private bool waitingForAnswer = false;

	private List<SpriteRenderer> fadeIns;
	private List<SpriteRenderer> fadeOuts;

	private string workingText;
	// Use this for initialization
	void Start () {

		fadeIns = new List<SpriteRenderer> ();
		fadeOuts = new List<SpriteRenderer> ();

		green = new Color (0, 0.45f, 0);
		black = new Color (0.055f, 0.055f, 0.055f);
		blue = new Color (0.227f, 0.251f, 0.66f);

		workingText = scripts [0].ToString();

		trimChars[0] = char.ConvertFromUtf32 (13)[0];
		trimChars[1] = char.ConvertFromUtf32 (10)[0];
		trimChars[2] = " "[0];

		currentExpression.color = new Color (255, 255, 255, 0);

		initialBoyPos = Vector3.zero;
		boyTargetPos = Vector3.zero;

		//score = GetComponent<BoyScore> ();
		flags = new List<Flag> ();
	}

	public void LoadFirstLine(){
		readLine (ReadNext (), 0);
	}
	
	// Update is called once per frame
	void Update () {
		if (waitTime > 0) {
			waitTime -= Time.deltaTime;
			if (waitTime <= 0) {
				readLine (ReadNext (), 0);
			}
		}
		if (Input.GetKeyDown (KeyCode.Space) || Input.GetMouseButtonDown (0) && !ended) {
			if(paused && !ended && !waitingForAnswer && writer.IsNotWriting() && activeButtons == 0){
				paused = false;
				readLine(ReadNext(), 0);
			}else if(!writer.IsNotWriting()){
				writer.Finish();
				waitTime = 0.0f;
			}
		}
		if (ended) {
			if (nightStage == 3) {
				boyFace.transform.localPosition = Vector3.Lerp (initialBoyPos, boyTargetPos, (Time.time - lerpTimer) / 2.0f);
				if ((Time.time - lerpTimer) > 2.0f) {
					boyFace.transform.localPosition = boyTargetPos;
					nightStage = 4;
				}
			} else if (nightStage == 4) {
				if (Input.GetKeyDown (KeyCode.Space) || Input.GetMouseButtonDown (0)) {
					nightStage = 5;
					fadeOuts.Add (nightBackground.GetComponent<SpriteRenderer> ());
					fadeOuts.Add (boyChart.GetComponent<SpriteRenderer> ());
					fadeOuts.Add (boyFace.GetComponent<SpriteRenderer> ());
					dayEndText.Clear ();
					endedTimer = Time.time + 1.0f;
				}
			}
			if (Time.time > endedTimer) {
				if (nightStage == 0) {
					nightBackground.SetActive (true);
					fadeIns.Add (nightBackground.GetComponent<SpriteRenderer> ());
					endedTimer = Time.time + 2.0f;
					nightStage = 1;
				} else if (nightStage == 1) {
					boyChart.SetActive (true);
					boyFace.SetActive (true);
					fadeIns.Add (boyChart.GetComponent<SpriteRenderer> ());
					fadeIns.Add (boyFace.GetComponent<SpriteRenderer> ());
					endedTimer = Time.time + 1.0f;
					nightStage = 2;
				} else if (nightStage == 2) {
					//initialBoyPos = new Vector3 (score.GetX () * 3 /100.0f, score.GetY () * 3/100.0f, boyFace.transform.localPosition.z);

					//boyTargetPos = new Vector3 ((tempScoreX + score.GetX ()) * 3 / 100.0f,
						//(tempScoreY + score.GetY ()) * 3 / 100.0f, initialBoyPos.z);
					lerpTimer = Time.time;
					paused = true;
					if (tempScoreX > 0) {
						dayEndText.WriteText ("Rowdy Boy: +" + tempScoreX);
						//score.AddRowdy (tempScoreX);
					} else if (tempScoreX < 0) {
						dayEndText.WriteText ("Soft Boy: +" + (tempScoreX*-1));
						//score.AddRowdy (tempScoreX);
					} else {
						doneWriting ();
					}
					tempScoreX = 0;
					nightStage = 3;
				}else if(nightStage == 5) {
					ended = false;
					paused = false;
					textbox.SetActive (true);
					nightStage = 0;
					nightBackground.SetActive (false);
					boyChart.SetActive (false);
					boyChart.SetActive (false);
					dayEndText.Clear ();
					workingText = nextScript.ToString ();
					doneWriting ();
				}
			}
		}
		for (int i = 0; i < fadeIns.Count; i++) {
			if(fadeIns[i].color.a>=1.0f){
				fadeIns.RemoveAt(i);
				i--;
			}else{
				fadeIns[i].color = new Color(fadeIns[i].color.r,fadeIns[i].color.g,fadeIns[i].color.b,fadeIns[i].color.a+fadeSpeed);
			}
		}
		for (int i = 0; i < fadeOuts.Count; i++) {
			if(fadeOuts[i].color.a<=0.0f){
				fadeOuts.RemoveAt(i);
				i--;
			}else{
				fadeOuts[i].color = new Color(fadeOuts[i].color.r,fadeOuts[i].color.g,fadeOuts[i].color.b,fadeOuts[i].color.a-fadeSpeed);
			}
		}
	}

	public void doneWriting(){
		if (ended&&tempScoreY!=0) {
			if (tempScoreY > 0) {
				dayEndText.HardReturn ();
				dayEndText.WriteText ("Perfect Boy: +" + tempScoreY);
				//score.AddPerfect (tempScoreY);
			} else if (tempScoreY < 0) {
				dayEndText.HardReturn ();
				dayEndText.WriteText ("Garbage Boy: +" + (tempScoreY*-1));
				//score.AddPerfect (tempScoreY);
			}
			tempScoreY = 0;
		}else if (!paused) {
			readLine(ReadNext(), 0);
		}
	}

	public List<string> ReadNext ()
	{
		List<string> list = new List<string> ();
		if (workingText == null) {
			Debug.Log("Reached end of working text");
		}
		int firstParen = workingText.IndexOf("(");
		if (firstParen == -1) {
			Debug.LogError ("Interpreter: Failed to read the next command, could not find start paren. Maybe you forgot an \"end\" or \"link\" command. Error code 03");
			return list;
		}
		list.Add(workingText.Substring (0, firstParen).TrimStart(trimChars));
		workingText = workingText.Substring (firstParen + 1).TrimStart(trimChars);
		WriteParamsToList (list);
		return list;
	}

	public string GetNext(){
		if (workingText == null) {
			Debug.Log("Reached end of working text");
			return "";
		}
		int firstParen = workingText.IndexOf("(");
		if (firstParen == -1) {
			Debug.LogError ("Interpreter: Failed to read the next command, could not find start paren. Maybe you forgot an \"end\" or \"link\" command. Error code 03");
			return "";
		}
		return workingText.Substring (0, firstParen).TrimStart (trimChars);
	}

	private void WriteParamsToList(List<string> list){
		int i;
		if (workingText.StartsWith("\"")) {
			workingText = workingText.Substring (1);
			int lastQuote = GetIndexOfNextQuote(0,workingText);
			if (lastQuote == -1) {
				Debug.LogError ("Interpreter: Unpaired quotation mark. Error code 04");
				return;
			}
			string quotationParam = workingText.Substring(0,lastQuote);
			int len = quotationParam.Length;
			quotationParam = quotationParam.Replace("\\\"","\"");
			list.Add (quotationParam);
			workingText = workingText.Substring (len+1).TrimEnd(trimChars);
			if (workingText.StartsWith (")")) {
				workingText = workingText.Substring (1);
				return;
			} else {
				workingText = workingText.Substring (workingText.IndexOf(",")+1).TrimStart(trimChars);
				WriteParamsToList (list);
				return;
			}
		}
		i = workingText.IndexOf(",");
		int p = workingText.IndexOf (")");
		if (i == -1 || p < i) {
			list.Add (workingText.Substring (0, p).TrimEnd (trimChars));
			workingText = workingText.Substring (p + 1).TrimStart (trimChars);
			return;
		}
		string sub = workingText.Substring (0, i).TrimEnd ();
		list.Add (sub);
		workingText = workingText.Substring (i + 1).TrimStart ();
		WriteParamsToList (list);
	}

	private TextAsset GetScript(string scriptName){
		foreach (TextAsset s in scripts) {
			if (s.name.Equals (scriptName)) {
				return s;
			}
		}
		Debug.LogError ("Could not find script \"" + scriptName + "\"");
		return null;
	}

	public void RecieveAnswer(string str){
		TextAsset s = GetScript (str);
		activeButtons = 0;
		for(int i = 0; i < 4; i++){
			buttons[i].SetActive(false);
			buttonText[i].text = "";
		}
		speakerText.gameObject.SetActive (true);
		workingText = s.ToString();
		ended = false;
		paused = false;
		textbox.SetActive(true);
		readLine(ReadNext(),0);
	}

	private int GetIndexOfNextQuote(int startingPoint, string s){
		int result = s.IndexOf ("\"");
		if (result == 0) {
			return result + startingPoint;
		}
		if (s.Substring (result - 1, 1).Equals ("\\")) {
			return GetIndexOfNextQuote(startingPoint + result + 1,s.Substring(result + 1));
		}
		return result + startingPoint;
	}

	private void Say(string text, string speaker, Color col){
		writer.SetColor (col);
		writer.Clear ();
		writer.WriteText (text, !speaker.Equals(""));
		speakerText.color = col;
		speakerText.text = speaker;
		textboxAddon.SetActive (!speaker.Equals (""));
	}

	private void Say(List<string> inp, string speaker, Color col){
		writer.SetColor (col);
		writer.Clear ();
		speakerText.color = col;
		speakerText.text = speaker;
		textboxAddon.SetActive (!speaker.Equals (""));
		List<float> waitTimes = new List<float> ();
		List<string> textQueue = new List<string> ();
		waitTimes.Clear ();
		textQueue.Add (inp [1]);
		for(int i = 2; i < inp.Count-1; i ++){
			waitTimes.Add(float.Parse(inp[i]));
			i++;
			textQueue.Add (inp [i]);
		}
		writer.WriteText (textQueue, waitTimes, !speaker.Equals (""));
		paused = true;
	}

	private void readLine(List<string> inp, int loopCatcher){
		if (loopCatcher > 100) {
			Debug.LogError ("Interpreter: Possible infinite loop detected");
			return;
		}
		switch (inp[0]) {
		case "show":
			if(inp[1].Equals("clark")){
				fadeIns.Add (clarkBase.GetComponent<SpriteRenderer>());
				fadeIns.Add (currentExpression);
				fadeOuts.Remove (clarkBase.GetComponent<SpriteRenderer>());
				fadeOuts.Remove(currentExpression);
				if(wearingTag){
					fadeIns.Add (nametag);
					fadeOuts.Remove(nametag);
				}
			}
			break;
		case "hide":
			if(inp[1].Equals("clark")){
				fadeOuts.Add (clarkBase.GetComponent<SpriteRenderer>());
				fadeOuts.Add (currentExpression);
				fadeIns.Remove (clarkBase.GetComponent<SpriteRenderer>());
				fadeIns.Remove(currentExpression);
				if(wearingTag){
					fadeOuts.Add (nametag);
					fadeIns.Remove(nametag);
				}
			}
			break;
		case "nametag":
			nametag.gameObject.SetActive(bool.Parse(inp[1]));
			break;
		case "expression":
			switch (inp[1]){
			case "surprised":
				currentExpression.sprite = surprised;
				break;
			case "normal":
				currentExpression.sprite = normal;
				break;
			case "normalside":
				currentExpression.sprite = normalSide;
				break;
			case "pleased":
				currentExpression.sprite = pleased;
				break;
			case "pleasedside":
				currentExpression.sprite = pleasedSide;
				break;
			case "embarrassed":
				currentExpression.sprite = embarrassed;
				break;
			case "impressed":
				currentExpression.sprite = impressed;
				break;
			case "down":
				currentExpression.sprite = lookingDown;
				break;
			case "nervous":
				currentExpression.sprite = nervous;
				break;
			case "nervousside":
				currentExpression.sprite = nervousSide;
				break;
			default:
				Debug.LogError("did not recognize expression \"" + inp[1] + "\"");
				break;
			}
			break;
		case "autopause":
			writer.autoPause = bool.Parse(inp [1]);
			break;
		case "link":
			nextScript = GetScript (inp [1]);
			break;
		case "directlink":
			workingText = GetScript (inp [1]).ToString ();
			break;
		case "next":
			if (inp.Count == 1) {
				Debug.LogError (" Error code 11");
				return;
			}
			break; 
		case "say":
			writer.Clear();
			writer.WriteText (inp [1]);
			paused = true;
			break;
		case "clark":
			if (inp.Count == 2) {
				Say (inp [1], "Clark", green);
			} else {
				Say (inp, "Clark", green);
			}
			paused = true;
			break;
		case "me":
			if (inp.Count == 2) {
				Say (inp [1], playerName, blue);
			} else {
				Say (inp, playerName, blue);
			}
			paused = true;
			break;
		case "narrate":
			if (inp.Count == 2) {
				Say (inp [1], "", black);
			} else {
				Say (inp, "", black);
			}
			paused = true;
			break;
		case "add":
			writer.WriteText (inp [1]);
			paused = true;
			break;
		case "clear":
			writer.Clear ();
			break;
		case "style":
			if (inp.Count == 1 || inp [1].Equals ("normal")) {
				writer.Style (FontStyle.Normal);
			}
			if (inp [1].Equals ("italics")) {
				writer.Style (FontStyle.Italic);
			}
			break;
		case "background":
			switch (inp [1]) {
			case "pharmacy":
				backdrop.sprite = pharmacy;
				break;
			case "blurrypharmacy":
				backdrop.sprite = blurryPharm;
				break;
			case "coffeeshop":
				backdrop.sprite = coffeeshop;
				break;
			case "blurrycafe":
				backdrop.sprite = blurryCafe;
				break;
			case "fadeout":
				fadeOuts.Add (backdrop);
				fadeIns.Remove (backdrop);
				break;
			case "fadein":
				fadeIns.Add (backdrop);
				fadeOuts.Remove (backdrop);
				break;
			}
			break;
		case "pause":
			paused = true;
			break;
		case "end":
			ended = true;
			endedTimer = Time.time + 0.2f;
			textbox.SetActive (false);
			textboxAddon.SetActive (false);
			writer.Clear ();
			speakerText.text = "";
			break;
		case "color":
			switch(inp[1]){
			case "green":
				writer.SetColor(green);
				break;
			case "black":
				writer.SetColor(black);
				break;
			case "blue":
				writer.SetColor(blue);
				break;
			}
			break;
		case "speaker":
			speakerText.text = inp [1];
			textboxAddon.SetActive (true);
			break;
		case "nospeaker":
			speakerText.text = "";
			textboxAddon.SetActive (false);
			break;
		case "ask":
			textbox.SetActive (false);
			speakerText.gameObject.SetActive (false);
			writer.Clear ();
			buttonText [activeButtons].text = inp [1];
			buttons [activeButtons].SetActive (true);
			buttons [activeButtons].GetComponent<BlueButton>().linkedTextName = inp [2];
			activeButtons++;
			break;
		case "flag":
			bool found = false;
			foreach (Flag f in flags) {
				if (f.nametag.Equals (inp [1])) {
					f.content = inp [2];
					found = true;
				}
			}
			if (!found) {
				flags.Add (new Flag (inp [1], inp [2]));
			}
			break;
		case "if":
			Flag check = new Flag();
			bool foundCheck = false;
			foreach (Flag f in flags) {
				if (f.nametag.Equals (inp [1])) {
					check = f;
					foundCheck = true;
				}
			}
			if (! ((foundCheck && check.content.Equals (inp [2])) || (inp[2].Equals("false") && !foundCheck))) {
				workingText = workingText.Substring (workingText.IndexOf ("endif()") + 7);
			}
			break;
		case "endif":
			break;
		case "freeze":
			if (inp.Count == 2) {
				waitTime = float.Parse (inp [1]);
			}
			break;
		case "perfect":
			tempScoreY += (float.Parse (inp [1]));
			break;
		case "garbage":
			tempScoreY -= (float.Parse (inp [1]));
			break;
		case "rowdy":
			tempScoreX += (float.Parse (inp [1]));
			break;
		case "soft":
			tempScoreX -= (float.Parse (inp [1]));
			break;
		case "EMPTYCOMMAND":
			Debug.LogError ("Error code 17");
			break;
		default:
			Debug.LogError ("Error code 18, command was " + inp[1]);
			break;
		}
		if (!paused && writer.IsNotWriting() && ! ended && waitTime <= 0) {
			readLine (ReadNext (), loopCatcher + 1);
		}
	}

}
