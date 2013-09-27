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
		
		foreach (var rock in Rock.Instances) {
			//Debug.Log(rock);
			//rock.BreakApart();
		}
	}
	
	// Update is called once per frame
	void Update () {
				
	}

	public Rock FindRockAt(int x, int y) {
		for (int i = 0; i < Rock._instances.Count; ++i) {
			var rock = Rock._instances [i].GetComponent<Rock> ();
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

	public UKTuple<int,int> FindNearestRockPosition(Vector3 positionOnGridPlane) {
		var cellPos = HexGrid.CellPositionFromView (positionOnGridPlane);

		UKTuple<int,int> minPos = new UKTuple<int, int>();
		float minDist = float.MaxValue;

		for (int dx = -1; dx <= 1; ++dx) {
			for (int dy = -1; dy <= 1; ++dy) {
				var pos = HexGrid.ViewCellPosition (cellPos.a + dx, cellPos.b + dy);
				var dist = Vector3.Distance (pos, positionOnGridPlane);
				if (dist < minDist) {
					minPos.a = cellPos.a + dx;
					minPos.b = cellPos.b + dy;
					minDist = dist;
				}
			}
		}

		return minPos;
	}
}
