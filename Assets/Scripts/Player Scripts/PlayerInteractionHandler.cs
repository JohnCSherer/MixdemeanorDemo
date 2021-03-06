// highest errorcode: 21

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionHandler : MonoBehaviour {

	private List<GameObject> dialogueSprites = new List<GameObject>();
	private List<GameObject> activeSprites = new List<GameObject>();
	private List<GameObject> answerButtons = new List<GameObject>();
	private GameObject textbox;
	private CircleCollider2D interactZone;
	private float interactionDistance = 3.0f;
	private PlayerMovementOverworld movementScript;
	private OldTextWriter writerScript;
	private Interaction dialogueScript;

	private bool interacting = false;
	private bool paused = false;
	private bool frozen = false;
	private float freezeTimer = 0.0f;
	private bool writingText = false;

	private float animationTimer;
	private const float animationLength = 0.25f;
	private Vector3 rightPosition = new Vector3 (0.836f, -0.973f, 1.0f);
	private Vector3 leftPosition = new Vector3 (-0.836f, -0.973f, 1.0f);
	private bool txtBoxFacingLeft;

	private List<LinearSlerper> activeSlerps = new List<LinearSlerper>();

	private int jDelay = 0; //cooldown between presses
	private int jDelayAmount = 20;



	private List<GameObject> party;

	// Use this for initialization
	void Start () {
		GameObject[] assets = Resources.LoadAll<GameObject>("Dialogue Prefabs");
		foreach (GameObject g in assets) {
			dialogueSprites.Add (g);
		}

		interactZone = GetComponent<CircleCollider2D> ();
		movementScript = GetComponent<PlayerMovementOverworld> ();

		party = new List<GameObject> ();

		foreach (Transform t in Camera.main.transform) {
			if (t.name.Equals("Textbox")) {
				textbox = t.gameObject;
			} else if(t.name.Contains("Answer")){
				answerButtons.Add (t.gameObject);
			}else{
				//dialogueSprites.Add (t.gameObject);
			}
		}
		if (answerButtons.Count != 4) {
			Debug.LogError ("Interpreter: Missing answer button references. Error code 06");
		}
		writerScript = textbox.GetComponent<OldTextWriter> ();
		animationTimer = 0.0f;
	}

	private bool isJPress(){
		if ((jDelay == 0) && Input.GetKeyDown (KeyCode.J)) {
			jDelay = jDelayAmount;
			return true;
		};
		return false;
	}

	// Update is called once per frame
	void Update () {
		if (interacting) {
			if (frozen) {
				if (freezeTimer > 0) {
					freezeTimer -= Time.deltaTime;
					if (freezeTimer <= 0) {
						frozen = false;
						freezeTimer = 0;
						readLine (dialogueScript.ReadNext (), 0);
					}
				}
			} else {
				if ((Input.GetKeyDown (KeyCode.J) || Input.GetMouseButtonDown (0))) {
					if (writingText) {
						writerScript.Accelerate ();
					} else {
						if (paused) {
							paused = false;
							readLine (dialogueScript.ReadNext (), 0);
						}
					}
				}
			}
			RunActiveSlerps ();
			if (jDelay > 0) {
				jDelay--;
			}
		} else {
			if (Input.GetKeyDown (KeyCode.J)) {
				GameObject obj = CheckForInteractable ();
				if (obj != null) {
					interacting = true;
					movementScript.freeze ();
					movementScript.faceObject (obj);
					movementScript.enabled = false;
					dialogueScript = obj.GetComponent<Interaction> ();
					dialogueScript.Interact (gameObject);
					readLine (dialogueScript.ReadNext (), 0);
				}
			}
		}
	}

	private GameObject CheckForInteractable(){
		RaycastHit2D[] hits = Physics2D.CircleCastAll (((Vector2) (transform.position)) + interactZone.offset, 
			interactZone.radius*interactionDistance, Vector2.zero, 0.0f);
		foreach(RaycastHit2D h in hits){
			Component interact = h.transform.gameObject.GetComponent<Interaction> ();
			if(interact != null){
				return h.transform.gameObject;
			}
		}
		return null;
	}

	private void ResizeDialogueSprites(){

	}

	public void EndInteraction(){
		interacting = false;
		movementScript.enabled = true;
		dialogueScript.EndInteraction ();
		writerScript.Clear ();
		writerScript.Hide ();
		foreach (GameObject g in activeSprites) {
			g.SetActive (false);
		}
	}



	private abstract class LinearSlerper
	{
		public Transform vect;
		public float start, stop;
		public float dist, baseTime;
		public LinearSlerper(Transform t, float srt, float stp, float bt){
			vect = t;
			start = srt;
			stop = stp;
			baseTime = bt;
			dist = stop - start;
		}
		public abstract void slerp (float percentage);
	}

	private class xSlerper : LinearSlerper
	{
		public xSlerper(Transform t, float srt, float stp, float bt): base(t,srt,stp,bt){
			vect.localPosition = new Vector3 (start, t.localPosition.y, t.localPosition.z);
		}

		public override void slerp (float percentage){
			vect.localPosition = new Vector3 (start + dist * Mathf.Sin (Mathf.PI * Mathf.Sqrt(percentage) / 2.0f), vect.localPosition.y, vect.localPosition.z);
		}
	}

	private class ySlerper : LinearSlerper
	{
		public ySlerper(Transform t, float srt, float stp, float bt): base(t,srt,stp,bt){
			vect.localPosition = new Vector3 (t.localPosition.x, start, t.localPosition.z);
		}

		public override void slerp (float percentage){
			vect.localPosition = new Vector3 (vect.localPosition.x, start + dist * Mathf.Sin (Mathf.PI * Mathf.Sqrt(percentage) / 2.0f), vect.localPosition.z);
		}
	}

	private void RunActiveSlerps(){
		for (int i = 0; i < activeSlerps.Count; i++) {
			LinearSlerper s = activeSlerps [i];
			float t = animationTimer - s.baseTime;
			s.slerp (t / animationLength);
			if (t > animationLength) {
				activeSlerps.Remove (s);
				i--;
			}
		}
		animationTimer += Time.deltaTime;
	}

	private GameObject GetDialogueSprite(string n){
		foreach (GameObject g in activeSprites) {
			if ((g.name.ToLower ()).Equals ("dialogue " + n.ToLower ())) {
				return g;
			}
		}
		foreach (GameObject g in dialogueSprites) {
			if ((g.name.ToLower ()).Equals ("dialogue " + n.ToLower ())) {
				GameObject obj = GameObject.Instantiate (g, Camera.main.transform);
				obj.name = "dialogue " + n;
				activeSprites.Add (obj);
				return obj;
			}
		}
		Debug.LogError ("Interpreter: Could not fine sprite \"" + n + "\", as requested by " + dialogueScript.currentTextName + ", posibly missing a reference, or mispelled name. Error code 07");
		return null;
	}

	public void FinishedWriting(){
		writingText = false;
		if (!frozen) {
			readLine (dialogueScript.ReadNext (), 0);
		}
	}

	public void WriteToTextbox(string text, bool left){
		if (!writerScript.visible||left!=txtBoxFacingLeft) {
			activeSlerps.Add (new ySlerper (textbox.transform, textbox.transform.localPosition.y * 3, textbox.transform.localPosition.y, animationTimer));
		}
		writerScript.Show ();
		writerScript.Clear ();
		writingText = true;
		writerScript.WriteText (text);
		textbox.GetComponent<SpriteRenderer> ().flipX = (left);
		txtBoxFacingLeft = left;
	}

	public void RecieveAnswer(int n){
		writerScript.ClearAnswers ();
		List<string> inp = dialogueScript.ReadNext();
		if(!inp[0].ToLower().Equals("link")){
			Debug.LogError("Interpreter: Encountered an \"answer\" without a \"link\" after it in " + dialogueScript.currentTextName + ", or the RecieveAnswer function was called out of place. Error code 08");
		}
		dialogueScript.Answer (inp[n]);
		frozen = false;
	}

	public void Prompt(GameObject obj){
		interacting = true;
		movementScript.freeze ();
		movementScript.faceObject (obj);
		movementScript.enabled = false;
		dialogueScript = obj.GetComponent<Interaction> ();
		dialogueScript.Interact (gameObject);
		readLine (dialogueScript.ReadNext (), 0);
	}

	private void readLine(List<string> inp, int loopCatcher){
		if (loopCatcher > 100) {
			Debug.LogError ("Interpreter: Possible infinite loop detected in " + dialogueScript.currentTextName + ", interaction aborted. Error code 09");
			EndInteraction ();
			return;
		}
		switch (inp[0]) {
		case "end":
			EndInteraction ();
			break;
		case "link":
			Debug.LogError ("Interpreter: Encountered a \"link\" command in " + dialogueScript.currentTextName + ", but could not find the associated \"answer\" command, interaction terminated. Error code 10");
			EndInteraction ();
			break;
		case "next":
			if (inp.Count == 1) {
				Debug.LogError ("Interpreter: Not enough instructions from the \"next()\" command in " + dialogueScript.currentTextName + ". This command takes 1 instruction, commmand ignored. Error code 11");
				return;
			}
			dialogueScript.SetNextScript (inp [1]);
			break; 
		case "show":
			if (inp.Count < 3) {
				Debug.LogError ("Interpreter: Not enough instructions from the \"next()\" command in " + dialogueScript.currentTextName + ". This command takes 2 instructions, command ignored. Error code 12");
				return;
			} else {
				GameObject showSprite = GetDialogueSprite (inp [1]);
				if (showSprite == null) {
					Debug.Log ("Interpreter: Could not show the sprite \"" + inp [1] + "\" as requested by " + dialogueScript.currentTextName + " because it doesn't exist in the \"Resources/Dialogue Prefabs\" folder");
				}
				showSprite.SetActive (true);
				if (inp [2].Equals ("left")) {
					showSprite.transform.localPosition = leftPosition;
					showSprite.GetComponent<SpriteRenderer> ().flipX = false;
				} else if (inp [2].Equals ("right")) {
					showSprite.transform.localPosition = rightPosition;
					showSprite.GetComponent<SpriteRenderer> ().flipX = true;
				} else {
					Debug.LogError ("Interpreter: Did not recognize \"" + inp[2] +"\" as \"left\" or \"right\" from the \"show\" command in " + dialogueScript.currentTextName + ". Error code 13");
				}
				activeSlerps.Add (new xSlerper (showSprite.transform, showSprite.transform.localPosition.x * 2, showSprite.transform.localPosition.x, animationTimer));
				break;
			}
		case "say":
			if (inp.Count < 3) {
				Debug.LogError ("Interpreter: Not enough instructions from the \"say()\" command in " + dialogueScript.currentTextName + ". This command takes 2 instructions, command ignored. Error code 14");
				return;
			}
			WriteToTextbox (inp [2], inp[1].Equals ("left"));
			break;
		case "speed":
			if (inp.Count == 2 && inp[1].Length != 0) {
				float speed;
				float.TryParse (inp [1], out speed);
				if (speed > 0) {
					writerScript.SetSpeed (speed);
				} else if (speed == 0) {
					Debug.LogError ("Interpreter: Could not read \"" + inp [1] + "\" as a number, as requested in " + dialogueScript.currentTextName + ". Command ignored. Error code 20");
				} else {
					Debug.LogError ("Interpreter: " + dialogueScript.currentTextName + " triedto set textcrawl speed to a negative value. Command ignored. Error code 20");
				}
			} else {
				writerScript.SetSpeed ();
			}
			break;
		case "pause":
			paused = true;
			break;
		case "freeze":
			frozen = true;
			if (inp.Count == 2) {
				freezeTimer = float.Parse(inp [1]);
			}
			break;
		case "react":
			if (inp.Count < 3) {
				Debug.LogError ("Interpreter: Not enough instructions from the \"react()\" command in " + dialogueScript.currentTextName + ". This command takes 2 instructions, command ignored. Error code 15");
				return;
			}
			GameObject reactSprite = GetDialogueSprite (inp [1]);
			reactSprite.GetComponent<Animator> ().Play (inp [2]);
			break;
		case "effect":
			List<string> parameters = new List<string>();
			if(inp.Count > 2){
				for(int i = 2; i < inp.Count; i++){
					parameters.Add (inp[i]);
				}
			}
			writerScript.AddEffect (inp[1], parameters);
			break;
		case "answers":
			if (inp.Count > 5) {
				Debug.LogError ("Interpreter: Too many instructions from the \"answers()\" command in " + dialogueScript.currentTextName + ". Max is 4, command ignored. Error code 16");
				return;
			}
			paused = true;
			frozen = true;
			writerScript.WriteAnswers (inp, answerButtons);
			break;
		case "follow":
			if(party.Count == 0){
				dialogueScript.Follow(transform);
			}else{
				dialogueScript.Follow(party[party.Count-1].transform );
			}
			party.Add (dialogueScript.gameObject);
			break;
		case "addflag":
			if(inp.Count<3){
				Debug.LogError ("Interpreter: Not enough instructions for the \"addflag()\" command in " + dialogueScript.currentTextName + ". Need 2, command ignored.");
				return;
			}
			Interaction.CreateFlag(inp[1],inp[2]);
			break;
		case "EMPTYCOMMAND":
			Debug.LogError ("Interpreter: Failed to read line (length = 0) in " + dialogueScript.currentTextName + ", possibly due to missing 'end()' statement. Error code 17");
			break;
		default:
			Debug.LogError ("Interpreter: Unrecognized command read from " + dialogueScript.currentTextName + ": \"" + inp[0] + "\". Command ignored. Error code 18");
			break;
		}
		if (!paused && !frozen && !writingText && interacting) {
			readLine (dialogueScript.ReadNext (), loopCatcher + 1);
		}
	}
}


