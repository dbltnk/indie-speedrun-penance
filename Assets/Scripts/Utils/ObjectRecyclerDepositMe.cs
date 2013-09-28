using UnityEngine;
using System.Collections;

public class ObjectRecyclerDepositMe : MonoBehaviour {
	public ObjectRecycler recycler;
	public string tag;

	public void Deposit()
	{
		recycler.depositObject(tag, gameObject);
	}
}
