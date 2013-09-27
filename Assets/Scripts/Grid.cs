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

	private Rock CreateRockObject(int x, int y) {
		var go = GameObject.Instantiate (prefabRock) as GameObject;
		go.name = string.Format ("cell_{0}_{1}", x, y);
		go.transform.parent = transform;
		rocks.Add (go);
		go.transform.position = HexGrid.ViewCellPosition(x,y);

		var rock = go.GetComponent<Rock> ();
		rock.gridX = x;
		rock.gridY = y;

		return rock;
	}

	// Use this for initialization
	void Start () {
		grid = new HexGrid (15, 15);
		rocks = new List<GameObject> ();

		foreach (var cell in grid.EnumCells()) {
			CreateRockObject (cell.x, cell.y);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDrawGizmos () {

	}

	public Rock FindRockAt(int x, int y) {
		for (int i = 0; i < Rock.instances.Count; ++i) {
			var rock = Rock.instances [i].GetComponent<Rock> ();
			if (rock.gridX == x && rock.gridY == y)
				return rock;
		}

		return null;
	}

	public void RemoveCellAt (int x, int y) {
		grid.RemoveCell (x, y);
		GameObject.Destroy (FindRockAt (x, y).gameObject);
	}

	public void AddCellAt (int x, int y) {
		grid.CreateCell (x, y);
		CreateRockObject (x, y);
	}
}
