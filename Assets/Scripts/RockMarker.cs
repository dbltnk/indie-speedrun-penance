using UnityEngine;
using System.Collections;

public class RockMarker : MonoBehaviour {
	public static RockMarker instance;

	public GameObject emptyMarker;
	public GameObject fullMarker;

	public bool isCarryingARock {
		get {
			return fullMarker.activeSelf;
		}
		set {
			emptyMarker.SetActive (value == false);
			fullMarker.SetActive (value == true);
		}
	}

	void Awake () {
		instance = this;
	}

	// Use this for initialization
	void Start () {
		isCarryingARock = false;
	}
}
