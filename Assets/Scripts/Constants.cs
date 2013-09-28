using UnityEngine;
using System.Collections;

public class Constants : MonoBehaviour {
	public static Constants instance;
	
	public float DESPAWN_HEIGHT;
	public float DROP_EVERY;
	public Vector3 SCREEN_PICK_RAY_START;
	
	void Awake () {
		instance = this;
	}
}
