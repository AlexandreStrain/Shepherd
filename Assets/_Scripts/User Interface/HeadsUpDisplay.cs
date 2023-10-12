using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadsUpDisplay : MonoBehaviour {

	public static HealthPanel _healthPanel;

	//public void Init (CharacterBody player) {
		//UserInterfaceController._HUD = this;
	//	_healthPanel.Init (player);
	//}
	
	public void Tick (float delta, StateController player) {
		_healthPanel.Tick (delta, player);
	}
}
