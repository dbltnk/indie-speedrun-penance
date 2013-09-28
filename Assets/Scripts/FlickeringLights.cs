using UnityEngine;
using System.Collections;

public class FlickeringLights : MonoBehaviour {

	public float minDuration;
	public float maxDuration;
    public Color color0 = Color.red;
    public Color color1 = Color.yellow;
	float lastChangeTime = 0f;
	
    void Update() {
		float duration;
		duration = Random.Range(minDuration, maxDuration);
		//Debug.Log(duration);
		if (Time.time - lastChangeTime > duration) {
        	float t = Mathf.PingPong(Time.time, duration) / duration;
        	light.color = Color.Lerp(color0, color1, t);
			lastChangeTime = Time.time;
		}
    }
}
