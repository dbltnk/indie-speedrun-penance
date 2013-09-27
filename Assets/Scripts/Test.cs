using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
		for (int x = -5; x < 5; ++x) {
			for (int y = -5; y < 5; ++y) {
				var v = HexGrid.ViewCellPosition (x, y);
				var c = HexGrid.CellPositionFromView (v);
				var v2 = HexGrid.ViewCellPosition (c.a, c.b);
				var d = Vector3.Distance (v, v2);

				if (d > 0f) Debug.Log(string.Format("{0}/{1} v={2} c={3} d={4}", x,y, v,c, d));
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
