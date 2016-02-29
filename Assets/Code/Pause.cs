using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Pause : MonoBehaviour {

	public Text pauseText;

	void ShowLoading () {
		pauseText.text = "Loading...";
	}
}
