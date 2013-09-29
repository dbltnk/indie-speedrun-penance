using UnityEngine;
using System.Collections;

public class God : MonoBehaviour {
	public GameObject startPosition;
	public GameObject endPosition;
	public GameObject limitPosition;
	public GameObject rootPosition;

	public float moveSpeed;

	// Use this for initialization
	void Start () {
		transform.position = startPosition.transform.position;
		//transform.position = endPosition.transform.position;
	}

	void Update () {
		AdjustPosition ();
	}

	// Update is called once per frame
	void AdjustPosition () {
		var s = startPosition.transform.position;
		var e = endPosition.transform.position;
		var l = limitPosition.transform.position;
		var r = rootPosition.transform.position;

		//var soulPos = Soul.instance.transform.position;
		//var soulPosOnLine = UKMathHelper.ProjectOntoLine(soulPos, r, r-l);

		var maxD = Constants.instance.STONES_NEEDED_AS_PENANCE;
		var g = Grid.instance;
		var d = Soul.instance.rocksPlaced;

		float f = UKMathHelper.MapIntoRange (d, 0f, maxD, 1f, 0f); 

		// Debug.Log (string.Format ("max={0} d={1} f={2}", maxD, d, f));
		
		float maxDistance = Vector3.Distance(s, e);
		float currentDistance = Vector3.Distance(transform.position, e);
		float adjustedSpeed = Mathf.Max(1, moveSpeed * (currentDistance / maxDistance));
		//Debug.Log(adjustedSpeed);
		//Debug.Log(moveSpeed);
		//Debug.Log(maxDistance);
		//Debug.Log(currentDistance);
		
		var p = Vector3.Lerp (e, s, f);
		transform.position = Vector3.MoveTowards (transform.position, p, Time.deltaTime * adjustedSpeed);
	}
}
