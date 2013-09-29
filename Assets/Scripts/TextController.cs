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

	IEnumerator CoShowLinePerPage(string text, char seperator, float timeDisplayed, float timeHidden) {
		var lines = text.Split (new char[]{seperator});

		foreach (var line in lines) {
			yield return StartCoroutine(CoShowPage(line, timeDisplayed));
			yield return new WaitForSeconds (timeHidden);
		}
	}

	IEnumerator CoShowText () {
		yield return new WaitForSeconds (1f);

		while (true) {
			yield return StartCoroutine(CoShowLinePerPage(@"
Indie Speed Run 2013
Penance & Tumbleweed
Team c4 @ Munich

Martin Dechant
Sebastian Dorda
Alexander Zacherl

", '\n', 3f, 1f));
			
			yield return new WaitForSeconds (1f);
			
			yield return StartCoroutine(CoShowPage(@"
This game was developed
as part of Indie Speed Run
2013 (www.indiespeedrun.com).
", 3f));

			yield return new WaitForSeconds (3600f);
		}
	}
}
