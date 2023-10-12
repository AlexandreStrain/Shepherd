using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

namespace Shepherd {
	public class OutGameMenus : MonoBehaviour {

		[Header("Debug")]
		public AudioMixer _mainAudioMixer;
		public AllEnums.OutGameUIState _outGameUIState;

		[Header("Title/Pause Screen")]
		public GameObject _titlePausePanel;
		public TitlePausePanel _titlePauseMenu;

		[Header("Load/Save Screen")]
		public AllEnums.LoadSaveUIState _outLoadSaveUIState;
		public GameObject _loadSavePanel;
		public LoadSavePanel _loadSaveMenu;
        public AllEnums.GameFileState _selectedGameFile;

		[Header("Options Screen")]
		public AllEnums.OptionsUIState _outOptionsUIState;
		public GameObject _optionsPanel;
		public OptionsPanel _optionsMenu;


		[Header("Private Variables")]
		private float _delta;
        [HideInInspector]
        public Resolution[] _availResolutions; //available screen resolutions for our game

        public void PreInit()
        {
            //moved from init
            CreateUIElements();

            //Load Player Settings
            CreateSaveSlotsOnUI();
            UserInterfaceController.Instance.SelectGameFile1(); //temp, will draw this load depending on last played

            //Load Games, if none then ready title screen
            _outGameUIState = AllEnums.OutGameUIState.TitleScreen;
            //UserInterfaceController.Instance.CycleOutGameUIMenus ();
        }

        public void Init() {
           
		}

		public void UpdateControlMappings() {
			//teardown any existing gameobjects
			StatisticsTemplate[] prevMaps = _optionsMenu._controlMapParent.GetComponentsInChildren<StatisticsTemplate>();
			for (int i = 0; i < prevMaps.Length; i++) {
				Destroy (prevMaps [i].gameObject);
			}

			SetupCurrentControlMappings ();
		}

		public void CreateUIElements() {
			SetupCurrentScreenResolution();
			SetupCurrentControlMappings ();
		}

		public void Tick(GameInputs input, float delta) {
			_delta = delta;

			HandleUIState (input);
		}

		private void HandleUIState(GameInputs input) {
            switch (_outGameUIState) {
			case AllEnums.OutGameUIState.None:
				HandlePauseMenu (input);
				break;
			case AllEnums.OutGameUIState.TitleScreen:
				HandleTitleMenu (input);
				break;
			case AllEnums.OutGameUIState.LoadSaveGame:
				break;
			case AllEnums.OutGameUIState.Options:
				HandleOptionsMenu (input);
				break;
			}
        }

		private void HandlePauseMenu(GameInputs input) {
			
		}

		private void HandleTitleMenu(GameInputs input) {
			if (input._pauseMenuButton) {
				Debug.LogWarning ("this shouldn't leave title screen, but it does");
			}
		}

		private void HandleOptionsMenu(GameInputs input) {
			//will depend on which tab is open in the options menu
		}

		private void SetupCurrentScreenResolution() {
            _availResolutions = Screen.resolutions;
            int curResolutionIndex = 0;
			_optionsMenu._resolutionDropdown.ClearOptions ();
			List<string> resolutionOptions = new List<string> ();
			for (int i = 0; i < _availResolutions.Length; i++) {
				string option = _availResolutions [i].width + "x" + _availResolutions [i].height;
				resolutionOptions.Add (option);

				if (_availResolutions [i].width == Screen.currentResolution.width && _availResolutions [i].height == Screen.currentResolution.height) {
					curResolutionIndex = i;
				}
			}
			_optionsMenu._resolutionDropdown.AddOptions (resolutionOptions);
			_optionsMenu._resolutionDropdown.value = curResolutionIndex;
			_optionsMenu._resolutionDropdown.RefreshShownValue ();
		}

		private void SetupCurrentControlMappings() {
			List<GameInputKeys> curControls;
			if (GameSessionController.Instance._isUsingController) {
				curControls = GameSessionController.Instance._wResources._gInput._controlScheme._gamepadInput;
				//CameraController._singleton._playerCam.m_XAxis.m_InvertAxis = true;
			} else {
				curControls = GameSessionController.Instance._wResources._gInput._controlScheme._keyboardInput;
				//CameraController._singleton._playerCam.m_XAxis.m_InvertAxis = false;
			}

			for (int i = 0; i < curControls.Count; i++) {
				GameObject gObject = Instantiate (_optionsMenu._controlMapTemplate) as GameObject;
				gObject.transform.SetParent (_optionsMenu._controlMapParent.transform);
				gObject.transform.localScale = Vector3.one;

				StatisticsTemplate cMapInfo = gObject.GetComponent<StatisticsTemplate> ();
				cMapInfo._description.text = curControls [i]._keyName;
				cMapInfo._stat.text = curControls [i]._key.ToString ();
				//cMapInfo._icon.sprite = ;

				gObject.SetActive (true);
			}
		}

        public void CreateSaveSlotsOnUI()
        {
            List<GameSaveSlotData> GameSaveSlots = GameSessionController.Instance._gameSavesControl.AllSaveData;
            for (int i = 0; i < GameSaveSlots.Count; i++)
            {
                GameObject gObject = Instantiate(_loadSaveMenu._saveInstanceTemplate) as GameObject;
                gObject.transform.SetParent(_loadSaveMenu._saveSlotsParent.transform);
                gObject.transform.localScale = Vector3.one;
                gObject.SetActive(true);


                GameSaveSlotUI sSlot = gObject.GetComponent<GameSaveSlotUI>();
                sSlot._playerName.text = GameSaveSlots[i]._playerName;
                sSlot._timeOfDay.text = GameSaveSlots[i]._timeOfDay;
                sSlot._flockCount.text = "Flock: " + GameSaveSlots[i]._flockCount;
                sSlot._levelName.text = GameSaveSlots[i]._levelName;
                sSlot._saveFile = GameSaveSlots[i]._gameFile;
                sSlot._saveNumber = GameSaveSlots[i]._saveNumber;
                sSlot._saveNumberText.text = GameSaveSlots[i]._saveNumberText;
                sSlot._autoSaveText.SetActive(GameSaveSlots[i]._isAutoSave);
                GameSaveSlots[i]._parent = sSlot;

               /* sSlot._playerName.text = GameSessionController.Instance._inputControl._character._biography._name.ToString();
                sSlot._timeOfDay.text = UserInterfaceController.Instance._timeOfDay.text + " | " + GameSessionController.Instance._MorningOrNight;
                sSlot._flockCount.text = "Flock: " + GameSessionController.Instance._wResourceControl._runtimeReferences._flock.Count;
                sSlot._autoSaveText.SetActive(false);
                sSlot._levelName.text = (AllEnums.StoryActs)GameSessionController.Instance._currentAct + " - " + (AllEnums.StoryChapters)GameSessionController.Instance._currentChapter + " - " + "The TestBuild"; //TODO: Level of name and act come from elsewhere
                sSlot._parent = gObject;

                sSlot._saveNumber = _allGameSlots[i]._saveNumber;
                sSlot._saveNumberText.text = _allGameSlots[i]._saveFile + " - " + "SAVE " + _allGameSlots[i]._saveNumber;  //TODO: Increment save number per save
                sSlot._saveFile = _allGameSlots[i]._saveFile;*/

            }
        }
    }
}