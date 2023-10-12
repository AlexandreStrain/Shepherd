using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Shepherd;

[System.Serializable]
public class EquipmentInventoryPanel {
	//tutorial has current item in a text format here, watch 28F youtube @23:00
	public Text _slotHeading;
	public Text _slotName;

	public GameObject _eqCenter;
	public GameObject _eqLeft;
	public GameObject _invCenter;
	public GameObject _invLeft;

	public InfoBoxUI _equipmentSlots;
	//SlotsInfoTemplate[] _equipment = _equipmentSlots._grid.GetComponentsInChildren<SlotsInfoTemplate> ();
	public InfoBoxUI _leftHandQSlots;
	public InfoBoxUI _leftHandStats;
	//SlotsInfoTemplate[] _leftSlots = _leftHandQSlots._grid.GetComponentsInChildren<SlotsInfoTemplate> ();
	public InfoBoxUI _rightHandQSlots;
	public InfoBoxUI _rightHandStats;
	//SlotsInfoTemplate[] _rightSlots = _rightHandQSlots._grid.GetComponentsInChildren<SlotsInfoTemplate> ();

	public InfoBoxUI _itemQSlots;
	//SlotsInfoTemplate[] _itemSlots = _itemQSlots._grid.GetComponentsInChildren<SlotsInfoTemplate> ();
	public InfoBoxUI _spellQSlots;
	//SlotsInfoTemplate[] _spellSlots = _spellQSlots._grid.GetComponentsInChildren<SlotsInfoTemplate> ();

	public StoragePanel _storage;

	public Text _statisticsHeading;
	public Text _reqSideEffectsHeading;
	public ItemInfoUI _selectedItem;

	public StatisticsPanel _eqStats;
	public StatisticsPanel _invStats;
}

[System.Serializable]
public class StoragePanel {
	public Text _heading;
	public Scrollbar _scrollBar;
	public GameObject _slotTemplate;
	public Transform _slotGrid;
}


//TODO: Rename this
[System.Serializable]
public class InfoBoxUI {
	public GameObject _template;
	public Transform _grid;
}

[System.Serializable]
public class StatisticsPanel {
	public InfoBoxUI _selectedReqSideEffects;
	//StatisticsTemplate[] _reqSideEffectList = _selectedReqSideEffects._grid.GetComponentsInChildren<StatisticsTemplate> ();

	public InfoBoxUI _attackDefendList;
	//StatisticsTemplate[] _attackDefenceList = _attackDefendList._grid.GetComponentsInChildren<StatisticsTemplate> ();

	public InfoBoxUI _healthList;

	public InfoBoxUI _resistanceList;
	//StatisticsTemplate[] _resistancesList = _resistanceList._grid.GetComponentsInChildren<StatisticsTemplate> ();

	public InfoBoxUI _costGainsList;
	//public List<CostGainTemplate> _costGainList = new List<CostGainTemplate> ();
}

[System.Serializable]
public class PlayerOverviewPanel {
	public StatisticsTemplate _status;
	public StatisticsTemplate _money;
	public StatisticsTemplate _flockPopulation;
	public StatisticsTemplate _carryWeight;
	public StatisticsTemplate _poise;

	public InfoBoxUI _healthList;
	//StatisticsTemplate[] _healthList = _healthList._grid.GetComponentsInChildren<StatisticsTemplate> ();

	public InfoBoxUI _attributeList;
	//StatisticsTemplate[] _attributes = _attributeList._grid.GetComponentsInChildren<StatisticsTemplate> ();

	public InfoBoxUI _defenceList;
	//StatisticsTemplate[] _defences = _defenceList._grid.GetComponentsInChildren<StatisticsTemplate> ();

	public InfoBoxUI _resistanceList;
	//StatisticsTemplate[] _resistances = _resistanceList._grid.GetComponentsInChildren<StatisticsTemplate> ();

	public InfoBoxUI _bodyNeedList;
	//StatisticsTemplate[] _bodyNeeds = _bodyNeedList._grid.GetComponentsInChildren<StatisticsTemplate> ();

	public StatisticsTemplate _selectedStat;
	public Text _selectedStatInfo;
}

[System.Serializable]
public class FlockOverviewPanel {
	public Text _status;
	public Text _name;
	public Text _gender;

    public bool _isRenaming;
    public InputField _renameSheep;
    public Toggle _starSheep;

    public StatsInfoTemplate[] _sheepArmorSlots;
	public StatsInfoTemplate _sheepHead;
	public StatsInfoTemplate _sheepTorso;
    public StatsInfoTemplate _sheepLegs;
    public StatsInfoTemplate _sheepWool;

    public List<GameObject> _allFlockUI;
    public Scrollbar _allFlockScrollbar;

	public InfoBoxUI _sheepEqSlots;

	public InfoBoxUI _healthList;
	//StatisticsTemplate[] _healthList = _healthList._grid.GetComponentsInChildren<StatisticsTemplate> ();

	public InfoBoxUI _attributeList;
	//StatisticsTemplate[] _attributes = _attributeList._grid.GetComponentsInChildren<StatisticsTemplate> ();

	public InfoBoxUI _defenceList;
	//StatisticsTemplate[] _defences = _defenceList._grid.GetComponentsInChildren<StatisticsTemplate> ();

	public InfoBoxUI _resistanceList;
	//StatisticsTemplate[] _resistances = _resistanceList._grid.GetComponentsInChildren<StatisticsTemplate> ();

	public InfoBoxUI _bodyNeedList;
	//StatisticsTemplate[] _bodyNeeds = _bodyNeedList._grid.GetComponentsInChildren<StatisticsTemplate> ();
}

[System.Serializable]
public class WeatherReportPanel
{
    public Text _reportTitle;
    public Text _goldBeforeText;
    public Text _goldAfterText;

    public Text _flockBeforeText;
    public Text _flockAfterText;

    public Text _locationBeforeText;
    public Text _locationAfterText;

    public Text _forecastBeforeText;
    public Text _forecastAfterText;

    public Scrollbar _eventLogScrollbar;

    public InfoBoxUI _eventLogGrid;
    public List<InfoFeedData> _eventFeedList = new List<InfoFeedData>();
    public bool _prepped = false;

    public void CopyOverDataFromInfoFeed(InfoFeedData data)
    {
        GameObject gObject = Object.Instantiate(_eventLogGrid._template) as GameObject;
        gObject.transform.SetParent(_eventLogGrid._grid.transform);
        gObject.transform.localScale = Vector3.one;
        InfoFeedData message = gObject.GetComponentInChildren<InfoFeedData>();
        message._timeStamp.text = data._timeStamp.text;
        message._infoImage.sprite = data._infoImage.sprite;
        message._description.text = data._description.text;
        message._action.text = data._action.text;

        gObject.SetActive(true);
        UserInterfaceController.Instance._inMenus._weatherReportPanel._eventFeedList.Add(message);
    }

    public void PrepReport()
    {
        if(_prepped)
        {
            return;
        }
        _prepped = true;

        _reportTitle.text = "End of " + GameSessionController.Instance._MorningOrNight + " " + GameSessionController.Instance._dayNightCycle._days + " Report";
       
        //Flock statuses -- and should any special event happen (births, deaths, pregnancies etc)
        int flockNum = GameSessionController.Instance._wResources._runtimeReferences._flock.Count;
        List<CharacterStateController> allFlock = GameSessionController.Instance._wResources._runtimeReferences._flock;
        for (int i = 0; i < flockNum; i++)
        {
            GameObject gObject = Object.Instantiate(_eventLogGrid._template) as GameObject;
            gObject.transform.SetParent(_eventLogGrid._grid.transform);
            gObject.transform.localScale = Vector3.one;
            InfoFeedData message = gObject.GetComponentInChildren<InfoFeedData>();
            message._timeStamp.text = GameSessionController.Instance._dayNightCycle._timeText.text;
            message._infoImage.sprite = UserInterfaceController.Instance._inMenus._icons._blankSpriteTemplate;
            message._description.text = allFlock[i]._biography._name;
            message._action.text = allFlock[i]._biography._currentBioCard._status.ToString();
            gObject.SetActive(true);
        }

        int prevGold = GameSessionController.Instance._wResources._runtimeReferences._prevGold;
        int curGold = GameSessionController.Instance._wResources._runtimeReferences._currentGold;
        _goldBeforeText.text = prevGold.ToString();
        _goldAfterText.text = curGold.ToString();
        if (prevGold > curGold)
        {
            _goldAfterText.color = UserInterfaceController.Instance._inMenus._loss;
        }
        else if (prevGold == curGold)
        {
            _goldAfterText.color = UserInterfaceController.Instance._inMenus._default;
        }
        else
        {
            _goldAfterText.color = UserInterfaceController.Instance._inMenus._gain;
        }

        int prevFlockCount = GameSessionController.Instance._wResources._runtimeReferences._prevFlockCount;
        _flockBeforeText.text = prevFlockCount.ToString();
        _flockAfterText.text = flockNum.ToString();
        if (prevFlockCount > flockNum)
        {
            _flockAfterText.color = UserInterfaceController.Instance._inMenus._loss;
        }
        else if (prevFlockCount == flockNum)
        {
            _flockAfterText.color = UserInterfaceController.Instance._inMenus._default;
        }
        else
        {
            _flockAfterText.color = UserInterfaceController.Instance._inMenus._gain;
        }

        _locationBeforeText.text = GameSessionController.Instance._levelControl._levelScenes[GameSessionController.Instance._currentScene]._currentWorldLocation.ToString();
        _locationAfterText.text = "???";

        _forecastBeforeText.text = GameSessionController.Instance._levelControl._levelScenes[GameSessionController.Instance._currentScene]._currentWeather.ToString();
        _forecastAfterText.text = "N/A";

        _eventLogScrollbar.value = 0;
        _eventLogScrollbar.onValueChanged.Invoke(_eventLogScrollbar.value);
    }

    public void TeardownReport()
    {
        for (int i = 0; i < _eventFeedList.Count; i++)
        {
            Object.Destroy(_eventFeedList[i].gameObject);
        }
        _eventFeedList.Clear();
        InfoFeedData[] infoGrid = _eventLogGrid._grid.GetComponentsInChildren<InfoFeedData>();
        for (int i = 0; i < infoGrid.Length; i++)
        {
            Object.Destroy(infoGrid[i].gameObject);
        }
    }
}

[System.Serializable]
public class TitlePausePanel {
	public GameObject _titleTextObject;
	public GameObject _pauseTextObject;
	public Text _mainButtonText; //update text for Start, Continue, or Resume
	public GameObject _mainButtons; //TODO: Rename
	public GameObject _newGameButton;
	public GameObject _saveGameButton;
	public GameObject _loadGameButton;

	//Confirm sub-panel
	[Header("Confirm Sub-Panel")]
	public GameObject _confirmPanel;
	public Text _confirmQuitText;
	public GameObject _quitToTitleButton;
	public GameObject _confirmToTitleButton;

	public GameObject _quitToDesktopButton;
	public GameObject _confirmToDesktopButton;
	//...

	public void PrepForPauseMenu() {
		_pauseTextObject.SetActive (true);
		_mainButtonText.text = "Resume";
		_loadGameButton.SetActive (true);
		_saveGameButton.SetActive (true);
		_quitToTitleButton.SetActive (true);

		_titleTextObject.SetActive(false);
		_newGameButton.SetActive (false);

        //TODO: THis is a temp workaround until I get one definite location for accessing if the player has died
        if(GameSessionController.Instance._inputControl._character._states._isDead)
        {
            _saveGameButton.SetActive(false);
            _pauseTextObject.GetComponent<Text>().text = "~ Game Over ~";
        } else
        {
            _pauseTextObject.GetComponent<Text>().text = "~ Paused ~";
        }
	}

	public void PrepForTitleMenu() {
		//checks if there is no files saved -- changes layout of title screen
		_titleTextObject.SetActive(true);
		_pauseTextObject.SetActive (false);
		if (GameSessionController.Instance._firstLaunch) {
			_mainButtonText.text = "Start";
			_loadGameButton.SetActive (false);
			_newGameButton.SetActive (false);
		} else {
			_mainButtonText.text = "Continue";
			_loadGameButton.SetActive (true);
			_newGameButton.SetActive (true);
		}
		_saveGameButton.SetActive (false);
		_quitToTitleButton.SetActive (false);
	}
}

[System.Serializable]
public class LoadSavePanel {
	[Header("General")]
	public Text _loadSaveDeletePanel;
	public GameSaveSlotUI _slotToBeUpdated;

	public GameObject _navButtons;
	public Button _navLoad;
	public Button _navSave;
	public Button _navDelete;
	public Button _navClose;
	public bool _fromTitle;

	[Header("Load")]
	public GameObject _gameFileParent;
	public Button _gameFile1;
	public Button _gameFile2;
	public Button _gameFile3;

	[Header("Save")]
	public GameObject _createNewSaveButton;
	public GameObject _saveSlotsParent;
	public GameObject _saveInstanceTemplate;

	[Header("Confirm Sub-Menu")]
	public GameObject _confirmPanel;
	public Text _confirmActionText;
	public GameSaveSlotUI _saveSlotInQuestion;
	public GameObject _loadSaveButton;
	public GameObject _overwriteSaveButton;
	public GameObject _deleteSaveButton;

}

[System.Serializable]
public class OptionsPanel {
	[Header("General")]
	public Button _navGameplay;
	public Button _navVideo;
	public Button _navAudio;
	public Button _navControls;

	public Event _keyMapEvent;
	public bool _hasKeyToBeMapped;

	[Header("Gameplay")]
	//Gameplay Sub-Menu
	public GameObject _gameplayPanel;

	[Header("Video")]
	//Video Sub-Menu
	public GameObject _videoPanel;
	public Dropdown _resolutionDropdown;
	public Dropdown _graphicsPresetsDropdown;
	public Dropdown _framerateDropdown;
	public Dropdown _vsyncDropdown;
	public Dropdown _antiAliasDropdown;
	public Dropdown _anisotropicDropdown;
	public Dropdown _shadowTypesDropdown;
	public Dropdown _shadowQualityDropdown;
	public Dropdown _textureQualityDropdown;

	//Audio Sub-Menu
	[Header("Audio")]
	public GameObject _audioPanel;
	public Text _masterValueText;
	public Text _musicValueText;
	public Text _ambientValueText;
	public Text _sfxValueText;
	public Text _dialogueValueText;
	public Text _subtitlesText;

	[Header("Controls")]
	//Controls Sub-Menu
	public GameObject _controlsPanel;
	public GameObject _controlMapParent;
	public GameObject _controlMapTemplate;
	public Text _curGameButtonToMap;
	public Button _controlKeyboard;
	public Button _controlGamepad;

	//Controls Sub-Menu Overlay
	public GameObject _awaitKeyMapPanel;
	public StatsInfoTemplate _awaitKeyPrompt;

	public void RefreshVideoPanel() {
		_resolutionDropdown.RefreshShownValue ();

		_antiAliasDropdown.RefreshShownValue ();
		_anisotropicDropdown.RefreshShownValue ();

		_shadowTypesDropdown.RefreshShownValue ();
		_shadowQualityDropdown.RefreshShownValue ();

		_framerateDropdown.RefreshShownValue ();

		_vsyncDropdown.RefreshShownValue ();

		_textureQualityDropdown.RefreshShownValue ();
	}
}

[System.Serializable]
public class ItemInfoUI {
	public Image _itemImage;
	public Text _itemName;
	public Image _itemTypeSlot;
	public Image _itemIcon;
	public Text _durability;
	public Text _itemDescription;

	public Text _weight;
	public Text _fear;
	public Text _luck;
	public Text _sound;
	public Text _duration;
}

//KEEP
[System.Serializable]
public class StatsInfoTemplate {
	public StatisticsTemplate _information;
}

[System.Serializable]
public class SlotsInfoTemplate {
	public InventorySlotTemplate _information;
	public EquipmentSlot _eqSlot;
}

[System.Serializable]
public class StatusBarTemplate {
	public InventoryStatusBarTemplate _information;
}

[System.Serializable]
public class CostGainTemplate {
	public AllEnums.BodyNeedsType _bodyNeedType;
	public AllEnums.StatusType _statusType;
	public StatisticsTemplate _information;
}

public struct StatusList {
	public float _max;
	public float _current;
	public StatusList(float max, float current) {
		_max = max;
		_current = current;
	}
}

public struct WeaponStatsInArray {
	public float _luck;
	public float _fear;
	public float _soundVolume;
	public float _weight;
	public float _maxDurability;
	public float _currentDurability;

	public float[] _requirements;
	public float[] _attackDefence;
	public float[] _resistanceChance;


	public void CreateArrays(WorldItemStats fromStats) {
		_requirements = new float[4];
		_requirements [0] = fromStats._strength;
		_requirements [1] = fromStats._endurance;
		_requirements [2] = fromStats._dexterity;
		_requirements [3] = fromStats._intelligence;

		_attackDefence = new float[9];
		_attackDefence [0] = fromStats._physical;
		_attackDefence [1] = fromStats._finesse;
		_attackDefence [2] = fromStats._magical;
		_attackDefence [3] = fromStats._fire;
		_attackDefence [4] = fromStats._wind;
		_attackDefence [5] = fromStats._light;
		_attackDefence [6] = fromStats._water;
		_attackDefence [7] = fromStats._earth;
		_attackDefence [8] = fromStats._dark;

		_resistanceChance = new float[6];
		_resistanceChance [0] = fromStats._bleed;
		_resistanceChance [1] = fromStats._poison;
		_resistanceChance [2] = fromStats._heat;
		_resistanceChance [3] = fromStats._disease;
		_resistanceChance [4] = fromStats._dizzy;
		_resistanceChance [5] = fromStats._freeze;

		_luck = fromStats._luckDmg;
		_fear = fromStats._fear;
		_soundVolume = fromStats._soundProduction;
		_weight = fromStats._weight;
		_maxDurability = fromStats._maxDurability;
	}

	public void CreatePlayerArrays(CharacterBody fromBody, WorldItemStats fromBodyStats) {
		_requirements = new float[8];
		_requirements [0] = fromBodyStats._strength;
		_requirements [1] = fromBodyStats._endurance;
		_requirements [2] = fromBodyStats._dexterity;
		_requirements [3] = fromBodyStats._intelligence;
		_requirements [4] = fromBodyStats._vitality;
		_requirements [5] = fromBodyStats._perception;
		_requirements [6] = fromBodyStats._courage;
		_requirements [7] = fromBodyStats._luck;

		_attackDefence = new float[9];
		_attackDefence [0] = fromBodyStats._physical;
		_attackDefence [1] = fromBodyStats._finesse;
		_attackDefence [2] = fromBodyStats._magical;
		_attackDefence [3] = fromBodyStats._fire;
		_attackDefence [4] = fromBodyStats._wind;
		_attackDefence [5] = fromBodyStats._light;
		_attackDefence [6] = fromBodyStats._water;
		_attackDefence [7] = fromBodyStats._earth;
		_attackDefence [8] = fromBodyStats._dark;

		_resistanceChance = new float[6];
		_resistanceChance [0] = fromBodyStats._bleed;
		_resistanceChance [1] = fromBodyStats._poison;
		_resistanceChance [2] = fromBodyStats._heat;
		_resistanceChance [3] = fromBodyStats._disease;
		_resistanceChance [4] = fromBodyStats._dizzy;
		_resistanceChance [5] = fromBodyStats._freeze;
	}
}
//...

[System.Serializable]
public class EquipmentSlotsUI {
	public List<EquipmentSlot> _weaponSlots = new List<EquipmentSlot>();
	public List<EquipmentSlot> _equipmentSlots = new List<EquipmentSlot> ();
	public List<EquipmentSlot> _consumableSlots = new List<EquipmentSlot> ();
	public List<EquipmentSlot> _spellSlots = new List<EquipmentSlot>();

	public List<EquipmentSlot> _sheepSlots = new List<EquipmentSlot>();

	public Sprite _blankSprite;

	public void UpdateEquipmentSlotV2(WorldItem item, AllEnums.ItemType iType, EquipmentSlot updatingSlot) {
		//Debug.Log (item);

		updatingSlot._icon._iconInventory.sprite = item._itemIcon;
		updatingSlot._icon._iconHUD.sprite = item._itemHUDIcon;
		updatingSlot._icon._iconHUD.transform.localScale = Vector3.one;
		updatingSlot._icon._iconInventory.transform.localScale = Vector3.one;
		updatingSlot._icon._description.text = item._itemName;
		updatingSlot._icon._ID = item._itemName;
	}

	//right now only returns weaponslots
	public EquipmentSlot GetSlot(int fromIndex, AllEnums.ItemType iType) {
		switch (iType) {
			case AllEnums.ItemType.Weapon:
				return _weaponSlots [fromIndex];
			case AllEnums.ItemType.Armor:
				return _equipmentSlots [fromIndex];
			case AllEnums.ItemType.Spell:
				return _spellSlots[fromIndex];
			case AllEnums.ItemType.Consumable:
				return _consumableSlots[fromIndex];
			default:
				return null;
		}
	}

	public void ClearEquipmentSlot(EquipmentSlot slotToClear, AllEnums.ItemType iType) {
		//not sure about this
		slotToClear._icon._description.text = slotToClear._slotName;
		slotToClear._icon._iconHUD.sprite = _blankSprite;
		slotToClear._icon._iconInventory.sprite = _blankSprite;
		slotToClear._icon._ID = "";
	}

	public void AddSlotOnList(EquipmentSlot newSlot) {
		switch (newSlot._slotItype) {
			case AllEnums.ItemType.Weapon:
				_weaponSlots.Add (newSlot);
				break;
			case AllEnums.ItemType.Armor:
				_equipmentSlots.Add (newSlot);
				break;
			case AllEnums.ItemType.Spell:
				_spellSlots.Add (newSlot);
				break;
			case AllEnums.ItemType.Consumable:
				_consumableSlots.Add (newSlot);
				break;
			default:
				break;
		}
	}
}