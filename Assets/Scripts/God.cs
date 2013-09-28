using UnityEngine;
using System.Collections;

public class God : MonoBehaviour {
	public GameObject startPosition;
	public GameObject endPosition;
	public GameObject limitPosition;
	public GameObject rootPosition;

	// Use this for initialization
	void Start () {
		transform.position = startPosition.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		var s = startPosition.transform.position;
		var e = endPosition.transform.position;
		var l = limitPosition.transform.position;

		var r = rootPosition.transform.position;

		var soulPos = Soul.instance.transform.position;
		var soulPosOnLine = UKMathHelper.ProjectOntoLine(soulPos, r, r-l);

		var maxD = Vector3.Distance (r, l);
		var d = Vector3.Distance (r, soulPosOnLine);

		float f = 1f;

		if (UKMathHelper.IsSameDirection (soulPosOnLine - r, r - l)) {
			// at the start
			f = 1f;
		} else if (UKMathHelper.IsSameDirection (l - r, soulPosOnLine - l)) {
			// at the end
			f = 0f;
		} else {
			f = UKMathHelper.MapIntoRange (d, 0f, maxD, 1f, 0f);
		}

		//Debug.Log (string.Format ("max={0} d={1} f={2}", maxD, d, f));

		transform.position = Vector3.Lerp (e, s, f);
	}
}
