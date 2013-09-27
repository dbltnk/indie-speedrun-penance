﻿using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Soul s = Soul.instance;
		if (s == null)
			return;

		transform.position = s.transform.position;
		var soulPointInForward = s.head.transform.position + s.head.transform.TransformDirection (Vector3.forward);
		transform.LookAt (soulPointInForward);
	}
}
