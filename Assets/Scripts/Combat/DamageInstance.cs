using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*An object used to send and recieve information about an attack that does damage */
public class DamageInstance {
	public int damage;
	public Actor source; //Entity that is attacking
	public string[] tags; //Used for unique effects

	public DamageInstance(){
		damage = 0;
		source = null;
		tags = new string[0];
	}

	public DamageInstance(int dmg){
		damage = dmg;
		source = null;
		tags = new string[0];
	}

	public DamageInstance(int dmg, Actor src){
		damage = dmg;
		source = src;
		tags = new string[0];
	}

	public DamageInstance(int dmg, string[] t){
		damage = dmg;
		source = null;
		tags = new string[t.Length];
		for (int i = 0; i < t.Length; i++) {
			tags [i] = t [i];
		}
	}

	public DamageInstance(int dmg, Actor src, string[] t){
		damage = dmg;
		source = src;
		tags = new string[t.Length];
		for (int i = 0; i < t.Length; i++) {
			tags [i] = t [i];
		}
	}
}