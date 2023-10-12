using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerLoadScene : MonoBehaviour {

	public string _loadLevel;
	public string _unloadLevel;

	void OnTriggerEnter(Collider other) {
		InputController inputControl = other.GetComponent<InputController> ();
		if (inputControl != null) {
			if (string.IsNullOrEmpty (_loadLevel) == false) {
				SceneController.Instance.LoadScene (_loadLevel);
			}

			if (string.IsNullOrEmpty (_unloadLevel) == false) {
				SceneController.Instance.UnloadScene (_unloadLevel);
			}
		}
	}
}