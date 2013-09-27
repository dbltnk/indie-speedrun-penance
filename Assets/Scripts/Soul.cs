using UnityEngine;
using System.Collections;

public class Soul : MonoBehaviour {
	public static Soul instance;

	private CharacterController _controller;

	public GameObject head;

	public Vector3 gravity;
	public float movementSpeed;
	public float rotateSpeed;

	void Awake () {
		instance = this;
		_controller = GetComponent<CharacterController> ();
	}

	// Use this for initialization
	void Start () {
	
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
				var cellPos = HexGrid.CellPositionFromView (hit);
				RockMarker.instance.transform.position = HexGrid.ViewCellPosition (cellPos.a, cellPos.b);
			}
		}
	}
}
