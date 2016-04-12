using UnityEngine;
using System.Collections;

public class SemiCollidableSwitch : MonoBehaviour {

	public float semiProbability = 0.5f;

	public void OnTriggerEnter2D(Collider2D other)
	{
		var layer = LayerMask.LayerToName(other.gameObject.layer);
		if (layer == "SemiCollidablePasser" || layer == "SemiCollidableNonPasser") {
			var p = Random.value;
			if (p < semiProbability) {
				other.gameObject.layer = LayerMask.NameToLayer("SemiCollidablePasser");
			} else {
				other.gameObject.layer = LayerMask.NameToLayer("SemiCollidableNonPasser");
			}
		}
	}
}
