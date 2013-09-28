using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UKListedBehaviour<T> : MonoBehaviour
	where T : Component
{
	public static List<UKListedBehaviour<T>> _instances = new List<UKListedBehaviour<T>>();

	public static IEnumerable<T> Instances {
		get {
			foreach(var it in _instances) if (it != null) yield return it.GetComponent<T>();
		}
	}

	public virtual void Awake () {
		_instances.Add (this);
	}

	public virtual void OnDestroy () {
		_instances.Remove (this);
	}
}