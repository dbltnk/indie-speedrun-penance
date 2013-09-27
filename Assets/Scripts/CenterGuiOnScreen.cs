using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class CenterGuiOnScreen : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}

	void Update () {
		CenterOnScreen();
	}

	// Update is called once per frame
	void CenterOnScreen () {
		transform.position = Vector3.zero;
		transform.localScale = Vector3.zero;

		float w = guiTexture.texture.width;
		float h = guiTexture.texture.height;

		guiTexture.pixelInset = new Rect ((Screen.width - w) / 2f, (Screen.height - h) / 2f, w, h);
	}
}
