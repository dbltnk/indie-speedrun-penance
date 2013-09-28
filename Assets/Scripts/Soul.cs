using UnityEngine;
using System.Collections;

public class Soul : MonoBehaviour {
	public static Soul instance;

	private CharacterController _controller;

	public GameObject head;

	public Vector3 gravity;
	public float movementSpeed;
	public float rotateSpeed;
	public float jumpSpeed;

	public float stepSoundTime;

	public float minPickupDistance;
	public float maxPickupDistance;

	public bool isCarryingARock;

	public bool isMoving;
	public float lastTimeOnGround;

	public Rock currentBelowRock;

	public Vector3 velocity;

	public bool jumpIfPossible;

	public bool isLongInAir() {
		return Time.time - lastTimeOnGround > Constants.instance.IN_AIR_TIMEOUT;
	}

	void PlayStepSoundIfMoving() {
		if (isMoving && isLongInAir() == false) {
			AudioManager.instance.playSound ("step");
		}
	}

	void Awake () {
		instance = this;
		_controller = GetComponent<CharacterController> ();
	}

	// Use this for initialization
	void Start () {
		InvokeRepeating ("PlayStepSoundIfMoving", stepSoundTime, stepSoundTime);
	}

	void UpdateMarker () {
		if (RockMarker.instance == null)
			return;
		RockMarker.instance.isCarryingARock = isCarryingARock;
	}

	void OnDrawGizmos () {
		if (Camera.main == null || Constants.instance == null)
			return;

		Plane p = new Plane (Vector3.up, Vector3.zero);
		Ray r = Camera.main.ViewportPointToRay (Constants.instance.SCREEN_PICK_RAY_START);
		float enter = 0f;
		if (p.Raycast (r, out enter)) {
			var hit = r.GetPoint (enter);
			//Debug.Log (Vector3.Distance (hit, transform.position));
			Gizmos.color = Color.magenta;
			Gizmos.DrawSphere (hit, 0.2f);
		}
	}

	bool IsInPickupDistance(Vector3 p) {
		float d = Vector3.Distance (transform.position, p);
		return minPickupDistance <= d && d <= maxPickupDistance;
	}

	// Update is called once per frame
	void Update () {
		// jump trigger
		if (Input.GetKeyDown (KeyCode.Space))
			jumpIfPossible = true;
		if (Input.GetKey (KeyCode.Space) == false)
			jumpIfPossible = false;


		bool markerInDist = IsInPickupDistance(RockMarker.instance.transform.position);
		RockMarker.instance.gameObject.SetActive(markerInDist);
		
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

		if (_controller.isGrounded == false) {
			velocity += gravity * Time.deltaTime;
		} else {
			if (jumpIfPossible) {
				// jump
				jumpIfPossible = false;
				velocity = Vector3.up * jumpSpeed;
				AudioManager.instance.playSound ("jump_up");
			} else {
				// on ground
				velocity = Vector3.zero;
			}
		}

		_controller.Move((movement + velocity) * Time.deltaTime);


		// dispenser?
		Ray r = Camera.main.ViewportPointToRay (Constants.instance.SCREEN_PICK_RAY_START);
		if (isCarryingARock == false && Input.GetMouseButtonDown(0)) {
			RaycastHit hit;
			if (RockDispenser.instance.HasRocks() && RockDispenser.instance._collider.Raycast (r, out hit, float.MaxValue)) {
				// near enough?
				var hitPosOnGround = new Vector3(hit.point.x, 0f, hit.point.z);
				if (IsInPickupDistance(hitPosOnGround)) {
					isCarryingARock = true;
					UpdateMarker ();
					AudioManager.instance.playSound ("pick_dispenser");
				}
			}
		}


		// select rock
		if (RockMarker.instance != null) {
			Plane p = new Plane (Vector3.up, Vector3.zero);
			float enter = 0f;
			if (p.Raycast (r, out enter)) {
				var hit = r.GetPoint (enter);
				var cellPos = Grid.instance.FindNearestRockPosition (hit);
				var cellWorldPos = HexGrid<Rock,int>.ViewCellPosition (cellPos.a, cellPos.b);
				RockMarker.instance.transform.position = cellWorldPos;

				// cheats
				if (Constants.instance.CHEATS_ENABLED) {
					if (Input.GetKey (KeyCode.Q)) {
						var cell = Grid.instance.grid.GetCellAt (cellPos.a, cellPos.b);
						if (cell == null) {
							UpdateMarker ();
							var rock = Grid.instance.CreateRockObject (cellPos.a, cellPos.b);
							Grid.instance.AddRockToGrid (rock, cellPos.a, cellPos.b);
							AudioManager.instance.playSound ("place_ground");
						}
					}
					if (Input.GetKey(KeyCode.E)) {
						var cell = Grid.instance.grid.GetCellAt (cellPos.a, cellPos.b);
						if (cell != null) {
							UpdateMarker ();
							var rock = Grid.instance.FindRockAt (cellPos.a, cellPos.b);
							Grid.instance.RemoveRockFromGrid (rock);
							GameObject.Destroy (rock.gameObject);
						}
					}
				}

				// pickup?
				if (Input.GetMouseButtonDown (0)) {
					var canBePicked = Grid.instance.CanRockBePicked (cellPos.a, cellPos.b);
					var hasCell = Grid.instance.grid.HasCellAt (cellPos.a, cellPos.b);

					// near enough?
					if (IsInPickupDistance(cellWorldPos)) {
						if (canBePicked && hasCell && !isCarryingARock) {
							// pickup
							isCarryingARock = true;
							UpdateMarker ();
							var rock = Grid.instance.FindRockAt (cellPos.a, cellPos.b);
							Grid.instance.RemoveRockFromGrid (rock);
							GameObject.Destroy (rock.gameObject);
							AudioManager.instance.playSound ("pick_ground");
						} else if (!hasCell && isCarryingARock) {
							// putdown
							isCarryingARock = false;
							UpdateMarker ();
							var rock = Grid.instance.CreateRockObject (cellPos.a, cellPos.b);
							Grid.instance.AddRockToGrid (rock, cellPos.a, cellPos.b);
							AudioManager.instance.playSound ("place_ground");
						}
					}
				}
			}
		}

		// below rock?
		var belowPos = Grid.instance.FindNearestRockPosition (transform.position);
		var newBelowRock = Grid.instance.FindRockAt (belowPos.a, belowPos.b);
		if (newBelowRock != currentBelowRock) {
			if (newBelowRock != null)
				newBelowRock.SoulEntersRock ();
			if (currentBelowRock != null)
				currentBelowRock.SoulLeavesRock ();
			currentBelowRock = newBelowRock;
		}

		// moving?
		isMoving = movement.sqrMagnitude > 0f;
		if (_controller.isGrounded) {
			if (isLongInAir ())
				AudioManager.instance.playSound ("jump_down");
			lastTimeOnGround = Time.time;
		}

		// dead?
		if (transform.position.y < Constants.instance.DESPAWN_HEIGHT) {
			transform.position = Grid.instance.GetRoot ().transform.position + Vector3.up * _controller.height;
		}
	}
}
