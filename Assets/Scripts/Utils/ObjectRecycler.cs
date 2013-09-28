using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ObjectRecycler : MonoBehaviour {
	public static ObjectRecycler instance;
	
	private Dictionary<string, Queue<GameObject>> cachedObjects = new Dictionary<string, Queue<GameObject>>();
	
	void Awake()
	{
		instance = this;
	}
	
	private Queue<GameObject> getQueueByTag(string tag)
	{
		if (cachedObjects.ContainsKey(tag) == false)
		{
			cachedObjects[tag] = new Queue<GameObject>();
		}
		
		return cachedObjects[tag];
	}
	
	private GameObject popObjectFromQueueByTag(string tag)
	{
		var queue = getQueueByTag(tag);
		
		while(queue.Count > 0)
		{
			GameObject o = queue.Dequeue();
			if (o != null)return o;
		}
		
		return null;
	}
	
	public GameObject getObject(string tag, System.Func<GameObject> factory)
	{
		GameObject o = popObjectFromQueueByTag(tag);
		if (o != null)
		{
			// recycle
			o.SendMessage("Restart", SendMessageOptions.DontRequireReceiver);
			o.SetActive(true);
		}
		else
		{
			// create new
			o = factory();			
		}
		
		return o;
	}
	
	public GameObject getObjectAt(string tag, Vector3 pos, Quaternion rot, System.Func<GameObject> factory)
	{
		GameObject o = getObject(tag, factory);
		o.transform.position = pos;
		o.transform.rotation = rot;
		return o;
	}
	
	public void depositObject(string tag, GameObject o)
	{
		o.SetActive(false);
		getQueueByTag(tag).Enqueue(o);
	}
	
	public IEnumerable<GameObject> enumAllByTag(string tag)
	{
		foreach(var o in getQueueByTag(tag))
		{
			yield return o;
		}
	}
	
	public IEnumerable<GameObject> enumAll()
	{
		foreach(var t in cachedObjects.Keys)
		foreach(var o in getQueueByTag(t))
		{
			yield return o;
		}
	}
}
