﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Grid : MonoBehaviour {
	public static Grid instance;

	public GameObject prefabRock;
	public GameObject goal;

	public HexGrid<Rock,int> grid;

	public List<GameObject> rocks;

	public int rootGridX;
	public int rootGridY;

	void Awake () {
		instance = this;
	}
	
	public bool groupsDirty;

	public Rock CreateRockObject(int x, int y) {
		var go = GameObject.Instantiate (prefabRock) as GameObject;
		go.name = string.Format ("rock_x{0}_y{1}", x, y);
		go.transform.parent = transform;
		rocks.Add (go);
		go.transform.position = HexGrid<Rock,int>.ViewCellPosition(x,y);

		var rock = go.GetComponent<Rock> ();

		return rock;
	}

	// Use this for initialization
	void Start () {
		grid = new HexGrid<Rock,int> (7, 7);
		rocks = new List<GameObject> ();

		foreach (var cell in grid.EnumCells().ToList()) {
			var rock = CreateRockObject (cell.x, cell.y);
			AddRockToGrid(rock, cell.x, cell.y);
		}

		RecalculateGroups ();

		StartCoroutine (CoBreakApartAtTheEdges ());
	}

	IEnumerator CoBreakApartAtTheEdges()
	{
		while (true) {
			yield return new WaitForSeconds (Random.Range (
				Constants.instance.DROP_EVERY_MIN,
				Constants.instance.DROP_EVERY_MAX));

			BreakApartAtTheEdges ();
		}
	}

	// Update is called once per frame
	void Update () {
		if (groupsDirty) RecalculateGroups();	
	}

	public Rock FindRockAt(int x, int y) {
		if (grid.HasCellAt (x, y) == false)
			return null;

		var cell = grid.GetCellAt (x, y);
		return cell.param;
	}

	public void RemoveRockFromGrid (Rock rock) {
		grid.RemoveCell (rock.cell.x, rock.cell.y);
		rock.Disconnect ();
		rock.cell = null;
		groupsDirty = true;
	}

	public void AddRockToGrid (Rock rock, int x, int y) {
		grid.CreateCell (x, y);
		rock.cell = grid.GetCellAt (x, y);
		rock.cell.param = rock;
		rock.Connect ();
		groupsDirty = true;
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
		
		groupsDirty = false;
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
	
	void BreakApartAtTheEdges () {
		var rootPos = GetRoot ().transform.position;
		Rock[] candidates = Rock.Instances.Where(it => 
             it.mode == Rock.Mode.IDLE && 
             !IsRoot(it) &&
             it.Age > Constants.instance.MIN_AGE_BEFORE_BREAK &&
             Vector3.Distance(it.transform.position, rootPos) > Constants.instance.MIN_RADIUS_TO_KEEP_AROUND_ROOT
           ).ToArray();
		
		Vector3 root = FindRockAt(rootGridX, rootGridY).transform.position;
		Vector3 goalObject = goal.transform.position;
		
		if (candidates.Length > 0) {
		
			System.Array.Sort(candidates, (a, b) => {
				float aDist = Vector3.Distance(goalObject, a.transform.position);
				float bDist = Vector3.Distance(goalObject, b.transform.position);
				
				if (aDist < bDist)
		          return 1;
		        else if (aDist > bDist)
		          return -1;
		        else {
					if (a.neighbours.Count > b.neighbours.Count)
						return 1;
					else if (a.neighbours.Count < b.neighbours.Count)
						return -1;
					else
						return 0;
				}
		          
			});
			
			//foreach (Rock rock in candidates) {
			//	Debug.Log(rock);
			//}
			
			candidates[0].BreakApart();
		}
	}

	public bool CanRockBePicked (int x, int y)
	{
		return x != rootGridX || y != rootGridY;
	}
	
	public bool IsRoot(Rock it) {
		if (it.gridX == rootGridX && it.gridY == rootGridY) {	
			return true;	
		}	
		else {
			return false;
		}
	}

	public Rock GetRoot ()
	{
		return FindRockAt (rootGridX, rootGridY);
	}
}
