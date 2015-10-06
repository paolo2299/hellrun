using UnityEngine;
using System.Collections;

public class StopWatch {

	private float startTime;
	private float stopTime;
	private bool stopped;

	public float time { 
		get {
			if (stopped) {
				return stopTime - startTime;
			} else {
				return Time.time - startTime;
			}
		} 
	}

	public string formattedTime {
		get {
			var elapsed = time;
			var minutes = (int)(elapsed / 60);
			var seconds = (int)(elapsed % 60);
			var fraction = (int)((elapsed * 100) % 100);
			return string.Format ("{0:00}:{1:00}:{2:00}", minutes, seconds, fraction);
		}
	}

	//constructor
	public StopWatch () {
		Start ();
	}

	public void Start () {
		startTime = Time.time;
		stopped = false;
	}

	public void Stop () {
		stopTime = Time.time;
		stopped = true;
	}
}
