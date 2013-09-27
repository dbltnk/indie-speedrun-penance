using UnityEngine;
using System.Collections;

public class Soul : MonoBehaviour {
	public static Soul instance;

	private CharacterController _controller;

	public Vector3 gravity;
	public float movementSpeed;

	void Awake () {
		instance = this;
		_controller = GetComponent<CharacterController> ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		var v = Input.GetAxis ("Vertical");
		var h = Input.GetAxis ("Horizontal");

		Vector3 movement = transform.TransformDirection (Vector3.forward) * movementSpeed * v;
		movement += transform.TransformDirection (Vector3.right) * movementSpeed * h;

		_controller.Move((movement + gravity) * Time.deltaTime);
	}
}
