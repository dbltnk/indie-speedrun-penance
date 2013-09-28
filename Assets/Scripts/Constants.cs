using UnityEngine;
using System.Collections;

public class Constants : MonoBehaviour {

	public static Constants instance;

	public bool CHEATS_ENABLED;
	public float DESPAWN_HEIGHT;
	public float DROP_EVERY_MIN;
	public float DROP_EVERY_MAX;
	public Vector3 SCREEN_PICK_RAY_START;
	public float MIN_AGE_BEFORE_BREAK;
	public float IN_AIR_TIMEOUT;
	public float CRACLE_TIME;
	public float MIN_RADIUS_TO_KEEP_AROUND_ROOT;
	
	void Awake () {
		instance = this;
	}
}
