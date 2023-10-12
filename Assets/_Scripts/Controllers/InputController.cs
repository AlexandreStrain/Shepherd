using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shepherd;

public class InputController : InputModule {
	[Header("Debug")]
	public GameInputs _input;
	public GameInputController _gInput;

	private float _delta;

    public float _inputTimer;
    public float _inputDelay = 0.01f;

	//test
	public TransformVariable _lockOnTransform;
	public int _lockOnIndex;
	private bool _toggleLockOn;

	private UserInterfaceController _UIController;

	private CameraController _camera;
	private Transform _cameraTransform;

	public void Init (WorldResourceController wRControl) {
        _character = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterStateController>();
		InitInGame (wRControl);

		//test
		_gInput = wRControl._gInput;

		_UIController = UserInterfaceController.Instance;
	}

	private void InitInGame(WorldResourceController wRControl) {
		_character._wRControl = wRControl;
		//_stateControl._wResourceControl = Resources.Load ("WorldResourceController") as WorldResourceController;
		//_stateControl._wResourceControl.Init (); //can init from main menu

		_character._invControl.PreInit (true);
		_character._myBrain = this;
		_character.Init ();
		_camera = CameraController.Instance;
		_camera.Init (_character);
		_cameraTransform = _camera.transform;

		//test
		//	moved to _stateControl._inventoryControl.SetupInventory()
		//UpdateLocalReferences();
		//...
	}
		
	void Update() {
		_delta = Time.deltaTime;
        if (!GameSessionController.Instance._successfulLaunch)
        {
            return;
        }

		//GetInputUpdate ();
		GetMyInputUpdate ();
        //HandleUserInterface ();

        _UIController._input = _input;
        _UIController.Tick(_delta, _character);

        switch (GameSessionController.Instance._curGameState) {
		case AllEnums.InputControlState.OutGame:
                //Force character into idle state?
                /*                _character._animator.SetFloat("Vertical", 0f);
                                _character._animator.SetFloat("Horizontal", 0f);*/

                if (!GameSessionController.Instance._endOfDay)
                {
                    GameSessionController.Instance._dayNightCycle._pauseTime = true;
                }

                break;

		case AllEnums.InputControlState.Game:
                if (!_character._states._isDead)
                {
                    GameSessionController.Instance._curGameState = AllEnums.InputControlState.Game;

                    HandleQuickslots();
                    HandleItemsOrSummonSlots();

                    if (!GameSessionController.Instance._endOfDay)
                    {
                        GameSessionController.Instance._dayNightCycle._pauseTime = false;
                    }
                    //added after

                    GameStatesUpdate();
                    _character.Tick(_delta);

                    //test
                    //TODO: SHOULD BE HANDLED IN UI CONTROLLER?
                    for (int i = 0; i < _character._wRControl._runtimeReferences._flock.Count; i++)
                    {
                        Biography flockBio = _character._wRControl._runtimeReferences._flock[i]._biography;
                        if (flockBio._currentBioCard._status != AllEnums.Status.Normal && flockBio._currentBioCard._status != AllEnums.Status.Happy && flockBio._currentBioCard._status != AllEnums.Status.Sleeping)
                        {
                            if (_character._wRControl._runtimeReferences._flockAlertSystem != AllEnums.FlockAlert.Danger)
                            {
                                _character._wRControl._runtimeReferences._flockAlertSystem = AllEnums.FlockAlert.Warning;
                                break;
                            }
                        }
                        else
                        {
                            if (_character._wRControl._runtimeReferences._flockAlertSystem != AllEnums.FlockAlert.Danger)
                            {
                                _character._wRControl._runtimeReferences._flockAlertSystem = AllEnums.FlockAlert.None;
                            }
                        }
                    }
                    //...
                } else
                {
                    _character.Tick(_delta);
                }
			break;
		case AllEnums.InputControlState.InGameMenu:
                /*_UIController._interactionPrompt.CloseHUDPrompt();

                //_input._vertical = 0f;
                //_input._horizontal = 0f;
                _character._animator.SetFloat("Vertical", 0f);
                _character._animator.SetFloat("Horizontal", 0f);*/

                //DayNightCycle.Instance._pauseTime = false;
                break;
        }
    }

	void FixedUpdate () {
		_delta = Time.fixedDeltaTime;
        if (!GameSessionController.Instance._successfulLaunch)
        {
            return;
        }

		//GetInputFixedUpdate ();
		GetMyInputFixedUpdate ();
		GameStatesFixedUpdate ();

		switch (GameSessionController.Instance._curGameState) {
		case AllEnums.InputControlState.OutGame:
			break;
		case AllEnums.InputControlState.Game:
                if (!_character._states._isDead)
                {

                    if (_input._pauseMenuButton && !GameSessionController.Instance._endOfDay)
                    {
                        GameSessionController.Instance._dayNightCycle._pauseTime = !GameSessionController.Instance._dayNightCycle._pauseTime;
                    }

                    _character.FixedTick(_delta);
                    _camera.FixedTick(_delta);

                    _character.MonitorStats();
                } else
                {
                    _character.FixedTick(_delta);
                }
			break;
		case AllEnums.InputControlState.InGameMenu:
			break;
		}

		//_stateControl.MonitorStats ();
	}

	void LateUpdate() {
        if (!GameSessionController.Instance._successfulLaunch)
        {
            return;
        }
		_character._animatorHook.LateTick ();
		ResetInputStates ();
	}

	public void GetMyInputUpdate() {
		/**E Keyboard / A BUTTON Controller**/
		_input._interactConfirmButton = _gInput.GetButton (InputStrings.Interact);

		/**LEFT CTRL Keyboard / B BUTTON Controller**/
		_input._dodgeCancelButton = _gInput.GetButton (InputStrings.Dodge); //mapped to left ctrl on keyboard

		/**F Keyboard / X BUTTON Controller**/
		_input._useButton = _gInput.GetButton (InputStrings.UseItem);

		/**SPACE Keyboard / Y BUTTON Controller**/
		_input._jumpButton = _gInput.GetButton (InputStrings.Jump);

		/**LEFT SHIFT Keyboard / LEFT JOYSTICK BUTTON Controller**/
		_input._runButton = _gInput.GetButton (InputStrings.Sprint);

		/**RIGHT TRIGGER Controller**/
		if (GameSessionController.Instance._isUsingController) {
			_input._strongRightAxis = Input.GetAxis (InputStrings.StrongAttackRight);
			if (_input._strongRightAxis != 0) {
				_input._strongRightButton = true;
			} else {
				_input._strongRightButton = false;
			}
		} else {
			_input._strongRightButton = _gInput.GetButton (InputStrings.StrongAttackRight);
		}

		/**LEFT TRIGGER Controller**/
		if (GameSessionController.Instance._isUsingController) {
			_input._strongLeftAxis = Input.GetAxis (InputStrings.StrongAttackLeft);
			if (_input._strongLeftAxis != 0) {
				_input._strongLeftButton = true;
			} else {
				_input._strongLeftButton = false;
			}
		} else {
			_input._strongLeftButton = _gInput.GetButton (InputStrings.StrongAttackLeft);
		}

		/**LEFT MOUSE CLICK Keyboard / RIGHT BUMPER Controller**/
		_input._weakRightButton = _gInput.GetButton (InputStrings.WeakAttackRight);

		/**RIGHT MOUSE CLICK Keyboard / LEFT BUMPER Controller**/
		_input._weakLeftButton = _gInput.GetButton (InputStrings.WeakAttackLeft);

		/**CAPS LOCK Keyboard**/
		_input._strongAttackKeyboard = _gInput.GetButton (InputStrings.StrongAttackKeyboard);
		if (_input._weakRightButton && _input._strongAttackKeyboard) {
			_input._weakRightButton = false;
			_input._strongRightButton = true;
		} else if (_input._weakLeftButton && _input._strongAttackKeyboard) {
			_input._weakLeftButton = false;
			_input._strongLeftButton = true;
		}

		/**MIDDLE MOUSE CLICK Keyboard / RIGHT JOYSTICK BUTTON Controller**/
		if (_gInput.GetButton (InputStrings.LockOn)) {
			_toggleLockOn = !_toggleLockOn;
		}

        if (_gInput.GetButton(InputStrings.LockOnRight))
        {
            _input._lockOnAxis = (_input._lockOnAxis >= 1f) ? 1f : _input._lockOnAxis + _delta;
        }
        else if (_gInput.GetButton(InputStrings.LockOnLeft))
        {
            _input._lockOnAxis = (_input._lockOnAxis <= -1f) ? -1f : _input._lockOnAxis - _delta;
        }
        else
        {
            _input._lockOnAxis = 0f;
        }

        if (_input._lockOnAxis <= -0.15f)
        { //hardcoded value!!
            _lockOnIndex--;
            _input._lockOnAxis = 0f;
        }
        else if (_input._lockOnAxis >= 0.15f)
        { //hardcoded value!!
            _lockOnIndex++;
            _input._lockOnAxis = 0f;
        }

        /**Z,C Keyboard / DPAD UP,DOWN Controller**/
        if (!GameSessionController.Instance._isUsingController) {
			if (_gInput.GetButton (InputStrings.CycleItems)) {
				_input._dDownPress = true;
				_input._dDownAxis = (_input._dDownAxis >= 1f) ? 1f : _input._dDownAxis + _delta;
				_input._itemSpellAxis = (_input._itemSpellAxis >= 1f) ? 1f : _input._itemSpellAxis + _delta;
			} else if (_gInput.GetButton (InputStrings.CycleCCommands)) {
				_input._dUpPress = true;
				_input._dUpAxis = (_input._dUpAxis >= 1f) ? 1f : _input._dUpAxis + _delta;
				_input._itemSpellAxis = (_input._itemSpellAxis <= -1f) ? -1f : _input._itemSpellAxis - _delta;
			} else {
				_input._itemSpellAxis = 0f;
			}
		} else {
			_input._itemSpellAxis = Input.GetAxis (InputStrings.ItemSpellCycle);
			if (_input._itemSpellAxis > 0f) {
				_input._dDownPress = true;
				_input._dDownAxis = (_input._dDownAxis >= 1f) ? 1f : _input._dDownAxis + _delta;
			} else if (_input._itemSpellAxis < 0f) {
				_input._dUpPress = true;
				_input._dUpAxis = (_input._dUpAxis >= 1f) ? 1f : _input._dUpAxis + _delta;
			}
		}

		/**1-0 Keyboard**/
		_input._shortcutItem1 = _gInput.GetButton (InputStrings.ShortcutItem1);
		_input._shortcutItem2 = _gInput.GetButton (InputStrings.ShortcutItem2);
		_input._shortcutItem3 = _gInput.GetButton (InputStrings.ShortcutItem3);
		_input._shortcutItem4 = _gInput.GetButton (InputStrings.ShortcutItem4);
		_input._shortcutItem5 = _gInput.GetButton (InputStrings.ShortcutItem5);
		_input._shortcutItem6 = _gInput.GetButton (InputStrings.ShortcutItem6);
		_input._shortcutItem7 = _gInput.GetButton (InputStrings.ShortcutItem7);
		_input._shortcutItem8 = _gInput.GetButton (InputStrings.ShortcutItem8);
		_input._shortcutItem9 = _gInput.GetButton (InputStrings.ShortcutItem9);
		_input._shortcutItem10 = _gInput.GetButton (InputStrings.ShortcutItem10);

		/**Q,R Keyboard / DPAD LEFT,RIGHT Controller**/
		if (!GameSessionController.Instance._isUsingController) {
			if (_gInput.GetButton (InputStrings.CycleRightHand)) {
				_input._dRightPress = true;
				_input._dRightAxis = (_input._dRightAxis >= 1f) ? 1f : _input._dRightAxis + _delta;
				_input._handSlotAxis = (_input._handSlotAxis >= 1f) ? 1f : _input._handSlotAxis + _delta;
			} else if (_gInput.GetButton (InputStrings.CycleLeftHand)) {
				_input._dLeftPress = true;
				_input._dLeftAxis = (_input._dLeftAxis >= 1f) ? 1f : _input._dLeftAxis + _delta;
				_input._handSlotAxis = (_input._handSlotAxis <= -1f) ? -1f : _input._handSlotAxis - _delta;
			} else {
				_input._handSlotAxis = 0f;
			}
		} else {
			_input._handSlotAxis = Input.GetAxis (InputStrings.HandSlotsCycle);
			if (_input._handSlotAxis > 0f) {
				_input._dLeftPress = true;
				_input._dLeftAxis = (_input._dLeftAxis >= 1f) ? 1f : _input._dLeftAxis + _delta;
			} else if (_input._handSlotAxis < 0f) {
				_input._dRightPress = true;
				_input._dRightAxis = (_input._dRightAxis >= 1f) ? 1f : _input._dRightAxis + _delta;
			}
		}
	}

	/// <summary>
	/// Captures inputs from both Keyboard/Mouse and Handheld controllers on Unity's Update call.
	/// </summary>
	private void GetInputUpdate() {
		/**E Keyboard / A BUTTON Controller**/
		_input._interactConfirmButton = Input.GetButtonDown (InputStrings.InteractConfirm);

		/**LEFT CTRL Keyboard / B BUTTON Controller**/
		_input._dodgeCancelButton = Input.GetButton (InputStrings.DodgeCancel); //mapped to left ctrl on keyboard

		//original
		/*
		if (_input._dodgeCancelButton) {
			_input._crouchAxis = (_input._crouchAxis >= 1f) ? 1f : _input._crouchAxis + _delta;
		}
		*/

		/**F Keyboard / X BUTTON Controller**/
		//_input._useButton = Input.GetButtonUp (InputStrings.UseItem);
		_input._useButton = Input.GetButton (InputStrings.UseItem);

		/**SPACE Keyboard / Y BUTTON Controller**/
		_input._jumpButton = Input.GetButtonDown (InputStrings.Jump);

		/**LEFT SHIFT Keyboard / LEFT JOYSTICK BUTTON Controller**/
		_input._runButton = Input.GetButton (InputStrings.Sprint);

		/**RIGHT TRIGGER Controller**/
		_input._strongRightButton = Input.GetButton(InputStrings.StrongAttackRight);
		_input._strongRightAxis = Input.GetAxis(InputStrings.StrongAttackRight);
		if (_input._strongRightAxis != 0) {
			_input._strongRightButton = true;
		}

		/**LEFT TRIGGER Controller**/
		_input._strongLeftButton = Input.GetButton(InputStrings.StrongAttackLeft);
		_input._strongLeftAxis = Input.GetAxis(InputStrings.StrongAttackLeft);
		if (_input._strongLeftAxis != 0) {
			_input._strongLeftButton = true;
		}

		/**LEFT MOUSE CLICK Keyboard / RIGHT BUMPER Controller**/
		_input._weakRightButton = Input.GetButton (InputStrings.WeakAttackRight);

		/**RIGHT MOUSE CLICK Keyboard / LEFT BUMPER Controller**/
		_input._weakLeftButton = Input.GetButton (InputStrings.WeakAttackLeft);

		/**CAPS LOCK Keyboard**/
		_input._strongAttackKeyboard = Input.GetButton (InputStrings.StrongAttackKeyboard);
		if (_input._weakRightButton && _input._strongAttackKeyboard) {
			_input._weakRightButton = false;
			//_strongAttackKeyboard = false;
			_input._strongRightButton = true;
		} else if (_input._weakLeftButton && _input._strongAttackKeyboard) {
			_input._weakLeftButton = false;
			//_strongAttackKeyboard = false;
			_input._strongLeftButton = true;
		}

		/**MIDDLE MOUSE CLICK Keyboard / RIGHT JOYSTICK BUTTON Controller**/
		_input._lockOnButton = Input.GetButtonDown (InputStrings.LockOn);
		_input._lockOnAxis = Input.GetAxis (InputStrings.LockOnAxis);
		if (_input._lockOnButton) {
			_toggleLockOn = !_toggleLockOn;
		}
		if (_input._lockOnAxis <= -1f) { //hardcoded value!!
			_lockOnIndex--;
		} else if (_input._lockOnAxis >= 1f) { //hardcoded value!!
			_lockOnIndex++; 
		}

		/**Z,C Keyboard / DPAD UP,DOWN Controller**/
		_input._itemSpellAxis = Input.GetAxis (InputStrings.ItemSpellCycle); //mapped to c on keyboard
		if (_input._itemSpellAxis > 0f) {
			_input._dDownPress = true;
			_input._dDownAxis = (_input._dDownAxis >= 1f) ? 1f : _input._dDownAxis + _delta;
		} else if (_input._itemSpellAxis < 0f) {
			_input._dUpPress = true;
			_input._dUpAxis = (_input._dUpAxis >= 1f) ? 1f : _input._dUpAxis + _delta;
		}
		/**1-0 Keyboard**/
		_input._shortcutItem1 = Input.GetButtonDown (InputStrings.ShortcutItem1);
		_input._shortcutItem2 = Input.GetButtonDown (InputStrings.ShortcutItem2);
		_input._shortcutItem3 = Input.GetButtonDown (InputStrings.ShortcutItem3);
		_input._shortcutItem4 = Input.GetButtonDown (InputStrings.ShortcutItem4);
		_input._shortcutItem5 = Input.GetButtonDown (InputStrings.ShortcutItem5);
		_input._shortcutItem6 = Input.GetButtonDown (InputStrings.ShortcutItem6);
		_input._shortcutItem7 = Input.GetButtonDown (InputStrings.ShortcutItem7);
		_input._shortcutItem8 = Input.GetButtonDown (InputStrings.ShortcutItem8);
		_input._shortcutItem9 = Input.GetButtonDown (InputStrings.ShortcutItem9);
		_input._shortcutItem10 = Input.GetButtonDown (InputStrings.ShortcutItem10);

		/**Q,R Keyboard / DPAD LEFT,RIGHT Controller**/
		_input._handSlotAxis = Input.GetAxis (InputStrings.HandSlotsCycle);
		if (_input._handSlotAxis > 0f) {
			_input._dLeftPress = true;
			_input._dLeftAxis = (_input._dLeftAxis >= 1f) ? 1f : _input._dLeftAxis + _delta;
		} else if (_input._handSlotAxis < 0f) {
			_input._dRightPress = true;
			_input._dRightAxis = (_input._dRightAxis >= 1f) ? 1f : _input._dRightAxis + _delta;
		}
	}

	///
	/// Captures Inputs from both keyboard/mouse and handheld controllers
	/// 
	private void GameStatesUpdate() {
		
		/*if (_UIController._inMenus._isInGameMenu || _UIController._inMenus._isInPauseMenu) {
			return;
		}*/

		//TEST
		_character._input = _input;

		if (_input._runButton) {
			_character._states._isRunning = (_character._moveAmount > 0f) && !_character._body._stamina.IsEmpty();//0f;
			_toggleLockOn = false;
		} else {
			_character._states._isRunning = false;
		}
			
		if (_input._dodgeCancelButton) {
			_input._crouchAxis += _delta;
			if (_input._crouchAxis > 0.5f) {
				_character._states._isCrouching = true;
				_character._states._isRunning = false;
				_character._states._isRolling = false;
			}
		} else {
			if (_input._crouchAxis > 0f && _input._crouchAxis <= 0.5f) {
				_character._states._isRolling = true;
				//_stateControl.HandleRolling();
			} else if (_character._controlColliderCrouch) {
				_input._crouchAxis = 1f;
				_character._states._isCrouching = true;
				_character._states._isRolling = false;
			} else {
				_input._crouchAxis = 0f;
				_character._states._isCrouching = false;
			}
		}

		if (_input._useButton) {
			_input._giveAxis += _delta;
			if (_input._giveAxis > 0.5f) {
				_character._states._isGivingItem = true;
				_character._states._isRunning = false;
				_character._states._isUsingItem = false;
			}
		} else {
			if (_input._giveAxis > 0f && _input._giveAxis <= 0.5f) {
				_character._states._isUsingItem = true;
				_character._states._isGivingItem = false;
				//_stateControl.HandleGivingItem();
			} else {
				_input._giveAxis = 0f;
				_character._states._isGivingItem = false;
			}
			_character._interactControl._myActiveInteraction = null;
		}
			
		//original
		/*
		if (_input._dodgeCancelButton && _input._crouchAxis > 0.5f) {
			_stateControl._states._isCrouching = true;
			_stateControl._states._isRunning = false;
			_stateControl._states._isRolling = false;

		} else if (!_input._dodgeCancelButton && (_input._crouchAxis > 0f && _input._crouchAxis < 0.5f)) {
			_stateControl._states._isRolling = true;
		}
		*/

		_character._states._isLockOn = _toggleLockOn;

		//TODO: THIS REALLY SHOULD BE HANDLED ELSEWHERE
		if (_toggleLockOn) {
			if (_character._body.GetLockOnObjects().Count == 0) { //prev _bodySenses
				_lockOnTransform._value = null;
				_toggleLockOn = false;
			} else {
				if (_lockOnIndex < 0) { 
					_lockOnIndex = _character._body.GetLockOnObjects().Count - 1; //prev _bodySenses
				} else if (_lockOnIndex > _character._body.GetLockOnObjects().Count - 1) { 
					_lockOnIndex = 0;
				}
				//_input._lockOnAxis = 0f;
				//LockOnTarget lockonTarget = _stateControl._bodySenses.getAudibleObjects () [_lockOnIndex].GetComponentInParent<LockOnTarget> (); //InChildren?
				LockOnTarget lockonTarget = _character._body.GetLockOnObjects() [_lockOnIndex].gameObject.GetComponentInChildren<LockOnTarget> ();
				if (lockonTarget != null) {
					_lockOnTransform._value = _character._body.GetLockOnObjects() [_lockOnIndex];
				}
			}
				
			if (_lockOnTransform._value == null) {
				_toggleLockOn = false;
			} else {
				float v = Vector3.Distance (_character._myTransform.position, _lockOnTransform._value.position);
				CharacterStateController lockontarget = _lockOnTransform._value.GetComponentInParent<CharacterStateController> ();

				if (v > _character._body.GetSenseRadius ()) { //distance from lockon target, prev _bodySenses
					_lockOnTransform._value = null;
					_toggleLockOn = false;
				} else if(lockontarget != null && lockontarget._states._isDead) {
					Debug.Log ("target Destroyed");
					_lockOnTransform._value = null;
					_toggleLockOn = false;
				}
			}
		} else {
			_camera.ResetLookat ();
		}

		if (_character._lockOnTransform != _lockOnTransform._value) {
			_character._lockOnTransform = _lockOnTransform._value;
		}
		//...
	}

	private void GetInputFixedUpdate() {
		/**W,A,S,D Keyboard / LEFT JOYSTICK Controller**/
		_input._vertical = Input.GetAxis (InputStrings.Vertical);
		_input._horizontal = Input.GetAxis (InputStrings.Horizontal);

		/**MOUSE Keyboard / RIGHT JOYSTICK Controller**/
		_input._camVertical = Input.GetAxis (InputStrings.CameraVertical);
		_input._camHorizontal = Input.GetAxis (InputStrings.CameraHorizontal);

		/**ESCAPE Keyboard / START Controller**/
		_input._pauseMenuButton = Input.GetButtonDown (InputStrings.PauseMenu);
		if (_input._pauseMenuButton) {
		}

		/*TAB Keyboard / SELECT BUTTON Controller*/
		_input._gameMenuButton = Input.GetButtonDown (InputStrings.InGameMenu);
		if (_input._gameMenuButton) {
		}
	}

	private void GetMyInputFixedUpdate() {
		/**W,A,S,D Keyboard / LEFT JOYSTICK Controller**/
		//_input._vertical = Input.GetAxis (InputStrings.Vertical);

		if (!GameSessionController.Instance._isUsingController) {
			if (_gInput.GetButton (InputStrings.MoveForward)) {
				_input._vertical = (_input._vertical >= 1f) ? 1f : _input._vertical + _delta;
			} else if (_gInput.GetButton (InputStrings.MoveBackward)) {
				_input._vertical = (_input._vertical <= -1f) ? -1f : _input._vertical - _delta;
			} else {
				_input._vertical = 0f;
			}

			//_input._horizontal = Input.GetAxis (InputStrings.Horizontal);
			if (_gInput.GetButton (InputStrings.MoveRight)) {
				_input._horizontal = (_input._horizontal >= 1f) ? 1f : _input._horizontal + _delta;
			} else if (_gInput.GetButton (InputStrings.MoveLeft)) {
				_input._horizontal = (_input._horizontal <= -1f) ? -1f : _input._horizontal - _delta;
			} else {
				_input._horizontal = 0f;
			}
		} else {
			_input._vertical = Input.GetAxis (InputStrings.Vertical);
			_input._horizontal = Input.GetAxis (InputStrings.Horizontal);
		}


		/**MOUSE Keyboard / RIGHT JOYSTICK Controller**/
		_input._camVertical = Input.GetAxis (InputStrings.CameraVertical);
		_input._camHorizontal = Input.GetAxis (InputStrings.CameraHorizontal);

		/**ESCAPE Keyboard / START Controller**/
		_input._pauseMenuButton = _gInput.GetButton (InputStrings.PauseMenu);
		if (_input._pauseMenuButton) {
		}

		/*TAB Keyboard / SELECT BUTTON Controller*/
		_input._gameMenuButton = _gInput.GetButton (InputStrings.InGameMenu);
		if (_input._gameMenuButton) {
		}
	}

	private void GameStatesFixedUpdate() {
		Vector3 vertical;
		Vector3 horizontal;

		vertical = _input._vertical * _cameraTransform.forward;
		vertical.y = 0f;
		horizontal = _input._horizontal * _cameraTransform.right;
		horizontal.y = 0f;

		_character._moveAmount = Mathf.Clamp01(Mathf.Abs(_input._vertical) + Mathf.Abs(_input._horizontal));
		_character._moveDirection = (vertical + horizontal).normalized;
	}
		
	private void ResetInputStates() {
		if (_character != null) {
			if (!_input._dodgeCancelButton) {
				_input._crouchAxis = 0f;
				_character._states._isCrouching = false;
				_character._states._isRolling = false;
			}

			if (!_input._useButton) {
				_input._giveAxis = 0f;
				//_stateControl._states._isUsingItem = false;
				_character._states._isGivingItem = false;
			}
			
			if (!_input._runButton) {
				_character._states._isRunning = false;
			}
				
			if (_input._handSlotAxis == 0 || _character._states._isAnimatorInAction) {
				_input._dLeftPress = false;
				_input._dLeftAxis = 0f;
				_input._dRightPress = false;
				_input._dRightAxis = 0f;
				//_stateControl._states._isChangingItem = false;
			}

			if (_input._itemSpellAxis == 0 || _character._states._isAnimatorInAction) {
				_input._dUpPress = false;
				_input._dUpAxis = 0f;
				_input._dDownPress = false;
				_input._dDownAxis = 0f;
			}

			if (_input._interactConfirmButton) {
				_input._interactConfirmButton = false;
			}
		}
	}
    
	private void HandleUserInterface() {
		if (_input._gameMenuButton && GameSessionController.Instance._curGameState != AllEnums.InputControlState.OutGame) {
			_UIController._toggleInGameMenu = !_UIController._toggleInGameMenu;
			_UIController._toggleOutGameMenu = false;

			//TEMP SOLUTION TO AFTER DEATH PROBLEMS
			if (!_character._states._isDead) {
				if (_UIController._toggleInGameMenu) {
					GameSessionController.Instance._curGameState = AllEnums.InputControlState.InGameMenu;
					_UIController.OpenInGameUI ();
					_UIController.CloseOutGameUI ();
				} else {
					GameSessionController.Instance._curGameState = AllEnums.InputControlState.Game;
					_UIController.CloseInGameUI ();
					_UIController.CloseOutGameUI ();
				}
			} else {
                GameSessionController.Instance._curGameState = AllEnums.InputControlState.OutGame;

				_UIController._toggleInGameMenu = false;
			}
		}

		//TODO: There is a bug where if you leap and pause, the camera stays in place and the player keeps moving -- player should not move but keep momentum
		if (_input._pauseMenuButton) {
			_UIController._toggleOutGameMenu = !_UIController._toggleOutGameMenu;
			_UIController._toggleInGameMenu = false;

			if (_UIController._toggleOutGameMenu) {
                GameSessionController.Instance._curGameState = AllEnums.InputControlState.OutGame;

				_UIController.OpenOutGameUI ();
			} else {
				GameSessionController.Instance._curGameState = AllEnums.InputControlState.Game;
				_UIController.CloseInGameUI ();
				_UIController.CloseOutGameUI ();
			}
		}
	}

	private void HandleItemsOrSummonSlots() {
		if (_character._states._isChangingItem || _character._states._isSpellCasting || _character._states._isUsingItem || _character._states._isGivingItem) { //don't know if latter part is necessary
			return;
		}

        if (Input.GetJoystickNames ().Length == 0) {// && Input.GetJoystickNames () [0].Length == 0) {
			HandleItemsKeyboard ();
			return;
		}

        //items or summon
        if (_input._itemSpellAxis != 0 && !_character._states._isAnimatorInAction) {
			if (_input._dUpPress) { //positive = up on dpad,
				if (_input._dUpAxis > 0.5f) {
					_character._input._dUpHold = true;
					_character._input._dDownHold = false;
				} else {
					_character._input._dUpPress = true;
					//cancel down dpad hold
					if (_character._input._dDownHold) {
						_character._input._dDownHold = false;
					}
					if (!_character._states._isDelayed) {
						//Cycle Consumables
						_character._states._isChangingItem = true;
						_character._states._isUsingItem = false;
						_character._states._isGivingItem = false;
						_character._invControl.CycleConsumables();
						_character._currentActionStatus = AllEnums.CharacterActionState.Interacting;
					}
				}
			}

			if (_input._dDownPress) { //negative = down on dpad
				if (_input._dDownAxis > 0.5f) {
					_character._input._dDownHold = true;
					_character._input._dUpHold = false;
				} else {
					//cancel up dpad hold
					_character._input._dDownPress = true;
					if (_character._input._dUpHold) {
						_character._input._dUpHold = false;
					}
					if (!_character._states._isDelayed) {
						//cycle companion commands
					}
				}
			}
			_character.UpdateGameUI();
		} /*else {
			_character._input._dUpPress = false;
			_character._input._dDownPress = false;
		}*/
    }

	private void HandleQuickslots() {
		if (_character._states._isChangingItem || _character._states._isSpellCasting || _character._states._isUsingItem) {
			return;
		}
			
		if (_input._handSlotAxis != 0 && !_character._states._isAnimatorInAction) {
			if (_input._dRightPress) {
				if (_input._dRightAxis > 0.5f) {
					_character._input._dRightHold = true;
					_character._input._dLeftHold = false;
					_character.HandleTwoHanded ();
				} else {
					_character._input._dRightPress = true;
					//cancel left dpad hold
					if (_character._input._dLeftHold) {
						_character._input._dLeftHold = false;
						_character.HandleTwoHanded ();
					}
					//right hand cycle weapons
					if (!_character._states._isDelayed) {
						_character._states._isChangingItem = true;
						_character._states._isUsingItem = false;
						_character._states._isGivingItem = false;
						_character._invControl.CycleWeapons(false);
						_character._currentActionStatus = AllEnums.CharacterActionState.Interacting;
					}
				}
			}
			if (_input._dLeftPress) {
				if (_input._dLeftAxis > 0.5f) {
					_character._input._dLeftHold = true;
					_character._input._dRightHold = false;
					_character.HandleTwoHanded ();
				} else {
					//cancel right dpad hold
					_character._input._dLeftPress = true;
					if (_character._input._dRightHold) {
						_character._input._dRightHold = false;
						_character.HandleTwoHanded ();
					}
					//left hand cycle weapons
					if (!_character._states._isDelayed) {
						_character._states._isChangingItem = true;
						_character._states._isUsingItem = false;
						_character._states._isGivingItem = false;
						_character._invControl.CycleWeapons(true);
						_character._currentActionStatus = AllEnums.CharacterActionState.Interacting;
					}
				}
			}
			_character.UpdateGameUI();
		} else {
			_character._input._dLeftPress = false;
			_character._input._dRightPress = false;
		}
	}

	public void HandleItemsKeyboard() {
		if (_character._states._isDelayed) {
			return;
		}

        if (_input._dUpPress) {
			_character._states._isChangingItem = true;
			_character._states._isUsingItem = false;
			_character._states._isGivingItem = false;
			_character._invControl.CycleConsumables();
			_character._currentActionStatus = AllEnums.CharacterActionState.Interacting;
		} else if (_input._dDownPress) {
			//cycle companion commands
			//_stateControl._currentActionStatus = AllEnums.CharacterActionState.Interacting;
		} else if (_input._shortcutItem1) {
			_character._invControl.CycleConsumablesToSlot(0);
			_character._currentActionStatus = AllEnums.CharacterActionState.Interacting;
		} else if (_input._shortcutItem2) {
			_character._invControl.CycleConsumablesToSlot(1);
			_character._currentActionStatus = AllEnums.CharacterActionState.Interacting;
		} else if (_input._shortcutItem3) {
			_character._invControl.CycleConsumablesToSlot(2);
			_character._currentActionStatus = AllEnums.CharacterActionState.Interacting;
		} else if (_input._shortcutItem4) {
			_character._invControl.CycleConsumablesToSlot(3);
			_character._currentActionStatus = AllEnums.CharacterActionState.Interacting;
		} else if (_input._shortcutItem5) {
			_character._invControl.CycleConsumablesToSlot(4);
			_character._currentActionStatus = AllEnums.CharacterActionState.Interacting;
		} else if (_input._shortcutItem6) {
			_character._invControl.CycleConsumablesToSlot(5);
			_character._currentActionStatus = AllEnums.CharacterActionState.Interacting;
		} else if (_input._shortcutItem7) {
			_character._invControl.CycleConsumablesToSlot(6);
			_character._currentActionStatus = AllEnums.CharacterActionState.Interacting;
		} else if (_input._shortcutItem8) {
			_character._invControl.CycleConsumablesToSlot(7);
			_character._currentActionStatus = AllEnums.CharacterActionState.Interacting;
		} else if (_input._shortcutItem9) {
			_character._invControl.CycleConsumablesToSlot(8);
			_character._currentActionStatus = AllEnums.CharacterActionState.Interacting;
		} else if (_input._shortcutItem10) {
			_character._invControl.CycleConsumablesToSlot(9);
			_character._currentActionStatus = AllEnums.CharacterActionState.Interacting;
		}

        _character.UpdateGameUI();
	}
}