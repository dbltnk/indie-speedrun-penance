using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Grid : MonoBehaviour {
	public static Grid instance;

	public GameObject prefabRock;

	public HexGrid<Rock,int> grid;

	public List<GameObject> rocks;

	public int rootGridX;
	public int rootGridY;

	void Awake () {
		instance = this;
	}

	private Rock CreateRockObject(int x, int y) {
		var go = GameObject.Instantiate (prefabRock) as GameObject;
		go.name = string.Format ("rock_x{0}_y{1}", x, y);
		go.transform.parent = transform;
		rocks.Add (go);
		go.transform.position = HexGrid<Rock,int>.ViewCellPosition(x,y);

		var rock = go.GetComponent<Rock> ();
		rock.cell = grid.GetCellAt (x, y);
		rock.cell.param = rock;

		return rock;
	}

	// Use this for initialization
	void Start () {
		grid = new HexGrid<Rock,int> (3, 3);
		rocks = new List<GameObject> ();

		foreach (var cell in grid.EnumCells()) {
			CreateRockObject (cell.x, cell.y);
		}
		
		foreach (var rock in Rock.Instances) {
			rock.Connect ();
			//Debug.Log(rock);
			//rock.BreakApart();
		}

		RecalculateGroups ();
	}
	
	// Update is called once per frame
	void Update () {
				
	}

	public Rock FindRockAt(int x, int y) {
		if (grid.HasCellAt (x, y) == false)
			return null;

		var cell = grid.GetCellAt (x, y);
		return cell.param;
	}

	public void RemoveCellAt (int x, int y) {
		var rock = FindRockAt (x, y);
		rock.Disconnect ();
		GameObject.Destroy (rock.gameObject);
		grid.RemoveCell (x, y);

		RecalculateGroups ();
	}

	public void AddCellAt (int x, int y) {
		grid.CreateCell (x, y);
		var rock = CreateRockObject (x, y);
		rock.Connect ();

		RecalculateGroups ();
	}

	public UKTuple<int,int> FindNearestRockPosition(Vector3 positionOnGridPlane) {
		var cellPos = HexGrid<Rock,int>.CellPositionFromView (positionOnGridPlane);

		UKTuple<int,int> minPos = new UKTuple<int, int>();
		float minDist = float.MaxValue;

		for (int dx = -1; dx <= 1; ++dx) {
			for (int dy = -1; dy <= 1; ++dy) {
				var pos = HexGrid<Rock,int>.ViewCellPosition (cellPos.a + dx, cellPos.b + dy);
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
		var startRock = FindRockAt (rootGridX, rootGridY);

		while (startRock != null) {
			startRock.groupNr = nextGroundNr;
			++nextGroundNr;

			ExpandAndRemove (startRock);

			var uncheckedRocks = Rock.Instances.Where(it => it.groupNr == 0);

			startRock = uncheckedRocks.FirstOrDefault ();
		}
	}

	void ExpandAndRemove (Rock startRock)
	{
		int nr = startRock.groupNr;

		Queue<Rock> border = new Queue<Rock> ();

		border.Enqueue (startRock);

		int safe = 10000;

		while (border.Count > 0) {
			--safe;
			if (safe < 0) break;

			// pick one and assign nr
			var cellRock = border.Dequeue();
			
			// check neighbours
			foreach (var rock in cellRock.neighbours) {
				// hole or already visited?
				if (rock.groupNr > 0)
					continue;

				// extend border
				rock.groupNr = nr;
				border.Enqueue (rock);
			}
		}
	}

	public bool CanRockBePicked (int x, int y)
	{
		return x != rootGridX || y != rootGridY;
	}
}
