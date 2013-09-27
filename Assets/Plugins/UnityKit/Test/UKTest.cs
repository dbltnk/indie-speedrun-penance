using UnityEngine;
using System.Collections;

public class UKTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		TestJobManager ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}



	#region UKJobManager

	public class TestJob : UKJobManager.Job {
		public override void OnStart ()
		{
			Debug.Log ("job started");
		}

		public override void OnFinish ()
		{
			Debug.Log ("job finished");
		}

		public override IEnumerator Run ()
		{
			int runsLeft = 10;
			Debug.Log ("job run started");
			while (!ShouldTerminate && runsLeft >= 0)
			{
				--runsLeft;
				Debug.Log(string.Format("job state {0}", _state));
				yield return WaitForSeconds (1f);
			}
			float t = Time.time;
			yield return WaitForCondition (() => Time.time - t > 3f);
			Debug.Log ("job run finished");
		}
	}

	void TestJobManager ()
	{
		UKJobManager.Run (new TestJob ());
	}

	#endregion
}
