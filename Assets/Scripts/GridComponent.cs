using UnityEngine;
using System.Collections;

public class GridComponent : MonoBehaviour {
	public static GridComponent instance;

	public HexGrid grid;

	void Awake () {
		instance = this;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
