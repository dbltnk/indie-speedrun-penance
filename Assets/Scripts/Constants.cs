using UnityEngine;
using System.Collections;

public class Constants : MonoBehaviour {
	public static Constants instance;
	
	public float DESPAWNHEIGHT;
	public float DROPEVERY;
	
	void Awake () {
		instance = this;
	}
}
