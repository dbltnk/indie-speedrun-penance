﻿using UnityEngine;
using System.Collections;

public class Rock : MonoBehaviour {
	public int gridX;
	public int gridY;

	public enum Mode {
		IDLE = 0,
		FALLING = 1,
	};

	public Mode mode = Mode.IDLE;

	public float fallDownIn = 0f;
	public float fallSpeed = 0f;

	// Use this for initialization
	void Start () {
		fallDownIn = Random.Range (2f, 10f);
		fallSpeed = Random.Range (2f, 5f);
	}
	
	// Update is called once per frame
	void Update () {
		if (mode == Mode.IDLE) fallDownIn -= Time.deltaTime;

		if (fallDownIn < 0f && mode == Mode.IDLE) {
			mode = Mode.FALLING;
		} 

		if (mode == Mode.FALLING) {
			transform.Translate (Vector3.down * fallSpeed * Time.deltaTime);
		}
	}
}