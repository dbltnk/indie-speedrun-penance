using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rock : UKListedBehaviour<Rock> {
	public HexGrid<Rock,int>.HexCell<Rock> cell;

	public bool isASoulOnThis;

	public float moveDownSpeed;
	public float moveUpSpeed;
	public float moveDownDepth;

	public float fallingRotationSpeed;
	public Vector3 fallingRotation;

	public GameObject rotator;

	public Vector3 velocity;

	public int c1;
	public int c2;

	public List<Mesh> meshVariants;
	public MeshFilter mainMeshfilter;

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
		CRACLE = 2,
	};

	public List<Rock> neighbours = new List<Rock>();

	public Mode mode = Mode.IDLE;

	public Vector3 gravitation;

	public float creationTime;

	public int Connectivitiy1 {
		get {
			return (neighbours != null ? neighbours.Count : 0) * 100;
		}
	}

	public int Connectivitiy2 {
		get {
			int c = Connectivitiy1;
			int cn = 0;
			if (neighbours != null) foreach(var n in neighbours) cn += n.Connectivitiy1;
			return (c + (cn / 6)) / 2;
		}
	}

	public float Age {
		get {
			return Time.time - creationTime;
		}
	}

	// Use this for initialization
	void Start () {
		InvokeRepeating ("CheckStillConnected", 0.25f, 0.25f);
		
		int randomNumber;
		randomNumber = Random.Range(1,6);
		float yRotation = 60f;
		yRotation = randomNumber * yRotation;
		Quaternion rotation = Quaternion.Euler(new Vector3(0, yRotation, 0));
		rotator.transform.rotation = rotation;

		creationTime = Time.time;

		mainMeshfilter.mesh = UKRandomHelper.PickRandom (meshVariants);
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

	void FallDown () {
		if (mode == Mode.FALLING)
			return;

		AudioManager.instance.playSoundAt (transform.position, "falldown", 10f, 20f);

		fallingRotation = new Vector3 (Random.Range (-fallingRotationSpeed, fallingRotationSpeed),
		                              Random.Range (-fallingRotationSpeed, fallingRotationSpeed),
		                              Random.Range (-fallingRotationSpeed, fallingRotationSpeed));

		if (Grid.instance.IsRoot(this) == false) {
			mode = Mode.FALLING;
			Grid.instance.RemoveRockFromGrid (this);
		}
	}

	public void BreakApart () {
		if (mode != Mode.IDLE)
			return;

		mode = Mode.CRACLE;
		Invoke ("FallDown", Constants.instance.CRACLE_TIME);
		AudioManager.instance.playSoundAt (transform.position, "cracle", 5f, 10f);
		var randRot = new Vector3 (Random.Range (-15f, 15f), Random.Range (-15f, 15f), Random.Range (-10f, 10f));
		Go.to (transform, Constants.instance.CRACLE_TIME, new TweenConfig ()
		       .rotation (randRot, true));
	}

	void CheckStillConnected () {
		var centerRock = Grid.instance.FindRockAt(Grid.instance.rootGridX,Grid.instance.rootGridY);
		
		if (mode == Mode.IDLE && groupNr > 0 && centerRock != null && groupNr != centerRock.groupNr) {
			BreakApart ();
		}
	}

	// Update is called once per frame
	void Update () {
		if (mode == Mode.FALLING) {
			velocity += gravitation * Time.deltaTime;
			transform.Translate (velocity * Time.deltaTime, Space.World);

			if (transform.position.y < Constants.instance.DESPAWN_HEIGHT) {
				GameObject.Destroy (gameObject);
			}

			transform.Rotate (fallingRotation * Time.deltaTime);
		} else if (mode == Mode.IDLE) {
			// push down if someone is on this
			var pIdle = GetIdlePosition ();

			if (isASoulOnThis) {
				var pDown = pIdle + Vector3.down * moveDownDepth;
				transform.position = Vector3.MoveTowards (transform.position, pDown, moveDownSpeed * Time.deltaTime);
			} else {
				transform.position = Vector3.MoveTowards (transform.position, pIdle, moveUpSpeed * Time.deltaTime);
			}
		}

		c1 = Connectivitiy1;
		c2 = Connectivitiy2;
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

		// connectivity
		Gizmos.color = Color.yellow;
		float c = UKMathHelper.MapIntoRange(Connectivitiy2, 0f, 600f, 0f, 10f);
		Gizmos.DrawLine(transform.position, transform.position + Vector3.up * c);
	}

	public override void OnDestroy() {
		if (cell != null)
			Grid.instance.RemoveRockFromGrid (this);
		base.OnDestroy();
	}

	
	public Vector3 GetIdlePosition(){
		if (cell != null) return cell.ViewPosition();
		else return transform.position;
	}
	
	public void SoulEntersRock () {
		//Debug.Log("enter", gameObject);
		// push down
		//Go.to (transform, 1f, new TweenConfig ().position (Vector3.down * 0.15f, true));
		isASoulOnThis = true;
	}
	
	public void SoulLeavesRock () {
		//Debug.Log("leave", gameObject);
		// move back to normal
		//var pos = GetIdlePosition();
		//Go.to (transform, 3f, new TweenConfig ().position (pos, false));
		isASoulOnThis = false;
	}
}
