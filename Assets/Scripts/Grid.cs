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
		rock.cell = grid.GetCellAt (x, y);

		return rock;
	}

	// Use this for initialization
	void Start () {
		grid = new HexGrid (5, 5);
		rocks = new List<GameObject> ();

		foreach (var cell in grid.EnumCells()) {
			CreateRockObject (cell.x, cell.y);
		}
		
		foreach (var rock in Rock.Instances) {
			rock.Connect ();
			//Debug.Log(rock);
			//rock.BreakApart();
		}

		//RecalculateGroups ();
	}
	
	// Update is called once per frame
	void Update () {
				
	}

	public Rock FindRockAt(int x, int y) {
		foreach (var rock in Rock.Instances) {
			if (rock != null && rock.gridX == x && rock.gridY == y)
				return rock;
		}

		return null;
	}

	public void RemoveCellAt (int x, int y) {
		grid.RemoveCell (x, y);
		var rock = FindRockAt (x, y);
		GameObject.Destroy (rock.gameObject);
		//RecalculateGroups ();
		rock.Disconnect ();
	}

	public void AddCellAt (int x, int y) {
		grid.CreateCell (x, y);
		var rock = CreateRockObject (x, y);
		rock.Connect ();
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
	
	public void RecalculateGroups() 
	{
		int nextGroundNr = 1;

		// reset all
		foreach (var rock in Rock.Instances) {
			rock.groupNr = 0;
		}

		// start
		var startRock = FindRockAt (0, 0);
		if (startRock == null)
			return;

		List<Rock> toCheck = new List<Rock> ();
		toCheck.AddRange (Rock.Instances);

		while (true) {
			toCheck.Remove (startRock);
			startRock.groupNr = nextGroundNr;
			++nextGroundNr;

			ExpandAndRemove (startRock, toCheck);

			if (toCheck.Count == 0)
				return;

			startRock = toCheck [0];
		}
	}

	void ExpandAndRemove (Rock startRock, List<Rock> toCheck)
	{
		int nr = startRock.groupNr;

		List<Rock> border = new List<Rock> ();

		border.Add (startRock);

		int safe = 10000;

		while (border.Count > 0) {
			--safe;
			if (safe < 0) break;

			// pick one and assign nr
			var cellRock = border [0];
			border.RemoveAt (0);
			toCheck.Remove (cellRock);

			// TODO XXX there is a bug!!!!

			// check neighbours
			foreach (var nCell in grid.EnumCellNeighbours(cellRock.gridX, cellRock.gridY)) {
				var rock = FindRockAt (nCell.x, nCell.y);

				// hole or already visited?
				if (rock == null || rock.groupNr > 0)
					continue;

				// extend border
				rock.groupNr = nr;
				border.Add (rock);
			}
		}
	}
}
