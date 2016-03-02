using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class SpawnDestroyPath : MonoBehaviour {

	public Transform start;
	public Transform end;
	public GameObject template;
	public float speed = 1f;
	public float spawnFrequency = 1f;
	public float delay = 0f;

	private float _lastSpawnTime;
	private float _maxDistanceToGoal = .1f;
	private List<GameObject> _gameObjects = new List<GameObject>();
	
	void Start () {
		if (start == null) {
			Debug.LogError("start cannot be null", gameObject);
			return;
		}

		if (end == null) {
			Debug.LogError("end cannot be null", gameObject);
			return;
		}

		if (template == null) {
			Debug.LogError("template cannot be null", gameObject);
			return;
		}

		_lastSpawnTime = Time.time - 1f/spawnFrequency + delay;
	}

	void Update () {
		if (LevelManager.Instance && LevelManager.Instance.Paused ()) {
			return;
		}

		var toRemove = new List<GameObject>();

		foreach (GameObject ob in _gameObjects) {
			ob.transform.position = Vector3.MoveTowards (ob.transform.position, end.position, Time.deltaTime * speed);

			var distanceSquared = (ob.transform.position - end.position).sqrMagnitude;
			
			if (distanceSquared < _maxDistanceToGoal){
				toRemove.Add (ob);
			}
		}

		foreach (GameObject ob in toRemove) {
			_gameObjects.Remove(ob);
			Destroy(ob);
		}

		if ((Time.time - _lastSpawnTime) * spawnFrequency > 1) {
			Spawn();
		}
	}

	void Spawn() {
		var ob = Object.Instantiate (template);
		ob.transform.position = start.position;
		_gameObjects.Add (ob);
		_lastSpawnTime = Time.time;
	}

	public void OnDrawGizmos() {
		if (start == null || end == null)
			return;

		Gizmos.DrawLine(start.position, end.position);
	}
}
