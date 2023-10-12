using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Shepherd
{
    [SerializeField] //temp
    public class GameSaveSlotUI : MonoBehaviour
    {

        public AllEnums.GameFileState _saveFile;
        public int _saveNumber;

        public Text _saveNumberText;
        public Text _playerName;
        public Text _flockCount;
        public Text _levelName;
        public Text _timeOfDay;
        public bool _isAutoSave;
        public GameObject _autoSaveText;
    }

    [Serializable]
    public class GameSaveSlotData
    {
        public GameSaveSlotUI _parent;
        public AllEnums.GameFileState _gameFile;
        public int _saveNumber;
        public string _saveNumberText;
        public string _playerName;
        public int _flockCount;
        public int _previousFlockCount;
        public int _goldCount;
        public int _previousGoldCount;

        public string _levelName;
        public int _actNumber, _chapterNumber, _sceneNumber;

        public float _currentTime;
        public string _timeOfDay;
        public bool _isAutoSave;

        public WorldItemStats _currentStatistics;
        public RuntimeReferences _testRefs;

        //CURRENT EQUIPMENT
        public List<string> _currentRightHand = new List<string>();
        public List<string> _currentLeftHand = new List<string>();
        //public List<string> _currentCompanion = new List<string>();
        public List<string> _currentConsumables = new List<string>();
        public List<string> _currentArmor = new List<string>();
        //ALL RUNTIME EQUIPMENT
        public List<RuntimeWeapon> _allWeapons = new List<RuntimeWeapon>();
        public List<RuntimeConsumable> _allConsumables = new List<RuntimeConsumable>();
        public List<RuntimeArmor> _allArmor = new List<RuntimeArmor>();

        //
        public List<BiocardSaveData> _network = new List<BiocardSaveData>();
        public List<BiocardSaveData> _allFlock = new List<BiocardSaveData>();
    }
}