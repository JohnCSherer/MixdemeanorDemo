using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SubmenuEnums;
using ActorFilterEnum;

public class BattleController : MonoBehaviour {

	public Grid friendlyGrid;
	public Grid enemyGrid;
	public ActionList actionList;

	private Actor hiActor;
	private List<Actor> secondaryHiActor;
	private Action hiAction;

	public Text headerText;

	private enum State
	{
		SelectCharacter,
		PlayerSelectCharacter,
		SelectionAction,
		RollAction,
		RunAction,
		DamageStep,
		ResolveEffects,
	}

	private void SetState(State s){
		gameState = s;
		UpdateDebugHeader ();
	}

	private bool playerTurn;

	private State gameState;
	private SubmenuType submenu;


	void Start () {
		SetState(State.SelectCharacter);
		playerTurn = true;
		secondaryHiActor = new List<Actor> ();
	}
	
	// Update is called once per frame
	void Update () {
		switch (gameState) {
		case State.SelectCharacter:
			GridSelectCharacter ();
			break;
		case State.PlayerSelectCharacter:
			PlayerGridSelectCharacter ();
			break;
		case State.SelectionAction:
			ActionListSelectAction ();
			break;
		case State.RollAction:
			RollAction ();
			break;
		case State.RunAction:
			ResolveAction ();
			break;
		case State.ResolveEffects:
			ResolveEffects ();
			break;
		//REMEMBER TO CLEAR SecondaryHiActor LIST
		default:
			Debug.Log ("Error, switch machine fell through");
			break;
		}
	}

	private void GridSelectCharacter(){
		if (playerTurn) {
			if (friendlyGrid.hasRemainingControllableActions ()) {
				SetState(State.PlayerSelectCharacter );
				friendlyGrid.UpdateHighlightedTile ();
			} else {
				List<Actor> validRollableActors = new List<Actor> ();
				foreach (Actor a in friendlyGrid.actors) {
					if (!a.filters.Contains (ActorFilter.Controllable)) {
						validRollableActors.Add (a);
					}
				}
				int randomIndex = Random.Range (0, validRollableActors.Count);
				hiActor = validRollableActors [randomIndex];
				SetState (State.RollAction);
			} 
		} else {
			//Assume all enemy actors are uncontrollable
			List<Actor> actorsWithActions = new List<Actor> ();
			foreach (Actor a in enemyGrid.actors) {
				if (a.HasActionsRemaining ()) {
					actorsWithActions.Add (a);
				}
			}
			int randomIndex = Random.Range (0, actorsWithActions.Count);
			hiActor = actorsWithActions [randomIndex];
			SetState(State.RollAction);
		}
	}



	private void PlayerGridSelectCharacter(){
		if (friendlyGrid.PlayerSelectActor (ActorFilter.Controllable)) {
			hiActor = friendlyGrid.HiGetActor ();
			SetState(State.SelectionAction);
			actionList.AddActions (hiActor.GetActionList());
		}
	}

	private void ActionListSelectAction(){
		int actionSelected = actionList.SelectAction ();
		if (actionSelected != -1) { //If an action has been selected...
			hiAction = hiActor.GetActionList() [actionSelected]; 
			hiAction.InitializeAction (this, friendlyGrid, enemyGrid, !playerTurn, hiActor, false);
			SetState(State.RunAction);
			actionList.Clear ();
		}
	}

	private void RollAction(){ 

		// ======================================================================================================
		// FOR FUTURE ROLL ACTION IMPLEMENTATION
		// Call function of Actor to determine

		List<Action> actionList = hiActor.GetActionList ();
		List<Action> validActions = new List<Action> ();
		foreach (Action a in actionList) {
			if (a.IsValidAction ()) {
				validActions.Add (a);
			}
		}
		if (validActions.Count == 0) {
			Debug.LogWarning ("No valid actions");
		}
		int randomIndex = Random.Range (0, validActions.Count);
		hiAction = validActions [randomIndex];
		hiAction.InitializeAction (this, friendlyGrid, enemyGrid, !playerTurn, hiActor, true);
		SetState(State.RunAction);
	}

	private void ResolveAction(){
		if (hiAction.RunAction ()) {
			SetState(State.ResolveEffects);
			hiActor.transform.Find ("Sprite").localPosition = Vector3.zero;
		}
	}

	private void ResolveEffects(){ //DANGER: serious bug if one side has no actions at all even after RestoreActions()
		hiActor.IncrementActionCount ();
		hiActor = null;
		if (playerTurn) {
			if (!friendlyGrid.hasRemainingActions ()) {
				playerTurn = false;
				enemyGrid.RestoreActions ();
				Debug.Log ("Enemy turn start");
			}
		} else {
			if (!enemyGrid.hasRemainingActions ()) {
				playerTurn = true;
				friendlyGrid.RestoreActions ();
				Debug.Log ("Friendly turn start");
			}
		}
		SetState(State.SelectCharacter);
	}

	public Actor GetHiActor(){
		return hiActor;
	}

	public List<Actor> GetSecondaryHiActor(){
		return secondaryHiActor;
	}

	private void UpdateDebugHeader(){
		switch (gameState) {
		case State.RunAction:
			headerText.text = "RunAction";
			Debug.Log ("Moved State to RunAction " + Time.time);
			break;
		case State.DamageStep:
			headerText.text = "DamageStep";
			Debug.Log ("Moved State to DamageStep " + Time.time);
			break;
		case State.PlayerSelectCharacter:
			headerText.text = "PlayerSelectCharacter";
			Debug.Log ("Moved State to PlayerSelectCharacter " + Time.time);
			break;
		case State.ResolveEffects:
			headerText.text = "ResolveEffects";
			Debug.Log ("Moved State to ResolveEffects " + Time.time);
			break;
		case State.SelectCharacter:
			headerText.text = "SelectCharacter";
			Debug.Log ("Moved State to SelectCharacter " + Time.time);
			break;
		case State.SelectionAction:
			headerText.text = "SelectionAction";
			Debug.Log ("Moved State to SelectionAction " + Time.time);
			break;
		case State.RollAction:
			headerText.text = "RollAction";
			Debug.Log ("Moved State to RollAction " + Time.time);
			break;
		default:
			headerText.text = "No State";
			Debug.LogWarning ("Reached invalid state " + Time.time);
			break;
		}
	}
}
