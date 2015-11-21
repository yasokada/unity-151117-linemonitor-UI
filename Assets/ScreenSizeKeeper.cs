using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/*
 * v0.2 2015/11/21
 *   - change myStandard from [Button] to [Text]
*/

public class ScreenSizeKeeper : MonoBehaviour {

	public Text myStandard; // set component whose width is used as standard

	bool isRunningOnAndroid() {
		return (Application.platform == RuntimePlatform.Android);
	}

	void Start () {
		if (isRunningOnAndroid () == false) {
			return;
		}
		float aspect = (float)Screen.height / (float)Screen.width;
		float buttonRatio = 0.9f; // 90%
		int buttonWidth = (int)myStandard.GetComponent<RectTransform> ().rect.width;
		float newWidth, newHeight;
		
		newWidth = buttonWidth / buttonRatio;
		newHeight = newWidth * aspect;
		
		Screen.SetResolution ((int)newWidth, (int)newHeight, false);
	}   

}
