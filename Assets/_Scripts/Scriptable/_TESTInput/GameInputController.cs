using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shepherd;

[CreateAssetMenu(menuName = "Shepherd/Single Instances/Game Input Controller")]
public class GameInputController : ScriptableObject {

	public InputProfile _defaultControls;

	public InputProfile _controlScheme;

	private Dictionary<string, KeyCode> _gameControls = new Dictionary<string, KeyCode>();

	public void Init() {

		_gameControls.Clear ();

		if (_controlScheme == null) {
			_controlScheme = _defaultControls;
		}

		if (GameSessionController.Instance._isControllerPluggedIn && GameSessionController.Instance._isUsingController) {
			//using gamepad
			for (int i = 0; i < _controlScheme._gamepadInput.Count; i++) {
				if (_gameControls.ContainsKey (_controlScheme._gamepadInput [i]._keyName)) {
					Debug.Log ("Uh oh, duplicate keybinding... " + _controlScheme._gamepadInput [i]._keyName);
				} else {
					_gameControls.Add (_controlScheme._gamepadInput [i]._keyName, _controlScheme._gamepadInput [i]._key);
				}
			}
		}
		else {
			//using keyboard & mouse
			for (int i = 0; i < _controlScheme._keyboardInput.Count; i++) {
				if (_gameControls.ContainsKey (_controlScheme._keyboardInput [i]._keyName)) {
					Debug.Log ("Uh oh, duplicate keybinding... " + _controlScheme._keyboardInput [i]._keyName);
				} else {
					_gameControls.Add (_controlScheme._keyboardInput [i]._keyName, _controlScheme._keyboardInput [i]._key);
				}
			}
		}
	}

	public KeyCode GetGameButton(string id) {
		KeyCode returnCode = KeyCode.None;

		if (_gameControls.TryGetValue (id, out returnCode)) {
			//returnCode = _controlScheme._keyboardInput[index]._key;
		} else {
			//Debug.Log(returnCode.ToString());
		}

		return returnCode;
	}

	public void SetGameButton(string id, KeyCode newKey) {
		KeyCode returnCode = KeyCode.None;

		if (_gameControls.TryGetValue (id, out returnCode)) {
			_gameControls [id] = newKey;
			Debug.Log (id + " set to new keycode: " + newKey.ToString ());
			//returnCode = _controlScheme._keyboardInput[index]._key;
		}
	}

	public bool GetButton(string id) {
		return Input.GetKey (GetGameButton (id));
	}
}
