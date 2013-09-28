using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class AudioManager : MonoBehaviour {
	public static AudioManager instance;
	
	private ObjectRecycler recycler;
	
	public bool useSoundEFX = true;
	public bool useMusic = true;
	
	[Serializable]
	public class ClipInformation
	{
		public string name;
		public AudioClip clip;
		public float volume = 1f;
	}
	
	public List<ClipInformation> soundEfx;
	public List<ClipInformation> musics;

    private AudioSource music;
	
	private List<System.Action> callIfActive = new List<System.Action>();
	
	void OnDestroy()
	{
		instance = null;
		
		if (music != null && music.gameObject != null)GameObject.Destroy(music.gameObject);
	}
	
	void Awake()
	{
		instance = this;
		
        GameObject go = new GameObject("music");
        music = go.AddComponent<AudioSource>();
		GameObject.DontDestroyOnLoad(go);
	}
	
	private Dictionary<string, List<ClipInformation> > namePatternMatchingClipsCache = new Dictionary<string, List<ClipInformation>>();
	
	private List<ClipInformation> findMatchingClipsByName(string namePattern, List<ClipInformation> list, string cachePrefix)
	{
		string key = cachePrefix + namePattern;
		
		if (namePatternMatchingClipsCache.ContainsKey(key) == false)
		{
			List<ClipInformation> matchingClips = new List<ClipInformation>();
		
			foreach(var clipInfo in list)
			{
				if( Regex.IsMatch(clipInfo.name, namePattern, RegexOptions.IgnoreCase) )
				{
					matchingClips.Add(clipInfo);
				}
			}
			
			namePatternMatchingClipsCache[key] = matchingClips;
		}
			
		return namePatternMatchingClipsCache[key];
	}
	
    private ClipInformation findByName(string namePattern, List<ClipInformation> list, string cachePrefix)
	{
		var matchingClips = findMatchingClipsByName(namePattern, list, cachePrefix);

		if (matchingClips.Count > 0)
		{
			var clip = UKRandomHelper.PickRandom(matchingClips);
			//Debug.Log(string.Format("picked clip: {0}", clip.name));
			return clip;
		}
		
		throw new Exception("there is no clip with name: " + namePattern);
	}
	
	public void enableSoundEFX( bool b ) 
	{
		useSoundEFX = b;
	}
	
	public void enableMusic( bool b ) 
	{
		useMusic = b;
		music.volume = useMusic?1:0;
		/*
		if( !useMusic ) {
			music.volume = useMusic?1:0;
		} else {
			music.volume = 1;
		}
		*/
	}
	
	private void tryToCall(System.Action fun)
	{
		if (gameObject.activeSelf)
		{
			if (fun != null)fun();
		}
		else
		{
			callIfActive.Add(fun);
		}
	}
	
	private IEnumerator coSwitchMusic(string namePattern)
	{
		// fade out
		if (music.isPlaying)
		{
			yield return StartCoroutine(coFadeMusic(0.5f, 0f, null));
		}
		
		ClipInformation clipInfo = null;
		try 
		{
	        clipInfo = findByName(namePattern, musics, "music");
		}
		catch
		{
			Debug.LogError(string.Format("could not play music pattern {0}", namePattern));
		}	
		
		// fade in
		if (clipInfo != null)
		{
	        music.Stop();
	        music.clip = clipInfo.clip;
	        music.volume = 0;
	        music.Play();
	        music.loop = true;
			
			yield return StartCoroutine(coFadeMusic(0.5f, clipInfo.volume, null));
		}
	}
	
	public void playMusic(string namePattern)
	{
		tryToCall(() => {
			if( !useMusic ) return;
		
			//Debug.Log(string.Format("play music {0}", namePattern));
			
			StartCoroutine(coSwitchMusic(namePattern));
		});
	}
	
	public void playSound(string namePattern)
	{
		Vector3 position = Vector3.zero;
		if (Camera.main != null)
		{
			position = Camera.main.transform.position;
		}
		
		playSoundAt(position, namePattern, 0f, 10000f);
	}
	
	public void playSoundAt(Vector3 position, string namePattern, float minDist, float maxDist)
	{
		{
			// purge old stuff
			foreach(var o in recycler.enumAll())
			{
				if (o != null)
				{
					float idle = Time.time - o.GetComponent<AudioManagerSfx>().lastUsageTime;
					
					if (idle > 30f)
					{
						GameObject.Destroy(o);
						//Debug.Log("purged sfx");
					}
				}
			}
		}
				
		tryToCall(() => {
			if( !useSoundEFX ) return;
	  
			try 
			{
				UnityEngine.Profiler.BeginSample("AudioManager.findByName");
				ClipInformation clipInfo = findByName(namePattern, soundEfx, "sfx");
				UnityEngine.Profiler.EndSample();
				
				string objName = "sound_" + clipInfo.name;
				
				GameObject o = recycler.getObject(objName, () => {
					GameObject go = new GameObject(objName);
			        go.transform.position = position;
			        
					UnityEngine.Profiler.BeginSample("AudioManager.AddComponent");
			        AudioSource source = go.AddComponent<AudioSource>();
					UnityEngine.Profiler.EndSample();
					
					var c = go.AddComponent<AudioManagerSfx>();
					
					var r = go.AddComponent<ObjectRecyclerDepositMe>();
					r.recycler = recycler;
					r.tag = objName;
			        
					UnityEngine.Profiler.BeginSample("AudioManager.Play");
			        source.clip = clipInfo.clip;
			        source.volume = clipInfo.volume;
			        source.Play();
					UnityEngine.Profiler.EndSample();
			
					GameObject.DontDestroyOnLoad(go);

					return go;
				});
				
				// deposit after time
				o.GetComponent<ObjectRecyclerDepositMe>().Invoke("Deposit", clipInfo.clip.length);

				o.audio.minDistance = minDist;
				o.audio.maxDistance = maxDist;
			}
			catch
			{
				Debug.LogError(string.Format("could not play sound pattern {0}", namePattern));
			}
		});
	}
	
	// Use this for initialization
	void Start () {
		recycler = GetComponent<ObjectRecycler>();
		
		enableMusic (true);
		enableSoundEFX (true);
	}
	
	private IEnumerator coFadeMusic(float fadeTime, float volume, System.Action endCallback)
	{
		if (music == null)yield break;
		float t0 = Time.time;
		float volume0 = music.volume;
		
		while(Time.time - t0 < fadeTime)
		{
			if (music == null)yield break;
			float f = (Time.time - t0) / fadeTime;
			music.volume = Mathf.Lerp(volume0, volume, f);
			yield return null;
		}

		if (music == null)yield break;		
		music.volume = volume;
		
		if (endCallback != null)endCallback();
	}
	
	void Update()
	{
		if (callIfActive != null && callIfActive.Count > 0)
		{
			foreach(System.Action fun in callIfActive)
			{
				if (fun != null)fun();
			}
			
			callIfActive.Clear();
		}
	}
	
	public void fadeOutMusic(float fadeTime)
	{
		tryToCall(() => {
			StartCoroutine(coFadeMusic(fadeTime, 0f, null));
		});
	}
}
