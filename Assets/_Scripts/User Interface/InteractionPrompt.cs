using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionPrompt : MonoBehaviour {
	//Prompt
	public Image _buttonImage;
	public Text _buttonDescription;
	public Text _actionPrompt;
	public Text _overlapNotify;

	public Sprite _aButton;
	public Sprite _bButton;
	public Sprite _xButton;
	public Sprite _yButton;

	public Sprite _crossButton;
	public Sprite _circleButton;
	public Sprite _squareButton;
	public Sprite _triangleButton;

	public Sprite _keyButton;

	public void OpenHUDPrompt(AllEnums.PromptType pType, bool hold = false, bool overlap = false) {
		gameObject.SetActive (true);

		if (hold) {
			_actionPrompt.text = "(Hold)\n";
		} else {
			_actionPrompt.text = "";
		}

		if (_overlapNotify != null) {
			if (overlap) { //multiple prompts overlapping
				_overlapNotify.text = "+";
			} else {
				_overlapNotify.text = "";
			}
		}

		_actionPrompt.text += (pType).ToString ();

		//if (Input.GetJoystickNames ().Length > 0 && Input.GetJoystickNames () [0].Length > 0) {
		if(Shepherd.GameSessionController.Instance._isControllerPluggedIn && Shepherd.GameSessionController.Instance._isUsingController) {
			//Debug.Log (Input.GetJoystickNames () [0]);
			switch (pType) {
			case AllEnums.PromptType.Give:
				_buttonImage.sprite = _xButton;
				break;
			case AllEnums.PromptType.Skip:
				_buttonImage.sprite = _keyButton;
				_buttonDescription.text = "L";
				_buttonDescription.gameObject.SetActive (true);
				break;
			case AllEnums.PromptType.Listen:
			case AllEnums.PromptType.Talk:
			case AllEnums.PromptType.Pickup:
			case AllEnums.PromptType.Open:
			case AllEnums.PromptType.Close:
			case AllEnums.PromptType.Examine:
			case AllEnums.PromptType.Continue:
			default:
				_buttonImage.sprite = _aButton;
				_buttonDescription.text = "";
				break;
			}

		} else {
			_buttonImage.sprite = _keyButton;
			switch (pType) {
			case AllEnums.PromptType.Give:
				_buttonDescription.text = "f".ToUpper();
				break;
			case AllEnums.PromptType.Skip:
				_buttonDescription.text = "Shift";
				break;
			case AllEnums.PromptType.Listen:
			case AllEnums.PromptType.Talk:
			case AllEnums.PromptType.Pickup:
			case AllEnums.PromptType.Open:
			case AllEnums.PromptType.Close:
			case AllEnums.PromptType.Examine:
			case AllEnums.PromptType.Continue:
			default:
				_buttonDescription.text = "e".ToUpper(); //for now, could use PlayerPrefs.GetString(What key we want);
				break;

			}
		}
		gameObject.SetActive (true);
	}

	public void CloseHUDPrompt() {
		gameObject.SetActive (false);
	}
}