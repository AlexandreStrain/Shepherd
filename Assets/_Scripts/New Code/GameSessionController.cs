using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Shepherd {
	public class GameSessionController : Singleton<GameSessionController> {
        
        [Header("Debug")]
        public bool _firstLaunch; //also should be first launch if there are no saved game files... should bring user to first level and title screen
        public GameSavesController _gameSavesControl;
		public WorldResourceController _wResources;
        public CameraController _cameraControl;
        public UserInterfaceController _userInterfaceControl;
        public SceneController _sceneControl;
        public LevelController _levelControl;
		public InputController _inputControl;

        public bool _successfulLaunch;
        public bool _loadFromFile;

        public bool _isControllerPluggedIn;
		public bool _isUsingController;

		[Header("Testing")]
		public AllEnums.InputControlState _curGameState;
        public GameObject _UserInterfaceGameObject;
        public GameSaveSlotData currentData;

        [Header("Other")]
		//Temp
		public int _equippedSet;
		public EquipmentData _startingSet1;
		public EquipmentData _startingSet2;
		public GameEvent _onPlayerUpdate;

        [Header("World")]
        public bool _endOfDay;
        public GameObject _currentWorld;
        [HideInInspector]
        public DayNightCycle _dayNightCycle;
        public float _gameTimeOfDay;
        public string _MorningOrNight;
        public int _currentAct;
        public int _currentChapter;
        public int _currentScene;
        public string _previousScene;
        public AllEnums.GameFileState _currentSaveFile;
        public int _currentSave;

        void Awake()
        {
            Debug.Log(this.name);
            this.enabled = true;
            _successfulLaunch = false;

            //  (1) Load in all world resources, and initialize defaults
            _wResources = Resources.Load("WorldResourceControl") as WorldResourceController;
            _wResources.Init();

            //  (2)  Load in Game Saves Data, initialize save locations and load in most recent game file data
            _gameSavesControl = GameSavesController.Instance;
            //Utilities.Singleton<GameSavesController>.Instance.Init();
            _gameSavesControl.Init();

            // (3)  Load in User Interface Controller and Initialize it for use
            _userInterfaceControl = UserInterfaceController.Instance;
            _userInterfaceControl.PreInit();

            // (4) Connect Camera
            _cameraControl = CameraController.Instance;

            _sceneControl = SceneController.Instance;

            _currentSaveFile = AllEnums.GameFileState.GameFile1;
            _currentSave = (_gameSavesControl.GameFile1SavesCount - 1 < 0) ? 0 : _gameSavesControl.GameFile1SavesCount - 1;
            currentData = _gameSavesControl.GetCurrentSave(_currentSave, _currentSaveFile);
            if (currentData != null)
            {
                _previousScene = "00-1";
                Debug.Log("Attempting to load data from file -- wish me luck!");
                _gameSavesControl.LoadGame(currentData);
                Instance._loadFromFile = true;
            }
            else
            {
                Instance._currentAct = 0;
                Instance._currentChapter = 0;
                Instance._currentScene = 1;
                _sceneControl.LoadGameScene("001");
                _previousScene = "00-1";
                Instance._loadFromFile = false;
            }
        }

		public void Init () {
            _currentWorld = GameObject.Find("World");
            _dayNightCycle = _currentWorld.GetComponent<DayNightCycle>();
            _dayNightCycle._timeText = _userInterfaceControl._HUDScripts.GetComponent<InfoFeed>()._currentTimeOfDayText;
            _dayNightCycle._timeOfDayImage = _userInterfaceControl._HUDScripts.GetComponent<InfoFeed>()._currentTimeOfDayImage;

            _levelControl = _currentWorld.GetComponent<LevelController>();

            if(Instance._loadFromFile)
            {
                if (GameSessionController.Instance._levelControl._flockSpawn)
                {
                    SpawnPoint flockSpawn = GameSessionController.Instance._levelControl._flockSpawn.GetComponent<SpawnPoint>();

                    if (flockSpawn)
                    {
                        flockSpawn._numberOfCharacters = currentData._allFlock.Count;
                        GameSessionController.Instance._levelControl._spawnFlock = true;
                    } else
                    {
                        GameSessionController.Instance._levelControl._spawnFlock = false;
                        Debug.LogWarning("FLOCK SPAWN EXISTS WITHOUT A SPAWN POINT SCRIPT ATTACHED TO IT");
                    }
                }
                else
                {
                    GameSessionController.Instance._levelControl._spawnFlock = false;
                }
                _wResources.InitPlayerFromFile(currentData);
            } else
            {
                GameSessionController.Instance._levelControl._spawnFlock = true;
                _wResources._runtimeReferences.isFromFile = false;
                _wResources.InitPlayerInventory();
                

                if (_equippedSet == 0)
                {
                    _wResources.CopyInventoryFromEquipData(_startingSet1);
                }
                else if (_equippedSet == 1)
                {
                    _wResources.CopyInventoryFromEquipData(_startingSet2);
                }
                
            }

            _inputControl.Init(_wResources);
            if (currentData != null && currentData._currentStatistics != null)
            {
                _inputControl._character._body.InitFromStats(currentData._currentStatistics);
            }

            _onPlayerUpdate.Raise();


            _levelControl.LoadLevelScene(0);


            //Init ui
            _userInterfaceControl.Init(_inputControl._character, _inputControl);
            _successfulLaunch = true;
        }

		void Update() {
            if (!_successfulLaunch)
            {
                return;
            }
            //listens each frame to see if a controller is plugged in
			if (Input.GetJoystickNames ().Length > 0 && Input.GetJoystickNames () [0].Length > 0) {
				//controller plugged in -- use controller layout
                if(!_isControllerPluggedIn)
                {
                    //make controller option available
                    UserInterfaceController.Instance.CycleOptionsUIMenus();
                }
				_isControllerPluggedIn = true;
			} else {
				//no controller plugged in -- use keyboard & mouse layout
				_isControllerPluggedIn = false;
                
                //revert to keyboard controls if previously was using controller!
                if (_isUsingController)
                {
                    UserInterfaceController.Instance._outMenus.UpdateControlMappings();
                    _wResources._gInput.Init();
                    //remove availabilty of controller being used
                    _isUsingController = false;
                    UserInterfaceController.Instance.CycleOptionsUIMenus();
                }

			}

			switch (_curGameState) {
			case AllEnums.InputControlState.OutGame:
				break;
			case AllEnums.InputControlState.InGameMenu:
                    if (Mathf.RoundToInt(_gameTimeOfDay) >= 64800 && Mathf.RoundToInt(_gameTimeOfDay) <= 64900)
                    {
                        _gameTimeOfDay = (_dayNightCycle.DayInSeconds / _dayNightCycle.DayInHours) * (int)AllEnums.TimeOfDay.Dusk;
                        _endOfDay = true;
                    }
                    else if (Mathf.RoundToInt(_gameTimeOfDay) >= 21600 && Mathf.RoundToInt(_gameTimeOfDay) <= 21700)
                    {
                        _gameTimeOfDay = (_dayNightCycle.DayInSeconds / _dayNightCycle.DayInHours) * (int)AllEnums.TimeOfDay.Dawn;
                        _endOfDay = true;
                    }
                    break;
			case AllEnums.InputControlState.Game:
                    if(_inputControl._character._states._isDead && (!_inputControl._character._states._isDelayed && !_inputControl._character._states._isAnimatorInAction))
                    {
                        _curGameState = AllEnums.InputControlState.OutGame;
                        UserInterfaceController.Instance.OpenOutGameUI();
                    }

                    if (Mathf.RoundToInt(_gameTimeOfDay) >= 64800 && Mathf.RoundToInt(_gameTimeOfDay) <= 64900)
                    {
                        _gameTimeOfDay = (_dayNightCycle.DayInSeconds / _dayNightCycle.DayInHours) * (int)AllEnums.TimeOfDay.Dusk;
                        _endOfDay = true;
                        UserInterfaceController.Instance._inMenus._invState = AllEnums.InventoryUIState.Report;
                        UserInterfaceController.Instance._inMenus._weatherReportPanel._prepped = false;
                        _curGameState = AllEnums.InputControlState.InGameMenu;
                        UserInterfaceController.Instance.CycleInventoryUIMenus();
                    }
                    else if (Mathf.RoundToInt(_gameTimeOfDay) >= 21600 && Mathf.RoundToInt(_gameTimeOfDay) <= 21700)
                    {
                        _gameTimeOfDay = (_dayNightCycle.DayInSeconds / _dayNightCycle.DayInHours) * (int)AllEnums.TimeOfDay.Dawn;
                        _endOfDay = true;
                        UserInterfaceController.Instance._inMenus._invState = AllEnums.InventoryUIState.Report;
                        UserInterfaceController.Instance._inMenus._weatherReportPanel._prepped = false;
                        _curGameState = AllEnums.InputControlState.InGameMenu;
                        UserInterfaceController.Instance.CycleInventoryUIMenus();
                    }
				break;
			}
		}
	}
}