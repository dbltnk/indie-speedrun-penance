using UnityEngine;
using System.Collections;

public class AudioManagerSfx : MonoBehaviour {
	public float lastUsageTime;
	private AudioSource audioSource;
		
	public void Start () 
	{
		lastUsageTime = Time.time;
		audioSource = GetComponent<AudioSource> ();
	}
	
	public void Restart () 
	{
		audioSource.Stop();
		audioSource.Play();
		lastUsageTime = Time.time;
	}
}
