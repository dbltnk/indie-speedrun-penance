using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridComponent : MonoBehaviour {
	public static GridComponent instance;

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
			var rock = GameObject.Instantiate (prefabRock) as GameObject;
			rock.name = string.Format ("cell_{0}_{1}", cell.x, cell.y);
			rock.transform.parent = transform;
			rocks.Add (rock);
			rock.transform.position = cell.ViewPosition ();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDrawGizmos () {

	}
}
