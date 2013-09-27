using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UKListedBehaviour<T> : MonoBehaviour
	where T : Component
{
	public static List<UKListedBehaviour<T>> instances = new List<UKListedBehaviour<T>>();

	public virtual void Awake () {
		instances.Add (this);
	}

	public virtual void OnDestroy () {
		instances.Remove (this);
	}
}