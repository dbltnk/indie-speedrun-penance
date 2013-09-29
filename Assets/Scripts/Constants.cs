using UnityEngine;
using System.Collections;

public class Constants : MonoBehaviour {

	public static Constants instance;
	public static Constants i {
		get { return instance; }
	}

	public bool CHEATS_ENABLED;
	public float DESPAWN_HEIGHT;
	public float REWIND_HEIGHT;
	public float REWIND_SPEED;
	public float DROP_EVERY_MIN;
	public float DROP_EVERY_MAX;
	public Vector3 SCREEN_PICK_RAY_START;
	public float MIN_AGE_BEFORE_BREAK;
	public float IN_AIR_TIMEOUT;
	public float CRACLE_TIME;
	public float MIN_RADIUS_TO_KEEP_AROUND_ROOT;
	public float WEIGHT_GOAL_DISTANCE;
	public float WEIGHT_CONNECTIVITY;
	public float STONES_NEEDED_AS_PENANCE;
	public float MIN_CARRY_DIST;
	
	void Awake () {
		instance = this;
	}
}
