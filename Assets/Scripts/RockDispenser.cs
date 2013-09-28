using UnityEngine;
using System.Collections;

public class RockDispenser : MonoBehaviour {
	public static RockDispenser instance;

	public int worldRockLimit = 500;
	public int gridX;
	public int gridY;

	public Collider _collider;

	void Awake() {
		instance = this;
		_collider = gameObject.FindComponentDeep<Collider> ();
	}

	// Use this for initialization
	void Start () {
		PlaceAtRoot ();
	}

	public void PlaceAtRoot () {
		var rock = Grid.instance.GetRoot ();
		transform.position = rock.transform.position;
	}
	
	// Update is called once per frame
	void Update () {

	}

	public bool HasRocks ()
	{
		return Rock._instances.Count < worldRockLimit;
	}
}
