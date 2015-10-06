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
			return StopWatch.Format(time);
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

	public static string Format(float timeInSecs) {
		var minutes = (int)(timeInSecs / 60);
		var seconds = (int)(timeInSecs % 60);
		var fraction = (int)((timeInSecs * 100) % 100);
		if (minutes > 0) {
			return string.Format ("{0:00}:{1:00}:{2:00}", minutes, seconds, fraction);
		} else {
			return string.Format ("{0:00}:{1:00}", seconds, fraction);
		}
	}
}
