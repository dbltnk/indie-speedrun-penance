using UnityEngine;
using System.Collections;

public class RockDispenser : MonoBehaviour {
	public static RockDispenser instance;

	public int worldRockLimit;
	public float disapearTreshold;
	public int gridX;
	public int gridY;

	public Material _material;
	public Collider _collider;

	void Awake() {
		instance = this;
		_collider = gameObject.FindComponentDeep<Collider> ();
		_material = gameObject.FindComponentDeep<Renderer> ().material;
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
		var f = UKMathHelper.MapIntoRange (Rock._instances.Count, 0f, worldRockLimit, 1f, disapearTreshold);
		_material.SetFloat ("_Amount", f);
	}

	public bool HasRocks ()
	{
		return Rock._instances.Count < worldRockLimit;
	}
}
