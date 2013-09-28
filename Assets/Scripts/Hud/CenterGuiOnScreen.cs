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

		float x = Screen.width * Constants.instance.SCREEN_PICK_RAY_START.x;
		float y = Screen.height * (1f - Constants.instance.SCREEN_PICK_RAY_START.y);

		guiTexture.pixelInset = new Rect (x - w / 2f, y - h / 2f, w, h);
	}
}
