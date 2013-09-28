using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class LoopSound : MonoBehaviour {
	public float volume = 1f;
	public float fadeTime = 1f;
	
	private AudioSource audioSource;
	
	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
		StartCoroutine(coFadeMusic(fadeTime, volume, null));
	}
	
	private IEnumerator coFadeMusic(float fadeTime, float volume, System.Action endCallback)
	{
		float t0 = Time.time;
		float volume0 = audioSource.volume;
		
		while(Time.time - t0 < fadeTime)
		{
			float f = (Time.time - t0) / fadeTime;
			audioSource.volume = Mathf.Lerp(volume0, volume, f);
			yield return null;
		}
		
		audioSource.volume = volume;
		
		if (endCallback != null)endCallback();
	}
	
	public void fadeOut(System.Action endCallback)
	{
		StartCoroutine(coFadeMusic(fadeTime, 0, endCallback));
	}
}
