using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextController : MonoBehaviour {
	public TextMesh textMesh;
	public Animation textAnimation;

	public AnimationClip animIn;
	public AnimationClip animOut;

	public Vector3 idleMovement;

	// Use this for initialization
	void Start () {
		StartCoroutine(CoShowText ());
	}

	IEnumerator CoShowPage (string text, float time) {
		textMesh.text = text;

		textAnimation.clip = animIn;
		textAnimation.Play ();
		yield return new WaitForSeconds (textAnimation.clip.length);

		var p = textMesh.transform.position;
		var t = Time.time;
		while (Time.time - t < time) {
			textMesh.transform.Translate (idleMovement * Time.deltaTime);
			yield return null;
		}
		textMesh.transform.position = p;

		textAnimation.clip = animOut;
		textAnimation.Play ();
		yield return new WaitForSeconds (textAnimation.clip.length);
	}

	IEnumerator CoShowText () {
		while (true) {

			yield return StartCoroutine(CoShowPage("Welcome", 1f));

			yield return new WaitForSeconds (3f);

			yield return StartCoroutine(CoShowPage("to this fancy\ngame", 1f));

			yield return new WaitForSeconds (3f);
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
