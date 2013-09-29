using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Soul : MonoBehaviour {
	public static Soul instance;

	private CharacterController _controller;

	public GameObject head;

	public Vector3 gravity;
	public float movementSpeed;
	public float carrySpeed;
	
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

	public Stack<Vector3> moveBackPositions = new Stack<Vector3>();
	public Vector3 lastPosOnGround;
	
	public int rocksPlaced = 0;
	public int lastRockX = 0;
	public int lastRockY = 0;
	
	public float lastPickUpTime = 0f;
	
	public bool useController = false;

	public float headUpDownAngle = 0f;

	public enum Mode {
		NORMAL = 0,
		FALLDOWN_AND_DIE = 1,
	};

	public Mode mode;

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
		InvokeRepeating ("TrackBackPositions", 0.1f, 0.1f);
		lastPickUpTime = Time.time;		
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

		// trail
		Gizmos.color = Color.yellow;
		Vector3 lastPos = transform.position;
		foreach (var pos in moveBackPositions) {
			Gizmos.DrawLine (lastPos, pos);
			lastPos = pos;
		}
		Gizmos.DrawLine (lastPos, lastPosOnGround);
	}

	bool IsInPickupDistance(Vector3 p) {
		float d = Vector3.Distance (transform.position, p);
		return minPickupDistance <= d && d <= maxPickupDistance;
	}

	bool IsInPickupDistanceIgnoringMinDist(Vector3 p) {
		float d = Vector3.Distance (transform.position, p);
		return d <= maxPickupDistance;
	}

	IEnumerator CoMoveTowards(Vector3 p, float speed, float distThreshold) {
		while (Vector3.Distance(p, transform.position) > distThreshold) {
			// move towards next point
			transform.position = Vector3.MoveTowards (transform.position, p, Time.deltaTime * speed);
			yield return null;
		}
	}

	IEnumerator CoRewind ()
	{
		mode = Mode.FALLDOWN_AND_DIE;

		var upDisplacement = Vector3.up * 0.1f;
		var distThreshold = 0.1f;

		// rewind
		while (moveBackPositions.Count > 0) {
			// next pos
			var p = moveBackPositions.Pop ();
			yield return StartCoroutine (CoMoveTowards (p + upDisplacement, Constants.i.REWIND_SPEED, distThreshold));
		}

		// put on ground
		yield return StartCoroutine (CoMoveTowards (lastPosOnGround + upDisplacement, Constants.i.REWIND_SPEED, distThreshold));

		// still in the air?
		if (Physics.Raycast (new Ray (transform.position, Vector3.down), _controller.height) == false) {
			// ok back to root
			var rootPos = Grid.instance.GetRoot ().transform.position;
			yield return StartCoroutine (CoMoveTowards (rootPos + Vector3.up * _controller.height, Constants.i.REWIND_SPEED, distThreshold));
		}

		mode = Mode.NORMAL;

		yield return null;
	}

	// Update is called once per frame
	void Update () {
		
		// look around		
		float mX;
		float mY;
		
		if (Input.GetKeyDown (KeyCode.F1))
			useController = !useController;
			
		if (useController == false) {
			mX = Input.GetAxis ("Mouse X");
			mY = Input.GetAxis ("Mouse Y");
		}
		else {
			mX = Input.GetAxis ("Gamepad Rotation X");
			mY = Input.GetAxis ("Gamepad Rotation Y");
		}					
		
		transform.Rotate (new Vector3 (0f, mX, 0f) * rotateSpeed);

		headUpDownAngle += (-1f) * mY * rotateSpeed;
		headUpDownAngle = Mathf.Clamp (headUpDownAngle, -85f, 85f);

		head.transform.localRotation = Quaternion.Euler (headUpDownAngle,0f,0f);

		// rewind?
		if (mode == Mode.NORMAL && transform.position.y < Constants.i.REWIND_HEIGHT) {
			StartCoroutine (CoRewind());
			return;
		}
		// stop there if we are in rewinding mode
		if (mode == Mode.FALLDOWN_AND_DIE)
			return;

		// jump trigger

		if (useController == false) {
			if (Input.GetKeyDown (KeyCode.Space))
			jumpIfPossible = true;
			if (Input.GetKey (KeyCode.Space) == false)
			jumpIfPossible = false;
		}
		else {
			if (Input.GetAxis ("JoystickJump") > 0.3)
			jumpIfPossible = true;
			if (Input.GetAxis ("JoystickJump") <= 0.3)
			jumpIfPossible = false;
		}			

		bool markerInDist = IsInPickupDistance(RockMarker.instance.transform.position);
		RockMarker.instance.gameObject.SetActive(markerInDist);
		
		// move wasd
		var kX = Input.GetAxis ("Vertical");
		var kY = Input.GetAxis ("Horizontal");
		
		Vector3 movement = new Vector3();
		
		if (isCarryingARock) {
			movement = transform.TransformDirection (Vector3.forward) * carrySpeed * kX;
			movement += transform.TransformDirection (Vector3.right) * carrySpeed * kY;
		}
		else {
			movement = transform.TransformDirection (Vector3.forward) * movementSpeed * kX;
			movement += transform.TransformDirection (Vector3.right) * movementSpeed * kY;	
		}

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
		if (isCarryingARock == false && (Input.GetMouseButtonDown(0) || Input.GetAxis ("JoystickJump") < -0.3)) {
			RaycastHit hit;
			if (RockDispenser.instance.HasRocks() && RockDispenser.instance._collider.Raycast (r, out hit, float.MaxValue)) {
				// near enough?
				var hitPosOnGround = new Vector3(hit.point.x, 0f, hit.point.z);
				if (IsInPickupDistanceIgnoringMinDist(hitPosOnGround)) {
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
					if (Input.GetKey (KeyCode.F)) {
						gravity = Vector3.zero;
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
				
				float dt = Time.time - lastPickUpTime;
				
				//Debug.Log (dt);
				//Debug.Log (Time.deltaTime);
				//Debug.Log (lastPickUpTime);
				
				// pickup?
				if ((Input.GetMouseButtonDown(0) || (Input.GetAxis ("JoystickJump") < -0.3)) && dt > 0.5f) {
					lastPickUpTime = Time.time;
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
							lastRockX = cellPos.a;
							lastRockY = cellPos.b;		
						} else if (!hasCell && isCarryingARock) {
							// putdown
							isCarryingARock = false;
							UpdateMarker ();
							var rock = Grid.instance.CreateRockObject (cellPos.a, cellPos.b);
							Grid.instance.AddRockToGrid (rock, cellPos.a, cellPos.b);
							AudioManager.instance.playSound ("place_ground");
							int deltaX = Mathf.Abs(lastRockX - cellPos.a);
							int deltaY = Mathf.Abs(lastRockY - cellPos.b);
							if (deltaX >= Constants.instance.MIN_CARRY_DIST || deltaY >= Constants.instance.MIN_CARRY_DIST) {
								rocksPlaced ++;	
							}								
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

		// reset path if on ground
		if (_controller.isGrounded) {
			lastPosOnGround = transform.position;
			moveBackPositions.Clear ();
		}
	}

	void TrackBackPositions() {
		if (_controller.isGrounded == false && mode == Mode.NORMAL)
			moveBackPositions.Push (transform.position);
	}
}
