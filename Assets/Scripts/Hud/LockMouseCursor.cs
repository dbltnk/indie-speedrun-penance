using UnityEngine;
using System.Collections;

public class LockMouseCursor : MonoBehaviour {
	public bool lockCursorOnStart;

	// Use this for initialization
	void Start () {
		Screen.lockCursor = lockCursorOnStart;
	}
}
