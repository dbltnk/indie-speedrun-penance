using UnityEngine;
using System.Collections;

public class TimeWarp : MonoBehaviour {
	public static TimeWarp instance;

	public float pitch;

	public float warpDepthStart;
	public float warpDepthEnd;

	private AudioSource audioSource;

	public void PlaySound() {
		audioSource.Stop ();
		audioSource.Play ();
	}

	void Awake () {
		instance = this;
		audioSource = GetComponent<AudioSource> ();
	}

	// Use this for initialization
	void Start () {
	
	}

	float CalculateTimeFactor(float f) {
		// 0.99942+0.211799 x-1.2089 x^2
		return 0.99942f + 0.211799f * f - 1.2089f * f * f;
	}

	// Update is called once per frame
	void Update () {
		if (Soul.instance == null)
			return;

		var h = Soul.instance.transform.position.y;
		var t = UKMathHelper.MapIntoRange (h, warpDepthEnd, warpDepthStart, 1f, 0f);
		var f = CalculateTimeFactor (t);
//		Debug.Log (string.Format ("{0} / {1} / {2}", h, t, f));
		Time.timeScale = f;

		pitch = f - 1f;
		audioSource.pitch = pitch;
	}
}
