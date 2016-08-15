using UnityEngine;
using System.Collections;

public class LevelFinish : MonoBehaviour {

	public KingKong kingKong;

	public void OnTriggerEnter2D(Collider2D other)
	{
		var player = other.gameObject.GetComponent<Player> ();
		if (player != null) {
			if (kingKong == null) {
				LevelManagerSingleton.Instance.LevelComplete();
			} else {
				KillKingKong();
			}
		}
	}

	private void KillKingKong() {
		StartCoroutine (KillKingKongCo());
	}
	
	private IEnumerator KillKingKongCo() {
		kingKong.Die ();
		yield return new WaitForSeconds (1f);
		LevelManagerSingleton.Instance.LevelComplete();
	}
}
