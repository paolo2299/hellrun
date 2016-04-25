using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class RandomSpawner : MonoBehaviour {

	public Transform spawnPoint;
	public GameObject template;

	public float spawnFrequency = 10f;
	public float spawnProbability = 0.05f;

	private static int spawnArraySize = 10;
	private float startTime;
	private float spawnTimeOffset = 0f;
	private float[] spawnTimes = new float[spawnArraySize];
	private int spawnTimesIndex = 0;

	// Use this for initialization
	void Start () {
		startTime = Time.fixedTime;

		Random.seed = 42;
		var index = 0;
		var time = 0f;
		while (index < spawnArraySize) {
			var p = Random.value;
			if (p < spawnProbability) {
				spawnTimes[index] = time;
				Debug.Log (time);
				index += 1;
			}
			time += 1f / spawnFrequency;
		}
	}

	void FixedUpdate () {
		if (ShouldSpawn ()) {
			Spawn ();
		}
	}

	private bool ShouldSpawn() {
		var result = false;
		while (spawnTimes[spawnTimesIndex] + spawnTimeOffset < Time.fixedTime) {
			result = true;
			IncrementIndex();
		}
		return result;
	}

	private void IncrementIndex() {
		if (spawnTimesIndex == spawnArraySize - 1) {
			spawnTimesIndex = 0;
			spawnTimeOffset = Time.fixedTime;
		} else {
			spawnTimesIndex += 1;
			Debug.Log (spawnTimesIndex);
		}
	}

	private void Spawn() {
		var ob = Object.Instantiate (template);
		ob.tag = "Destroyable";
		ob.transform.position = spawnPoint.position;
	}
}
