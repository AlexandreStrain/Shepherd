using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Shepherd;
using System;

public class MainMenuButtons : MonoBehaviour {

	public OutGameMenus _outUI;
			
	public void Continue() {
		if (GameSessionController.Instance._firstLaunch) {
			Debug.Log ("Starting game...");
        } else {
			Debug.Log ("Continuing game...");
            if (GameSessionController.Instance._inputControl._character._states._isDead && SceneManager.GetActiveScene().buildIndex == 1)
            {
                SceneManager.LoadScene(1);
            }
        }
		GameSessionController.Instance._curGameState = AllEnums.InputControlState.Game;
		UserInterfaceController.Instance.CloseInGameUI ();

		CameraController.Instance.ResumeAllCameras ();

		UserInterfaceController.Instance._outGameMenus.SetActive (false);
		UserInterfaceController.Instance._inMenus._invState = AllEnums.InventoryUIState.None;

        UserInterfaceController.Instance._toggleOutGameMenu = false;

		_outUI._outGameUIState = AllEnums.OutGameUIState.None;
		//SceneController._singleton.PressStartGame ();
	}

	public void NewGame() {
		Debug.Log ("New game start...");
		_outUI._outGameUIState = AllEnums.OutGameUIState.TitleScreen;
		//revert scenes back to first -- might need to buffer it with a loading screen
	}

	public void LoadGame() {
		Debug.Log ("Loading game from file...");
		_outUI._outLoadSaveUIState = AllEnums.LoadSaveUIState.LoadGame;
		_outUI._outGameUIState = AllEnums.OutGameUIState.LoadSaveGame;
		UserInterfaceController.Instance.CycleOutGameUIMenus ();
	}
	public void SaveGame() {
		Debug.Log ("Preparing to save game into file...");
		_outUI._outGameUIState = AllEnums.OutGameUIState.LoadSaveGame;
		_outUI._outLoadSaveUIState = AllEnums.LoadSaveUIState.SaveGame;
		UserInterfaceController.Instance.CycleOutGameUIMenus ();
	}
	public void DeleteGame() {
		Debug.Log ("Preparing to delete game saves...");
		_outUI._outGameUIState = AllEnums.OutGameUIState.LoadSaveGame;
		_outUI._outLoadSaveUIState = AllEnums.LoadSaveUIState.DeleteSaves;
		UserInterfaceController.Instance.CycleOutGameUIMenus ();
	}
	public void CloseLoadSavePanel() {
		if (_outUI._loadSaveMenu._fromTitle) {
			_outUI._outGameUIState = AllEnums.OutGameUIState.TitleScreen;
		} else {
			_outUI._outGameUIState = AllEnums.OutGameUIState.None;
		}
		UserInterfaceController.Instance.CloseLoadSavePanel ();
		UserInterfaceController.Instance.OpenOutGameUI ();
	}

	public void Options() {
		Debug.Log ("Sending to Options...");
		_outUI._outGameUIState = AllEnums.OutGameUIState.Options;
		UserInterfaceController.Instance.CycleOutGameUIMenus ();
	}

	public void ConfirmQuit(bool toDesktop) {
		if (toDesktop) {
			_outUI._titlePauseMenu._confirmQuitText.text = "Quit to Desktop?";
			_outUI._titlePauseMenu._confirmToDesktopButton.SetActive (true);
			_outUI._titlePauseMenu._confirmToTitleButton.SetActive (false);
		} else {
			_outUI._titlePauseMenu._confirmQuitText.text = "Quit to Title?"; 
			_outUI._titlePauseMenu._confirmToTitleButton.SetActive (true);
			_outUI._titlePauseMenu._confirmToDesktopButton.SetActive (false);
		}
		if (_outUI._outGameUIState == AllEnums.OutGameUIState.None) {
			_outUI._titlePauseMenu._confirmQuitText.text += "\n(Unsaved Progress will be lost!)";
		}
		_outUI._titlePauseMenu._confirmPanel.SetActive (true);
		_outUI._titlePauseMenu._mainButtons.SetActive (false);
	}
	public void CancelQuit() {
		_outUI._titlePauseMenu._mainButtons.SetActive (true);
		_outUI._titlePauseMenu._confirmPanel.SetActive (false);
	}

	public void QuitToTitle() {
		Debug.Log ("Returning to TitleScreen");
		_outUI._outGameUIState = AllEnums.OutGameUIState.TitleScreen;
		CancelQuit ();
		UserInterfaceController.Instance.CycleOutGameUIMenus ();
	}

	public void Quit() {
		Debug.Log ("Exiting game...");
		_outUI._outGameUIState = AllEnums.OutGameUIState.None;
		CancelQuit ();
		Application.Quit ();
	}


	//Options Menu -- General Navigation
	public void OpenAudioPanel() {
		_outUI._outOptionsUIState = AllEnums.OptionsUIState.Audio;
		UserInterfaceController.Instance.CycleOptionsUIMenus ();
	}
	public void OpenVideoPanel() {
		_outUI._outOptionsUIState = AllEnums.OptionsUIState.Video;
		UserInterfaceController.Instance.CycleOptionsUIMenus ();
	}
	public void OpenControlsPanel() {
		_outUI._outOptionsUIState = AllEnums.OptionsUIState.Controls;
		UserInterfaceController.Instance.CycleOptionsUIMenus ();
	}
	public void CloseOptionsPanel() {
		if (_outUI._loadSaveMenu._fromTitle) {
			_outUI._outGameUIState = AllEnums.OutGameUIState.TitleScreen;
		} else {
			_outUI._outGameUIState = AllEnums.OutGameUIState.None;
		}
		_outUI._outOptionsUIState = AllEnums.OptionsUIState.Gameplay;
		UserInterfaceController.Instance.CloseOptionsPanel ();
		UserInterfaceController.Instance.OpenOutGameUI ();
	}

	//Options Menu -- Video
	//TODO: Expand this so that they can pick and choose what they want (ie. their own graphics settings and not a prechosen selection)
	public void SetGraphicsQuality(int qualityIndex) {
		QualitySettings.SetQualityLevel (qualityIndex);

		//update
		_outUI._optionsMenu._vsyncDropdown.value = QualitySettings.vSyncCount;
		_outUI._optionsMenu._shadowQualityDropdown.value = (int)QualitySettings.shadowResolution;
		_outUI._optionsMenu._shadowTypesDropdown.value = (int)QualitySettings.shadows;
		_outUI._optionsMenu._antiAliasDropdown.value = QualitySettings.antiAliasing;
		_outUI._optionsMenu._anisotropicDropdown.value = (int)QualitySettings.anisotropicFiltering;
		_outUI._optionsMenu._textureQualityDropdown.value = QualitySettings.masterTextureLimit;

		_outUI._optionsMenu.RefreshVideoPanel();
	}

	public void SetScreenMode(int screenIndex) {
		Screen.fullScreenMode = (FullScreenMode)screenIndex;
	}

	public void SetScreenResolution(int resolutionIndex) {
		Resolution newResolution = _outUI._availResolutions [resolutionIndex];
		Screen.SetResolution (newResolution.width, newResolution.height, Screen.fullScreenMode);
	}

	public void SetFramerate(int framerateIndex) {
		switch (framerateIndex) {
		case 0:
			Application.targetFrameRate = 24;
			break;
		case 1:
			Application.targetFrameRate = 30;
			break;
		case 2:
			Application.targetFrameRate = 60;
			break;
		case 3:
			Application.targetFrameRate = 120;
			break;
		case 4:
			Application.targetFrameRate = -1; //Default, which is maximum achieveable framerate per platform
			break;
		}
		//cancels vsync
		QualitySettings.vSyncCount = 0;
		_outUI._optionsMenu._vsyncDropdown.value = 0;

		//update
		_outUI._optionsMenu.RefreshVideoPanel();
	}

	public void SetAntiAliasing(int aliasIndex) {
		QualitySettings.antiAliasing = aliasIndex;

		//update
		_outUI._optionsMenu.RefreshVideoPanel();
	}

	public void SetAnisotropicTextures(int anisoIndex) {
		QualitySettings.anisotropicFiltering = (AnisotropicFiltering)anisoIndex;

		//update
		_outUI._optionsMenu.RefreshVideoPanel();
	}

	public void SetShadowQuality(int shadowIndex) {
		QualitySettings.shadowResolution = (ShadowResolution)shadowIndex;
		//update
		_outUI._optionsMenu.RefreshVideoPanel();
	}

	public void SetShadowTypes(int shadowIndex) {
		QualitySettings.shadows = (ShadowQuality)shadowIndex;
		_outUI._optionsMenu.RefreshVideoPanel();
	}

	public void SetVsync(int vsyncIndex) {
		QualitySettings.vSyncCount = vsyncIndex;

		//update
		_outUI._optionsMenu.RefreshVideoPanel();
	}

	public void SetTextureQuality(int textureIndex) {
		QualitySettings.masterTextureLimit = textureIndex;

		//update
		_outUI._optionsMenu.RefreshVideoPanel();
	}

	//Options Menu -- Audio
	public void SetMasterVolume(float newVolume) {
		_outUI._mainAudioMixer.SetFloat ("Master Volume", newVolume);
		_outUI._optionsMenu._masterValueText.text = (Mathf.Round(newVolume+100f)).ToString ();
	}
	public void SetMusicVolume(float newVolume) {
		_outUI._mainAudioMixer.SetFloat ("Music Volume", newVolume);
		_outUI._optionsMenu._musicValueText.text = (Mathf.Round(newVolume+100f)).ToString ();
	}
	public void SetAmbientVolume(float newVolume) {
		_outUI._mainAudioMixer.SetFloat ("Ambient Volume", newVolume);
		_outUI._optionsMenu._ambientValueText.text = (Mathf.Round(newVolume+100f)).ToString ();
	}
	public void SetSoundEffectsVolume(float newVolume) {
		_outUI._mainAudioMixer.SetFloat ("SFX Volume", newVolume);
		_outUI._optionsMenu._sfxValueText.text = (Mathf.Round(newVolume+100f)).ToString ();
	}
	public void SetDialogueVolume(float newVolume) {
		_outUI._mainAudioMixer.SetFloat ("Dialogue Volume", newVolume);
		_outUI._optionsMenu._dialogueValueText.text = (Mathf.Round(newVolume+100f)).ToString ();
	}
	public void SetSubtitles(bool isOn) {
		if (isOn) {
			_outUI._optionsMenu._subtitlesText.text = "On";
			DialogueController._singleton._subtitlesOn = true;
		} else {
			_outUI._optionsMenu._subtitlesText.text = "Off";
			DialogueController._singleton._subtitlesOn = false;
		}
	}

	//Options -- Controls
	public void AwaitNewKeyMapping(Text fromGameButton) {
		_outUI._optionsMenu._curGameButtonToMap = fromGameButton;
	}

	public void UseKeyboardControls() {
		GameSessionController.Instance._isUsingController = false;
		UserInterfaceController.Instance.SelectKeyboardMouseControls ();
		_outUI.UpdateControlMappings ();
		GameSessionController.Instance._wResources._gInput.Init ();
	}

	public void UseGamepadControls() {
		if (GameSessionController.Instance._isControllerPluggedIn) {
			GameSessionController.Instance._isUsingController = true;
			UserInterfaceController.Instance.SelectGamepadControls ();
			_outUI.UpdateControlMappings ();
			GameSessionController.Instance._wResources._gInput.Init ();
		} else
        {
            UseKeyboardControls();
        }
	}

	

	//Load-Save Panel
	public void CreateNewSave(bool isAutoSave) {
		GameObject gObject = Instantiate (_outUI._loadSaveMenu._saveInstanceTemplate) as GameObject;
		gObject.transform.SetParent (_outUI._loadSaveMenu._saveSlotsParent.transform);
		gObject.transform.localScale = Vector3.one;
		gObject.SetActive (true);

        string[] tempTime = GameSessionController.Instance._dayNightCycle._currentTime.ToString().Split(":"[0]);

        string levelName = "" + GameSessionController.Instance._currentAct + GameSessionController.Instance._currentChapter + GameSessionController.Instance._currentScene;
        RuntimeReferences rtRefs = GameSessionController.Instance._wResources._runtimeReferences;

        GameSaveSlotUI sSlot = gObject.GetComponent<GameSaveSlotUI> ();
        GameSaveSlotData sSlotData = new GameSaveSlotData
        {
            _playerName = GameSessionController.Instance._inputControl._character._biography._name.ToString(),
            _timeOfDay = tempTime[0]+":"+tempTime[1] + " | " + GameSessionController.Instance._MorningOrNight,
            _currentTime = GameSessionController.Instance._gameTimeOfDay,
            _isAutoSave = isAutoSave,
            _levelName = (AllEnums.StoryActs)GameSessionController.Instance._currentAct + " - " + GameSessionController.Instance._wResources._levelNames.GetLevelName(levelName),
            _saveNumber = GetSavesFromFile(UserInterfaceController.Instance._outMenus._selectedGameFile).Count,
            _saveNumberText = UserInterfaceController.Instance._outMenus._selectedGameFile + " - " + "SAVE " + sSlot._saveNumber,
            _actNumber = GameSessionController.Instance._currentAct,
            _chapterNumber = GameSessionController.Instance._currentChapter,
            _sceneNumber = GameSessionController.Instance._currentScene,
            _gameFile = UserInterfaceController.Instance._outMenus._selectedGameFile,
            _testRefs = GameSessionController.Instance._wResources._runtimeReferences,
            _currentStatistics = GameSessionController.Instance._inputControl._character._biography._currentBioCard._currentStatistics
        };
        Debug.LogWarning("THIS IS TEMPORARY");
        sSlotData._chapterNumber += 1;
        sSlotData._sceneNumber = 0;
        Debug.LogWarning("END OF TEMPORARY");

        //CURRENT EQUIPMENT
        if (rtRefs._rightHand._value.Count == 0)
        {
            rtRefs._rightHand._value.Add(GameSessionController.Instance._wResources.GetItem("Unarmed"));
        }
        for (int i = 0; i < rtRefs._rightHand._value.Count; i++)
        {
            sSlotData._currentRightHand.Add(rtRefs._rightHand._value[i]._itemName);
        }
        if (rtRefs._leftHand._value.Count == 0)
        {
            rtRefs._leftHand._value.Add(GameSessionController.Instance._wResources.GetItem("Unarmed"));
        }
        for (int i = 0; i < rtRefs._leftHand._value.Count; i++)
        {
            sSlotData._currentLeftHand.Add(rtRefs._leftHand._value[i]._itemName);
        }
     
        for (int i = 0; i < rtRefs._consumable._value.Count; i++)
        {
            sSlotData._currentConsumables.Add(rtRefs._consumable._value[i]._itemName);
        }
        for (int i = 0; i < rtRefs._armor._value.Count; i++)
        {
            sSlotData._currentArmor.Add(rtRefs._armor._value[i]._itemName);
        }
        //RUNTIME EQUIPMENT
        for (int i = 0; i < rtRefs._runtimeWeapons.Count; i++)
        { 
            rtRefs._runtimeWeapons[i]._itemName = rtRefs._runtimeWeapons[i]._instance._itemName;
            rtRefs._runtimeWeapons[i]._instance = GameSessionController.Instance._wResources.GetItem(rtRefs._runtimeWeapons[i]._instance._itemName);
            
            sSlotData._allWeapons.Add(rtRefs._runtimeWeapons[i]);
        }
        for (int i = 0; i < rtRefs._runtimeConsumables.Count; i++)
        {
            rtRefs._runtimeConsumables[i]._itemName = rtRefs._runtimeConsumables[i]._instance._itemName;
            rtRefs._runtimeConsumables[i]._instance = GameSessionController.Instance._wResources.GetItem(rtRefs._runtimeConsumables[i]._instance._itemName);
            sSlotData._allConsumables.Add(rtRefs._runtimeConsumables[i]);
        }
        for (int i = 0; i < rtRefs._runtimeArmors.Count; i++)
        {
            rtRefs._runtimeArmors[i]._itemName = rtRefs._runtimeArmors[i]._instance._itemName;
            rtRefs._runtimeArmors[i]._instance = GameSessionController.Instance._wResources.GetItem(rtRefs._runtimeArmors[i]._instance._itemName);
            sSlotData._allArmor.Add(rtRefs._runtimeArmors[i]);
        }

        //OTHER
        sSlotData._goldCount = rtRefs._currentGold;
        sSlotData._previousGoldCount = rtRefs._prevGold;
        sSlotData._flockCount = rtRefs._flock.Count;
        sSlotData._previousFlockCount = rtRefs._prevFlockCount;

        for (int i = 0; i < sSlotData._testRefs._flock.Count; i++)
        {
            BiocardSaveData biocardSaveData = new BiocardSaveData();
            biocardSaveData.CopyDataFromBiocard(sSlotData._testRefs._flock[i]._biography._currentBioCard);
            biocardSaveData._gender = sSlotData._testRefs._flock[i]._biography._gender;
            biocardSaveData._faction = sSlotData._testRefs._flock[i]._biography._faction;
            biocardSaveData._nameOfCharacter = sSlotData._testRefs._flock[i]._biography._name.ToString().TrimEnd('\r');
            sSlotData._allFlock.Add(biocardSaveData);
        }

        foreach(KeyValuePair<Biography, BioCard> entry in GameSessionController.Instance._wResources._runtimeReferences._runtimeNetwork)
        {
            BiocardSaveData biocardSaveData = new BiocardSaveData();
            biocardSaveData.CopyDataFromBiocard(entry.Value);
            biocardSaveData._nameOfCharacter = entry.Key._name.ToString().TrimEnd('\r');
            sSlotData._network.Add(biocardSaveData);
        }


        sSlot._playerName.text = sSlotData._playerName;
        sSlot._timeOfDay.text = sSlotData._timeOfDay;
        sSlot._flockCount.text = "Flock: " + sSlotData._flockCount;
		sSlot._autoSaveText.SetActive (sSlotData._isAutoSave);
        sSlot._levelName.text = sSlotData._levelName;

        sSlot._saveNumber = sSlotData._saveNumber;
        sSlot._saveNumberText.text = sSlotData._saveNumberText;
        sSlot._saveFile = sSlotData._gameFile;

       
        //TEST
        GameSessionController.Instance._gameSavesControl.SaveGame(sSlotData);
        sSlotData._parent = sSlot;
        GameSessionController.Instance._gameSavesControl.AllSaveData.Add(sSlotData); //CAN BE MOVED INTO GAMESAVESCONTROLLER
        GameSessionController.Instance._currentSave = GetSavesFromFile(GameSessionController.Instance._currentSaveFile).Count;
    }
	public void ConfirmSaveSlot(GameSaveSlotUI slot) {
		_outUI._loadSaveMenu._slotToBeUpdated = slot;

        //update
        _outUI._loadSaveMenu._saveSlotInQuestion._saveFile = slot._saveFile;
        _outUI._loadSaveMenu._saveSlotInQuestion._saveNumber = slot._saveNumber;
		_outUI._loadSaveMenu._saveSlotInQuestion._playerName.text = slot._playerName.text;
		_outUI._loadSaveMenu._saveSlotInQuestion._flockCount.text = slot._flockCount.text;
		_outUI._loadSaveMenu._saveSlotInQuestion._levelName.text = slot._levelName.text;
		_outUI._loadSaveMenu._saveSlotInQuestion._saveNumberText.text = slot._saveNumberText.text;
		_outUI._loadSaveMenu._saveSlotInQuestion._timeOfDay.text = slot._timeOfDay.text;
		if (slot._isAutoSave) {
			_outUI._loadSaveMenu._saveSlotInQuestion._autoSaveText.SetActive (true);
		} else {
			_outUI._loadSaveMenu._saveSlotInQuestion._autoSaveText.SetActive (false);
		}

		switch (_outUI._outLoadSaveUIState) {
		case AllEnums.LoadSaveUIState.LoadGame:
			_outUI._loadSaveMenu._confirmActionText.text = "Load Save?";
			_outUI._loadSaveMenu._loadSaveButton.SetActive (true);
			_outUI._loadSaveMenu._overwriteSaveButton.SetActive (false);
			_outUI._loadSaveMenu._deleteSaveButton.SetActive (false);
			break;
		case AllEnums.LoadSaveUIState.SaveGame:
			_outUI._loadSaveMenu._confirmActionText.text = "Overwrite Save?"; 
			_outUI._loadSaveMenu._overwriteSaveButton.SetActive (true);
			_outUI._loadSaveMenu._loadSaveButton.SetActive (false);
			_outUI._loadSaveMenu._deleteSaveButton.SetActive (false);
			break;
		case AllEnums.LoadSaveUIState.DeleteSaves:
			_outUI._loadSaveMenu._confirmActionText.text = "Delete Save?"; 
			_outUI._loadSaveMenu._deleteSaveButton.SetActive (true);
			_outUI._loadSaveMenu._loadSaveButton.SetActive (false);
			_outUI._loadSaveMenu._overwriteSaveButton.SetActive (false);
			break;
		}

		_outUI._loadSaveMenu._gameFileParent.SetActive (false);
		_outUI._loadSaveMenu._navButtons.SetActive (false);
		_outUI._loadSaveMenu._saveSlotsParent.SetActive (false);
		_outUI._loadSaveMenu._confirmPanel.SetActive (true);
	}
	public void CancelSaveSlot() {
		_outUI._loadSaveMenu._gameFileParent.SetActive (true);
		_outUI._loadSaveMenu._navButtons.SetActive (true);
		_outUI._loadSaveMenu._saveSlotsParent.SetActive (true);
		_outUI._loadSaveMenu._confirmPanel.SetActive (false);

        if(UserInterfaceController.Instance._outMenus._selectedGameFile == AllEnums.GameFileState.GameFile1)
        {
            UserInterfaceController.Instance.SelectGameFile1();
        } else if (UserInterfaceController.Instance._outMenus._selectedGameFile == AllEnums.GameFileState.GameFile2)
        {
            UserInterfaceController.Instance.SelectGameFile2();
        } else if (UserInterfaceController.Instance._outMenus._selectedGameFile == AllEnums.GameFileState.GameFile3)
        {
            UserInterfaceController.Instance.SelectGameFile3();
        }
    }

	//temp
	public void DeleteSaveSlot() {
        List<GameSaveSlotData> GameSaveSlots = GameSessionController.Instance._gameSavesControl.AllSaveData;
        for (int i = 0; i < GameSaveSlots.Count; i++) {
			if (GameSaveSlots[i]._saveNumber == _outUI._loadSaveMenu._slotToBeUpdated._saveNumber && GameSaveSlots[i]._gameFile == _outUI._loadSaveMenu._slotToBeUpdated._saveFile) {

                GameSessionController.Instance._gameSavesControl.DeleteSaveSlot(GameSaveSlots[i]);
                Destroy(GameSaveSlots[i]._parent.gameObject);
                GameSaveSlots.RemoveAt(i);
                //GameSessionController.Instance._currentSave = GetSavesFromFile(_outUI._allGameSlots[i]._gameFile).Count;
                break;
			}
		}
		CancelSaveSlot ();
	}

    //TODO: THIS IS VERY SIMILAR TO CREATE NEW SAVE
	public void OverwriteSaveSlot() {

		GameSaveSlotUI sSlot = null;
        GameSaveSlotData sSlotData = null;
        List<GameSaveSlotData> GameSaveSlots = GameSessionController.Instance._gameSavesControl.AllSaveData;

        for (int i = 0; i < GameSaveSlots.Count; i++) {
			if (GameSaveSlots[i]._saveNumber == _outUI._loadSaveMenu._slotToBeUpdated._saveNumber && GameSaveSlots[i]._gameFile == _outUI._loadSaveMenu._slotToBeUpdated._saveFile) {
				sSlot = GameSaveSlots[i]._parent;
                sSlotData = GameSaveSlots[i];
				break;
			}
		}

        string[] tempTime = GameSessionController.Instance._dayNightCycle._currentTime.ToString().Split(":"[0]);

        if (sSlot != null && sSlotData != null) {
            string levelName = "" + GameSessionController.Instance._currentAct + GameSessionController.Instance._currentChapter + GameSessionController.Instance._currentScene;
            RuntimeReferences rtRefs = GameSessionController.Instance._wResources._runtimeReferences;

            sSlotData._playerName = GameSessionController.Instance._inputControl._character._biography._name;
            sSlotData._timeOfDay = tempTime[0]+":"+tempTime[1] + " | " + GameSessionController.Instance._MorningOrNight;
            sSlotData._currentTime = GameSessionController.Instance._gameTimeOfDay;
            sSlotData._isAutoSave = false;
            sSlotData._levelName = (AllEnums.StoryActs)GameSessionController.Instance._currentAct + " - " + GameSessionController.Instance._wResources._levelNames.GetLevelName(levelName);
            sSlotData._saveNumberText = UserInterfaceController.Instance._outMenus._selectedGameFile + " - " + "SAVE " + sSlotData._saveNumber;  //TODO: Increment save number per save
            sSlotData._actNumber = GameSessionController.Instance._currentAct;
            sSlotData._chapterNumber = GameSessionController.Instance._currentChapter;
            sSlotData._sceneNumber = GameSessionController.Instance._currentScene;
            sSlotData._gameFile = GameSessionController.Instance._currentSaveFile;
            sSlotData._testRefs = GameSessionController.Instance._wResources._runtimeReferences;
            //CURRENT EQUIPMENT
            sSlotData._currentArmor.Clear();
            sSlotData._currentConsumables.Clear();
            sSlotData._currentLeftHand.Clear();
            sSlotData._currentRightHand.Clear();
            for (int i = 0; i < rtRefs._rightHand._value.Count; i++)
            {
                sSlotData._currentRightHand.Add(rtRefs._rightHand._value[i]._itemName);
            }
            if (!sSlotData._currentRightHand.Contains("Unarmed") && sSlotData._currentRightHand.Count == 0)
            {
                sSlotData._currentRightHand.Add("Unarmed");
            }
            for (int i = 0; i < rtRefs._leftHand._value.Count; i++)
            {
                sSlotData._currentLeftHand.Add(rtRefs._leftHand._value[i]._itemName);
            }
            if (!sSlotData._currentLeftHand.Contains("Unarmed") && sSlotData._currentLeftHand.Count == 0)
            {
                sSlotData._currentLeftHand.Add("Unarmed");
            }
            for (int i = 0; i < rtRefs._consumable._value.Count; i++)
            {
                sSlotData._currentConsumables.Add(rtRefs._consumable._value[i]._itemName);
            }
            for (int i = 0; i < rtRefs._armor._value.Count; i++)
            {
                sSlotData._currentArmor.Add(rtRefs._armor._value[i]._itemName);
            }
            //RUNTIME EQUIPMENT
            sSlotData._allArmor.Clear();
            sSlotData._allConsumables.Clear();
            sSlotData._allWeapons.Clear();
            for (int i = 0; i < rtRefs._runtimeWeapons.Count; i++)
            {
                rtRefs._runtimeWeapons[i]._instance = GameSessionController.Instance._wResources.GetItem(rtRefs._runtimeWeapons[i]._itemName);
                sSlotData._allWeapons.Add(rtRefs._runtimeWeapons[i]);
            }
            for (int i = 0; i < rtRefs._runtimeConsumables.Count; i++)
            {
                rtRefs._runtimeConsumables[i]._instance = GameSessionController.Instance._wResources.GetItem(rtRefs._runtimeConsumables[i]._itemName);
                sSlotData._allConsumables.Add(rtRefs._runtimeConsumables[i]);
            }
            for (int i = 0; i < rtRefs._runtimeArmors.Count; i++)
            {
                rtRefs._runtimeArmors[i]._instance = GameSessionController.Instance._wResources.GetItem(rtRefs._runtimeArmors[i]._itemName);
                sSlotData._allArmor.Add(rtRefs._runtimeArmors[i]);
            }

            //OTHER
            sSlotData._goldCount = rtRefs._currentGold;
            sSlotData._previousGoldCount = rtRefs._prevGold;
            sSlotData._flockCount = rtRefs._flock.Count;
            sSlotData._previousFlockCount = rtRefs._prevFlockCount;


            sSlotData._currentStatistics = GameSessionController.Instance._inputControl._character._biography._currentBioCard._currentStatistics;

            sSlot._playerName.text = sSlotData._playerName;
            sSlot._timeOfDay.text = sSlotData._timeOfDay;
            sSlot._flockCount.text = "Flock: " + sSlotData._flockCount.ToString();
			sSlot._autoSaveText.SetActive (sSlotData._isAutoSave);
            sSlot._levelName.text = sSlotData._levelName;
            sSlot._saveNumberText.text = sSlotData._saveNumberText;
            sSlot._saveFile = sSlotData._gameFile;

            //test
            GameSessionController.Instance._gameSavesControl.SaveGame(sSlotData);
            sSlotData._parent = sSlot;

        } else {
			CreateNewSave (false);
		}
        CancelSaveSlot();
	}

    public void LoadGameFromSaveSlot()
    {
        GameSessionController.Instance._previousScene = "" + GameSessionController.Instance._currentAct + GameSessionController.Instance._currentChapter + GameSessionController.Instance._currentScene;
        GameSaveSlotData data = GameSessionController.Instance._gameSavesControl.GetCurrentSave(_outUI._loadSaveMenu._slotToBeUpdated._saveNumber, _outUI._loadSaveMenu._slotToBeUpdated._saveFile);
        GameSessionController.Instance._currentAct = data._actNumber;
        GameSessionController.Instance._currentChapter = data._chapterNumber;
        GameSessionController.Instance._currentScene = data._sceneNumber;
        GameSessionController.Instance._loadFromFile = true;
        GameSessionController.Instance.currentData = GameSessionController.Instance._gameSavesControl.GetCurrentSave(_outUI._loadSaveMenu._slotToBeUpdated._saveNumber, _outUI._loadSaveMenu._slotToBeUpdated._saveFile);
        GameSessionController.Instance._gameSavesControl.LoadGame(GameSessionController.Instance.currentData);
        GameSessionController.Instance._successfulLaunch = false;
        UserInterfaceController.Instance._outMenus._outGameUIState = AllEnums.OutGameUIState.None;
        GameSessionController.Instance._userInterfaceControl.CloseLoadSavePanel();
        GameSessionController.Instance._userInterfaceControl.CloseOutGameUI();
    }

    public void LoadGameFile1Saves()
    {
        UserInterfaceController.Instance.SelectGameFile1();
        GameSessionController.Instance._currentSaveFile = AllEnums.GameFileState.GameFile1;
        //GameSessionController.Instance._previousScene = GameSessionController.Instance._currentScene;
        GameSessionController.Instance._currentSave = 0;
    }
    public void LoadGameFile2Saves()
    {
        UserInterfaceController.Instance.SelectGameFile2();
        GameSessionController.Instance._currentSaveFile = AllEnums.GameFileState.GameFile2;
       // GameSessionController.Instance._previousScene = GameSessionController.Instance._currentScene;
        GameSessionController.Instance._currentSave = 0;
    }
    public void LoadGameFile3Saves()
    {
        UserInterfaceController.Instance.SelectGameFile3();
        GameSessionController.Instance._currentSaveFile = AllEnums.GameFileState.GameFile3;
        //GameSessionController.Instance._previousScene = GameSessionController.Instance._currentScene;
        GameSessionController.Instance._currentSave = 0;
    }

    public List<GameSaveSlotUI> GetSavesFromFile(AllEnums.GameFileState saveFile)
    {

        List<GameSaveSlotUI> savesRequested = new List<GameSaveSlotUI>();
        List<GameSaveSlotData> GameSaveSlots = GameSessionController.Instance._gameSavesControl.AllSaveData;
        for (int i = 0; i < GameSaveSlots.Count; i++)
        {

            if (GameSaveSlots[i]._parent)
            {
                if (GameSaveSlots[i]._gameFile == saveFile)
                {
                    GameSaveSlots[i]._parent.gameObject.SetActive(true);
                    savesRequested.Add(GameSaveSlots[i]._parent);
                }
                else
                {
                    GameSaveSlots[i]._parent.gameObject.SetActive(false);
                }
            }
        }
        return savesRequested;
    }
}