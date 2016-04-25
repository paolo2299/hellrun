using UnityEngine;
using System.Collections;

public class SemiCollidableSwitch : MonoBehaviour {

	public float semiProbability = 0.5f;
	
	private string[] switchValues = new string[100];
	private int switchValueIndex = 0;

	void Start () {
		Random.seed = 42;
		for (int i = 0; i < 100; i++) {
			var p = Random.value;
			if (p < semiProbability) {
				switchValues[i] = "SemiCollidablePasser";
			} else {
				switchValues[i] = "SemiCollidableNonPasser";
			}
		}
	}

	private string getNextSwitchValue() {
		var value = switchValues [switchValueIndex];
		switchValueIndex = (switchValueIndex + 1) % 100;
		return value;
	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		var layer = LayerMask.LayerToName(other.gameObject.layer);
		if (layer == "SemiCollidablePasser" || layer == "SemiCollidableNonPasser") {
			other.gameObject.layer = LayerMask.NameToLayer(getNextSwitchValue());
		}
	}
}
