using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubmenuEnums;
using ActorFilterEnum;

public abstract class Action  {
	protected BattleController controller;
	protected Grid friendlyGrid;
	protected Grid enemyGrid;
	public Actor hiActor;
	protected bool automate;


	public string actionName;
	protected int currentSubmenu = 0; //Starts at 0
	protected float startTime;

	//Gets a reference to the Battle Controller for animation purposes
	public void DesignateBattleController (BattleController con){
		controller = con;
	}

	public void DesignateGrids(Grid friendlyGrid, Grid enemyGrid){
		this.friendlyGrid = friendlyGrid;
		this.enemyGrid = enemyGrid;
	}

	public void DesignateGrids(Grid friendlyGrid, Grid enemyGrid, bool invert){
		if (invert) {
			this.enemyGrid = friendlyGrid;
			this.friendlyGrid = enemyGrid;
		} else {
			this.friendlyGrid = friendlyGrid;
			this.enemyGrid = enemyGrid;
		}
	}

	public void InitializeAction(BattleController c, Grid friendlyGrid, Grid enemyGrid, bool invert, Actor hiActor, bool auto){
		if (invert) {
			this.enemyGrid = friendlyGrid;
			this.friendlyGrid = enemyGrid;
		} else {
			this.friendlyGrid = friendlyGrid;
			this.enemyGrid = enemyGrid;
		}
		controller = c;
		this.hiActor = hiActor;
		automate = auto;
	}

	public abstract bool RunAction (); //The 'main' method for each action

	public abstract bool IsValidAction ();
}
