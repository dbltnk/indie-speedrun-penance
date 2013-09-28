using UnityEngine;
using System.Collections;

public class AudioManagerSfx : MonoBehaviour {
	public float lastUsageTime;
		
	public void Start () 
	{
		lastUsageTime = Time.time;
	}
	
	public void Restart () 
	{
		var source = GetComponent<AudioSource>();
		source.Stop();
		source.Play();
		lastUsageTime = Time.time;
	}
}
