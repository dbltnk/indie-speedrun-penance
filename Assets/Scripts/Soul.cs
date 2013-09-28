using UnityEngine;
using System.Collections;

public class Soul : MonoBehaviour {
	public static Soul instance;

	private CharacterController _controller;

	public GameObject head;

	public Vector3 gravity;
	public float movementSpeed;
	public float rotateSpeed;

	public bool isCarryingARock;

	void Awake () {
		instance = this;
		_controller = GetComponent<CharacterController> ();
	}

	// Use this for initialization
	void Start () {
	
	}

	void OnDrawGizmos () {
		Plane p = new Plane (Vector3.up, Vector3.zero);
		Ray r = Camera.main.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0f));
		float enter = 0f;
		if (p.Raycast (r, out enter)) {
			var hit = r.GetPoint (enter);
			Gizmos.color = Color.magenta;
			Gizmos.DrawSphere (hit, 0.2f);
		}
	}

	// Update is called once per frame
	void Update () {
		var mX = Input.GetAxis ("Mouse X");
		var mY = Input.GetAxis ("Mouse Y");

		// look around
		transform.Rotate (new Vector3 (0f, mX, 0f) * rotateSpeed);
		head.transform.Rotate (new Vector3 (-mY, 0f, 0f) * rotateSpeed);
			
		var kX = Input.GetAxis ("Vertical");
		var kY = Input.GetAxis ("Horizontal");

		// move wasd
		Vector3 movement = transform.TransformDirection (Vector3.forward) * movementSpeed * kX;
		movement += transform.TransformDirection (Vector3.right) * movementSpeed * kY;

		_controller.Move((movement + gravity) * Time.deltaTime);

		// select rock
		if (RockMarker.instance != null) {
			Plane p = new Plane (Vector3.up, Vector3.zero);
			Ray r = Camera.main.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0f));
			float enter = 0f;
			if (p.Raycast (r, out enter)) {
				var hit = r.GetPoint (enter);
				var cellPos = Grid.instance.FindNearestRockPosition (hit);
				RockMarker.instance.transform.position = HexGrid<Rock,int>.ViewCellPosition (cellPos.a, cellPos.b);

				// pickup?
				if (Input.GetMouseButtonDown (0)) {
					var canBePicked = Grid.instance.CanRockBePicked (cellPos.a, cellPos.b);
					var hasCell = Grid.instance.grid.HasCellAt (cellPos.a, cellPos.b);

					if (canBePicked && hasCell && !isCarryingARock) {
						// pickup
						isCarryingARock = true;
						Grid.instance.RemoveCellAt (cellPos.a, cellPos.b);
					} else if(!hasCell && isCarryingARock) {
						// putdown
						isCarryingARock = false;
						Grid.instance.AddCellAt (cellPos.a, cellPos.b);
					}
				}
			}
		}
	}
}
