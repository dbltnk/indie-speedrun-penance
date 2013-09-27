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
		
		transform.Rotate (new Vector3 (0f, mX, 0f) * rotateSpeed);
		head.transform.Rotate (new Vector3 (-mY, 0f, 0f) * rotateSpeed);
			
		var kX = Input.GetAxis ("Vertical");
		var kY = Input.GetAxis ("Horizontal");

		Vector3 movement = transform.TransformDirection (Vector3.forward) * movementSpeed * kX;
		movement += transform.TransformDirection (Vector3.right) * movementSpeed * kY;

		_controller.Move((movement + gravity) * Time.deltaTime);
	}
}
