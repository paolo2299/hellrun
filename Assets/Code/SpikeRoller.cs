using UnityEngine;
using System.Collections;

public class SpikeRoller : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log (this.gameObject.layer);
		Debug.Log (LayerMask.NameToLayer ("Platforms"));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
