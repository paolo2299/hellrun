using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class RandomSpawner : MonoBehaviour {

	public Transform spawnPoint;
	public GameObject template;
	public float spawnFrequency = 10f;
	public float spawnProbability = 0.05f;
	public float delay = 0f;
	
	private float _lastSpawnAttempt;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time - _lastSpawnAttempt > 1f / spawnFrequency) {
			MaybeSpawn();
		}
	}

	void MaybeSpawn() {
		var p = Random.value;
		if (p < spawnProbability) {
			Spawn();
		}
		_lastSpawnAttempt = Time.time;
	}

	void Spawn() {
		var ob = Object.Instantiate (template);
		ob.tag = "Destroyable";
		ob.transform.position = spawnPoint.position;
	}
}
