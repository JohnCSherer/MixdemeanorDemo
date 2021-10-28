using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour {
	public string dialogueScriptFolderName;
	public List<TextAsset> dialogueScripts = new List<TextAsset> ();
	private TextAsset nextText;
	private Wander wanderScript;
	public PlayerInteractionHandler interactionScript;
	private int timesTalked = 0;
	private string workingText;
	public string currentTextName = "null";
	private char[] trimChars = new char[3];
	private SpriteRenderer sprite;
	private bool right, left;
	private bool following;

	private static List<Flag> flags;

	private Animator animator;

	void Start(){
		TextAsset[] assets = Resources.LoadAll<TextAsset> ("Dialogue Scripts/" +dialogueScriptFolderName);
		foreach(TextAsset t in assets){
			dialogueScripts.Add (t);
			if (t.name == "Default") {
				nextText = t;
			}
		}
		if (nextText == null) {
			Debug.LogError ("Interpreter: Could not find \"Default\" dialoge script in the Resources/Dialogue Scripts/" + dialogueScriptFolderName + " folder. Error code 01");
		}
		animator = GetComponent<Animator> ();
		wanderScript = GetComponent<Wander> ();
		sprite = GetComponent<SpriteRenderer> ();
		right = sprite.flipX;
		left = !right;
		trimChars[0] = char.ConvertFromUtf32 (13)[0];
		trimChars[1] = char.ConvertFromUtf32 (10)[0];
		trimChars[2] = " "[0];

		flags = new List<Flag> ();
	}

	public void Interact(GameObject playerRef) {
		wanderScript.enabled = false;
		GetComponent<Rigidbody2D> ().simulated = false;
		GetComponent<Rigidbody2D> ().velocity = Vector3.zero;
		float xDiff = playerRef.transform.position.x - transform.position.x;
		float yDiff = playerRef.transform.position.y - transform.position.y;
		if (Mathf.Abs (xDiff) > Mathf.Abs (yDiff)) {
			animator.Play ("Stand");
			if (xDiff > 0) {
				sprite.flipX = right;
			} else {
				sprite.flipX = left;
			}
		} else {
			if (yDiff > 0) {
				animator.Play ("Face North");
			} else {
				animator.Play ("Face South");
			}
		}
	}

	public void SetNextScript(string scriptName){
		foreach (TextAsset text in dialogueScripts) {
			if (text.name.ToLower ().Equals (scriptName.ToLower ())) {
				nextText = text;
				return;
			}
		}
		Debug.Log ("Interpreter: Could not set \"" + scriptName + "\" as the next script because it was not found in the Resources/Dialogue Scripts/" + dialogueScriptFolderName + " folder. Error code 02");
		interactionScript.EndInteraction ();
	}



	public List<string> ReadNext ()
	{
		List<string> list = new List<string> ();
		if (workingText == null) {
			workingText = nextText.ToString ();
			currentTextName = nextText.name;
		}
		int firstParen = workingText.IndexOf("(");
		if (firstParen == -1) {
			Debug.LogError ("Interpreter: Failed to read the next command in " + currentTextName + ", could not find start paren. Maybe you forgot an \"end\" or \"link\" command. Error code 03");
			return list;
		}
		list.Add(workingText.Substring (0, firstParen).TrimStart(trimChars));
		workingText = workingText.Substring (firstParen + 1).TrimStart(trimChars);
		WriteParamsToList (list);
		return list;
	}

	private void WriteParamsToList(List<string> list){
		int i;
		if (workingText.StartsWith("\"")) {
			workingText = workingText.Substring (1);
			int lastQuote = workingText.IndexOf ("\"");
			if (lastQuote == -1) {
				Debug.LogError ("Interpreter: Unpaired quotation mark. Error code 04");
				return;
			}
			list.Add (workingText.Substring (0, lastQuote));

			workingText = workingText.Substring (lastQuote + 1).TrimEnd(trimChars);
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

	public void Answer(string scriptName){
		workingText = null;
		foreach (TextAsset text in dialogueScripts) {
			if (text.name.ToLower ().Equals (scriptName.ToLower ())) {
				workingText = text.ToString ();
				currentTextName = text.name;
				interactionScript.Prompt (gameObject);
			}
		}
		if (workingText == null) {
			Debug.Log ("Interpreter: Could not set \"" + scriptName + "\" as the next script because it was not found in the Resources/Dialogue Scripts/" + dialogueScriptFolderName + " folder. Error code 05");
			interactionScript.EndInteraction ();
			currentTextName = "null";
		}
	}

	public static void CreateFlag(string name, string value){
		flags.Add (new Flag (name, value));
	}

	public static void EditFlag(string name, string value){
		foreach (Flag f in flags) {
			if(f.nametag.Equals(name)){
				f.content = value;
				return;
			}
		}
		Debug.LogError ("Interpreter: Could not edit the contents of flag \"" + name + "\" because it was not found in the list of flags");
	}

	public static void IncrementFlag(string name){
		foreach (Flag f in flags) {
			if(f.nametag.Equals(name)){
				f.content = (float.Parse(f.content) + 1).ToString();
				return;
			}
		}
	}

	public void Follow(Transform t){
		following = true;
		wanderScript.enabled = false;
		GetComponent<Follow> ().enabled = true;
		GetComponent<Follow> ().target = t;
	}

	public void EndInteraction(){
		timesTalked++;
		workingText = null;
		currentTextName = null;
		if (!following) {
			wanderScript.enabled = true;
		}
		GetComponent<Rigidbody2D> ().simulated = true;
		wanderScript.freezeAndReset ();
	}
}
