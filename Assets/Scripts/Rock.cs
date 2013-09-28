﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rock : UKListedBehaviour<Rock> {
	public HexGrid<Rock,int>.HexCell<Rock> cell;

	public int gridX {
		get { return cell != null ? cell.x : 0; }
	}

	public int gridY {
		get { return cell != null ? cell.y : 0; }
	}

	public int groupNr;

	public enum Mode {
		IDLE = 0,
		FALLING = 1,
	};

	public List<Rock> neighbours = new List<Rock>();

	public Mode mode = Mode.IDLE;

	public float fallDownIn = 0f;
	public float fallSpeed = 0f;

	// Use this for initialization
	void Start () {
		fallDownIn = Random.Range (2f, 10f);
		fallSpeed = Random.Range (2f, 5f);

		InvokeRepeating ("CheckStillConnected", 0.25f, 0.25f);
	}

	public void Connect() {
		neighbours.Clear ();

		foreach (var nPos in HexGrid<Rock,int>.EnumNeighbourPositions (gridX, gridY)) {
			var nRock = Grid.instance.FindRockAt (nPos.a, nPos.b);
			if (nRock != null) {
				neighbours.Add (nRock);
				if (nRock.neighbours.Contains(this) == false) nRock.neighbours.Add(this);
			}
		}
	}

	public void Disconnect() {
		foreach (var nRock in neighbours) {
			nRock.neighbours.Remove (this);
		}
		neighbours.Clear();
	}

	public void BreakApart () {
		if (Grid.instance.IsRoot(this) == false && mode == Mode.IDLE) {
			mode = Mode.FALLING;
			Disconnect();
			Grid.instance.groupsDirty = true;
		}
	}

	void CheckStillConnected () {
		var centerRock = Grid.instance.FindRockAt(Grid.instance.rootGridX,Grid.instance.rootGridY);
		
		if (mode == Mode.IDLE && groupNr > 0 && centerRock != null && groupNr != centerRock.groupNr) {
			BreakApart ();
		}
	}

	// Update is called once per frame
	void Update () {
		if (mode == Mode.IDLE) fallDownIn -= Time.deltaTime;

		//if (fallDownIn < 0f && mode == Mode.IDLE) {
		//	BreakApart ();
		//} 

		if (mode == Mode.FALLING) {
			transform.Translate (Vector3.down * fallSpeed * Time.deltaTime);
		}

		// Debug.Log (instances.Count);

		if (transform.position.y < Constants.instance.DESPAWNHEIGHT) {
			Grid.instance.RemoveCellAt (gridX, gridY);
		}

	}

	void OnDrawGizmosSelected () {
		Gizmos.color = Color.red;
		Gizmos.DrawSphere (HexGrid<Rock,int>.ViewCellPosition (gridX, gridY), 0.1f);
		
		Gizmos.color = Color.green;
		foreach (var nRock in neighbours) {
			if (nRock == null)
				continue;
			Gizmos.DrawLine (transform.position, nRock.transform.position);
		}

		Gizmos.color = GizmosHelper.COLORS[groupNr % GizmosHelper.COLORS.Length];
		Gizmos.DrawWireCube (transform.position, new Vector3(1.5f, 0.25f, 1.5f));
	}

	public override void OnDestroy() {
		base.OnDestroy();
	}
}
