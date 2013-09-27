using UnityEngine;
using System.Collections;

public class Constants : MonoBehaviour {
	public static Constants instance;
	
	public float DESPAWNHEIGHT;
	
	void Awake () {
		instance = this;
	}
}
