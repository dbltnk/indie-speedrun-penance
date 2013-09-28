using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class HexGridIsConnected {
	public static bool IsPathBetween(HexGrid grid, int cellAX, int cellAY, int cellBX, int cellBY)
	{
		if (grid.HasCellAt (cellAX, cellAY) == false || grid.HasCellAt (cellBX, cellBY) == false)
			return false;

		if (cellAX == cellBX && cellAY == cellBY)
			return true;

		List<UKTuple<int,int>> border = new List<UKTuple<int, int>> ();
		List<UKTuple<int,int>> alreadyVisited = new List<UKTuple<int, int>> ();

		border.Add (new UKTuple<int, int> (cellAX, cellAY));
		
		int safe = 100;
		
		while (border.Count > 0) {
			--safe;
			if (safe < 0) break;
			
			// pick one
			var cell = border [0];
			border.RemoveAt (0);
			alreadyVisited.Add(new UKTuple<int, int>(cell.a, cell.b));

			// check neighbours
			foreach (var nPos in HexGrid.EnumNeighbourPositions(cell.a, cell.b)) {
				// hole?
				if (grid.HasCellAt (nPos.a, nPos.b) == false)
					continue;
				// already checked or planned?
				if (alreadyVisited.Contains (nPos) || border.Contains(nPos))
					continue;

				// is b?
				if (nPos.a == cellBX && nPos.b == cellBY)
					return true;

				// extend border
				border.Add (nPos);
			}
		}

		return false;
	}
}
