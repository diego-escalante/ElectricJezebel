using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class flashText : MonoBehaviour {

	private Text fader;

	void Start () {
		fader = GetComponent<Text>();
		StartCoroutine(fade());
	}

	IEnumerator fade(){
		float timeTotal = 0.75f;
		float timeElapsed = 0f;
		bool fadeOut = true;

		while(true){
			if(fadeOut) fader.color = Color.Lerp(Color.white, Color.clear, timeElapsed/timeTotal);
			else fader.color = Color.Lerp(Color.clear, Color.white, timeElapsed/timeTotal);

			timeElapsed += Time.deltaTime;
			if(timeElapsed >= timeTotal) {
				timeElapsed = 0;
				fadeOut = !fadeOut;
			}
			yield return null;
		}
	}
}
