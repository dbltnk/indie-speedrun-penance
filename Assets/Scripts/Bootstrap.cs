using UnityEngine;
using System.Collections;

public class Bootstrap : MonoBehaviour {

	// Use this for initialization
	void Start () {
		AudioManager.instance.playMusic ("ambience");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
