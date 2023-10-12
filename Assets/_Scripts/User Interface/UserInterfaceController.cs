using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Shepherd;
using System;
using Utilities;

public class UserInterfaceController : Singleton<UserInterfaceController> {

	public HealthPanel _healthPanel;
	public InteractionPrompt _interactionPrompt;
	public InfoFeed _informationFeed;
	public DialogueController _dController;

	public InGameMenus _inMenus;
	public OutGameMenus _outMenus;

	//public static HeadsUpDisplay _HUD;

	public GameObject _outGameMenus;
	public GameObject _inGameOverlayMenu;
	public GameObject _inGameMenus;
	public GameObject _headsUpDisplay;

	public bool _toggleInGameMenu;
	public bool _toggleOutGameMenu;
	public Text _fpsCounter;

    //need private?
	public InputController _inputControl;

    [Header("TESTING")]
    public GameObject _HUDScripts;


    [HideInInspector]
    public GameInputs _input;

	//test
	private KeyCode newKey;

	void Update() {
		_fpsCounter.text = (Mathf.Round(1/Time.deltaTime)).ToString() + " fps";
	}

	//test
	void OnGUI() {
		_outMenus._optionsMenu._keyMapEvent = Event.current;
		if (_outMenus._optionsMenu._keyMapEvent.isKey && _outMenus._optionsMenu._hasKeyToBeMapped) {
			newKey = _outMenus._optionsMenu._keyMapEvent.keyCode;
			_outMenus._optionsMenu._hasKeyToBeMapped = false;
		}
	}

	public void PreInit() {
        _healthPanel = _HUDScripts.GetComponent<HealthPanel>();
        _dController = _HUDScripts.GetComponent<DialogueController>();
        //...
        
        _inMenus.PreInit ();
		_outMenus.PreInit ();
	}
		
	public void Init(CharacterStateController player, InputController iControl) {
        //_HUD.Init (player._body);
        _healthPanel.Init (player._body);

		_inMenus.Init (player);

		_inputControl = iControl;
		_interactionPrompt.CloseHUDPrompt ();
		_dController.Init (player._myTransform);
        CycleOutGameUIMenus();
	}
		
	public void Tick (float delta, CharacterStateController player) {
        if(!GameSessionController.Instance._successfulLaunch)
        {
            return;
        }
        HandleInputState(delta, player);
        switch (GameSessionController.Instance._curGameState)
        {
           /* case AllEnums.InputControlState.OutGame:
                //_outMenus.Tick(_input, delta);
                break;*/
            case AllEnums.InputControlState.Game:
                //_HUD.Tick(player, delta);
                _healthPanel.Tick(delta, player);
                _informationFeed.Tick(delta);
                break;
          /*  case AllEnums.InputControlState.InGameMenu:
                //_inMenus.Tick(_input, delta);

                break;*/
            default:
                //Force character into idle state?
                player._animator.SetFloat("Vertical", 0f);
                player._animator.SetFloat("Horizontal", 0f);

                _interactionPrompt.CloseHUDPrompt();
                break;
        }
    }

    public void NavigateToInMenu(int state)
    {
        _inMenus.NavigateInGameMenus(state);
    }

    private void HandleInputState(float delta, CharacterStateController player)
    {
        _inputControl._inputTimer -= delta;
        if (_inputControl._inputTimer > 0f)
        {
            return;
        }
        else if (_inputControl._inputTimer < 0f)
        {
            _inputControl._inputTimer = 0f;
        }

        if(_input._gameMenuButton && GameSessionController.Instance._curGameState != AllEnums.InputControlState.OutGame)
        {
            _toggleInGameMenu = !_toggleInGameMenu;
            _toggleOutGameMenu = false;

            if (_toggleInGameMenu)
            {
                GameSessionController.Instance._curGameState = AllEnums.InputControlState.InGameMenu;
                OpenInGameUI();
                CloseOutGameUI();
            }
            else
            {
                GameSessionController.Instance._curGameState = AllEnums.InputControlState.Game;
                CloseInGameUI();
                CloseOutGameUI();
            }
        }

        if (_input._pauseMenuButton)
        {
            _toggleOutGameMenu = !_toggleOutGameMenu;
            _toggleInGameMenu = false;

            if (_toggleOutGameMenu)
            {
                GameSessionController.Instance._curGameState = AllEnums.InputControlState.OutGame;

                OpenOutGameUI();
            }
            else
            {
                GameSessionController.Instance._curGameState = AllEnums.InputControlState.Game;
                CloseInGameUI();
                CloseOutGameUI();
            }
        }

        switch (GameSessionController.Instance._curGameState)
        {
            case AllEnums.InputControlState.OutGame:
                _outMenus.Tick(_input, delta);
                break;
            case AllEnums.InputControlState.Game:
                HandleWorldInteractions(delta, player);
                //_HUD.Tick(player, delta);
                //_healthPanel.Tick(delta, player);
                //_informationFeed.Tick(delta);
                //_dController.Tick(_input._interactConfirmButton, _input._runButton, delta);
                break;
            case AllEnums.InputControlState.InGameMenu:
                _inMenus.Tick(_input, delta);
                break;
        }


        if (_inputControl._input.CheckInput())
        {
            _inputControl._inputTimer = _inputControl._inputDelay;
        }
        else
        {
            _inputControl._inputTimer = 0f;
        }
    }

    private void HandleWorldInteractions(float delta, CharacterStateController player)
    {
        if (!_dController._dialogueActive)
        {
            //player is not in animation -- doesn't currently work when picking up things
            if ((!player._states._isAnimatorInAction && !player._states._isDelayed) && player._interactControl._interactions.Count != 0)
            {
                if (_input._jumpButton)
                {
                    player._interactControl._index++;
                    //TODO: IF JUMP BUTTON IS STILL ACTIVE HERE, THE PLAYER JUMPS IF THERE ARE TWO OR MORE INTERACTABLES NEARBY
                    // BUT IF JUMP BUTTON IS INACTIVE HERE, THE PLAYER CYCLES THROUGH TWO OR MORE INTERACTABLES NEARBY TOO FAST, BUT DOES NOT JUMP
                }

                if (player._interactControl._index > (player._interactControl._interactions.Count - 1))
                {
                    player._interactControl._index = 0;
                }

                bool overlapping = player._interactControl._interactions.Count > 1;
                WorldInteraction availableInteraction = player._interactControl.GetInteraction();
                if (availableInteraction != null)
                {
                    _interactionPrompt.OpenHUDPrompt(availableInteraction._prompt, false, overlapping);
                    if (_input._interactConfirmButton)
                    {
                        player.HandleInteractions();
                        _interactionPrompt.CloseHUDPrompt();
                    }
                }
                else
                {
                    _interactionPrompt.CloseHUDPrompt();
                }

            }
            else
            {
                _interactionPrompt.CloseHUDPrompt();
            }
        }
        else
        {
            _interactionPrompt.CloseHUDPrompt();
        }
        _dController.Tick(_input._interactConfirmButton, _input._runButton, delta);
    }

	public void CycleInventoryUIMenus() {
		switch (_inMenus._invState) {
		case AllEnums.InventoryUIState.Equipment:
			OpenEquipmentUI ();
			break;
		case AllEnums.InventoryUIState.Inventory:
			OpenItemsUI ();
			break;
		case AllEnums.InventoryUIState.Overview:
			OpenPlayerOverview ();
			break;
		case AllEnums.InventoryUIState.Flock:
			OpenFlockOverview ();
			break;
		case AllEnums.InventoryUIState.Gestures:
			OpenGesturesUI ();
			break;
            case AllEnums.InventoryUIState.Report:
                OpenWeatherReportUI();
                break;
		default:
			OpenInGameUI ();
			break;
		}
	}

    private void OpenWeatherReportUI()
    {
        _inMenus.Reset();
        _inGameMenus.SetActive(true);
        _inMenus._weatherReportMenu.SetActive(true);
        _inMenus._helpPanel.SetActive(true);
        _inMenus._inGameNavigatePanel.SetupPanel(_inMenus._invState);
        _inMenus._inGameNavigatePanel._navigateHelpText.text = "Currently under development, nothing to see here!";


        _inMenus._weatherReportPanel.PrepReport();


        _inGameOverlayMenu.SetActive(false);
        _inMenus._gesturesMenu.SetActive(false);
        _inMenus._gesturesPanel.HandleGesturesMenu(false);

        _inMenus._equipmentInventoryMenu.SetActive(false);
        _inMenus._playerOverview.SetActive(false);
        _inMenus._flockOverview.SetActive(false);
        _inMenus._titlePanel.SetActive(false);

        _headsUpDisplay.SetActive(false);
        _outGameMenus.SetActive(false);
        CloseInfoFeed();
    }

    public void CycleOutGameUIMenus() {
		switch (_outMenus._outGameUIState) {
		case AllEnums.OutGameUIState.None:
			OpenTitlePausePanel (false);
			break;
		case AllEnums.OutGameUIState.TitleScreen:
			OpenTitlePausePanel (true);
			break;
		case AllEnums.OutGameUIState.LoadSaveGame:
			OpenLoadSavePanel ();
			break;
		case AllEnums.OutGameUIState.Options:
			OpenOptionsPanel ();
			break;
		}
	}

	public void CycleOptionsUIMenus() {
		switch (_outMenus._outOptionsUIState) {
		case AllEnums.OptionsUIState.Gameplay:
			OpenOptionsPanel ();
			break;
		case AllEnums.OptionsUIState.Video:
			OpenOptionsVideoPanel ();
			break;
		case AllEnums.OptionsUIState.Audio:
			OpenOptionsAudioPanel ();
			break;
		case AllEnums.OptionsUIState.Controls:
			OpenOptionsControlsPanel ();
			break;
		}
	}

	public void OpenInfoFeed() {
		_informationFeed._infoFeedPanel.SetActive (true);
	}

	public void CloseInfoFeed() {
		_informationFeed.ClearFeed ();
		_informationFeed._infoFeedPanel.SetActive (false);
	}
		
	public void OpenEquipmentUI() {
		_inMenus.Reset ();

		_inMenus._equipmentInventoryMenu.SetActive (true); // for now, but game menu should be the mediator in case we want to go into other menus
		_inMenus._eqInvPanel._eqCenter.SetActive (true);
		_inMenus._eqInvPanel._eqLeft.SetActive (true);

		_inMenus._eqInvPanel._invCenter.SetActive (false);
		_inMenus._eqInvPanel._invLeft.SetActive (false);

		_inMenus._eqInvPanel._slotHeading.text = "Current Equipment :";
		_inMenus._eqInvPanel._reqSideEffectsHeading.text = "Requirements";


		_inMenus.LoadEquipment ();

		_inGameMenus.SetActive (true);
		_inGameOverlayMenu.SetActive (false);
		//_inventory._inventoryMenu.SetActive (true); //why doesn't this show up
		_inMenus._titlePanel.SetActive(true);
		_inMenus._helpPanel.SetActive (true);
		_inMenus._inGameNavigatePanel.SetupPanel (_inMenus._invState);
		_inMenus._inGameNavigatePanel._navigateHelpText.text = "Select an Equipment Slot...";

		_inMenus._playerOverview.SetActive (false);
		_inMenus._flockOverview.SetActive (false);
		_inMenus._gesturesMenu.SetActive (false);
		_inMenus._gesturesPanel.HandleGesturesMenu (false);
        _inMenus._weatherReportMenu.SetActive(false);

		//TODO: Put this in another class called Heads-Up-Display, and have that static in this class
		//_HUD.gameObject.SetActive(false);
		_headsUpDisplay.SetActive(false);
		_outGameMenus.SetActive (false);
		CloseInfoFeed ();
	}

	public void OpenFlockOverview() {
		_inMenus.Reset ();
		_inGameMenus.SetActive (true);
		_inMenus._flockOverview.SetActive (true);

		_inGameOverlayMenu.SetActive (false);
		_inMenus._gesturesMenu.SetActive (false);
		_inMenus._gesturesPanel.HandleGesturesMenu (false);

		_inMenus._titlePanel.SetActive (true);
		_inMenus._helpPanel.SetActive (true);
		_inMenus._inGameNavigatePanel.SetupPanel (_inMenus._invState);
		_inMenus._inGameNavigatePanel._navigateHelpText.text = "Currently under development, nothing to see here!";

		_inMenus._equipmentInventoryMenu.SetActive (false);
		_inMenus._playerOverview.SetActive (false);
        _inMenus._weatherReportMenu.SetActive(false);

        _headsUpDisplay.SetActive(false);
		_outGameMenus.SetActive (false);
		CloseInfoFeed ();
	}

	public void OpenInGameUI() {
		_inGameOverlayMenu.SetActive (true);
		_inMenus._inGamePanel.HandleGesturesMenu (true);
		_inMenus.Reset ();

		//test
		_inMenus._invState = AllEnums.InventoryUIState.None;

		_inGameMenus.SetActive (false);
		//_inventory._inventoryMenu.SetActive (true); //why doesn't this show up
		_inMenus._equipmentInventoryMenu.SetActive (false); // for now, but game menu should be the mediator in case we want to go into other menus
		_inMenus._titlePanel.SetActive(false);
		_inMenus._helpPanel.SetActive (false);

		_inMenus._playerOverview.SetActive (false);
		_inMenus._flockOverview.SetActive (false);
		_inMenus._gesturesMenu.SetActive (false);
		_inMenus._gesturesPanel.HandleGesturesMenu (false);
        _inMenus._weatherReportMenu.SetActive(false);

        //TODO: Put this in another class called Heads-Up-Display, and have that static in this class
        //_HUD.gameObject.SetActive(false);
        _headsUpDisplay.SetActive(true);
		_outGameMenus.SetActive (false);
		CloseInfoFeed ();
	}

	public void CloseInGameUI() {
		_inMenus.Reset ();

		//test
		_inMenus._invState = AllEnums.InventoryUIState.None;

		_inGameMenus.SetActive (false);
		_inGameOverlayMenu.SetActive (false);
		_inMenus._equipmentInventoryMenu.SetActive (false); // for now, but game menu should be the mediator in case we want to go into other menus
		_inMenus._titlePanel.SetActive(false);
		_inMenus._helpPanel.SetActive (false);
		_inMenus._playerOverview.SetActive (false);
		_inMenus._flockOverview.SetActive (false);
		_inMenus._gesturesMenu.SetActive (false);
        _inMenus._weatherReportMenu.SetActive(false);

        _inMenus._gesturesPanel.HandleGesturesMenu (false);

		_headsUpDisplay.SetActive(true);

        GameSessionController.Instance._curGameState = AllEnums.InputControlState.Game;

		_outGameMenus.SetActive (false);
		//OpenInfoFeed ();
	}

	public void OpenItemsUI() {
		_inMenus.Reset ();
		_inMenus._eqInvPanel._slotHeading.text = "Current Inventory :";
		_inMenus._eqInvPanel._reqSideEffectsHeading.text = "Side Effects";

		_inMenus._equipmentInventoryMenu.SetActive (true);  // for now, but game menu should be the mediator in case we want to go into other menus
		_inMenus._eqInvPanel._eqCenter.SetActive (false);
		_inMenus._eqInvPanel._eqLeft.SetActive (false);

		_inMenus._eqInvPanel._invCenter.SetActive (true);
		_inMenus._eqInvPanel._invLeft.SetActive (true);
		_inMenus.LoadEquipment ();


		_inGameMenus.SetActive (true);
		_inGameOverlayMenu.SetActive (false);
		//_inventory._inventoryMenu.SetActive (true); //why doesn't this show up

		_inMenus._titlePanel.SetActive(true);
		_inMenus._helpPanel.SetActive (true);
		_inMenus._inGameNavigatePanel.SetupPanel (_inMenus._invState);
		_inMenus._inGameNavigatePanel._navigateHelpText.text = "Select an Equipment Slot...";

		_inMenus._playerOverview.SetActive (false);
		_inMenus._flockOverview.SetActive (false);
		_inMenus._gesturesPanel.HandleGesturesMenu (false);
		_inMenus._gesturesMenu.SetActive (false);
        _inMenus._weatherReportMenu.SetActive(false);

        //TODO: Put this in another class called Heads-Up-Display, and have that static in this class
        //_HUD.gameObject.SetActive(false);
        _headsUpDisplay.SetActive(false);
		_outGameMenus.SetActive (false);
		CloseInfoFeed ();
	}

	public void OpenPlayerOverview() {
		_inMenus.Reset ();
		_inGameMenus.SetActive (true);
		_inMenus._playerOverview.SetActive (true);

		_inGameOverlayMenu.SetActive (false);
		_inMenus._gesturesMenu.SetActive (false);
		_inMenus._gesturesPanel.HandleGesturesMenu (false);

		_inMenus._titlePanel.SetActive (true);
		_inMenus._helpPanel.SetActive (true);
		_inMenus._inGameNavigatePanel.SetupPanel (_inMenus._invState);
		_inMenus._inGameNavigatePanel._navigateHelpText.text = "Select a Statistic for further detail about it...";

		_inMenus._equipmentInventoryMenu.SetActive (false);
		_inMenus._flockOverview.SetActive (false);
        _inMenus._weatherReportMenu.SetActive(false);

        _headsUpDisplay.SetActive(false);
		_outGameMenus.SetActive (false);
		CloseInfoFeed ();
	}

	public void OpenGesturesUI() {
		_inGameMenus.SetActive (true);
		_inGameOverlayMenu.SetActive (false);
		_inMenus._gesturesMenu.SetActive (true);
		_inMenus._gesturesPanel.HandleGesturesMenu (true);

		_inMenus._titlePanel.SetActive(true);
		_inMenus._helpPanel.SetActive (true);
		_inMenus._inGameNavigatePanel.SetupPanel (_inMenus._invState);
		_inMenus._inGameNavigatePanel._navigateHelpText.text = "Select a Gesture to act out...";

		_inMenus._equipmentInventoryMenu.SetActive (false);
		_inMenus._playerOverview.SetActive (false);
		_inMenus._flockOverview.SetActive (false);
        _inMenus._weatherReportMenu.SetActive(false);

        _headsUpDisplay.SetActive(false);
		_outGameMenus.SetActive (false);
		CloseInfoFeed ();
	}

	public void OpenOutGameUI() {
		_outGameMenus.SetActive (true);
		if(_outMenus._outGameUIState != AllEnums.OutGameUIState.TitleScreen) {
			_outMenus._outGameUIState = AllEnums.OutGameUIState.None;
			_outMenus._loadSaveMenu._fromTitle = false;
			_outMenus._titlePauseMenu.PrepForPauseMenu ();
		} else {
			_outMenus._loadSaveMenu._fromTitle = true;
			_outMenus._titlePauseMenu.PrepForTitleMenu ();
		}
		GameSessionController.Instance._curGameState = AllEnums.InputControlState.OutGame;
			
		_outMenus._titlePausePanel.SetActive (true);

		//close
		_outMenus._optionsPanel.SetActive(false);
		_outMenus._loadSavePanel.SetActive (false);

		CameraController.Instance.PauseAllCameras ();
	}

	public void CloseOutGameUI() {
		_outGameMenus.SetActive (false);
        UserInterfaceController.Instance._toggleOutGameMenu = false;
        CameraController.Instance.ResumeAllCameras ();

		CloseTitleScreenPanel ();
		CloseOptionsPanel ();
		CloseLoadSavePanel ();
	}
		
	public void OpenTitlePausePanel(bool isTitle) {
		//open
		_outGameMenus.SetActive(true);
		_outMenus._titlePausePanel.SetActive (true);

		if (_outMenus._outGameUIState == AllEnums.OutGameUIState.TitleScreen) {
			_outMenus._loadSaveMenu._fromTitle = true;
			_outMenus._titlePauseMenu.PrepForTitleMenu ();
		} else if (_outMenus._outGameUIState == AllEnums.OutGameUIState.None) {
			_outMenus._loadSaveMenu._fromTitle = false;
			_outMenus._titlePauseMenu.PrepForPauseMenu ();
		}
		//close
		_outMenus._optionsPanel.SetActive(false);
		_outMenus._loadSavePanel.SetActive (false);
	}
	public void CloseTitleScreenPanel() {
		_outMenus._titlePausePanel.SetActive (false);
	}

	public void OpenOptionsPanel () {
		//open
		_outGameMenus.SetActive (true);
		_outMenus._optionsPanel.SetActive (true);
		_outMenus._optionsMenu._gameplayPanel.SetActive (true);

		_outMenus._outOptionsUIState = AllEnums.OptionsUIState.Gameplay;

		_outMenus._optionsMenu._navControls.interactable = true;
		_outMenus._optionsMenu._navAudio.interactable = true;
		_outMenus._optionsMenu._navVideo.interactable = true;

		//modify
		_outMenus._optionsMenu._navGameplay.interactable = false;

		//close
		CloseTitleScreenPanel();
		CloseLoadSavePanel();

		_outMenus._optionsMenu._videoPanel.SetActive (false);
		_outMenus._optionsMenu._audioPanel.SetActive (false);
		_outMenus._optionsMenu._controlsPanel.SetActive (false);
	}
	public void CloseOptionsPanel() {
		_outMenus._optionsPanel.SetActive (false);
		_outMenus._optionsMenu._gameplayPanel.SetActive (true);

		_outMenus._outOptionsUIState = AllEnums.OptionsUIState.Gameplay;

		_outMenus._optionsMenu._navControls.interactable = true;
		_outMenus._optionsMenu._navAudio.interactable = true;
		_outMenus._optionsMenu._navVideo.interactable = true;

		//modify
		_outMenus._optionsMenu._navGameplay.interactable = false;

		//close
		//CloseTitleScreenPanel();
		//CloseLoadSavePanel();

		_outMenus._optionsMenu._videoPanel.SetActive (false);
		_outMenus._optionsMenu._audioPanel.SetActive (false);
		_outMenus._optionsMenu._controlsPanel.SetActive (false);
	}

	public void OpenOptionsVideoPanel() {
		//open
		_outGameMenus.SetActive (true);
		_outMenus._optionsPanel.SetActive (true);
		_outMenus._optionsMenu._videoPanel.SetActive (true);

		_outMenus._outOptionsUIState = AllEnums.OptionsUIState.Video;

		_outMenus._optionsMenu._navGameplay.interactable = true;
		_outMenus._optionsMenu._navControls.interactable = true;
		_outMenus._optionsMenu._navAudio.interactable = true;

		//modify
		_outMenus._optionsMenu._navVideo.interactable = false;

		//close
		CloseTitleScreenPanel();
		CloseLoadSavePanel();

		_outMenus._optionsMenu._gameplayPanel.SetActive (false);
		_outMenus._optionsMenu._audioPanel.SetActive (false);
		_outMenus._optionsMenu._controlsPanel.SetActive (false);
	}
	public void CloseOptionsVideoPanel() {
		_outMenus._optionsMenu._videoPanel.SetActive (false);
		_outMenus._optionsMenu._navVideo.interactable = true;
	}

	public void OpenOptionsAudioPanel() {
		//open
		_outGameMenus.SetActive (true);
		_outMenus._optionsPanel.SetActive (true);
		_outMenus._optionsMenu._audioPanel.SetActive (true);

		_outMenus._outOptionsUIState = AllEnums.OptionsUIState.Audio;

		_outMenus._optionsMenu._navGameplay.interactable = true;
		_outMenus._optionsMenu._navControls.interactable = true;
		_outMenus._optionsMenu._navVideo.interactable = true;

		//modify
		_outMenus._optionsMenu._navAudio.interactable = false;

		//close
		CloseTitleScreenPanel();
		CloseLoadSavePanel();

		_outMenus._optionsMenu._videoPanel.SetActive (false);
		_outMenus._optionsMenu._gameplayPanel.SetActive (false);
		_outMenus._optionsMenu._controlsPanel.SetActive (false);
	}
	public void CloseOptionsAudioPanel() { //not sure if needed
		_outMenus._optionsMenu._audioPanel.SetActive (false);
		_outMenus._optionsMenu._navAudio.interactable = true;
	}

	public void OpenOptionsControlsPanel() {
		//open
		_outGameMenus.SetActive (true);
		_outMenus._optionsPanel.SetActive (true);
		_outMenus._optionsMenu._controlsPanel.SetActive (true);

		_outMenus._outOptionsUIState = AllEnums.OptionsUIState.Controls;

		_outMenus._optionsMenu._navGameplay.interactable = true;
		_outMenus._optionsMenu._navAudio.interactable = true;
		_outMenus._optionsMenu._navVideo.interactable = true;

		//modify
		_outMenus._optionsMenu._navControls.interactable = false;
        if (GameSessionController.Instance._isControllerPluggedIn)
        {
            _outMenus._optionsMenu._controlGamepad.interactable = true;
            if (GameSessionController.Instance._isUsingController)
            {
                SelectGamepadControls();
            }
            else
            {
                SelectKeyboardMouseControls();
            }
        } else
        {
            _outMenus._optionsMenu._controlGamepad.gameObject.SetActive(false);

        }

		//close
		CloseTitleScreenPanel();
		CloseLoadSavePanel();

		_outMenus._optionsMenu._videoPanel.SetActive (false);
		_outMenus._optionsMenu._gameplayPanel.SetActive (false);
		_outMenus._optionsMenu._audioPanel.SetActive (false);
	}
	public void CloseOptionsControlsPanel() {
		_outMenus._optionsMenu._controlsPanel.SetActive (false);
		_outMenus._optionsMenu._navControls.interactable = true;
	}

	public void SelectKeyboardMouseControls() {
		_outMenus._optionsMenu._controlKeyboard.interactable = false;

        if (GameSessionController.Instance._isControllerPluggedIn)
        {
            _outMenus._optionsMenu._controlGamepad.gameObject.SetActive(true);
            _outMenus._optionsMenu._controlGamepad.interactable = true;
        }
	}
	public void SelectGamepadControls() {
		_outMenus._optionsMenu._controlKeyboard.interactable = true;
		_outMenus._optionsMenu._controlGamepad.interactable = false;
	}

	//TODO: RENAME
	public void StartAssignment(Text keyName) {
		if (!_outMenus._optionsMenu._hasKeyToBeMapped) {
			StartCoroutine (AssignKey (keyName.text));
		}
	}
	//TODO: RENAME
	IEnumerator WaitForKey() {
		while (!_outMenus._optionsMenu._keyMapEvent.isKey) {
			yield return null;
		}
	}

	IEnumerator WaitForGamepad1() {
		while (_outMenus._optionsMenu._keyMapEvent.keyCode == KeyCode.None) {
			yield return null;
		}
	}

	IEnumerator WaitForGamepad() {
		bool pressedButton = false;
		System.Array values = System.Enum.GetValues (typeof(KeyCode));
		while (!pressedButton) {
			foreach (KeyCode code in values) {
				if (Input.GetKeyDown (code)) {
					pressedButton = true;
					Debug.Log ("NEW KEYCODE " + code.ToString());
					newKey = code;
					break;
				}
			}
			yield return null;
		}
	}
	//TODO: RENAME
	public IEnumerator AssignKey(string keyName) {
		_outMenus._optionsMenu._hasKeyToBeMapped = true;
		_outMenus._optionsMenu._awaitKeyMapPanel.SetActive (true);
		_outMenus._optionsMenu._awaitKeyPrompt._information._description.text = keyName;
		if (!GameSessionController.Instance._isUsingController) {
			_outMenus._optionsMenu._awaitKeyPrompt._information._stat.text = "(Keyboard)";
			yield return WaitForKey ();

			Debug.Log ("NEW KEYCODE " + _outMenus._optionsMenu._keyMapEvent.keyCode.ToString());

			GameSessionController.Instance._wResources._gInput.SetGameButton (keyName, newKey);
			_outMenus._optionsMenu._curGameButtonToMap.text = newKey.ToString();
		} else {
			_outMenus._optionsMenu._awaitKeyPrompt._information._stat.text = "(Controller/Gamepad)";

			yield return WaitForGamepad ();

			GameSessionController.Instance._wResources._gInput.SetGameButton (keyName, newKey);
			_outMenus._optionsMenu._curGameButtonToMap.text = newKey.ToString();
			_outMenus._optionsMenu._hasKeyToBeMapped = false;
		}

		_outMenus._optionsMenu._awaitKeyMapPanel.SetActive (false);
		yield return null;
	}

	public void OpenLoadSavePanel() {
		_outGameMenus.SetActive (true);
		_outMenus._loadSavePanel.SetActive (true);
		_outMenus._loadSaveMenu._confirmPanel.SetActive (false);
        _outMenus._loadSaveMenu._gameFileParent.SetActive(true);
        _outMenus._loadSaveMenu._navButtons.SetActive(true);
        _outMenus._loadSaveMenu._saveSlotsParent.SetActive(true);

		//modify
		switch (_outMenus._outLoadSaveUIState) {
		case AllEnums.LoadSaveUIState.LoadGame:
			_outMenus._loadSaveMenu._loadSaveDeletePanel.text = "~ Load Game ~";
			if (_outMenus._loadSaveMenu._fromTitle) {
				_outMenus._loadSaveMenu._navSave.gameObject.SetActive (false);
			} else {
				_outMenus._loadSaveMenu._navSave.gameObject.SetActive (true);
			}
			_outMenus._loadSaveMenu._navSave.interactable = true;
			_outMenus._loadSaveMenu._navDelete.interactable = true;

			_outMenus._loadSaveMenu._createNewSaveButton.SetActive (false);
			_outMenus._loadSaveMenu._navLoad.interactable = false;
			break;
		case AllEnums.LoadSaveUIState.SaveGame:
			_outMenus._loadSaveMenu._loadSaveDeletePanel.text = "~ Save Game ~";
			_outMenus._loadSaveMenu._navSave.gameObject.SetActive (true);
			_outMenus._loadSaveMenu._createNewSaveButton.SetActive (true);
			_outMenus._loadSaveMenu._navLoad.interactable = true;
			_outMenus._loadSaveMenu._navDelete.interactable = true;

			_outMenus._loadSaveMenu._navSave.interactable = false;
			break;
		case AllEnums.LoadSaveUIState.DeleteSaves:
			_outMenus._loadSaveMenu._loadSaveDeletePanel.text = "~ Delete Saves ~";
			if (_outMenus._loadSaveMenu._fromTitle) {
				_outMenus._loadSaveMenu._navSave.gameObject.SetActive (false);
			} else {
				_outMenus._loadSaveMenu._navSave.gameObject.SetActive (true);
			}
			_outMenus._loadSaveMenu._navLoad.gameObject.SetActive (true);
			_outMenus._loadSaveMenu._navLoad.interactable = true;
			_outMenus._loadSaveMenu._navSave.interactable = true;

			_outMenus._loadSaveMenu._createNewSaveButton.SetActive (false);
			_outMenus._loadSaveMenu._navDelete.interactable = false;
			break;

		}

		//close
		CloseTitleScreenPanel();
		CloseOptionsPanel ();
	}
	public void CloseLoadSavePanel() {
		_outMenus._loadSaveMenu._navLoad.interactable = true;
		_outMenus._loadSaveMenu._navSave.interactable = true;
		_outMenus._loadSaveMenu._navDelete.interactable = true;

		//modify

		//close
		//CloseTitleScreenPanel();
		//CloseLoadSavePanel();

		_outMenus._loadSavePanel.SetActive (false);
		_outMenus._loadSaveMenu._confirmPanel.SetActive (false);
	}

	public void SelectGameFile1() {
		_outMenus._loadSaveMenu._gameFile1.interactable = false;
		_outMenus._loadSaveMenu._gameFile2.interactable = true;
		_outMenus._loadSaveMenu._gameFile3.interactable = true;
        _outMenus._selectedGameFile = AllEnums.GameFileState.GameFile1;
        LoadCorrectFilesOnUI();
    }
	public void SelectGameFile2() {
		_outMenus._loadSaveMenu._gameFile1.interactable = true;
		_outMenus._loadSaveMenu._gameFile2.interactable = false;
		_outMenus._loadSaveMenu._gameFile3.interactable = true;
        _outMenus._selectedGameFile = AllEnums.GameFileState.GameFile2;
        LoadCorrectFilesOnUI();
    }
	public void SelectGameFile3() {
		_outMenus._loadSaveMenu._gameFile1.interactable = true;
		_outMenus._loadSaveMenu._gameFile2.interactable = true;
		_outMenus._loadSaveMenu._gameFile3.interactable = false;
        _outMenus._selectedGameFile = AllEnums.GameFileState.GameFile3;
        LoadCorrectFilesOnUI();
    }

    private void LoadCorrectFilesOnUI()
    {
        List<GameSaveSlotData> GameSaveSlots = GameSessionController.Instance._gameSavesControl.AllSaveData;
        for (int i = 0; i < GameSaveSlots.Count; i++)
        {
            if (GameSaveSlots[i]._parent)
            {
                if (GameSaveSlots[i]._gameFile == _outMenus._selectedGameFile)
                {
                    GameSaveSlots[i]._parent.gameObject.SetActive(true);
                    GameSaveSlots[i]._parent.gameObject.GetComponent<Button>().interactable = true;
                }
                else
                {
                    GameSaveSlots[i]._parent.gameObject.SetActive(false);
                    GameSaveSlots[i]._parent.gameObject.GetComponent<Button>().interactable = false;
                }
            }
        }
    }
}