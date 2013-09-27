using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {
	public static Grid instance;

	public GameObject prefabRock;

	public HexGrid grid;

	public List<GameObject> rocks;

	void Awake () {
		instance = this;
	}

	// Use this for initialization
	void Start () {
		grid = new HexGrid (15, 15);
		rocks = new List<GameObject> ();

		foreach (var cell in grid.EnumCells()) {
			var go = GameObject.Instantiate (prefabRock) as GameObject;
			go.name = string.Format ("cell_{0}_{1}", cell.x, cell.y);
			go.transform.parent = transform;
			rocks.Add (go);
			go.transform.position = cell.ViewPosition ();

			var rock = go.GetComponent<Rock> ();
			rock.gridX = cell.x;
			rock.gridY = cell.y;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDrawGizmos () {

	}
}
