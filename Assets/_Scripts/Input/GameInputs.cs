using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameInputs {

	public float _vertical;
	public float _horizontal;
	public float _camVertical;
	public float _camHorizontal;

	public float _dUpAxis;
	public float _dLeftAxis;
	public float _dDownAxis;
	public float _dRightAxis;

	public float _crouchAxis;
	public float _handSlotAxis;
	public float _itemSpellAxis;
	public float _strongLeftAxis;
	public float _strongRightAxis;
	public float _lockOnAxis;
	public float _giveAxis;

	public bool _runButton;
	public bool _useButton;
	public bool _jumpButton;
	public bool _lockOnLeft;
	public bool _lockOnRight;
	public bool _lockOnButton;
	public bool _dodgeCancelButton;
	public bool _interactConfirmButton;

	public bool _dLeftPress;
	public bool _dRightPress;
	public bool _dUpPress;
	public bool _dDownPress;

	public bool _dLeftHold;
	public bool _dRightHold;
	public bool _dUpHold;
	public bool _dDownHold;

	public bool _shortcutItem1;
	public bool _shortcutItem2;
	public bool _shortcutItem3;
	public bool _shortcutItem4;
	public bool _shortcutItem5;
	public bool _shortcutItem6;
	public bool _shortcutItem7;
	public bool _shortcutItem8;
	public bool _shortcutItem9;
	public bool _shortcutItem10;

	public bool _gameMenuButton;
	public bool _pauseMenuButton;

	public bool _weakLeftButton; //left bumper
	public bool _weakRightButton; //right bumper
	public bool _strongLeftButton;
	public bool _strongRightButton;
	public bool _strongAttackKeyboard;


	public bool CheckAttackButtons() {
		return _weakLeftButton || _weakRightButton || _strongLeftButton || _strongRightButton;
	}

    public bool CheckInput() {
        return (_vertical != 0) || (_horizontal != 0) ||
            (_interactConfirmButton || _dodgeCancelButton || _weakRightButton || _weakLeftButton) ||
            (_useButton || _jumpButton || _dDownPress || _dUpPress) ||
            (_dRightPress || _dLeftPress) || _pauseMenuButton || _gameMenuButton || _useButton || _lockOnButton || _dDownPress || _dUpPress;
    }
}

[System.Serializable]
public class GameInputKeys {
	public string _keyName;
	public KeyCode _key;
}