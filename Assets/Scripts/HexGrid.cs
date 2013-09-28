using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class HexGrid {
	private const float CELL_SIDE = 1.155f;
	private const float CELL_DIAMETER_LONG = 2f * CELL_SIDE;
	private const float CELL_DIAMETER_SHORT = CELL_SIDE * 1.73205080756888f;
	
	// center
	public static Vector3 ViewCellPosition(int x, int y)
	{
		Vector2 v2d = new Vector2();
		
		v2d.x = (float)x * CELL_DIAMETER_SHORT;
		v2d.y = (float)y * (CELL_SIDE + CELL_SIDE / 2f);

		if (Math.Abs(y) % 2 == 1)
		{
			v2d.x += CELL_DIAMETER_SHORT / 2f;
		}
		
		return new Vector3(v2d.x, 0f, v2d.y);
	}

	public static UKTuple<int,int> CellPositionFromView(Vector3 p) {
		int y = Mathf.FloorToInt (p.z / (CELL_SIDE + CELL_SIDE / 2f));
		float delta = Math.Abs(y) % 2 == 0 ? 0f : Mathf.FloorToInt(CELL_DIAMETER_SHORT / 2f);
		int x = Mathf.FloorToInt ((p.x + delta) / CELL_DIAMETER_SHORT);
		return new UKTuple<int, int> (x, y);
	}
		
	// cell x,y, even if there are no cells
	public static IEnumerable< UKTuple<int,int> > EnumNeighbourPositions(int x, int y)
	{
		// basic cross
		yield return new UKTuple<int, int>(x+1, y+0);
		yield return new UKTuple<int, int>(x-1, y+0);
		yield return new UKTuple<int, int>(x+0, y+1);
		yield return new UKTuple<int, int>(x+0, y-1);

		// additional fields
		if (Math.Abs(y) % 2 == 0)
		{
			yield return new UKTuple<int, int>(x-1, y+1);
			yield return new UKTuple<int, int>(x-1, y-1);
		}
		else
		{
			yield return new UKTuple<int, int>(x+1, y+1);
			yield return new UKTuple<int, int>(x+1, y-1);
		}
	}

	// ----------------------------------------------
	#region edge
	
	public class HexEdge {
		public int cellAX;
		public int cellAY;
		
		public int cellBX;
		public int cellBY;
		
		public HexEdge(int cellAX, int cellAY, int cellBX, int cellBY)
		{
			this.cellAX = cellAX;
			this.cellAY = cellAY;

			this.cellBX = cellBX;
			this.cellBY = cellBY;
		}
		
		public Vector2 viewPosition()
		{
			return (ViewCellPosition(cellAX, cellAY) + ViewCellPosition(cellBX, cellBY)) / 2f;
		}
		
		public float viewAngleInDegree()
		{
			Vector2 a = ViewCellPosition(cellAX, cellAY);
			Vector2 b = ViewCellPosition(cellBX, cellBY);
			return Vector2.Angle(new Vector2(0f, -1f), (a-b));
		}
		
		public bool IsConnectedToCell(int x, int y)
		{
			return (cellAX == x && cellAY == y) || (cellBX == x && cellBY == y);
		}
		
		public override string ToString ()
		{
			return string.Format ("[HexEdge ax:{0} ay:{1} bx:{2} by:{3} ang:{4}]", 
				cellAX, cellAY, 
				cellBX, cellBY,
				viewAngleInDegree());
		}
	}
	
	#endregion
	// ----------------------------------------------
	#region cell
	
	public class HexCell {
		public int x;
		public int y;

		public HexCell(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
		
		// center
		public Vector3 ViewPosition()
		{
			return ViewCellPosition(x,y);
		}
		
		public override string ToString ()
		{
			return string.Format ("[HexCell x={0} y={1} d={5}]", 
				x, y, ViewPosition());
		}
	}
	
	#endregion
	// ----------------------------------------------
	#region grid
	
	private int width;
	private int height;
	
	private Dictionary<string, HexCell> cells;

	private List<HexEdge> edges;
	
	public HexGrid(int width, int height)
	{
		this.width = width;
		this.height = height;
			
		this.cells = new Dictionary<string, HexCell>();
		this.edges = new List<HexEdge>();

		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				CreateCellInternal(x, y);
			}
		}
		
		AddMissingEdges();
	}
	
	private string CellKey(int x, int y)
	{
		return x.ToString() + "_" + y.ToString();
	}
	
	public bool HasCellAt(int x, int y)
	{
		return cells.ContainsKey(CellKey(x,y));
	}
	
	public void RemoveCell(int x, int y)
	{
		string key = CellKey(x,y);
		cells.Remove(key);
		RemoveUnconnectedEdges();
	}

	public void CreateCell(int x, int y)
	{
		CreateCellInternal (x, y);
		AddMissingEdges ();
	}

	private void CreateCellInternal(int x, int y)
	{
		string key = CellKey(x,y);
		
		HexCell cell = new HexCell(x,y);
		cells[key] = cell;
	}
	
	public IEnumerable<HexCell> EnumCells()
	{
		foreach(var cell in cells.Values)
		{
			yield return cell;
		}
	}
	
	public IEnumerable<HexEdge> EnumEdges()
	{
		foreach(var edge in edges)
		{
			yield return edge;
		}		
	}
	
	private void AddMissingEdges()
	{
		foreach(var cell in cells.Values)
		{
			foreach(var neighbourPos in EnumNeighbourPositions(cell.x, cell.y))
			{
				int nx = neighbourPos.a;
				int ny = neighbourPos.b;
				
				if (HasCellAt(nx, ny) && IsEdgeBetween(cell.x, cell.y, nx, ny) == false)
				{
					edges.Add(new HexEdge(cell.x, cell.y, nx, ny));
				}
			}
		}
	}
	
	private void RemoveUnconnectedEdges()
	{
		List<HexEdge> removeEdge = new List<HexEdge>();
		
		foreach(var edge in edges)
		{
			
			if (HasCellAt(edge.cellAX, edge.cellAY) == false ||
				HasCellAt(edge.cellAX, edge.cellAY) == false)
			{
				removeEdge.Add(edge);
			}
		}
		
		foreach(var edge in removeEdge)
		{
			edges.Remove(edge);
		}
	}
	
	public HexEdge GetEdgeBetween(int cellAX, int cellAY, int cellBX, int cellBY)
	{
		foreach(var edge in edges)
		{
			if (edge.IsConnectedToCell(cellAX, cellAY) && edge.IsConnectedToCell(cellBX, cellBY))
				return edge;
		}
				
		throw new Exception(string.Format("not cell between {0},{1} and {0},{1}", 
			cellAX, cellAY, cellBX, cellBY));
	}
	
	public bool IsEdgeBetween(int cellAX, int cellAY, int cellBX, int cellBY)
	{
		foreach(var edge in edges)
		{
			if (edge.IsConnectedToCell(cellAX, cellAY) && edge.IsConnectedToCell(cellBX, cellBY))
				return true;
		}
				
		return false;
	}
	
	public HexCell GetCellAt(int x, int y)
	{
		if (HasCellAt(x,y))return cells[CellKey(x,y)];
		else throw new Exception(string.Format("not cell at x:{0} y:{1}", x,y));
	}
	
	public IEnumerable<HexCell> EnumCellNeighbours(int x, int y)
	{
		foreach(var p in EnumNeighbourPositions(x,y))
		{
			if (HasCellAt(p.a, p.b))yield return GetCellAt(p.a, p.b);
		}
	}
	
	/*
	// xy from cell
	public HexEdge getEdgeAt(int x, int y, int edgeIndex)
	{
		// TODO
	}
	*/
	
	#endregion
}
