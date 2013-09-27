using UnityEngine;
using System.Collections;

public class Rock : UKListedBehaviour<Rock> {
	public int gridX;
	public int gridY;

	public enum Mode {
		IDLE = 0,
		FALLING = 1,
	};

	public Mode mode = Mode.IDLE;

	public float fallDownIn = 0f;
	public float fallSpeed = 0f;

	// Use this for initialization
	void Start () {
		fallDownIn = Random.Range (2f, 10f);
		fallSpeed = Random.Range (2f, 5f);
	}

	public void BreakApart () {
		mode = Mode.FALLING;
	}

	// Update is called once per frame
	void Update () {
		if (mode == Mode.IDLE) fallDownIn -= Time.deltaTime;

		if (fallDownIn < 0f && mode == Mode.IDLE) {
			// BreakApart ();
		} 

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
		Gizmos.DrawSphere (HexGrid.ViewCellPosition (gridX, gridY), 0.1f);
	}
}
