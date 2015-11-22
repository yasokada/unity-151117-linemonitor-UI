using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PanelDisplayControl : MonoBehaviour {

	public GameObject PageSelect;
	public GameObject PanelGroup;
	
	void Start() {
		int idx = 0;
		foreach (Transform child in PanelGroup.transform) {
			bool isOn = getIsOn (/* idx_st1= */ idx + 1);
			DisplayPanel (/* idx_st1= */ idx + 1, isOn);
			
			idx++;
		}
	}
	
	bool getIsOn(int idx_st1) {
		Toggle selected = null;
		string name;
		foreach (Transform child in PageSelect.transform) {
			name = child.gameObject.name;
			if (name.Contains(idx_st1.ToString())) { // e.g. "SelP1", "SelP2"...
				selected = child.gameObject.GetComponent(typeof(Toggle)) as Toggle;
				break;
			}
		}
		return selected.isOn;
	}
	
	void DisplayPanel(int idx_st1, bool doShow)
	{
		GameObject [] panels = GameObject.FindGameObjectsWithTag ("TagP" + idx_st1.ToString ());
		
		if (panels == null) {
			return;
		}
		
		foreach (GameObject panel in panels) {
			RectTransform rect = panel.GetComponent (typeof(RectTransform)) as RectTransform;
			Vector2 size = rect.localScale;
			if (doShow) {
				size.x = 1.0f;
				size.y = 1.0f;
			} else {
				size.x = 0;
				size.y = 0;
			}
			rect.localScale = size;
		}
	}
	
	public void ToggleValueChanged(int idx_st1) {
		bool isOn = getIsOn (idx_st1);
		DisplayPanel (idx_st1, isOn);
		Debug.Log (isOn.ToString ());
	}
}
