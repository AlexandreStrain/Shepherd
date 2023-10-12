using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Shepherd {
	public class InGameMenus : MonoBehaviour {

		public AllEnums.InventoryUIState _invState;

		public UserInterfaceIcons _icons;
		public UserInterfaceStatInfo _statInfo;

		public GameObject _titlePanel;
		public GameObject _helpPanel;
		public GameObject _playerOverview;
		public GameObject _equipmentInventoryMenu;
		public GameObject _flockOverview;
		public GameObject _gesturesMenu;
        public GameObject _weatherReportMenu;

		public EquipmentSlotsUI _eqSlotsUI;

		public EquipmentInventoryPanel _eqInvPanel;
		public GesturesPanel _gesturesPanel;
		public PlayerOverviewPanel _playerOverviewPanel;
		public FlockOverviewPanel _flockOverviewPanel;
		public GesturesPanel _inGamePanel;
        public WeatherReportPanel _weatherReportPanel;
		public NavigatePanel _inGameNavigatePanel;

		public Transform _eqSlotsParent;
		public Transform _invSlotsParent;
		public Transform _sheepSlotsParent;

		private EquipmentSlot[,] _eqSlots;
		private Vector2 _eqSelectPosition;
		private EquipmentSlot _curEqSlot;
		private EquipmentSlot _prevEqSlot;

		private EquipmentSlot[] _sSlots;
		private int _sheepSelectPosition;
        private int _sheepArmorPosition;
		private EquipmentSlot _curSheepSlot;
		private EquipmentSlot _prevSheepSlot;

		public Transform _overviewSlotsParent;
		public RectTransform _overviewSelector;
		private StatisticsTemplate[,] _overviewSlots; //houses all statistics to view (bodyNeeds, resistances, etc)
		private Vector2 _overviewSelectorPos; //currently selected statistic on player
		private StatisticsTemplate _curOvSlot;
		private StatisticsTemplate _prevOvSlot;

		private List<InventorySlotTemplate> _storageSlots;
		private int _curStorageIndex;
		private int _maxStorageIndex;
		private int _prevStorageIndex;
		private InventorySlotTemplate _curStorageSlot;

		private float _delta;

		private List<InventorySlotTemplate> _eqStorageSlotsCreated = new List<InventorySlotTemplate>();

		//temp
		public bool _isSwitchingItem;
        public bool _isSwitchingSheepItem;

		public Color _unselected;
		public Color _selected;
		public Color _gain;
		public Color _loss;
		public Color _default;

		private CharacterStateController _player;
		private WorldResourceController _wRControl;

        
		public void PreInit() {
            _wRControl = GameSessionController.Instance._wResources;

			//UserInterfaceController._inventoryUI = this;
			_statInfo = Resources.Load ("UI StatInfo") as UserInterfaceStatInfo;
			_statInfo.Init ();

			_eqSlotsUI._blankSprite = _icons._blankSpriteTemplate;
			CreateUIElements ();
			InitEquipmentSlots ();
		}

		public void Init(CharacterStateController player) {
			_player = player;

            if(_gesturesPanel._initialized && _inGamePanel._initialized)
            {
                return;
            }

			//_gesturesPanel = GetComponent<GesturesPanel> ();
			UserInterfaceController.Instance._inMenus._gesturesPanel = _gesturesPanel;
			_gesturesPanel.Init ();
			_gesturesPanel.CreateGesturesUI ();

			//test
			UserInterfaceController.Instance._inMenus._inGamePanel = _inGamePanel;
			_inGamePanel.Init();
			_inGamePanel.CreateGesturesUI ();

			CreateFlockOverviewPanel ();
		}

		private void CreateUIElements() {
			//CreateEquipmentPanel ();
			//CreateItemsPanel ();
			CreateEquipmentInventoryPanel ();
			CreatePlayerOverviewPanel ();

		}

		private void InitEquipmentSlots() {
			EquipmentSlot[] equipment = _eqSlotsParent.GetComponentsInChildren<EquipmentSlot> ();
			//_equipmentSlots.AddRange (equipment);
			_eqSlots = new EquipmentSlot[10,7];

			//_equipmentSlots.AddRange (inventory);

			for (int i = 0; i < equipment.Length; i++) {
				equipment [i].Init (this);
				int x = Mathf.RoundToInt (equipment [i]._slotPosition.x);
				int y = Mathf.RoundToInt (equipment [i]._slotPosition.y);
				_eqSlots [x, y] = equipment [i];
				//_equipmentSlots [x, y].Init (this);
				//_equipmentSlots [i].Init (this);
			}


			EquipmentSlot[] inventory = _invSlotsParent.GetComponentsInChildren<EquipmentSlot> ();
			for (int i = 0; i < inventory.Length; i++) {
				inventory [i].Init (this);
				int x = Mathf.RoundToInt (inventory [i]._slotPosition.x);
				int y = Mathf.RoundToInt (inventory [i]._slotPosition.y);
				_eqSlots [x, y] = inventory [i];
				//_equipmentSlots [x, y].Init (this);
			}
		}

		private void CreateEquipmentInventoryPanel() {
			//Equipment Slots
			int row = 0;
			for (int i = 0; i < AllEnums.ArmorTypeNumber; i++) {
				if ((AllEnums.ArmorType)i != AllEnums.ArmorType.Weapon) {
					SlotsInfoTemplate eqSlotTemp = new SlotsInfoTemplate ();
					GameObject gObject = Instantiate (_eqInvPanel._equipmentSlots._template) as GameObject;
					gObject.transform.SetParent (_eqInvPanel._equipmentSlots._grid);
					gObject.transform.localScale = Vector3.one;

					eqSlotTemp._information = gObject.GetComponentInChildren<InventorySlotTemplate> ();
					eqSlotTemp._information._description.text = ((AllEnums.ArmorType)i).ToString ();
					eqSlotTemp._information._iconHUD.sprite = _icons._blankSpriteTemplate;
					eqSlotTemp._information._iconInventory.sprite = _icons._blankSpriteTemplate;

					eqSlotTemp._eqSlot = gObject.GetComponentInChildren<EquipmentSlot> ();
					eqSlotTemp._eqSlot._slotEqType = (AllEnums.ArmorType)i;
					eqSlotTemp._eqSlot._slotItype = AllEnums.ItemType.Armor;
					eqSlotTemp._eqSlot._slotName = ((AllEnums.ArmorType)i).ToString ();

					if (i % 3 == 0) {
						row++;
					}
					eqSlotTemp._eqSlot._slotPosition.x = i % 3;
					eqSlotTemp._eqSlot._slotPosition.y = row - 1;
					eqSlotTemp._eqSlot._itemPosition = i;

					gObject.SetActive (true);
				} else {
					//TODO: Clean up
					//Left Hand Quickslots
					for (int j = 0; j < 3; j++) { //i <  number of player left hand quickslots
						SlotsInfoTemplate lSlotTemp = new SlotsInfoTemplate ();
						GameObject gObject1 = Instantiate (_eqInvPanel._leftHandQSlots._template) as GameObject;
						gObject1.transform.SetParent (_eqInvPanel._leftHandQSlots._grid);
						gObject1.transform.localScale = Vector3.one;

						lSlotTemp._information = gObject1.GetComponentInChildren<InventorySlotTemplate> ();
						lSlotTemp._information._description.text = ("Left " + (j + 1)).ToString ();
						lSlotTemp._information._iconHUD.sprite = _icons._blankSpriteTemplate;
						lSlotTemp._information._iconInventory.sprite = _icons._blankSpriteTemplate;

						gObject1.SetActive (true);

						lSlotTemp._eqSlot = gObject1.GetComponentInChildren<EquipmentSlot> ();
						lSlotTemp._eqSlot._slotEqType = (AllEnums.ArmorType)i;
						lSlotTemp._eqSlot._slotName = "Left " + (j + 1);
						lSlotTemp._eqSlot._slotItype = AllEnums.ItemType.Weapon;
						lSlotTemp._eqSlot._slotPosition.x = j % 3;
						lSlotTemp._eqSlot._slotPosition.y = row;
						lSlotTemp._eqSlot._itemPosition = j;

						StatsInfoTemplate lSlot = new StatsInfoTemplate ();
						GameObject gObject2 = Instantiate (_eqInvPanel._leftHandStats._template) as GameObject;
						gObject2.transform.SetParent (_eqInvPanel._leftHandStats._grid);
						gObject2.transform.localScale = Vector3.one;
						lSlot._information = gObject2.GetComponentInChildren<StatisticsTemplate> ();
						lSlot._information._description.text = "L" + (j+1).ToString();
						lSlot._information._stat.text = "000 |";
						gObject2.SetActive (true);
					}
					//row++;

					//Right Hand Quickslots
					for (int k = 0; k < 3; k++) { //i <  number of player right hand quickslots
						SlotsInfoTemplate rSlotTemp = new SlotsInfoTemplate ();
						GameObject gObject3 = Instantiate (_eqInvPanel._rightHandQSlots._template) as GameObject;
						gObject3.transform.SetParent (_eqInvPanel._rightHandQSlots._grid);
						gObject3.transform.localScale = Vector3.one;
						rSlotTemp._information = gObject3.GetComponentInChildren<InventorySlotTemplate> ();
						rSlotTemp._information._description.text = ("Right " + (k + 1)).ToString ();
						rSlotTemp._information._iconHUD.sprite = _icons._blankSpriteTemplate;
						rSlotTemp._information._iconInventory.sprite = _icons._blankSpriteTemplate;

						gObject3.SetActive (true);

						rSlotTemp._eqSlot = gObject3.GetComponentInChildren<EquipmentSlot> ();
						rSlotTemp._eqSlot._slotEqType = (AllEnums.ArmorType)i;
						rSlotTemp._eqSlot._slotItype = AllEnums.ItemType.Weapon;
						rSlotTemp._eqSlot._slotPosition.x = (k % 3) + 3;
						rSlotTemp._eqSlot._slotPosition.y = row;
						rSlotTemp._eqSlot._slotName = "Right " + (k + 1);
						rSlotTemp._eqSlot._itemPosition = k + 3;

						StatsInfoTemplate rSlot = new StatsInfoTemplate ();
						GameObject gObject4 = Instantiate (_eqInvPanel._rightHandStats._template) as GameObject;
						gObject4.transform.SetParent (_eqInvPanel._rightHandStats._grid);
						gObject4.transform.localScale = Vector3.one;
						rSlot._information = gObject4.GetComponentInChildren<StatisticsTemplate> ();
						rSlot._information._description.text = "R" + (k + 1).ToString ();
						rSlot._information._stat.text = "000 |";
						gObject4.SetActive (true);
					}
				}
			}

			//Item Quickslots
			for (int i = 0; i < 10; i++) { //hardcoded, but should be amount of player item quickslots (10 max)
				SlotsInfoTemplate iQSlotTemp = new SlotsInfoTemplate();
				GameObject gObject = Instantiate (_eqInvPanel._itemQSlots._template) as GameObject;
				gObject.transform.SetParent (_eqInvPanel._itemQSlots._grid);
				gObject.transform.localScale = Vector3.one;
				iQSlotTemp._information = gObject.GetComponentInChildren<InventorySlotTemplate> ();
				iQSlotTemp._eqSlot = gObject.GetComponentInChildren<EquipmentSlot> ();
				iQSlotTemp._eqSlot._slotEqType = AllEnums.ArmorType.Accessory1;
				iQSlotTemp._eqSlot._slotItype = AllEnums.ItemType.Consumable;
				iQSlotTemp._eqSlot._slotPosition.x = i;
				iQSlotTemp._eqSlot._slotPosition.y = 5;
				iQSlotTemp._eqSlot._slotName = "Item Slot " + (i + 1).ToString();
				iQSlotTemp._eqSlot._itemPosition = i;

				iQSlotTemp._information._description.text = (i + 1).ToString();
				iQSlotTemp._information._iconHUD.sprite = _icons._blankSpriteTemplate;
				iQSlotTemp._information._iconInventory.sprite = _icons._blankSpriteTemplate;
				gObject.SetActive (true);
			}
			//Spell QuickSlots
			for (int i = 0; i < 4; i++) { //hardcoded, but should be amount of player spell quickslots (4 max)
				SlotsInfoTemplate sQSlotTemp = new SlotsInfoTemplate();
				GameObject gObject = Instantiate (_eqInvPanel._spellQSlots._template) as GameObject;
				gObject.transform.SetParent (_eqInvPanel._spellQSlots._grid);
				gObject.transform.localScale = Vector3.one;
				sQSlotTemp._information = gObject.GetComponentInChildren<InventorySlotTemplate> ();
				sQSlotTemp._eqSlot = gObject.GetComponentInChildren<EquipmentSlot> ();
				sQSlotTemp._eqSlot._slotEqType = AllEnums.ArmorType.Accessory1;
				sQSlotTemp._eqSlot._slotItype = AllEnums.ItemType.Spell;
				sQSlotTemp._eqSlot._slotPosition.x = i;
				sQSlotTemp._eqSlot._slotPosition.y = 6;
				sQSlotTemp._eqSlot._slotName = "Spell Slot " + (i + 1);
				sQSlotTemp._eqSlot._itemPosition = i;

				sQSlotTemp._information._description.text = (i + 1).ToString();
				sQSlotTemp._information._iconHUD.sprite = _icons._blankSpriteTemplate;
				sQSlotTemp._information._iconInventory.sprite = _icons._blankSpriteTemplate;
				gObject.SetActive (true);
			}

			//Selected Weapon Information
			_eqInvPanel._selectedItem._itemIcon.sprite = _icons._blankSpriteTemplate;
			_eqInvPanel._selectedItem._itemImage.sprite = _icons._blankSpriteTemplate;
			_eqInvPanel._selectedItem._itemName.text = "";
			_eqInvPanel._selectedItem._durability.text = "";
			_eqInvPanel._selectedItem._itemDescription.text = "";

			//TODO Put this into a list, then fill in the blanks with the selected weapon
			_eqInvPanel._selectedItem._weight.text = "Weight: "; //+ selected weapon weight value;
			_eqInvPanel._selectedItem._fear.text = "Fear: "; //+ selected weapon weight value;
			_eqInvPanel._selectedItem._luck.text = "Luck: "; //+ selected weapon weight value;
			_eqInvPanel._selectedItem._sound.text = "Sound: "; //+ selected weapon weight value;
			_eqInvPanel._selectedItem._duration.text = "Duration: "; //+ selected weapon weight value;

			//Selected Weapon
			InitDisplaySelected(_eqInvPanel._eqStats);
			InitDisplaySelected (_eqInvPanel._invStats);
		}

		private void CreateEquipmentPanel() {
			//Equipment Slots
			int row = 0;
			for (int i = 0; i < AllEnums.ArmorTypeNumber; i++) {
				if ((AllEnums.ArmorType)i != AllEnums.ArmorType.Weapon) {
					SlotsInfoTemplate eqSlotTemp = new SlotsInfoTemplate ();
					GameObject gObject = Instantiate (_eqInvPanel._equipmentSlots._template) as GameObject;
					gObject.transform.SetParent (_eqInvPanel._equipmentSlots._grid);
					gObject.transform.localScale = Vector3.one;

					eqSlotTemp._information = gObject.GetComponentInChildren<InventorySlotTemplate> ();
					eqSlotTemp._information._description.text = ((AllEnums.ArmorType)i).ToString ();
					eqSlotTemp._information._iconHUD.sprite = _icons._blankSpriteTemplate;
					eqSlotTemp._information._iconInventory.sprite = _icons._blankSpriteTemplate;

					eqSlotTemp._eqSlot = gObject.GetComponentInChildren<EquipmentSlot> ();
					eqSlotTemp._eqSlot._slotEqType = (AllEnums.ArmorType)i;
					eqSlotTemp._eqSlot._slotItype = AllEnums.ItemType.Armor;
					eqSlotTemp._eqSlot._slotName = ((AllEnums.ArmorType)i).ToString ();

					if (i % 3 == 0) {
						row++;
					}
					eqSlotTemp._eqSlot._slotPosition.x = i % 3;
					eqSlotTemp._eqSlot._slotPosition.y = row - 1;
					eqSlotTemp._eqSlot._itemPosition = i;

					gObject.SetActive (true);
				} else {
					//TODO: Clean up
					//Left Hand Quickslots
					for (int j = 0; j < 3; j++) { //i <  number of player left hand quickslots
						SlotsInfoTemplate lSlotTemp = new SlotsInfoTemplate ();
						GameObject gObject1 = Instantiate (_eqInvPanel._leftHandQSlots._template) as GameObject;
						gObject1.transform.SetParent (_eqInvPanel._leftHandQSlots._grid);
						gObject1.transform.localScale = Vector3.one;

						lSlotTemp._information = gObject1.GetComponentInChildren<InventorySlotTemplate> ();
						lSlotTemp._information._description.text = ("Left " + (j + 1)).ToString ();
						lSlotTemp._information._iconHUD.sprite = _icons._blankSpriteTemplate;
						lSlotTemp._information._iconInventory.sprite = _icons._blankSpriteTemplate;

						gObject1.SetActive (true);

						lSlotTemp._eqSlot = gObject1.GetComponentInChildren<EquipmentSlot> ();
						lSlotTemp._eqSlot._slotEqType = (AllEnums.ArmorType)i;
						lSlotTemp._eqSlot._slotName = "Left " + (j + 1);
						lSlotTemp._eqSlot._slotItype = AllEnums.ItemType.Weapon;
						lSlotTemp._eqSlot._slotPosition.x = j % 3;
						lSlotTemp._eqSlot._slotPosition.y = row;
						lSlotTemp._eqSlot._itemPosition = j;

						StatsInfoTemplate lSlot = new StatsInfoTemplate ();
						GameObject gObject2 = Instantiate (_eqInvPanel._leftHandStats._template) as GameObject;
						gObject2.transform.SetParent (_eqInvPanel._leftHandStats._grid);
						gObject2.transform.localScale = Vector3.one;
						lSlot._information = gObject2.GetComponentInChildren<StatisticsTemplate> ();
						lSlot._information._description.text = "L" + (j+1).ToString();
						lSlot._information._stat.text = "000 |";
						gObject2.SetActive (true);
					}
					//row++;

					//Right Hand Quickslots
					for (int k = 0; k < 3; k++) { //i <  number of player right hand quickslots
						SlotsInfoTemplate rSlotTemp = new SlotsInfoTemplate ();
						GameObject gObject3 = Instantiate (_eqInvPanel._rightHandQSlots._template) as GameObject;
						gObject3.transform.SetParent (_eqInvPanel._rightHandQSlots._grid);
						gObject3.transform.localScale = Vector3.one;
						rSlotTemp._information = gObject3.GetComponentInChildren<InventorySlotTemplate> ();
						rSlotTemp._information._description.text = ("Right " + (k + 1)).ToString ();
						rSlotTemp._information._iconHUD.sprite = _icons._blankSpriteTemplate;
						rSlotTemp._information._iconInventory.sprite = _icons._blankSpriteTemplate;

						gObject3.SetActive (true);

						rSlotTemp._eqSlot = gObject3.GetComponentInChildren<EquipmentSlot> ();
						rSlotTemp._eqSlot._slotEqType = (AllEnums.ArmorType)i;
						rSlotTemp._eqSlot._slotItype = AllEnums.ItemType.Weapon;
						rSlotTemp._eqSlot._slotPosition.x = (k % 3) + 3;
						rSlotTemp._eqSlot._slotPosition.y = row;
						rSlotTemp._eqSlot._slotName = "Right " + (k + 1);
						rSlotTemp._eqSlot._itemPosition = k + 3;

						StatsInfoTemplate rSlot = new StatsInfoTemplate ();
						GameObject gObject4 = Instantiate (_eqInvPanel._rightHandStats._template) as GameObject;
						gObject4.transform.SetParent (_eqInvPanel._rightHandStats._grid);
						gObject4.transform.localScale = Vector3.one;
						rSlot._information = gObject4.GetComponentInChildren<StatisticsTemplate> ();
						rSlot._information._description.text = "R" + (k + 1).ToString ();
						rSlot._information._stat.text = "000 |";
						gObject4.SetActive (true);
					}
				}
			}
			//Selected Weapon Information
			_eqInvPanel._selectedItem._itemIcon.sprite = _icons._blankSpriteTemplate;
			_eqInvPanel._selectedItem._itemImage.sprite = _icons._blankSpriteTemplate;
			_eqInvPanel._selectedItem._itemName.text = "";
			_eqInvPanel._selectedItem._durability.text = "";
			_eqInvPanel._selectedItem._itemDescription.text = "";

			//TODO Put this into a list, then fill in the blanks with the selected weapon
			_eqInvPanel._selectedItem._weight.text = "Weight : "; //+ selected weapon weight value;
			_eqInvPanel._selectedItem._fear.text = "Fear : "; //+ selected weapon weight value;
			if (_eqInvPanel._selectedItem._luck != null) {
				_eqInvPanel._selectedItem._luck.text = "Luck : "; //+ selected weapon weight value;
			}
			if(_eqInvPanel._selectedItem._sound != null) {
				_eqInvPanel._selectedItem._sound.text = "Sound : "; //+ selected weapon weight value;
			}
			if(_eqInvPanel._selectedItem._duration != null) {
				_eqInvPanel._selectedItem._duration.text = "Duration : "; //+ selected weapon weight value;
			}

			//Selected Weapon
			InitDisplaySelected(_eqInvPanel._eqStats);
		}

		private void InitDisplaySelected(StatisticsPanel currentPanel) {
			//Selected Weapon Requirements
			for (int i = 0; i < (AllEnums.AttributeNumber - 4); i++) {
				StatsInfoTemplate infoTemp = new StatsInfoTemplate ();
				GameObject gObject = Instantiate (currentPanel._selectedReqSideEffects._template) as GameObject;
				gObject.transform.SetParent (currentPanel._selectedReqSideEffects._grid);
				gObject.transform.localScale = Vector3.one;
				infoTemp._information = gObject.GetComponentInChildren<StatisticsTemplate> ();
				infoTemp._information._description.text = ((AllEnums.AttributeType)i).ToString();
				infoTemp._information._icon.sprite = _icons._allAttributeIcons [i];
				gObject.SetActive (true);
			}
			//Health Benefits
			if (currentPanel._healthList._grid != null) {
				for (int i = 1; i < AllEnums.StatusTypeNumber; i++) {
					StatsInfoTemplate infoTemp = new StatsInfoTemplate ();
					GameObject gObject = Instantiate (currentPanel._healthList._template) as GameObject;
					gObject.transform.SetParent (currentPanel._healthList._grid);
					gObject.transform.localScale = Vector3.one;
					infoTemp._information = gObject.GetComponentInChildren<StatisticsTemplate> ();
					infoTemp._information._description.text = ((AllEnums.StatusType)i).ToString ();
					infoTemp._information._icon.sprite = _icons._allStatusBarIcons [i-1];
					gObject.SetActive (true);
				}
			}
			//Weapon Stats
			for (int i = 0; i < AllEnums.AttackDefenceNumber; i++) {
				StatsInfoTemplate infoTemp = new StatsInfoTemplate ();
				GameObject gObject = Instantiate (currentPanel._attackDefendList._template) as GameObject;
				gObject.transform.SetParent (currentPanel._attackDefendList._grid);
				gObject.transform.localScale = Vector3.one;
				infoTemp._information = gObject.GetComponentInChildren<StatisticsTemplate> ();
				infoTemp._information._description.text = ((AllEnums.AttackDefenceType)i).ToString();
				infoTemp._information._icon.sprite = _icons._allAttackDefenceIcons [i];
				gObject.SetActive (true);
			}
			//Weapon Resistances
			for (int i = 0; i < AllEnums.ResisistancesNumber; i++) {
				StatsInfoTemplate infoTemp = new StatsInfoTemplate ();
				GameObject gObject = Instantiate (currentPanel._resistanceList._template) as GameObject;
				gObject.transform.SetParent (currentPanel._resistanceList._grid);
				gObject.transform.localScale = Vector3.one;
				infoTemp._information = gObject.GetComponentInChildren<StatisticsTemplate> ();
				infoTemp._information._description.text = ((AllEnums.ResistanceType)i).ToString();
				infoTemp._information._icon.sprite = _icons._allResistancesIcons [i];
				gObject.SetActive (true);
			}

			if (currentPanel._healthList._grid == null) {
				//Selected Weapon Cost Gain Statistics
				CostGainTemplate cgTemp1 = new CostGainTemplate ();
				GameObject gObject1 = Instantiate (currentPanel._costGainsList._template) as GameObject;
				gObject1.transform.SetParent (currentPanel._costGainsList._grid);
				gObject1.transform.localScale = Vector3.one;
				cgTemp1._information = gObject1.GetComponentInChildren<StatisticsTemplate> ();
				cgTemp1._information._icon.sprite = _icons._allStatusBarIcons [1];
				cgTemp1._information._description.text = AllEnums.StatusType.Stamina.ToString () + " Cost :";
				gObject1.SetActive (true);

				CostGainTemplate cgTemp2 = new CostGainTemplate ();
				GameObject gObject2 = Instantiate (currentPanel._costGainsList._template) as GameObject;
				gObject2.transform.SetParent (currentPanel._costGainsList._grid);
				gObject2.transform.localScale = Vector3.one;
				cgTemp2._information = gObject2.GetComponentInChildren<StatisticsTemplate> ();
				cgTemp2._information._icon.sprite = _icons._blankSpriteTemplate;
				cgTemp2._information._description.text = "Other Cost :";
				gObject2.SetActive (true);
			} else {
				for (int i = 1; i < AllEnums.BodyNeedsNumber; i++) {
					CostGainTemplate cgTemp = new CostGainTemplate ();
					GameObject gObject = Instantiate (currentPanel._costGainsList._template) as GameObject;
					gObject.transform.SetParent (currentPanel._costGainsList._grid);
					gObject.transform.localScale = Vector3.one;
					cgTemp._information = gObject.GetComponentInChildren<StatisticsTemplate> ();
					cgTemp._information._icon.sprite = _icons._allBodyNeedsIcons [i-1];
					//cgTemp._information._description.text = AllEnums.StatusType.Stamina.ToString () + " Cost :";
					gObject.SetActive (true);
				}
			}
		}

		private void CreateItemsPanel() {
			//Item Quickslots
			for (int i = 0; i < 10; i++) { //hardcoded, but should be amount of player item quickslots (10 max)
				SlotsInfoTemplate iQSlotTemp = new SlotsInfoTemplate();
				GameObject gObject = Instantiate (_eqInvPanel._itemQSlots._template) as GameObject;
				gObject.transform.SetParent (_eqInvPanel._itemQSlots._grid);
				gObject.transform.localScale = Vector3.one;
				iQSlotTemp._information = gObject.GetComponentInChildren<InventorySlotTemplate> ();
				iQSlotTemp._eqSlot = gObject.GetComponentInChildren<EquipmentSlot> ();
				iQSlotTemp._eqSlot._slotEqType = AllEnums.ArmorType.Accessory1;
				iQSlotTemp._eqSlot._slotItype = AllEnums.ItemType.Consumable;
				iQSlotTemp._eqSlot._slotPosition.x = i;
				iQSlotTemp._eqSlot._slotPosition.y = 5;
				iQSlotTemp._eqSlot._slotName = "Item Slot " + (i + 1).ToString();
				iQSlotTemp._eqSlot._itemPosition = i;

				iQSlotTemp._information._description.text = (i + 1).ToString();
				iQSlotTemp._information._iconHUD.sprite = _icons._blankSpriteTemplate;
				iQSlotTemp._information._iconInventory.sprite = _icons._blankSpriteTemplate;
				gObject.SetActive (true);
			}
			//Spell QuickSlots
			for (int i = 0; i < 4; i++) { //hardcoded, but should be amount of player spell quickslots (4 max)
				SlotsInfoTemplate sQSlotTemp = new SlotsInfoTemplate();
				GameObject gObject = Instantiate (_eqInvPanel._spellQSlots._template) as GameObject;
				gObject.transform.SetParent (_eqInvPanel._spellQSlots._grid);
				gObject.transform.localScale = Vector3.one;
				sQSlotTemp._information = gObject.GetComponentInChildren<InventorySlotTemplate> ();
				sQSlotTemp._eqSlot = gObject.GetComponentInChildren<EquipmentSlot> ();
				sQSlotTemp._eqSlot._slotEqType = AllEnums.ArmorType.Accessory1;
				sQSlotTemp._eqSlot._slotItype = AllEnums.ItemType.Spell;
				sQSlotTemp._eqSlot._slotPosition.x = i;
				sQSlotTemp._eqSlot._slotPosition.y = 6;
				sQSlotTemp._eqSlot._slotName = "Spell Slot " + (i + 1);
				sQSlotTemp._eqSlot._itemPosition = i;

				sQSlotTemp._information._description.text = (i + 1).ToString();
				sQSlotTemp._information._iconHUD.sprite = _icons._blankSpriteTemplate;
				sQSlotTemp._information._iconInventory.sprite = _icons._blankSpriteTemplate;
				gObject.SetActive (true);
			}

			//Selected Weapon Information
			_eqInvPanel._selectedItem._itemIcon.sprite = _icons._blankSpriteTemplate;
			_eqInvPanel._selectedItem._itemImage.sprite = _icons._blankSpriteTemplate;
			_eqInvPanel._selectedItem._itemName.text = "";
			_eqInvPanel._selectedItem._durability.text = "";
			_eqInvPanel._selectedItem._itemDescription.text = "";

			//TODO Put this into a list, then fill in the blanks with the selected weapon
			_eqInvPanel._selectedItem._weight.text = "Weight : "; //+ selected weapon weight value;
			_eqInvPanel._selectedItem._fear.text = "Fear : "; //+ selected weapon weight value;
			if (_eqInvPanel._selectedItem._luck != null) {
				_eqInvPanel._selectedItem._luck.text = "Luck : "; //+ selected weapon weight value;
			}
			if(_eqInvPanel._selectedItem._sound != null) {
				_eqInvPanel._selectedItem._sound.text = "Sound : "; //+ selected weapon weight value;
			}
			if(_eqInvPanel._selectedItem._duration != null) {
				_eqInvPanel._selectedItem._duration.text = "Duration : "; //+ selected weapon weight value;
			}

			//Selected Item
			InitDisplaySelected(_eqInvPanel._invStats);
		}

		private void CreatePlayerOverviewPanel() {
			
			_overviewSlots = new StatisticsTemplate[9, 6];

			_playerOverviewPanel._status._stat.text = "Normal"; //hard coded, but this will display if the player is poisoned, hungry, etc.
			_playerOverviewPanel._flockPopulation._stat.text = "0"; //hardcoded, but will display the player's current sheep population
			_playerOverviewPanel._flockPopulation._description.text = "Flock";
			_overviewSlots[0, 5] = _playerOverviewPanel._flockPopulation;
			_playerOverviewPanel._money._stat.text = "0"; //hard coded, but will display the player's current gold amount
			_playerOverviewPanel._money._description.text = "Gold";
			_overviewSlots[1, 5] = _playerOverviewPanel._money;
			_playerOverviewPanel._carryWeight._stat.text = "000/000"; //hardcoded, but will display the player's current carry weight threshhold
			_overviewSlots[2, 5] = _playerOverviewPanel._carryWeight;
			_playerOverviewPanel._poise._stat.text = "000/000"; //hard coded, but will display the player's current poise threshold
			_overviewSlots[3, 5] = _playerOverviewPanel._poise;

			_playerOverviewPanel._selectedStat._icon.sprite = _icons._blankSpriteTemplate;
			_playerOverviewPanel._selectedStat._stat.text = "";
			_playerOverviewPanel._selectedStat._description.text = "";
			_playerOverviewPanel._selectedStatInfo.text = "";

			_overviewSelector.gameObject.SetActive (false);

			//TODO: Refactor this so that there is one function used for all

			//Status Bars
			for (int i = 0; i < AllEnums.StatusTypeNumber; i++) {
				if ((AllEnums.StatusType)i == AllEnums.StatusType.None) {
					_overviewSlots [0, 0] = _playerOverviewPanel._status;
					continue;
				}
				StatusBarTemplate sBarTemp = new StatusBarTemplate ();
				GameObject gObject = Instantiate (_playerOverviewPanel._healthList._template) as GameObject;
				gObject.transform.SetParent (_playerOverviewPanel._healthList._grid);
				gObject.transform.localScale = Vector3.one;
				sBarTemp._information = gObject.GetComponentInChildren<InventoryStatusBarTemplate> ();
				sBarTemp._information._description.text = ((AllEnums.StatusType)i).ToString ();
				sBarTemp._information._sliderColor.sprite = _icons._allStatusBarIcons [i-1];
				sBarTemp._information._stat.text = "0/0";
				gObject.SetActive (true);

				StatisticsTemplate newStat = gObject.GetComponentInChildren<StatisticsTemplate> ();
				newStat._icon.sprite = _icons._allStatusBarIcons [i-1];
				newStat._description.text = ((AllEnums.StatusType)i).ToString ();
				newStat._stat.text = "0/0";
				_overviewSlots [i, 0] = newStat;
			}

			//Attributes
			for (int i = 0; i < (AllEnums.AttributeNumber); i++) {
				StatsInfoTemplate infoTemp = new StatsInfoTemplate ();
				GameObject gObject = Instantiate (_playerOverviewPanel._attributeList._template) as GameObject;
				gObject.transform.SetParent (_playerOverviewPanel._attributeList._grid);
				gObject.transform.localScale = Vector3.one;
				infoTemp._information = gObject.GetComponentInChildren<StatisticsTemplate> ();
				infoTemp._information._description.text = ((AllEnums.AttributeType)i).ToString();
				infoTemp._information._icon.sprite = _icons._allAttributeIcons [i];
				gObject.SetActive (true);
				_overviewSlots [i, 1] = infoTemp._information;
			}

			//Defences
			for (int i = 0; i < AllEnums.AttackDefenceNumber; i++) {
				StatsInfoTemplate infoTemp = new StatsInfoTemplate ();
				GameObject gObject = Instantiate (_playerOverviewPanel._defenceList._template) as GameObject;
				gObject.transform.SetParent (_playerOverviewPanel._defenceList._grid);
				gObject.transform.localScale = Vector3.one;
				infoTemp._information = gObject.GetComponentInChildren<StatisticsTemplate> ();
				infoTemp._information._description.text = ((AllEnums.AttackDefenceType)i).ToString();
				infoTemp._information._icon.sprite = _icons._allAttackDefenceIcons [i];
				gObject.SetActive (true);

				_overviewSlots [i, 2] = infoTemp._information;
			}
			//Resistances
			for (int i = 0; i < AllEnums.ResisistancesNumber; i++) {
				StatsInfoTemplate infoTemp = new StatsInfoTemplate ();
				GameObject gObject = Instantiate (_playerOverviewPanel._resistanceList._template) as GameObject;
				gObject.transform.SetParent (_playerOverviewPanel._resistanceList._grid);
				gObject.transform.localScale = Vector3.one;
				infoTemp._information = gObject.GetComponentInChildren<StatisticsTemplate> ();
				infoTemp._information._description.text = ((AllEnums.ResistanceType)i).ToString();
				infoTemp._information._icon.sprite = _icons._allResistancesIcons [i];
				gObject.SetActive (true);

				_overviewSlots [i, 3] = infoTemp._information;
			}
			//Body Needs
			for (int i = 1; i < AllEnums.BodyNeedsNumber; i++) {
				CostGainTemplate cgTemp = new CostGainTemplate ();
				GameObject gObject = Instantiate (_playerOverviewPanel._bodyNeedList._template) as GameObject;
				gObject.transform.SetParent (_playerOverviewPanel._bodyNeedList._grid);
				gObject.transform.localScale = Vector3.one;
				cgTemp._information = gObject.GetComponentInChildren<StatisticsTemplate> ();
				cgTemp._information._icon.sprite = _icons._allBodyNeedsIcons [i-1];
				cgTemp._information._description.text = ((AllEnums.BodyNeedsType)i).ToString ();
				gObject.SetActive (true);

				_overviewSlots [i-1, 4] = cgTemp._information;
			}
		}

		private void CreateFlockOverviewPanel() {
			_flockOverviewPanel._status.text = "Status: "; //hard coded, but this will display if the sheep is poisoned, hungry, etc.
			_flockOverviewPanel._name.text = ""; //hard coded, but this will display if the sheep's name.
			_flockOverviewPanel._gender.text = "Gender: "; //hard coded, but this will display  the sheep's gender

			int row = 0;
			for (int i = 0; i < _wRControl._runtimeReferences._flock.Count; i++) {
				SlotsInfoTemplate sheepSlotTemp = new SlotsInfoTemplate ();
				GameObject gObject = Instantiate (_flockOverviewPanel._sheepEqSlots._template) as GameObject;
				gObject.transform.SetParent (_flockOverviewPanel._sheepEqSlots._grid);
				gObject.transform.localScale = Vector3.one;

				Biography sheepBio = _wRControl._runtimeReferences._flock [i]._biography;

				sheepSlotTemp._information = gObject.GetComponentInChildren<InventorySlotTemplate> ();
				sheepSlotTemp._information._description.text = (sheepBio._name).Split('[')[0];
				sheepSlotTemp._information._iconHUD.sprite = _icons._blankSpriteTemplate;
                sheepSlotTemp._information._iconInventory.sprite = sheepBio._portrait;

				sheepSlotTemp._eqSlot = gObject.GetComponentInChildren<EquipmentSlot> ();
				//sheepSlotTemp._eqSlot._slotEqType = (AllEnums.ArmorType)i;
				//sheepSlotTemp._eqSlot._slotItype = AllEnums.ItemType.Armor;
				sheepSlotTemp._eqSlot._slotName = sheepBio._name;

				if (i % 5 == 0) {
					row++;
				}
				sheepSlotTemp._eqSlot._slotPosition.x = i % 5;
				sheepSlotTemp._eqSlot._slotPosition.y = row - 1;
				sheepSlotTemp._eqSlot._itemPosition = i;
				gObject.SetActive (true);
			}

			//Status Bars
			for (int i = 0; i < AllEnums.StatusTypeNumber; i++) {
				if ((AllEnums.StatusType)i == AllEnums.StatusType.None) {
					continue;
				}
				StatusBarTemplate sBarTemp = new StatusBarTemplate ();
				GameObject gObject = Instantiate (_flockOverviewPanel._healthList._template) as GameObject;
				gObject.transform.SetParent (_flockOverviewPanel._healthList._grid);
				gObject.transform.localScale = Vector3.one;
				sBarTemp._information = gObject.GetComponentInChildren<InventoryStatusBarTemplate> ();
				sBarTemp._information._description.text = ((AllEnums.StatusType)i).ToString ();
				sBarTemp._information._sliderColor.sprite = _icons._allStatusBarIcons [i-1];
				sBarTemp._information._stat.text = "0/0";
				gObject.SetActive (true);

				StatisticsTemplate newStat = gObject.GetComponentInChildren<StatisticsTemplate> ();
				newStat._icon.sprite = _icons._allStatusBarIcons [i-1];
				newStat._description.text = ((AllEnums.StatusType)i).ToString ();
				newStat._stat.text = "0/0";
			}

			//Attributes
			/*for (int i = 0; i < (AllEnums.AttributeNumber); i++) {
				StatsInfoTemplate infoTemp = new StatsInfoTemplate ();
				GameObject gObject = Instantiate (_flockOverviewPanel._attributeList._template) as GameObject;
				gObject.transform.SetParent (_flockOverviewPanel._attributeList._grid);
				gObject.transform.localScale = Vector3.one;
				infoTemp._information = gObject.GetComponentInChildren<StatisticsTemplate> ();
				infoTemp._information._description.text = ((AllEnums.AttributeType)i).ToString();
				infoTemp._information._icon.sprite = _icons._allAttributeIcons [i];
				gObject.SetActive (true);
			}*/

			//Defences
			for (int i = 0; i < AllEnums.AttackDefenceNumber; i++) {
				StatsInfoTemplate infoTemp = new StatsInfoTemplate ();
				GameObject gObject = Instantiate (_flockOverviewPanel._defenceList._template) as GameObject;
				gObject.transform.SetParent (_flockOverviewPanel._defenceList._grid);
				gObject.transform.localScale = Vector3.one;
				infoTemp._information = gObject.GetComponentInChildren<StatisticsTemplate> ();
				infoTemp._information._description.text = ((AllEnums.AttackDefenceType)i).ToString();
				infoTemp._information._icon.sprite = _icons._allAttackDefenceIcons [i];
				gObject.SetActive (true);

			}
			//Resistances
			for (int i = 0; i < AllEnums.ResisistancesNumber; i++) {
				StatsInfoTemplate infoTemp = new StatsInfoTemplate ();
				GameObject gObject = Instantiate (_flockOverviewPanel._resistanceList._template) as GameObject;
				gObject.transform.SetParent (_flockOverviewPanel._resistanceList._grid);
				gObject.transform.localScale = Vector3.one;
				infoTemp._information = gObject.GetComponentInChildren<StatisticsTemplate> ();
				infoTemp._information._description.text = ((AllEnums.ResistanceType)i).ToString();
				infoTemp._information._icon.sprite = _icons._allResistancesIcons [i];
				gObject.SetActive (true);

			}
			//Body Needs
			for (int i = 1; i < AllEnums.BodyNeedsNumber; i++) {
				CostGainTemplate cgTemp = new CostGainTemplate ();
				GameObject gObject = Instantiate (_flockOverviewPanel._bodyNeedList._template) as GameObject;
				gObject.transform.SetParent (_flockOverviewPanel._bodyNeedList._grid);
				gObject.transform.localScale = Vector3.one;
				cgTemp._information = gObject.GetComponentInChildren<StatisticsTemplate> ();
				cgTemp._information._icon.sprite = _icons._allBodyNeedsIcons [i-1];
				cgTemp._information._description.text = ((AllEnums.BodyNeedsType)i).ToString ();
				gObject.SetActive (true);
			}

			EquipmentSlot[] flock = _sheepSlotsParent.GetComponentsInChildren<EquipmentSlot> ();
            //Debug.Log("Flock count: " + _wRControl._runtimeReferences._flock.Count);

            _sSlots = new EquipmentSlot[_wRControl._runtimeReferences._flock.Count];
			for (int i = 0; i < flock.Length; i++) {
				flock [i]._icon = flock [i].GetComponent<InventorySlotTemplate> ();
				_eqSlotsUI._sheepSlots.Add(flock[i]);
                _flockOverviewPanel._allFlockUI.Add(flock[i].gameObject);
                //int x = Mathf.RoundToInt (flock [i]._slotPosition.x);
                //int y = Mathf.RoundToInt (flock [i]._slotPosition.y);
                _sSlots[i] = flock[i];
				//_equipmentSlots [x, y].Init (this);
			}
		}
			
		public void Tick(GameInputs input, float delta) {
			_delta = delta;

			HandleUIState (input);

			//Does this go in HandleEquipSlot... or here?
			if (_invState == AllEnums.InventoryUIState.Equipment || _invState == AllEnums.InventoryUIState.Inventory) {
				if (_prevEqSlot != _curEqSlot) {
					if (_curEqSlot == null) {
						return;
					}

					if (_curStorageSlot == null) {
						if (_invState == AllEnums.InventoryUIState.Equipment) {
							_eqInvPanel._slotName.text = _curEqSlot._slotName;
							_eqInvPanel._slotName.text = "";
						} else if (_invState == AllEnums.InventoryUIState.Inventory) {
							_eqInvPanel._slotName.text = _curEqSlot._slotName;
							_eqInvPanel._slotName.text = "";
						}
							
						LoadItemFromSlot (_curEqSlot._icon);
					}
				}
				_prevEqSlot = _curEqSlot;

				//Again, does this go here? or somewhere else
				if (_storageSlots != null && _storageSlots.Count > 0) {
					if (_prevStorageIndex != _curStorageIndex) {
						if (_curStorageSlot != null) {
							_curStorageSlot._background.color = _unselected;
						}

						if (_curStorageIndex >= 0 && _curStorageIndex < _maxStorageIndex) {
							_curStorageSlot = _storageSlots [_curStorageIndex];
						}

						if (_curStorageSlot != null) {
							_curStorageSlot._background.color = _selected;
							LoadItemFromSlot (_curStorageSlot);
						}
					}
				}
				_prevStorageIndex = _curStorageIndex;
			} else if (_invState == AllEnums.InventoryUIState.Overview) {
				if (_prevOvSlot != _curOvSlot) {
					if (_curOvSlot == null) {
						return;
					}
					_prevOvSlot = _curOvSlot;
					_overviewSelector.transform.SetParent (_overviewSlots [Mathf.RoundToInt (_overviewSelectorPos.x), Mathf.RoundToInt (_overviewSelectorPos.y)].transform);
					_overviewSelector.anchoredPosition = Vector2.zero;
				}
				if (_overviewSelector.gameObject.activeInHierarchy) {
					_curOvSlot = _overviewSlots [Mathf.RoundToInt (_overviewSelectorPos.x), Mathf.RoundToInt (_overviewSelectorPos.y)];

					LoadStatFromSelection (_curOvSlot);
				}
			} else if (_invState == AllEnums.InventoryUIState.Flock) {
				if (_prevSheepSlot != _curSheepSlot) {
					if (_curSheepSlot == null) {
						return;
					}

					LoadSheepSlot (_curSheepSlot._icon);
				}
				_prevSheepSlot = _curSheepSlot;
			}
		}

		private void UpdatePlayerOverviewPanel () {
			CharacterBody body;
			body = _player._body;

			_playerOverviewPanel._carryWeight._stat.text = body._carryWeight.GetMeter().ToString() + "/" + body._carryWeight.GetMaximum().ToString();
			_playerOverviewPanel._poise._stat.text = body._poise.GetMeter().ToString () + "/" + body._poise.GetMaximum().ToString ();

			if (_invState != AllEnums.InventoryUIState.Overview) {
				return;
			}

			_playerOverviewPanel._status._stat.text = ": " + _player._biography._currentBioCard._status.ToString ();
			_playerOverviewPanel._money._stat.text = (1000000).ToString(); //temp
            _playerOverviewPanel._flockPopulation._stat.text = _player._wRControl._runtimeReferences._flock.Count.ToString();

			List<StatusList> status = new List<StatusList> (); //TODO: This might be a memory leak, since this runs
			status.Add(new StatusList(body._health.GetMaximum(), body._health.GetMeter()));
			status.Add(new StatusList(body._stamina.GetMaximum(), body._stamina.GetMeter()));
			status.Add(new StatusList(body._courage.GetMaximum(), body._courage.GetMeter()));
			status.Add(new StatusList(body._immuneSystem.GetMaximum(), body._immuneSystem.GetMeter()));

			InventoryStatusBarTemplate[] statusBars = _playerOverviewPanel._healthList._grid.GetComponentsInChildren<InventoryStatusBarTemplate> ();
			for (int i = 0; i < statusBars.Length; i++) {
				statusBars [i]._slider.maxValue = status[i]._max;
				statusBars [i]._slider.value = Mathf.Lerp (statusBars [i]._slider.value, status[i]._current, _delta * 2f);
				statusBars [i]._stat.text = (Mathf.Round (Mathf.Lerp (statusBars [i]._slider.value, status[i]._current, _delta * 2f)) + "/" + status[i]._max).ToString ();
			}
			WeaponStatsInArray playerStats = new WeaponStatsInArray();
			playerStats.CreatePlayerArrays (body, _player._biography._currentBioCard._currentStatistics);

			//Attributes
			StatisticsTemplate[] playerAttributes = _playerOverviewPanel._attributeList._grid.GetComponentsInChildren<StatisticsTemplate>();
			for (int i = 0; i < playerAttributes.Length; i++) {
				playerAttributes[i]._stat.text = (playerStats._requirements[i]).ToString();
			}
			//Defences
			StatisticsTemplate[] playerDefences = _playerOverviewPanel._defenceList._grid.GetComponentsInChildren<StatisticsTemplate>();
			for (int i = 0; i < playerDefences.Length; i++) {
				playerDefences[i]._stat.text = (playerStats._attackDefence[i]).ToString();
			}
			//Resistances
			StatisticsTemplate[] playerResistances = _playerOverviewPanel._resistanceList._grid.GetComponentsInChildren<StatisticsTemplate>();
			for (int i = 0; i < playerResistances.Length; i++) {
				playerResistances[i]._stat.text = (playerStats._resistanceChance[i]).ToString();
			}
			//Body Needs
			List<StatusList> bodyNeeds = new List<StatusList> ();
			bodyNeeds.Add(new StatusList(body._thirst.GetMaximum(), body._thirst.GetMeter()));
			bodyNeeds.Add(new StatusList(body._waste.GetMaximum(), body._waste.GetMeter()));
			bodyNeeds.Add(new StatusList(body._hunger.GetMaximum(), body._hunger.GetMeter()));
			bodyNeeds.Add(new StatusList(body._sleep.GetMaximum(), body._sleep.GetMeter()));
			bodyNeeds.Add(new StatusList(body._pleasure.GetMaximum(), body._pleasure.GetMeter()));

			StatisticsTemplate[] playerBodyNeeds = _playerOverviewPanel._bodyNeedList._grid.GetComponentsInChildren<StatisticsTemplate>();
			for (int i = 0; i < playerBodyNeeds.Length; i++) {
				playerBodyNeeds [i]._stat.text = (bodyNeeds [i]._current).ToString () + "/" + (bodyNeeds [i]._max).ToString ();
			}
		}

        public void UpdateSheepSlot()
        {
            LoadSheepSlot(_curSheepSlot._icon);
        }

		private void LoadSheepSlot (InventorySlotTemplate slot) {
			if (_invState != AllEnums.InventoryUIState.Flock) {
				return;
			}

			Biography sheepBio = _wRControl._runtimeReferences._flock [_curSheepSlot._itemPosition]._biography;
			CharacterBody body = _wRControl._runtimeReferences._flock [_curSheepSlot._itemPosition]._body;

			_flockOverviewPanel._status.text = "Status: " + sheepBio._currentBioCard._status.ToString ();
			_flockOverviewPanel._name.text = "" + (sheepBio._name).Split('[')[0]; //temp
			_flockOverviewPanel._gender.text = "Gender: " + sheepBio._gender.ToString();

			_flockOverviewPanel._sheepHead._information._description.text = "Head";
			_flockOverviewPanel._sheepTorso._information._description.text = "Torso";
            _flockOverviewPanel._sheepLegs._information._description.text = "Legs";
			_flockOverviewPanel._sheepWool._information._description.text = "Wool";

            _flockOverviewPanel._sheepArmorSlots = new StatsInfoTemplate[] { _flockOverviewPanel._sheepHead, _flockOverviewPanel._sheepTorso, _flockOverviewPanel._sheepLegs, _flockOverviewPanel._sheepWool };

            _flockOverviewPanel._starSheep.isOn = sheepBio._currentBioCard._isMarkedAsImportant;

			List<RuntimeArmor> sheepEquipment = _wRControl._runtimeReferences._flock [_curSheepSlot._itemPosition]._invControl._currentArmorSet;
			for (int i = 0; i < sheepEquipment.Count; i++) {
				Armor sArmor = (Armor)sheepEquipment [i]._instance;
                switch (sArmor._armorType) {
				case AllEnums.ArmorType.Accessory1:
					_flockOverviewPanel._sheepHead._information._description.text = sheepEquipment [i]._instance._itemName;
					_flockOverviewPanel._sheepHead._information._icon.sprite = _eqSlotsUI._blankSprite;
					break;
				case AllEnums.ArmorType.Accessory2:
					_flockOverviewPanel._sheepTorso._information._description.text = sheepEquipment [i]._instance._itemName;
					_flockOverviewPanel._sheepTorso._information._icon.sprite = _eqSlotsUI._blankSprite;
					break;
				case AllEnums.ArmorType.Accessory3:
					_flockOverviewPanel._sheepLegs._information._description.text = sheepEquipment [i]._instance._itemName;
					_flockOverviewPanel._sheepLegs._information._icon.sprite = _eqSlotsUI._blankSprite;
					break;
                    case AllEnums.ArmorType.Torso:
                        _flockOverviewPanel._sheepWool._information._description.text = sheepEquipment[i]._instance._itemName;
                        _flockOverviewPanel._sheepWool._information._icon.sprite = _eqSlotsUI._blankSprite;
                        break;

				}
			}


			List<StatusList> status = new List<StatusList> ();
			status.Add(new StatusList(body._health.GetMaximum(), body._health.GetMeter()));
			status.Add(new StatusList(body._stamina.GetMaximum(), body._stamina.GetMeter()));
			status.Add(new StatusList(body._courage.GetMaximum(), body._courage.GetMeter()));
			status.Add(new StatusList(body._immuneSystem.GetMaximum(), body._immuneSystem.GetMeter()));

			InventoryStatusBarTemplate[] statusBars = _flockOverviewPanel._healthList._grid.GetComponentsInChildren<InventoryStatusBarTemplate> ();
			for (int i = 0; i < statusBars.Length; i++) {
				statusBars [i]._slider.maxValue = status[i]._max;
				statusBars [i]._slider.value = status[i]._current;
				statusBars [i]._stat.text = status[i]._current.ToString() + "/" + status[i]._max.ToString ();
			}
			WeaponStatsInArray sheepStats = new WeaponStatsInArray();
			sheepStats.CreatePlayerArrays (body, sheepBio._currentBioCard._currentStatistics);

			//Attributes
			/*StatisticsTemplate[] sheepAttributes = _flockOverviewPanel._attributeList._grid.GetComponentsInChildren<StatisticsTemplate>();
			for (int i = 0; i < sheepAttributes.Length; i++) {
				sheepAttributes[i]._stat.text = (sheepStats._requirements[i]).ToString();
			}*/
			//Defences
			StatisticsTemplate[] sheepDefences = _flockOverviewPanel._defenceList._grid.GetComponentsInChildren<StatisticsTemplate>();
			for (int i = 0; i < sheepDefences.Length; i++) {
				sheepDefences[i]._stat.text = (sheepStats._attackDefence[i]).ToString();
			}
			//Resistances
			StatisticsTemplate[] sheepResistances = _flockOverviewPanel._resistanceList._grid.GetComponentsInChildren<StatisticsTemplate>();
			for (int i = 0; i < sheepResistances.Length; i++) {
				sheepResistances[i]._stat.text = (sheepStats._resistanceChance[i]).ToString();
			}
			//Body Needs
			List<StatusList> bodyNeeds = new List<StatusList> ();
			
			bodyNeeds.Add(new StatusList(body._thirst.GetMaximum(), body._thirst.GetMeter()));
            bodyNeeds.Add(new StatusList(body._waste.GetMaximum(), body._waste.GetMeter()));
            bodyNeeds.Add(new StatusList(body._hunger.GetMaximum(), body._hunger.GetMeter()));
            bodyNeeds.Add(new StatusList(body._sleep.GetMaximum(), body._sleep.GetMeter()));
			bodyNeeds.Add(new StatusList(body._pleasure.GetMaximum(), body._pleasure.GetMeter()));

			StatisticsTemplate[] playerBodyNeeds = _flockOverviewPanel._bodyNeedList._grid.GetComponentsInChildren<StatisticsTemplate>();
			for (int i = 0; i < playerBodyNeeds.Length; i++) {
				playerBodyNeeds [i]._stat.text = (Mathf.Round(bodyNeeds [i]._current)).ToString () + "/" + (bodyNeeds [i]._max).ToString ();
			}
		}

		private void LoadItemFromSlot(InventorySlotTemplate slot) {
			if (_curEqSlot == null) {
				return;
			}
			switch (_curEqSlot._slotItype) {
			case AllEnums.ItemType.Weapon:
				_eqInvPanel._statisticsHeading.text = "Weapon Statistics";
				LoadWeaponItem (slot);
				break;
			case AllEnums.ItemType.Armor:
				_eqInvPanel._statisticsHeading.text = "Armor Statistics";
				LoadArmorItem (slot);
				break;
			case AllEnums.ItemType.Consumable:
				_eqInvPanel._statisticsHeading.text = "Consumable Statistics";
				LoadConsumableItem (slot);
				break;
			case AllEnums.ItemType.Spell:
				_eqInvPanel._statisticsHeading.text = "Companion Statistics";
				//LoadSpellItem (ResourceController._singleton, slot);
				break;
			default:
				return;
			}
		}

		private void LoadWeaponItem(InventorySlotTemplate slot) {
			WorldItem item = _wRControl.GetItem (slot._ID);
			if (item == null) {
				item = _wRControl.GetItem ("Empty"); //I think this will work...
			}

			WeaponStatsInArrayV2 playerStats = new WeaponStatsInArrayV2 ();
			Shepherd.Weapon playerArmDamage = null;
			Shepherd.RuntimeWeapon playerRTweapon = new Shepherd.RuntimeWeapon ();

			//LocalPlayerReferences playerLoadout = _wRControl._runtimeReferences._curEquipment;//_player._invControl._currentEquipment;
			RuntimeReferences playerLoadout = _wRControl._runtimeReferences;

			switch (_curEqSlot._slotName) {
			case "Left 1":
				if (playerLoadout._leftHand._value.Count > 0) {
					playerArmDamage = _wRControl.GetWeapon ((playerLoadout._leftHand._value [0]._itemName));
					playerRTweapon = _wRControl._runtimeReferences.FindRuntimeWeapons (AllEnums.PreferredHand.Left) [0];
				}
				break;
			case "Left 2":
				if (playerLoadout._leftHand._value.Count > 1) {
					playerArmDamage = _wRControl.GetWeapon ((playerLoadout._leftHand._value [1]._itemName));
					playerRTweapon = _wRControl._runtimeReferences.FindRuntimeWeapons (AllEnums.PreferredHand.Left) [1];
				}
				break;
			case "Left 3":
				if (playerLoadout._leftHand._value.Count > 2) {
					playerArmDamage = _wRControl.GetWeapon ((playerLoadout._leftHand._value [2]._itemName));
					playerRTweapon = _wRControl._runtimeReferences.FindRuntimeWeapons (AllEnums.PreferredHand.Left) [2];
				}
				break;
			case "Right 1":
				if (playerLoadout._rightHand._value.Count > 0) {
					playerArmDamage = _wRControl.GetWeapon (playerLoadout._rightHand._value [0]._itemName);
					playerRTweapon = _wRControl._runtimeReferences.FindRuntimeWeapons (AllEnums.PreferredHand.Right) [0];
				}
				break;
			case "Right 2":
				if (playerLoadout._rightHand._value.Count > 1) {
					playerArmDamage = _wRControl.GetWeapon (playerLoadout._rightHand._value [1]._itemName);
					playerRTweapon = _wRControl._runtimeReferences.FindRuntimeWeapons (AllEnums.PreferredHand.Right) [1];
				}
				break;
			case "Right 3":
				if (playerLoadout._rightHand._value.Count > 2) {
					playerArmDamage = _wRControl.GetWeapon (playerLoadout._rightHand._value [2]._itemName);
					playerRTweapon = _wRControl._runtimeReferences.FindRuntimeWeapons (AllEnums.PreferredHand.Right) [2];
				}
				break;
			}

			if (playerArmDamage == null) {
				playerArmDamage = _wRControl.GetWeapon ("Unarmed");
			}

			if (_curStorageSlot == null) {
				playerStats.CreateArrays (playerArmDamage._itemStats);
				playerStats._requirements [0] = _player._biography._currentBioCard._currentStatistics._strength;
				playerStats._requirements [1] = _player._biography._currentBioCard._currentStatistics._endurance;
				playerStats._requirements [2] = _player._biography._currentBioCard._currentStatistics._dexterity;
				playerStats._requirements [3] = _player._biography._currentBioCard._currentStatistics._intelligence;
				playerStats._requirements [4] = _player._biography._currentBioCard._currentStatistics._vitality;
				playerStats._requirements [5] = _player._biography._currentBioCard._currentStatistics._perception;
				playerStats._requirements [6] = _player._biography._currentBioCard._currentStatistics._courage;
				playerStats._requirements [7] = _player._biography._currentBioCard._currentStatistics._luck;
			} else {
				playerStats.CreateArrays (playerArmDamage._itemStats);
				if (item is Weapon) {
					playerArmDamage = _wRControl.GetWeapon (_curStorageSlot._ID);
				} else {
					Debug.Log ("whoops: " + item.name);
					playerArmDamage = _wRControl.GetWeapon (_curEqSlot._icon._ID);
				}
			}

			if (playerArmDamage == null) {
				playerArmDamage = _wRControl.GetWeapon ("Unarmed");
			}

			DisplaySelected (_eqInvPanel._eqStats, playerArmDamage, playerRTweapon, playerStats);
		}

		private void LoadArmorItem(InventorySlotTemplate slot) {
			List<RuntimeArmor> currentRTArmors = _wRControl._runtimeReferences.FindRuntimeArmors (true);
			Shepherd.RuntimeArmor curArmor = new RuntimeArmor ();
			for (int i = 0; i < currentRTArmors.Count; i++) {
				Shepherd.Armor armorInstance = (Shepherd.Armor)currentRTArmors [i]._instance;
				if (armorInstance._armorType == _curEqSlot._slotEqType) {
					curArmor = currentRTArmors [i];
					break;
				}
			}
				
			if (curArmor._instance == null) {
				curArmor._instance = (Shepherd.Armor)_wRControl.GetItem ("Empty");
			}

			//compare with stored armor or itself
			WeaponStatsInArrayV2 playerStats = new WeaponStatsInArrayV2 ();
			WorldItem item;
			if (!_isSwitchingItem || _curStorageSlot == null) {
				item = _wRControl.GetItem (slot._ID);
				if (item == null) {
					item = _wRControl.GetItem ("Empty"); //I think this will work...
				}
				playerStats.CreateArrays (curArmor._instance._itemStats);
			} else {
				item = _wRControl.GetItem (_curStorageSlot._ID);
				if (item == null) {
					item = _wRControl.GetItem ("Empty"); //I think this will work...
				}

				WorldItem pItem = _wRControl.GetItem (_curEqSlot._icon._ID);
				if (pItem == null) {
					pItem = _wRControl.GetItem ("Empty"); //I think this will work...
				}
				playerStats.CreateArrays (pItem._itemStats);
			}

			playerStats._requirements [0] = _player._biography._currentBioCard._currentStatistics._strength;
			playerStats._requirements [1] = _player._biography._currentBioCard._currentStatistics._endurance;
			playerStats._requirements [2] = _player._biography._currentBioCard._currentStatistics._dexterity;
			playerStats._requirements [3] = _player._biography._currentBioCard._currentStatistics._intelligence;
			playerStats._requirements [4] = _player._biography._currentBioCard._currentStatistics._vitality;
			playerStats._requirements [5] = _player._biography._currentBioCard._currentStatistics._perception;
			playerStats._requirements [6] = _player._biography._currentBioCard._currentStatistics._courage;
			playerStats._requirements [7] = _player._biography._currentBioCard._currentStatistics._luck;
		
			DisplaySelected (_eqInvPanel._eqStats, item, curArmor, playerStats);
		}

		private void LoadConsumableItem(InventorySlotTemplate slot) {
			List<RuntimeConsumable> currentRTConsumables = _wRControl._runtimeReferences.FindRuntimeConsumables (true);
			RuntimeConsumable curConsumable = new RuntimeConsumable ();
			for (int i = 0; i < currentRTConsumables.Count; i++) {
				Consumable consumableInstance = (Consumable)currentRTConsumables [i]._instance;
                if(_wRControl.CompareWorldItem(consumableInstance._itemName, _curEqSlot._icon._ID)) { 
					curConsumable = currentRTConsumables [i];
					break;
				}
			}

			if (curConsumable._instance == null) {
				curConsumable._instance = _wRControl.GetItem ("Hands"); //for now, refers to Hands scriptable
			}

			//test
			List<RuntimeConsumable> curRTStored = _wRControl._runtimeReferences.FindRuntimeConsumables(false);
			RuntimeConsumable storedConsumable = new RuntimeConsumable ();
			for (int i = 0; i < curRTStored.Count; i++) {
				Consumable consumableInstance = (Consumable)curRTStored [i]._instance;
                if(_wRControl.CompareWorldItem(consumableInstance._itemName, slot._ID)) { 
					storedConsumable = curRTStored [i];
					break;
				}
			}

			if (storedConsumable._instance == null) {
				storedConsumable._instance = _wRControl.GetItem ("Hands"); //for now, refers to Hands scriptable
			}

			WeaponStatsInArrayV2 playerStats = new WeaponStatsInArrayV2 ();

            //if not switching item or not -- grab from stored or current -- may not be necessary
            WorldItem item = (_isSwitchingItem) ? storedConsumable._instance : curConsumable._instance;
				
            playerStats.CreateArrays(curConsumable._instance._itemStats);
            playerStats.CreateHealthEffects(curConsumable._instance);
            //...

            //compare with stored consumable or itself
            /*WeaponStatsInArrayV2 playerStats = new WeaponStatsInArrayV2 ();
			WorldItem item;
			if (!_isSwitchingItem) {
				item = _wRControl.GetItem (slot._ID);
				if (item == null) {
					item = _wRControl.GetItem ("Hands"); //I think this will work...
				}
				playerStats.CreateArrays (curConsumable._instance._itemStats);
				playerStats.CreateHealthEffects (curConsumable._instance);
			} else {
				item = _wRControl.GetItem (_curStorageSlot._ID);
				if (item == null) {
					item = _wRControl.GetItem ("Hands"); //I think this will work...
				}

				WorldItem pItem = _wRControl.GetItem (_curEqSlot._icon._ID);
				if (pItem == null) {
					pItem = _wRControl.GetItem ("Hands"); //I think this will work...
				}
				playerStats.CreateArrays (pItem._itemStats);
				playerStats.CreateHealthEffects (pItem);
			}*/

            playerStats._requirements [0] = _player._biography._currentBioCard._currentStatistics._strength;
			playerStats._requirements [1] = _player._biography._currentBioCard._currentStatistics._endurance;
			playerStats._requirements [2] = _player._biography._currentBioCard._currentStatistics._dexterity;
			playerStats._requirements [3] = _player._biography._currentBioCard._currentStatistics._intelligence;
			playerStats._requirements [4] = _player._biography._currentBioCard._currentStatistics._vitality;
			playerStats._requirements [5] = _player._biography._currentBioCard._currentStatistics._perception;
			playerStats._requirements [6] = _player._biography._currentBioCard._currentStatistics._courage;
			playerStats._requirements [7] = _player._biography._currentBioCard._currentStatistics._luck;

			DisplaySelected (_eqInvPanel._invStats, item, curConsumable, playerStats);
		}

		private void DisplaySelected(StatisticsPanel currentPanel, WorldItem currentSelection, RuntimeWorldItem currentRTSelection, WeaponStatsInArrayV2 currentPlayerStats) {
			WeaponStatsInArrayV2 currentSelectStats = new WeaponStatsInArrayV2 ();
			currentSelectStats.CreateArrays (currentSelection._itemStats);
			if (currentSelection is Shepherd.Consumable) {
				currentSelectStats.CreateHealthEffects (currentSelection);
			}
			//Selected Weapon Information
			_eqInvPanel._selectedItem._itemIcon.sprite = currentSelection._itemHUDIcon;
			_eqInvPanel._selectedItem._itemImage.sprite = currentSelection._itemIcon;
			_eqInvPanel._selectedItem._itemName.text = currentSelection._itemName;
			if (_invState == AllEnums.InventoryUIState.Equipment) {
				_eqInvPanel._selectedItem._durability.text = currentRTSelection._durability.ToString () + "/" + currentSelectStats._maxDurability.ToString ();
			} else if (_invState == AllEnums.InventoryUIState.Inventory) {
				_eqInvPanel._selectedItem._durability.text = "x" + currentRTSelection._durability.ToString();
			}
			_eqInvPanel._selectedItem._itemDescription.text = currentSelection._itemDescription;

			//TODO Put this into a list
			_eqInvPanel._selectedItem._weight.text = "Weight: " + currentSelectStats._weight; //may be part of item class
			_eqInvPanel._selectedItem._fear.text = "Fear: " + currentSelectStats._fear; //may be part of item class
			if (_eqInvPanel._selectedItem._luck != null) {
				_eqInvPanel._selectedItem._luck.text = "Luck: " + currentSelectStats._luck; //may be part of item class
			}
			if(_eqInvPanel._selectedItem._sound != null) {
				_eqInvPanel._selectedItem._sound.text = "Sound: " + currentSelectStats._soundVolume; //may be part of item clss
			}
			if (_invState == AllEnums.InventoryUIState.Inventory && _eqInvPanel._selectedItem._duration != null) {
				_eqInvPanel._selectedItem._duration.text = "Duration: " + currentSelectStats._maxDurability + " s"; //may be part of item class
			} else {
				_eqInvPanel._selectedItem._duration.text = "Duration: N/A";
			}
				
			//Selected Weapon Requirements
			StatisticsTemplate[] weaponReq = currentPanel._selectedReqSideEffects._grid.GetComponentsInChildren<StatisticsTemplate> ();
			for (int i = 0; i < weaponReq.Length; i++) {
				StatisticsTemplate infoTemp = weaponReq [i];
				infoTemp._description.text = ((AllEnums.AttributeType)i).ToString ();
				infoTemp._icon.sprite = _icons._allAttributeIcons [i];
				infoTemp._stat.text = currentSelectStats._requirements [i].ToString ();

				if (currentSelectStats._requirements[i] > currentPlayerStats._requirements [i]) {
					infoTemp._stat.color = _loss;
				} else if (currentSelectStats._requirements[i] == currentPlayerStats._requirements [i]) {
					infoTemp._stat.color = _default;
				} else {
					infoTemp._stat.color = _gain;
				}
			}
			//Health Benefits
			if (_invState == AllEnums.InventoryUIState.Inventory && currentPanel._healthList._grid != null) {
				StatisticsTemplate[] healthBenefits = currentPanel._healthList._grid.GetComponentsInChildren<StatisticsTemplate> ();
				for(int i = 0; i < healthBenefits.Length; i++) {
					StatisticsTemplate infoTemp = healthBenefits [i];
					infoTemp._description.text = ((AllEnums.StatusType)i+1).ToString ();
					infoTemp._icon.sprite = _icons._allStatusBarIcons [i];
					infoTemp._stat.text = currentSelectStats._healthEffects [i].ToString ();

					infoTemp._stat.text = currentSelectStats._healthEffects [i].ToString () + "/" + currentPlayerStats._healthEffects[i].ToString();
						
					if (currentSelectStats._healthEffects[i] > currentPlayerStats._healthEffects [i]) {
						infoTemp._stat.color = _gain;
					} else if (currentSelectStats._healthEffects[i] == currentPlayerStats._healthEffects [i]) {
						infoTemp._stat.color = _default;
					} else {
						infoTemp._stat.color = _loss;
					}
				}
			}
				
			//Weapon Stats
			StatisticsTemplate[] weaponStats = currentPanel._attackDefendList._grid.GetComponentsInChildren<StatisticsTemplate> ();
			for (int i = 0; i < AllEnums.AttackDefenceNumber; i++) {
				StatisticsTemplate infoTemp = weaponStats [i];
				infoTemp._description.text = ((AllEnums.AttackDefenceType)i).ToString ();
				infoTemp._icon.sprite = _icons._allAttackDefenceIcons [i];

				infoTemp._stat.text = currentSelectStats._attackDefence [i].ToString () + "/" + currentPlayerStats._attackDefence[i];

				if (currentSelectStats._attackDefence [i] < currentPlayerStats._attackDefence[i]) {
					infoTemp._stat.color = _loss;
				} else if (currentSelectStats._attackDefence [i] == currentPlayerStats._attackDefence[i]) {
					infoTemp._stat.color = _default;
				} else {
					infoTemp._stat.color = _gain;
				}
			}

			//Weapon Resistances
			StatisticsTemplate[] weaponResistances = currentPanel._resistanceList._grid.GetComponentsInChildren<StatisticsTemplate> ();
			for (int i = 0; i < AllEnums.ResisistancesNumber; i++) {
				StatisticsTemplate infoTemp = weaponResistances [i];
				infoTemp._description.text = ((AllEnums.ResistanceType)i).ToString ();
				infoTemp._icon.sprite = _icons._allResistancesIcons [i];
				infoTemp._stat.text = currentSelectStats._resistanceChance [i].ToString () + "/" + currentPlayerStats._resistanceChance[i];

				if (currentSelectStats._resistanceChance [i] < currentPlayerStats._resistanceChance[i]) {
					infoTemp._stat.color = _loss;
				} else if (currentSelectStats._resistanceChance [i] == currentPlayerStats._resistanceChance[i]) {
					infoTemp._stat.color = _default;
				} else {
					infoTemp._stat.color = _gain;
				}
			}

			if (currentSelection is Shepherd.Consumable && _invState == AllEnums.InventoryUIState.Inventory) {
				//Body Benefits
				StatisticsTemplate[] bodyBenefits = currentPanel._costGainsList._grid.GetComponentsInChildren<StatisticsTemplate>();
				for (int i = 0; i < bodyBenefits.Length; i++) {
					StatisticsTemplate infoTemp = bodyBenefits [i];
					//infoTemp._description.text = ((AllEnums.BodyNeedsType)i+1).ToString ();
					infoTemp._icon.sprite = _icons._allBodyNeedsIcons [i];

					infoTemp._stat.text = currentSelectStats._bodyEffects [i].ToString () + "/" + currentPlayerStats._bodyEffects[i].ToString();

					if (currentSelectStats._bodyEffects [i] < currentPlayerStats._bodyEffects[i] || i == 1) { //i = 1 being waste, waste is bad
						infoTemp._stat.color = _loss;
					} else if (currentSelectStats._bodyEffects [i] == currentPlayerStats._bodyEffects[i]) {
						infoTemp._stat.color = _default;
					} else {
						infoTemp._stat.color = _gain;
					}
				}
			} else {
				//Weapon Energy Costs
				/*CostGainTemplate cgTemp1 = new CostGainTemplate ();
				GameObject gObject1 = Instantiate (currentPanel._costGainsList._template) as GameObject;
				gObject1.transform.SetParent (currentPanel._costGainsList._grid);
				gObject1.transform.localScale = Vector3.one;
				cgTemp1._information = gObject1.GetComponentInChildren<StatisticsTemplate> ();
				cgTemp1._information._icon.sprite = _icons._allStatusBarIcons [1];
				cgTemp1._information._description.text = AllEnums.StatusType.Stamina.ToString () + " Cost :";
				gObject1.SetActive (true);

				CostGainTemplate cgTemp2 = new CostGainTemplate ();
				GameObject gObject2 = Instantiate (currentPanel._costGainsList._template) as GameObject;
				gObject2.transform.SetParent (currentPanel._costGainsList._grid);
				gObject2.transform.localScale = Vector3.one;
				cgTemp2._information = gObject2.GetComponentInChildren<StatisticsTemplate> ();
				cgTemp2._information._icon.sprite = _icons._blankSpriteTemplate;
				Shepherd.Weapon test = (Shepherd.Weapon)currentSelection;
				cgTemp2._information._description.text = "Other Cost :";
				gObject2.SetActive (true);*/
			}
		}

		private void LoadStatFromSelection(StatisticsTemplate selected) {
			if (selected._description != null) {
				_playerOverviewPanel._selectedStat._description.text = selected._description.text;
			}
			_playerOverviewPanel._selectedStat._icon.sprite = selected._icon.sprite;
			_playerOverviewPanel._selectedStat._stat.text = selected._stat.text.Trim(":".ToCharArray());

			string loadStat = (selected._description.text + "StatInfo");
			if (_statInfo.GetStatInfo(loadStat) != null) {
				_playerOverviewPanel._selectedStatInfo.text = _statInfo.GetStatInfo(loadStat)._variable;
			} else {
				
				_playerOverviewPanel._selectedStatInfo.text = _statInfo.GetStatInfo ("NoneStatInfo")._variable;
			}
		}

		public void HandleUIState(GameInputs input) {
            switch (_invState) {
			case AllEnums.InventoryUIState.None:
				HandleInGameMenuMovement(input);
				break;
			case AllEnums.InventoryUIState.Equipment:
				if (!_isSwitchingItem) {
					HandleEquipSlotMovement (input);
				} else {
					HandleStorageSlotMovement (input);
				}
				UpdatePlayerOverviewPanel ();
				break;
			case AllEnums.InventoryUIState.Inventory:
				if (!_isSwitchingItem) {
					HandleEquipSlotMovement (input);
				} else {
					HandleStorageSlotMovement (input);
				}
				UpdatePlayerOverviewPanel ();
				break;
			case AllEnums.InventoryUIState.Overview:
				UpdatePlayerOverviewPanel ();
				HandlePlayerOverviewMovement (input);
				break;
			case AllEnums.InventoryUIState.Flock:
                    if (!_isSwitchingSheepItem)
                    {
                        HandleFlockOverview(input);
                    } else
                    {
                        HandleSheepEquipment(input);
                    }
				break;
			case AllEnums.InventoryUIState.Gestures:
				HandleGesturesMovement(input);
				break;
                case AllEnums.InventoryUIState.Report:
                    HandleWeatherReportMovement(input);
                    break;
			default:
				
				break;
			}
        }

        private void HandleWeatherReportMovement(GameInputs input)
        {
            if(input._dDownPress)
            {
                _weatherReportPanel._eventLogScrollbar.value -= 0.15f;
                _weatherReportPanel._eventLogScrollbar.onValueChanged.Invoke(_weatherReportPanel._eventLogScrollbar.value);
            }
            if (input._dUpPress)
            {
                _weatherReportPanel._eventLogScrollbar.value += 0.15f;
                _weatherReportPanel._eventLogScrollbar.onValueChanged.Invoke(_weatherReportPanel._eventLogScrollbar.value);
            }


            if (input._interactConfirmButton)
            {
                UserInterfaceController.Instance.CloseInGameUI();
                _invState = AllEnums.InventoryUIState.None;
                GameSessionController.Instance._curGameState = AllEnums.InputControlState.Game;
                //TODO: Check if day ends with special event campfire - else close and head into next level / night
                GameSessionController.Instance._endOfDay = false;
                GameSessionController.Instance._gameTimeOfDay += 101;
                GameSessionController.Instance._currentScene += 1;
                _weatherReportPanel.TeardownReport();

                UserInterfaceController.Instance.gameObject.GetComponent<MainMenuButtons>().CreateNewSave(true);
                return;
            }
        }

        private void HandleGesturesMovement(GameInputs input) {
			if (input._dUpPress) {
				//y--;
				
			}
			if (input._dDownPress) {
				//y++;
				
			}
			if (input._dRightPress) {
				_gesturesPanel.SelectGesture (true);
				
			}
			if (input._dLeftPress) {
				_gesturesPanel.SelectGesture (false);
				
				return;
			}

			if (input._dodgeCancelButton) {
				Reset ();
				_curStorageSlot = null;

				UserInterfaceController.Instance.OpenInGameUI ();
				
				return;
			}

			if (input._weakLeftButton) {
				NavigateInGameMenus (false);
				return;
			}

			if (input._weakRightButton) {
				NavigateInGameMenus (true);
				return;
			}

			if (input._interactConfirmButton) {
				Reset ();
				_curStorageSlot = null;

				UserInterfaceController.Instance.CloseInGameUI ();
                GameSessionController.Instance._curGameState = AllEnums.InputControlState.Game;

				_gesturesPanel.HandleGesturesMenu (false);
				if (_gesturesPanel._closeWeapons) {
					_player._states._closeWeapons = true;
				}
				_player.PlayAnimation (_gesturesPanel._currentGestureAnimation, false);
				
				return;
			}
		}

		/// <summary>
		/// Navigates the in game menus.
		/// if positive:True = Next Page, 
		/// if postive:False = Previous Page
		/// </summary>
		public void NavigateInGameMenus(bool positive) {
			Reset ();
			_curStorageSlot = null;
			AllEnums.InventoryUIState prevState = _invState;

			if (positive) {
				_invState++;
			} else {
				_invState--;
			}

			//TODO: This reaches end of enum if on Gestures, and reads as 6, temp fix to revert back to start of enum
			if (prevState == AllEnums.InventoryUIState.Gestures && _invState == (AllEnums.InventoryUIState)6) {
				_invState = AllEnums.InventoryUIState.Equipment;
			} else if (prevState == AllEnums.InventoryUIState.Equipment && _invState == AllEnums.InventoryUIState.None) {
				_invState = AllEnums.InventoryUIState.Gestures;
			}

			UserInterfaceController.Instance.CycleInventoryUIMenus ();
		}

        public void NavigateInGameMenus(int toInventoryState)
        {
            Reset();
            _curStorageSlot = null;
            AllEnums.InventoryUIState prevState = _invState;
            if(toInventoryState <= 0 || toInventoryState >= 5)
            {
                //keep only in correct menus, for now...
                _invState = prevState;
            } else
            {
                _invState = (AllEnums.InventoryUIState)toInventoryState;
            }
            UserInterfaceController.Instance.CycleInventoryUIMenus();
        }

		private void HandleFlockOverview(GameInputs input) {
			int x = _sheepSelectPosition;


            if (!_flockOverviewPanel._isRenaming)
            {
                if (input._dUpPress)
                {
                    x -= 5;
                    _flockOverviewPanel._allFlockScrollbar.value += 0.15f;
                    _flockOverviewPanel._allFlockScrollbar.onValueChanged.Invoke(_flockOverviewPanel._allFlockScrollbar.value);
                }
                if (input._dDownPress)
                {
                    x += 5;
                    _flockOverviewPanel._allFlockScrollbar.value -= 0.15f;
                    _flockOverviewPanel._allFlockScrollbar.onValueChanged.Invoke(_flockOverviewPanel._allFlockScrollbar.value);
                }

                if (input._dRightPress)
                {
                    x++;
                }

                if (input._dLeftPress)
                {
                    x--;

                }
            }

			if (x < 0) {
				x = _sSlots.Length-1;
                _flockOverviewPanel._allFlockScrollbar.onValueChanged.Invoke(0f);
            }
            if (x > _sSlots.Length-1) {
				x = 0;
                _flockOverviewPanel._allFlockScrollbar.onValueChanged.Invoke(1f);
            }

			if (_curSheepSlot != null) {
				_curSheepSlot._icon._background.color = _unselected;
			}

            if (_sSlots.Length > 0)
            {
                //Choose slot based off of if it is active on ui
                int slots = 0;
                while (!_sSlots[x].gameObject.activeSelf)
                {
                    x++;
                    slots++;
                    if (x > _sSlots.Length - 1)
                    {
                        x = 0;
                        _flockOverviewPanel._allFlockScrollbar.onValueChanged.Invoke(1f);
                        if (slots > _sSlots.Length)
                        {
                            break;
                        }
                    } else if (x % 5 == 0)
                    {
                        _flockOverviewPanel._allFlockScrollbar.value -= 0.15f;
                        _flockOverviewPanel._allFlockScrollbar.onValueChanged.Invoke(_flockOverviewPanel._allFlockScrollbar.value);
                    }
                }
                _curSheepSlot = _sSlots[x];
            }
			_sheepSelectPosition = x;

			if (_curSheepSlot != null) {
				_curSheepSlot._icon._background.color = _selected;
			}

			if (input._dodgeCancelButton) {
                if (!_flockOverviewPanel._isRenaming) { 
				    Reset ();
				    _curStorageSlot = null;

				    UserInterfaceController.Instance.OpenInGameUI ();
				} else
                {
                    _flockOverviewPanel._isRenaming = false;
                    _flockOverviewPanel._renameSheep.gameObject.SetActive(false);
                }
				return;
			}

			if (input._weakLeftButton && GameSessionController.Instance._isUsingController) {
				NavigateInGameMenus (false);
				return;
			}

			if (input._weakRightButton && GameSessionController.Instance._isUsingController) {
				NavigateInGameMenus (true);
				return;
			}

			if (input._interactConfirmButton && !_flockOverviewPanel._isRenaming) {
                _isSwitchingSheepItem = !_isSwitchingSheepItem;
                _sheepArmorPosition = 0;
				return;
			}
		}

        private void HandleSheepEquipment(GameInputs input)
        {
            int x = _sheepArmorPosition;


            if (!_flockOverviewPanel._isRenaming)
            {
                if (input._dUpPress)
                {
                    x -= 4;
                }
                if (input._dDownPress)
                {
                    x += 4;
                }

                if (input._dRightPress)
                {
                    x++;
                }

                if (input._dLeftPress)
                {
                    x--;

                }
            }

            if (x < 0)
            {
                x = _flockOverviewPanel._sheepArmorSlots.Length - 1;
            }
            if (x > _flockOverviewPanel._sheepArmorSlots.Length - 1)
            {
                x = 0;
            }

            _sheepArmorPosition = x;

            for (int i = 0; i < _flockOverviewPanel._sheepArmorSlots.Length; i++)
            {
                if (i == x)
                {
                    _flockOverviewPanel._sheepArmorSlots[i]._information.gameObject.GetComponent<Image>().color = _selected;
                } else
                {
                    _flockOverviewPanel._sheepArmorSlots[i]._information.gameObject.GetComponent<Image>().color = _unselected;
                }
            }

            if (input._dodgeCancelButton)
            {
                _isSwitchingSheepItem = !_isSwitchingSheepItem;

                return;
            }
        }

		private void HandleInGameMenuMovement(GameInputs input) {
            if (input._dUpPress) {
				//y--;
				
			}
			if (input._dDownPress) {
				//y++;
				
			}
			if (input._dRightPress) {
				_inGamePanel.SelectGesture (true);
				
			}
			if (input._dLeftPress) {
				_inGamePanel.SelectGesture (false);
				
			}

			if (input._dodgeCancelButton) {
				Reset ();
				_curStorageSlot = null;

				UserInterfaceController.Instance.CloseInGameUI ();
				_invState = AllEnums.InventoryUIState.None;
                GameSessionController.Instance._curGameState = AllEnums.InputControlState.Game;
				
				return;
			}

			if (input._interactConfirmButton) {
				AllEnums.InventoryUIState newInvState = (AllEnums.InventoryUIState)(_inGamePanel._gesturesIndex + 1);
				Debug.Log(newInvState + " is active");

				_invState = newInvState;
				UserInterfaceController.Instance.CycleInventoryUIMenus ();
				
			}
		}

		//NOT Sure this needs to be here... could be in userinterfacecontroller or even inputcontroller
		private void HandleEquipSlotMovement(GameInputs input) {
			//TODO: make this less hard coded
			int x = Mathf.RoundToInt (_eqSelectPosition.x);
			int y = Mathf.RoundToInt (_eqSelectPosition.y);

			ClosePreviousItems ();
			if (_curEqSlot != null) {
				LoadCurrentItems (_curEqSlot._slotItype);
			}

            if (input._lockOnButton) {
				//HideShowPanels ();
				return;
			}

			if (input._dUpPress) {
				y--;
				
			}
			if (input._dDownPress) {
				y++;
				
			}
			if (input._dRightPress) {
				x++;
				
			}

			if (input._dLeftPress) {
				x--;
				
			} 

			/*
		 * hard coded a 10x7 grid, starting from 0
		 * [0,	1,	2,	-,	-,	-,	-,	-,	-,	-] accessories, 7 blank
		 * [3,	4,	5,	-,	-,	-,	-,	-,	-,	-] left arm, head, right arm, 7 blank
		 * [6,	7,	8,	-,	-,	-,	-,	-,	-,	-] left hand, torso, right hand, 7 blank
		 * [9,	10,	11,	 -,	-,	-,	-,	-,	-,	-] left foot, legs, right foot, 7 blank
		 * [12,	13,	14,	15,	16,	17,	-,	-,	-,	-] 12,13,14 = left-hand weapons, 15,16,17 = right-hand weapons, 4 blank
		 * [18,	19,	20,	21,	22,	23,	24,	25,	26,	27] item quickslots, 0 blank
		 * [28,	29,	30,	31, -, -,	-,	-,	-,	-] spells, 6 blank
		*/
			if (_invState == AllEnums.InventoryUIState.Equipment) {

				if (x > 2 && y != 4) { 
					x = 0;
				}
				if (x < 0 && y != 4) {
					x = 2;
				}

				if (x < 0 && y == 4) {
					x = 5;
				}
				if (x > 5 && y == 4) {
					x = 0;
				}

				if (y > 4) {
					y = 0;
				}
				if (y < 0) {
					y = 4;
				}
			} else if (_invState == AllEnums.InventoryUIState.Inventory) {
				if (y == 5 && x > 9) {
					x = 0;
				} else if (y == 6 && x > 3) {
					x = 0;
				}

				if (y == 5 && x < 0) {
					x = 9;
				}

				if (y == 6 && x < 0) {
					x = 3;
				}

				//Debug.Log (x + " " + y);
				if (y > 6) {
					y = 5;
					x = 0;
				} 
				if (y < 5) {
					y = 6;
					x = 0;
				}
			}

			if (_curEqSlot != null) {
				_curEqSlot._icon._background.color = _unselected;
			}

			_curEqSlot = _eqSlots [x, y];

			_eqSelectPosition.x = x;
			_eqSelectPosition.y = y;

			if (_curEqSlot != null) {
				_curEqSlot._icon._background.color = _selected;
			}

			if (input._useButton) {
				switch (_curEqSlot._slotItype) {
				case AllEnums.ItemType.Weapon:
					ClearWeapon ();
					break;
				case AllEnums.ItemType.Consumable:
					ClearConsumable ();
					break;
				case AllEnums.ItemType.Spell:
					//ClearSpell ();
					break;
				case AllEnums.ItemType.Armor:
					ClearArmor ();
					break;
				}
				LoadEquipment (true);
				_isSwitchingItem = false;
				_curStorageSlot = null;
				
				return;
			}

			if (input._dodgeCancelButton) {
				Reset ();
				_curStorageSlot = null;

				UserInterfaceController.Instance.OpenInGameUI ();
				
				//HideShowPanels ();
				return;
			}

			if (input._weakLeftButton) {
				NavigateInGameMenus (false);
				return;
			}

			if (input._weakRightButton) {
				NavigateInGameMenus (true);
				return;
			}

			//we have selected an item we wish to swap out
			if (input._interactConfirmButton) {
				_isSwitchingItem = !_isSwitchingItem;
				_curStorageSlot = null;
				_curStorageIndex = 0;
				_prevStorageIndex = -1;
				
				return;
			}
		}

		//NOT Sure this needs to be here... could be in userinterfacecontroller or even inputcontroller
		private void HandleStorageSlotMovement(GameInputs input) {
			if (_curEqSlot == null) {
				Debug.Log ("I Get RUN!");
				return;
			}
				
			ClosePreviousItems ();
			LoadCurrentItems (_curEqSlot._slotItype);
			InventorySlotTemplate[] storageEntries;
			if (_invState == AllEnums.InventoryUIState.Equipment) {
				storageEntries = _eqInvPanel._storage._slotGrid.GetComponentsInChildren<InventorySlotTemplate> ();
			} else if (_invState == AllEnums.InventoryUIState.Inventory) {
				storageEntries = _eqInvPanel._storage._slotGrid.GetComponentsInChildren<InventorySlotTemplate> ();
			} else {
				storageEntries = null;
			}

			if (storageEntries != null && storageEntries.Length == 0) {
				Debug.Log ("NO GO");
				_isSwitchingItem = !_isSwitchingItem;
				_curStorageSlot = null;
				_curStorageIndex = 0;
				_prevStorageIndex = -1;
				return;
			}

            if (input._dUpPress) {
				_curStorageIndex -= 9;
				
			}

			if (input._dDownPress) {
				_curStorageIndex += 9;
				
			}

			if (input._dRightPress) {
				_curStorageIndex++;
				
			}

			if (input._dLeftPress) {
				_curStorageIndex--;
				
			}

			//Handle end of Storage Array
			if (_curStorageIndex > (_maxStorageIndex - 1)) {
				_curStorageIndex = 0;
			} else if (_curStorageIndex < 0) {
				_curStorageIndex = (_maxStorageIndex - 1);
			}

			if (_curStorageSlot != null) {
				_curStorageSlot._background.color = _selected;
			}

			if (_curStorageSlot != null && !_curStorageSlot.isActiveAndEnabled) {
				_isSwitchingItem = false;
				return;
			}

			if (input._interactConfirmButton) {
				if (_curStorageSlot != null) {
					switch (_curEqSlot._slotItype) {
					case AllEnums.ItemType.Weapon:
						SwapWeapon ();
						break;
					case AllEnums.ItemType.Consumable:
						SwapConsumable ();
						break;
					case AllEnums.ItemType.Spell:
						//SwapSpell ();
						break;
					case AllEnums.ItemType.Armor:
						SwapArmor ();
						break;
					default:
						break;
					}
				}
				LoadEquipment (true);
				_isSwitchingItem = false;
				if (_curStorageSlot != null) {
					_curStorageSlot._background.color = _unselected;
				}
				_curStorageSlot = null;
				
				return;
			}

			if (input._dodgeCancelButton) {
				_isSwitchingItem = false;
				
				if (_curStorageSlot != null) {
					_curStorageSlot._background.color = _unselected;
				}
				_curStorageSlot = null;
				return;
			}
		}

		private void HandlePlayerOverviewMovement(GameInputs input) {
			//TODO: make this less hard coded
			int x = Mathf.RoundToInt (_overviewSelectorPos.x);
			int y = Mathf.RoundToInt (_overviewSelectorPos.y);

			if (input._lockOnButton) {
				//HideShowPanels ();
				return;
			}

            if (input._dUpPress) {
				x++;
			}
			if (input._dDownPress) {
				x--;
			}
			if (input._dRightPress) {
				y++;
			}

			if (input._dLeftPress) {
				y--;
			} 

			/*
		 * hard coded a 9x5 grid, starting from 0
		 * [0,	1,	2,	3,	4,	-,	-,	-,	-] status, health, stamina, courage, immunesystem, 4 blank
		 * [6,	7,	8,	9,	10,	11,	12,	13,	-] attributes, 1 blank
		 * [15,	16,	17,	18,	19,	20,	21,	22,	23] defences, 0 blank
		 * [25,	26,	27,	28,	29,	30,	-,	-,	-] resistances, 3 blank
		 * [32,	32,	33,	34,	35,	-,	-,	-,	-] body needs,  4 blank
		 * [36, 37, 38, 39,	-, 	-,	-,	-,	-] money, flock population, carry weight, poise
		*/
			if (y == 0 && x > 4) {
				x = 0;
			} else if (y == 1 && x > 7) {
				x = 0;
			} else if (y == 2 && x > 8) {
				x = 0;
			} else if (y == 3 && x > 5) {
				x = 0;
			} else if (y == 4 && x > 4) {
				x = 0;
			} else if (y == 5 && x > 3) {
				x = 0;
			}

			if (y == 0 && x < 0) {
				x = 4;
			} else if (y == 1 && x < 0) {
				x = 7;
			} else if (y == 2 && x < 0) {
				x = 8;
			} else if (y == 3 && x < 0) {
				x = 5;
			} else if (y == 4 && x < 0) {
				x = 4;
			} else if (y == 5 && x < 0) {
				x = 3;
			}

			//Debug.Log (x + " " + y);
			if (y > 5) {
				y = 0;
				x = 0;
			} 
			if (y < 0) {
				y = 5;
				x = 0;
			}
				
			_overviewSelectorPos.x = x;
			_overviewSelectorPos.y = y;

			if (input._dodgeCancelButton) {
				Reset ();
				_curStorageSlot = null;

				UserInterfaceController.Instance.OpenInGameUI ();
				return;
			}

			if (input._weakLeftButton) {
				NavigateInGameMenus (false);
				return;
			}

			if (input._weakRightButton) {
				NavigateInGameMenus (true);
				return;
			}

			if (input._interactConfirmButton) {
				_overviewSelector.gameObject.SetActive (true);
			}
		}

		public void LoadEquipment(bool loadOnCharacter = false) {
			//Shepherd.InventoryController iController;
			//iController = _player._inventoryControl;
			//LocalPlayerReferences playerLoadout = _wRControl._runtimeReferences._curEquipment; //_player._invControl._currentEquipment;
			RuntimeReferences playerLoadout = _wRControl._runtimeReferences;

			if (loadOnCharacter) {
				//iController.ClearRuntimeGear ();
			}

			if (_invState == AllEnums.InventoryUIState.Equipment) {
				//equipment
				//for (int i = 0; i < iController._equippedItems._data.Count; i++) {
				for (int i = 0; i < playerLoadout._armor._value.Count; i++) {
					if (i > 11) {
						//hardcoded so that there is only 12 pieces of armor allowed on at a time
						break;
					}
					for (int j = 0; j < _eqSlotsUI._equipmentSlots.Count; j++) {
						EquipmentSlot eqSlot = _eqSlotsUI._equipmentSlots [j];
						Shepherd.Armor armor = (Shepherd.Armor)playerLoadout._armor._value [i];
						if (eqSlot._slotEqType == armor._armorType) {
							_eqSlotsUI.UpdateEquipmentSlotV2 (playerLoadout._armor._value [i], AllEnums.ItemType.Armor, eqSlot);
						}
					}
				}

				//weapons

				StatisticsTemplate[] leftStats = _eqInvPanel._leftHandStats._grid.GetComponentsInChildren<StatisticsTemplate> ();
				for (int i = 0; i < playerLoadout._leftHand._value.Count; i++) {
					if (i > 2) {
						//hardcoded so that there is only 3 weapons allowed on quickslots
						break;
					}
					
					EquipmentSlot eqSlot = _eqSlotsUI._weaponSlots [i]; // _equipmentSlotsUI[0,1,2] onward are lefthand weapons

					//ItemInventoryInstance iInstance = _session.GetWeaponItem (iController._leftHandQslots [i]);
					//iInstance._slot = eqSlot;
					//iInstance._equipIndex = i;
					_eqSlotsUI.UpdateEquipmentSlotV2 (playerLoadout._leftHand._value [i], AllEnums.ItemType.Weapon, eqSlot);

					//Temporary
					leftStats [i]._stat.text = (StatsCalculator.CalculateRawDamageOfWeapon (playerLoadout._leftHand._value [i]._itemStats)).ToString () + " |";
				}

				StatisticsTemplate[] rightStats = _eqInvPanel._rightHandStats._grid.GetComponentsInChildren<StatisticsTemplate> ();
				for (int i = 0; i < playerLoadout._rightHand._value.Count; i++) {
					if (i > 2) {
						//hardcoded so that there is only 3 weapons allowed on quickslots
						break;
					}

					EquipmentSlot eqSlot = _eqSlotsUI._weaponSlots [i + 3]; //+3 _equipmentSlotsUI[3,4,5] onward are righthand weapons

					//ItemInventoryInstance iInstance = _session.GetWeaponItem (iController._rightHandQslots [i]);
					//iInstance._slot = eqSlot;
					//iInstance._equipIndex = (i + 3);

					_eqSlotsUI.UpdateEquipmentSlotV2 (playerLoadout._rightHand._value [i], AllEnums.ItemType.Weapon, eqSlot);

					//Temporary
					rightStats [i]._stat.text = (StatsCalculator.CalculateRawDamageOfWeapon (playerLoadout._rightHand._value [i]._itemStats)).ToString () + " |";
				}
			}


			//spells
			/*for (int i = 0; i < iController._spellsQslots.Count; i++) {
				if (i > 3) {
					//hardcoded so that there is only 4 spells allowed on quickslots
					break;
				}
				EquipmentSlot eqSlot = _equipmentSlotsUI._spellSlots [i];

				if (iController._spellsQslots [i] == (-1)) {
					_equipmentSlotsUI.ClearEquipmentSlot (eqSlot, eqSlot._slotItype);
				} else {
					ItemInventoryInstance iInstance = _session.GetSpellItem (iController._spellsQslots [i]);
					iInstance._slot = eqSlot;
					iInstance._equipIndex = i;

					_equipmentSlotsUI.UpdateEquipmentSlot (iInstance._uniqueID, AllEnums.ItemType.Spell, eqSlot);
				}
			}*/

			//consumables
			if (_invState == AllEnums.InventoryUIState.Inventory) {
				for (int i = 0; i < 10; i++) {
					EquipmentSlot eqSlot = _eqSlotsUI._consumableSlots [i];
					_eqSlotsUI.ClearEquipmentSlot (eqSlot, AllEnums.ItemType.Consumable);
				}
				for (int i = 0; i < playerLoadout._consumable._value.Count; i++) {
					if (i > 9) {
						//hardcoded so that there is only 9 items allowed on quickslots
						break;
					}

					EquipmentSlot eqSlot = _eqSlotsUI._consumableSlots [i];
					//ItemInventoryInstance iInstance = _session.GetConsumableItem (iController._consumableItemQslots [i]);
					//iInstance._slot = eqSlot;
					//iInstance._equipIndex = i;
					_eqSlotsUI.UpdateEquipmentSlotV2 (playerLoadout._consumable._value [i], AllEnums.ItemType.Consumable, eqSlot);
				}
			}

			if(_invState == AllEnums.InventoryUIState.Flock) {
				for (int i = 0; i < playerLoadout._flock.Count; i++) {
					Biography sheepBio = playerLoadout._flock [i]._biography;
					EquipmentSlot eqSlot = _eqSlotsUI._sheepSlots [i];
					eqSlot._icon._iconHUD.sprite = _eqSlotsUI._blankSprite;
					eqSlot._icon._iconHUD.transform.localScale = Vector3.one;
					eqSlot._icon._iconInventory.transform.localScale = Vector3.one;
					eqSlot._icon._description.text = (sheepBio._name).Split('[')[0];;
					eqSlot._icon._ID = sheepBio._name;
				}
			}

			//Armor
			//LoadArmor ();

			if (loadOnCharacter) {
				//temporary, leaves lots for garbage collector to cleanup and duplicate references -- FIXED?
				//iController.LoadInventory ();
				//_player._actionControl.UpdateActionsWithCurrentItems();
			}
		}

		public void Reset() {
			_isSwitchingItem = false;
            _isSwitchingSheepItem = false;
            _prevEqSlot = null;
			_prevSheepSlot = null;
			if (_curEqSlot != null) {
				_curEqSlot._icon._background.color = _unselected;
			}
			if (_curSheepSlot != null) {
				_curSheepSlot._icon._background.color = _unselected;
			}
			_eqSelectPosition = Vector2.zero;
			_curEqSlot = _eqSlots [0, 0];
            if (_sSlots != null && _sSlots.Length > 0) {
                _curSheepSlot = _sSlots[0];
            }

			if (_curStorageSlot != null) {
				_curStorageSlot._background.color = _unselected;
			}
			_curStorageIndex = 0;
			_prevStorageIndex = -1;

			_curStorageSlot = null;
			_curEqSlot = null;
			_curSheepSlot = null;



			InventoryStatusBarTemplate[] statusBars = _playerOverviewPanel._healthList._grid.GetComponentsInChildren<InventoryStatusBarTemplate> ();
			if (_invState == AllEnums.InventoryUIState.Overview) {
				for (int i = 0; i < statusBars.Length; i++) {
					statusBars [i]._slider.maxValue = 0f;
					statusBars [i]._slider.value = 0f;
					statusBars [i]._stat.text = "000/000";
				}
				//Debug.Log ("resetting status bars");
			}
			_playerOverviewPanel._selectedStat._description.text = "";
			_playerOverviewPanel._selectedStat._icon.sprite = _icons._blankSpriteTemplate;
			_playerOverviewPanel._selectedStat._stat.text = "";
			_playerOverviewPanel._selectedStatInfo.text = "";
			_overviewSelectorPos = Vector2.zero;
			_curOvSlot = _overviewSlots [0, 0];
			_prevOvSlot = null;
			_overviewSelector.gameObject.SetActive (false);
		}

		private void ClosePreviousItems() {
			for (int i = 0; i < _eqStorageSlotsCreated.Count; i++) {
				_eqStorageSlotsCreated [i].gameObject.SetActive(false);
			}
		}

		//might need to rename this
		public void LoadCurrentItems(AllEnums.ItemType iType) {
			List<RuntimeWorldItem> rtItems;

			GameObject equipStorageTemplate;
			Transform equipStorageParent;
			if (iType == AllEnums.ItemType.Armor) {
				equipStorageTemplate = _eqInvPanel._storage._slotTemplate;
				equipStorageParent = _eqInvPanel._storage._slotGrid;

				rtItems = _wRControl._runtimeReferences.FindRuntimeArmorsByType (false, _curEqSlot._slotEqType);
			} else if (iType == AllEnums.ItemType.Weapon) {
				equipStorageTemplate = _eqInvPanel._storage._slotTemplate;
				equipStorageParent = _eqInvPanel._storage._slotGrid;

				rtItems = _wRControl._runtimeReferences.FindRTWeapons (AllEnums.PreferredHand.Ambidextrous);
			} else {
				equipStorageTemplate = _eqInvPanel._storage._slotTemplate;
				equipStorageParent = _eqInvPanel._storage._slotGrid;

				rtItems = _wRControl._runtimeReferences.FindRTConsumables (false);
			}

			//helps so that if we create too many storage slots (i.e we have 3 helmets but 1 chest armour in storage), we can hide the slots we don't need
			int storedDifference = _eqStorageSlotsCreated.Count - rtItems.Count;

			int extraSlots = (storedDifference > 0) ? storedDifference : 0;

			_storageSlots = new List<InventorySlotTemplate> ();

			for (int i = 0; i < rtItems.Count + extraSlots; i++) {
				if (i > (rtItems.Count - 1)) {
					_eqStorageSlotsCreated [i].gameObject.SetActive (false);
					continue;
				}
				WorldItem item = rtItems[i]._instance;
                if (_wRControl.CompareWorldItem(item._itemName, "Unarmed") || _wRControl.CompareWorldItem(item._itemName, "Hands"))
                {
                    //ignore these items as they are only available when player has nothing in weapon or consumable slots respectively
                    continue;
                }

				InventorySlotTemplate storageSlot = null;
				if ((_eqStorageSlotsCreated.Count - 1) < i) {
					GameObject gObject = Instantiate (equipStorageTemplate) as GameObject;
					gObject.SetActive (true);
					gObject.transform.SetParent (equipStorageParent);
					gObject.transform.localScale = Vector3.one;
					storageSlot = gObject.GetComponent<InventorySlotTemplate> ();
					_eqStorageSlotsCreated.Add (storageSlot);
				} else {
					storageSlot = _eqStorageSlotsCreated [i];
				}
				//not sure if needed
				storageSlot._iconHUD.enabled = true;
				storageSlot._iconInventory.enabled = true;

				storageSlot.gameObject.SetActive (true);

				storageSlot._iconHUD.sprite = item._itemHUDIcon;
				storageSlot._iconInventory.sprite = item._itemIcon;
				storageSlot._description.text = item._itemName; //or item name?
				storageSlot._ID = item._itemName;
				_storageSlots.Add (storageSlot);
			}
			_maxStorageIndex = _storageSlots.Count;
		}

		private void ClearWeapon() {
			int targetIndex = _curEqSlot._itemPosition;
			//unequip weapon in...
			bool rightHand = (_curEqSlot._itemPosition > 2) ? true : false;

			//variables needed
			Shepherd.Weapon defaultEmpty = (Shepherd.Weapon)_player._invControl._emptyHandsDefault;

			List<WorldItem> rightSlots = _player._invControl._runtimeRefs._rightHand._value;
			List<RuntimeWeapon> rightRTWeapons = _wRControl._runtimeReferences.FindRuntimeWeapons (AllEnums.PreferredHand.Right);
			Shepherd.RuntimeWeapon currentRightWeapon = _player._invControl._currentRightWeapon;
			StatisticsTemplate[] rightStats = _eqInvPanel._rightHandStats._grid.GetComponentsInChildren<StatisticsTemplate> (); //not sure if necessary

			List<WorldItem> leftSlots = _player._invControl._runtimeRefs._leftHand._value; //_player._invControl._currentEquipment._leftHand._value;
			List<RuntimeWeapon> leftRTWeapons = _wRControl._runtimeReferences.FindRuntimeWeapons (AllEnums.PreferredHand.Left);
			Shepherd.RuntimeWeapon currentLeftWeapon = _player._invControl._currentLeftWeapon;
			StatisticsTemplate[] leftStats = _eqInvPanel._leftHandStats._grid.GetComponentsInChildren<StatisticsTemplate> ();

			List<RuntimeWorldItem> unequippedWeapons = _wRControl._runtimeReferences.FindRTWeapons (AllEnums.PreferredHand.Ambidextrous);


			//TODO: This can be simplified so we can use the same code twice but only write it once
			if (rightHand) {
				//...right hand
				targetIndex -= 3; //minus offset

				if (!string.IsNullOrEmpty (_curEqSlot._icon._ID)) {
                    //there is something equipped in this slot
                    if (_wRControl.CompareWorldItem(_curEqSlot._icon._ID, currentRightWeapon._instance._itemName)) {
                        //slot is currently equipped weapon
                        if (rightSlots.Count == 1)
                        {
                            //there is only one weapon equipped on this hand, therefore we need to Instantiate an Unarmed Weapon
                            for (int i = 0; i < unequippedWeapons.Count; i++)
                            {
                                if (_wRControl.CompareWorldItem(defaultEmpty._itemName, unequippedWeapons[i]._instance._itemName))
                                {
                                    Shepherd.RuntimeWeapon weaponToEquip = (Shepherd.RuntimeWeapon)unequippedWeapons[i];
                                    weaponToEquip._equippedHand = AllEnums.PreferredHand.Right;

                                    //instantiate weapon to be equipped to currentRightWeapon
                                    Shepherd.Weapon weaponToInstanitate = (Shepherd.Weapon)weaponToEquip._instance;
                                    weaponToEquip._itemName = weaponToEquip._instance._itemName;
                                    GameObject gObject = Instantiate(weaponToInstanitate._modelPrefab) as GameObject;
                                    gObject.name = weaponToEquip._instance._itemName;
                                    weaponToEquip._rtModel = gObject;
                                    weaponToEquip._weaponHook = gObject.GetComponentInChildren<WeaponHook>();
                                    weaponToEquip._weaponHook.InitDamageColliders(_player);

                                    //Destroy currentRightWeapon Model and label it as unequipped
                                    currentRightWeapon._equippedHand = AllEnums.PreferredHand.Ambidextrous;
                                    Destroy(currentRightWeapon._rtModel);
                                    currentRightWeapon._rtModel = null;
                                    currentRightWeapon._weaponHook = null;

                                    //Setup newly equipped weapon
                                    _player._invControl._currentRightWeapon = weaponToEquip;
                                    currentRightWeapon = weaponToEquip;
                                    rightSlots[targetIndex] = weaponToEquip._instance;
                                    _player._invControl.EquipWeapon(currentRightWeapon, false);
                                    _eqSlotsUI.UpdateEquipmentSlotV2(weaponToEquip._instance, AllEnums.ItemType.Weapon, _curEqSlot);
                                    Debug.Log("Replacing Currently Equipped Right with Unarmed Default");
                                    break;
                                }
                            }
                        }
                        else
                        {
                            //there is another weapon equipped on this hand, therefore we will swap to that weapon
                            for (int i = 0; i < rightRTWeapons.Count; i++)
                            {
                                if (_wRControl.CompareWorldItem(_curEqSlot._icon._ID, rightRTWeapons[i]._instance._itemName))
                                {
                                    //Destroy currentRightWeapon Model and label it as unequipped
                                    currentRightWeapon._equippedHand = AllEnums.PreferredHand.Ambidextrous;
                                    Destroy(currentRightWeapon._rtModel);
                                    currentRightWeapon._rtModel = null;
                                    currentRightWeapon._weaponHook = null;

                                    //redetermine what weapons are equipped in hand
                                    rightRTWeapons = _wRControl._runtimeReferences.FindRuntimeWeapons(AllEnums.PreferredHand.Right);

                                    Shepherd.RuntimeWeapon weaponToEquip = (Shepherd.RuntimeWeapon)rightRTWeapons[0];
                                    //instantiate weapon to be equipped to currentRightWeapon
                                    Shepherd.Weapon weaponToInstanitate = (Shepherd.Weapon)weaponToEquip._instance;
                                    GameObject gObject = Instantiate(weaponToInstanitate._modelPrefab) as GameObject;
                                    gObject.name = weaponToEquip._instance._itemName;
                                    weaponToEquip._itemName = weaponToEquip._instance._itemName;
                                    weaponToEquip._rtModel = gObject;
                                    weaponToEquip._weaponHook = gObject.GetComponentInChildren<WeaponHook>();
                                    weaponToEquip._weaponHook.InitDamageColliders(_player);

                                    //Setup newly equipped weapon
                                    _player._invControl._currentRightWeapon = weaponToEquip;
                                    currentRightWeapon = weaponToEquip;

                                    _player._invControl.EquipWeapon(currentRightWeapon, false);
                                    Debug.Log("Unequipping Current Left Weapon, shifting to another Weapon in slots");
                                    break;
                                }
                            }
                            //clear UI
                            for (int j = 0; j < _eqSlotsUI._weaponSlots.Count; j++)
                            {
                                EquipmentSlot eqSlot = _eqSlotsUI._weaponSlots[j];
                                if (eqSlot._slotItype == AllEnums.ItemType.Weapon && eqSlot._slotPosition.x >= targetIndex + 3)
                                {
                                    _eqSlotsUI.ClearEquipmentSlot(eqSlot, AllEnums.ItemType.Weapon);
                                }
                            }
                            for (int i = 0; i < 3; i++)
                            {
                                rightStats[i]._stat.text = "000 |";
                            }
                            rightSlots.RemoveAt(targetIndex);
                            _player._invControl.CycleWeapons(false);
                        }
                    } else {
                        //slot has weapon and can be discarded easily
                        for (int i = 0; i < rightRTWeapons.Count; i++) {
                            if (_wRControl.CompareWorldItem(_curEqSlot._icon._ID, rightRTWeapons[i]._instance._itemName)) {
                                rightRTWeapons[i]._equippedHand = AllEnums.PreferredHand.Ambidextrous;
                                rightSlots.RemoveAt(targetIndex);
                                Debug.Log("Clearing Weapon from Right Slot");
                                break;
                            }
                        }

                        //clear UI
                        for (int j = 0; j < _eqSlotsUI._weaponSlots.Count; j++) {
                            EquipmentSlot eqSlot = _eqSlotsUI._weaponSlots[j];
                            if (eqSlot._slotItype == AllEnums.ItemType.Weapon && eqSlot._slotPosition.x >= targetIndex + 3) {
                                _eqSlotsUI.ClearEquipmentSlot(eqSlot, AllEnums.ItemType.Weapon);
                            }
                        }
                        for (int i = 0; i < 3; i++) {
                            rightStats[i]._stat.text = "000 |";
                        }
                    }
				}
			} else {
				//...left hand

				if (!string.IsNullOrEmpty (_curEqSlot._icon._ID)) {
					//there is something equipped in this slot
					if (_wRControl.CompareWorldItem(_curEqSlot._icon._ID, currentLeftWeapon._instance._itemName)){
						//slot is currently equipped weapon
						if (leftSlots.Count == 1) {
							//there is only one weapon equipped on this hand, therefore we need to Instantiate an Unarmed Weapon
							for (int i = 0; i < unequippedWeapons.Count; i++) {
								if (_wRControl.CompareWorldItem(defaultEmpty._itemName, unequippedWeapons[i]._instance._itemName)) {
									Shepherd.RuntimeWeapon weaponToEquip = (Shepherd.RuntimeWeapon)unequippedWeapons [i];
									weaponToEquip._equippedHand = AllEnums.PreferredHand.Left;

									//instantiate weapon to be equipped to currentLeftWeapon
									Shepherd.Weapon weaponToInstanitate = (Shepherd.Weapon)weaponToEquip._instance;
									GameObject gObject = Instantiate (weaponToInstanitate._modelPrefab) as GameObject;
									gObject.name = weaponToEquip._instance._itemName;
                                    weaponToEquip._itemName = weaponToEquip._instance._itemName;
                                    weaponToEquip._rtModel = gObject;
									weaponToEquip._weaponHook = gObject.GetComponentInChildren<WeaponHook> ();
									weaponToEquip._weaponHook.InitDamageColliders (_player);

									//Destroy currentLeftWeapon Model and label it as unequipped
									currentLeftWeapon._equippedHand = AllEnums.PreferredHand.Ambidextrous;
									Destroy (currentLeftWeapon._rtModel);
									currentLeftWeapon._rtModel = null;
									currentLeftWeapon._weaponHook = null;

									//Setup newly equipped weapon
									_player._invControl._currentLeftWeapon = weaponToEquip;
									currentLeftWeapon = weaponToEquip;
									leftSlots [targetIndex] = weaponToEquip._instance;
									_player._invControl.EquipWeapon (currentLeftWeapon, true);
									_eqSlotsUI.UpdateEquipmentSlotV2 (weaponToEquip._instance, AllEnums.ItemType.Weapon, _curEqSlot);
									Debug.Log ("Replacing Currently Equipped Left with Unarmed Default");
									break;
								}
							}
						} else {
							//there is another weapon equipped on this hand, therefore we will swap to that weapon
							for (int i = 0; i < leftRTWeapons.Count; i++) {
								if (_wRControl.CompareWorldItem(_curEqSlot._icon._ID, leftRTWeapons[i]._instance._itemName)) {
									//Destroy currentLeftWeapon Model and label it as unequipped
									currentLeftWeapon._equippedHand = AllEnums.PreferredHand.Ambidextrous;
									Destroy (currentLeftWeapon._rtModel);
									currentLeftWeapon._rtModel = null;
									currentLeftWeapon._weaponHook = null;

									//redetermine what weapons are equipped in hand
									leftRTWeapons = _wRControl._runtimeReferences.FindRuntimeWeapons (AllEnums.PreferredHand.Left);

									Shepherd.RuntimeWeapon weaponToEquip = (Shepherd.RuntimeWeapon)leftRTWeapons [0];
									//instantiate weapon to be equipped to currentLeftWeapon
									Shepherd.Weapon weaponToInstanitate = (Shepherd.Weapon)weaponToEquip._instance;
									GameObject gObject = Instantiate (weaponToInstanitate._modelPrefab) as GameObject;
									gObject.name = weaponToEquip._instance._itemName;
                                    weaponToEquip._itemName = weaponToEquip._instance._itemName;
                                    weaponToEquip._rtModel = gObject;
									weaponToEquip._weaponHook = gObject.GetComponentInChildren<WeaponHook> ();
									weaponToEquip._weaponHook.InitDamageColliders (_player);

									//Setup newly equipped weapon
									_player._invControl._currentLeftWeapon = weaponToEquip;
									currentLeftWeapon = weaponToEquip;

									_player._invControl.EquipWeapon (currentLeftWeapon, true);
									Debug.Log ("Unequipping Current Left Weapon, shifting to another Weapon in slots");
									break;
								}
							}
							//clear UI
							for (int j = 0; j < _eqSlotsUI._weaponSlots.Count; j++) {
								EquipmentSlot eqSlot = _eqSlotsUI._weaponSlots [j];
								if (eqSlot._slotItype == AllEnums.ItemType.Weapon && eqSlot._slotPosition.x >= targetIndex) {
									_eqSlotsUI.ClearEquipmentSlot (eqSlot, AllEnums.ItemType.Weapon);
								}
							}
							for (int i = 0; i < 3; i++) {
								leftStats [i]._stat.text = "000 |";
							}
							leftSlots.RemoveAt (targetIndex);
							_player._invControl.CycleWeapons (true);
						}
					} else {
						//slot has weapon and can be discarded easily
						for (int i = 0; i < leftRTWeapons.Count; i++) {
							if (_wRControl.CompareWorldItem(_curEqSlot._icon._ID, leftRTWeapons[i]._instance._itemName)) {
								leftRTWeapons [i]._equippedHand = AllEnums.PreferredHand.Ambidextrous;
								leftSlots.RemoveAt (targetIndex);
								Debug.Log ("Clearing Weapon from Left Slot");
								break;
							}
						}

						//clear UI
						for (int j = 0; j < _eqSlotsUI._weaponSlots.Count; j++) {
							EquipmentSlot eqSlot = _eqSlotsUI._weaponSlots [j];
							if (eqSlot._slotItype == AllEnums.ItemType.Weapon && eqSlot._slotPosition.x >= targetIndex) {
								_eqSlotsUI.ClearEquipmentSlot (eqSlot, AllEnums.ItemType.Weapon);
							}
						}
						for (int i = 0; i < 3; i++) {
							leftStats [i]._stat.text = "000 |";
						}
					}
				}
			}
			_player._actionControl.UpdateActionsWithCurrentItems ();
			_player.UpdateGameUI ();
			LoadEquipment ();
		}

		private void SwapWeapon() {
			int targetIndex = _curEqSlot._itemPosition;
			//swap weapon in...
			bool rightHand = (_curEqSlot._itemPosition > 2) ? true : false;

			//TODO:Clean this up, so that we don't have to duplicate this code for the left hand and only use this code once

			//variables needed
			List<WorldItem> rightSlots = _player._invControl._runtimeRefs._rightHand._value; //_player._invControl._currentEquipment._rightHand._value;
			List<RuntimeWeapon> rightRTWeapons = _wRControl._runtimeReferences.FindRuntimeWeapons (AllEnums.PreferredHand.Right);
			Shepherd.RuntimeWeapon currentRightWeapon = _player._invControl._currentRightWeapon;

			List<WorldItem> leftSlots = _player._invControl._runtimeRefs._leftHand._value; //_player._invControl._currentEquipment._leftHand._value;
			List<RuntimeWeapon> leftRTWeapons = _wRControl._runtimeReferences.FindRuntimeWeapons (AllEnums.PreferredHand.Left);
			Shepherd.RuntimeWeapon currentLeftWeapon = _player._invControl._currentLeftWeapon;

			List<RuntimeWorldItem> unequippedWeapons = _wRControl._runtimeReferences.FindRTWeapons (AllEnums.PreferredHand.Ambidextrous);

			if (rightHand) {
				//...right hand
				targetIndex -= 3; //minus offset

				if (string.IsNullOrEmpty (_curEqSlot._icon._ID)) {
					//slot is empty
					for (int i = 0; i < unequippedWeapons.Count; i++) {
						//find storage slot item
						if (_wRControl.CompareWorldItem (unequippedWeapons [i]._instance._itemName, _curStorageSlot._ID)) {
							Shepherd.RuntimeWeapon weaponToEquip = (Shepherd.RuntimeWeapon)unequippedWeapons [i];
							weaponToEquip._equippedHand = AllEnums.PreferredHand.Right;
                            weaponToEquip._itemName = weaponToEquip._instance._itemName;
                            rightSlots.Add (weaponToEquip._instance);
							//_eqSlotsUI.UpdateEquipmentSlotV2 (weaponToEquip._instance, AllEnums.ItemType.Weapon, _curEqSlot);
							Debug.Log ("Added Item to Right Slots");
							break;
						}
					}
				} else {
					//slot is currently occupied
					if (_wRControl.CompareWorldItem (_curEqSlot._icon._ID, currentRightWeapon._instance._itemName)) {
						//slot is currently equipped weapon -- need to replace instanitation

						//TODO: SIMILAR TO RuntimeReferences UpdateCurrentRuntimeWeapon
						for (int i = 0; i < unequippedWeapons.Count; i++) {
							//find storage slot item
							if (_wRControl.CompareWorldItem (unequippedWeapons [i]._instance._itemName, _curStorageSlot._ID)) {
								Shepherd.RuntimeWeapon weaponToEquip = (Shepherd.RuntimeWeapon)unequippedWeapons [i];
								weaponToEquip._equippedHand = AllEnums.PreferredHand.Right;

								//instantiate weapon to be equipped to currentRightWeapon
								Shepherd.Weapon weaponToInstanitate = (Shepherd.Weapon)weaponToEquip._instance;
								GameObject gObject = Instantiate (weaponToInstanitate._modelPrefab) as GameObject;
								gObject.name = weaponToEquip._instance._itemName;
								weaponToEquip._rtModel = gObject;
                                weaponToEquip._itemName = weaponToEquip._instance._itemName;
                                weaponToEquip._weaponHook = gObject.GetComponentInChildren<WeaponHook> ();
								weaponToEquip._weaponHook.InitDamageColliders (_player);

								//Destroy currentRightWeapon Model and label it as unequipped
								currentRightWeapon._equippedHand = AllEnums.PreferredHand.Ambidextrous;
								Destroy (currentRightWeapon._rtModel);
								currentRightWeapon._rtModel = null;
								currentRightWeapon._weaponHook = null;

								//Setup newly equipped weapon
								_player._invControl._currentRightWeapon = weaponToEquip;
								currentRightWeapon = weaponToEquip;
								rightSlots [targetIndex] = weaponToEquip._instance;
								_player._invControl.EquipWeapon (currentRightWeapon, false);
								_eqSlotsUI.UpdateEquipmentSlotV2 (weaponToEquip._instance, AllEnums.ItemType.Weapon, _curEqSlot);
								Debug.Log ("Replacing Currently Equipped Right");
								break;
							}
						}
					} else {
						//slot has weapon in but can easily be replaced
						for (int i = 0; i < unequippedWeapons.Count; i++) {
							//find storage slot item
							if (_wRControl.CompareWorldItem (unequippedWeapons [i]._instance._itemName, _curStorageSlot._ID)) {
								Shepherd.RuntimeWeapon weaponToEquip = (Shepherd.RuntimeWeapon)unequippedWeapons [i];
								weaponToEquip._equippedHand = AllEnums.PreferredHand.Right;
								for (int j = 0; j < rightRTWeapons.Count; j++) {
									//find current slot item
									if (_wRControl.CompareWorldItem (rightRTWeapons [j]._instance._itemName, _curEqSlot._icon._ID)) {
										Debug.Log ("yup");
										rightRTWeapons [j]._equippedHand = AllEnums.PreferredHand.Ambidextrous;
										break;
									}
								}
								rightSlots [targetIndex] = weaponToEquip._instance;
								_eqSlotsUI.UpdateEquipmentSlotV2 (weaponToEquip._instance, AllEnums.ItemType.Weapon, _curEqSlot);
								Debug.Log ("Swapping Item in Right Slot");
								break;
							}
						}
					}
				}
			} else {
				//...left hand

				if (string.IsNullOrEmpty (_curEqSlot._icon._ID)) {
					//slot is empty
					for (int i = 0; i < unequippedWeapons.Count; i++) {
						//find storage slot item
						if (_wRControl.CompareWorldItem (unequippedWeapons [i]._instance._itemName, _curStorageSlot._ID)) {
							Shepherd.RuntimeWeapon weaponToEquip = (Shepherd.RuntimeWeapon)unequippedWeapons [i];
							weaponToEquip._equippedHand = AllEnums.PreferredHand.Left;
                            weaponToEquip._itemName = weaponToEquip._instance._itemName;
							leftSlots.Add (weaponToEquip._instance);
							//_eqSlotsUI.UpdateEquipmentSlotV2 (weaponToEquip._instance, AllEnums.ItemType.Weapon, _curEqSlot);
							Debug.Log ("Added Item to Left Slots");
							break;
						}
					}
				} else {
					//slot is currently occupied
					if (_wRControl.CompareWorldItem (_curEqSlot._icon._ID, currentLeftWeapon._instance._itemName)) {
						//slot is currently equipped weapon -- need to replace instanitation
						for (int i = 0; i < unequippedWeapons.Count; i++) {
							//find storage slot item
							if (_wRControl.CompareWorldItem (unequippedWeapons [i]._instance._itemName, _curStorageSlot._ID)) {
								Shepherd.RuntimeWeapon weaponToEquip = (Shepherd.RuntimeWeapon)unequippedWeapons [i];
								weaponToEquip._equippedHand = AllEnums.PreferredHand.Left;

								//instantiate weapon to be equipped to currentLeftWeapon
								Shepherd.Weapon weaponToInstanitate = (Shepherd.Weapon)weaponToEquip._instance;
								GameObject gObject = Instantiate (weaponToInstanitate._modelPrefab) as GameObject;
								gObject.name = weaponToEquip._instance._itemName;
                                weaponToEquip._itemName = weaponToEquip._instance._itemName;
                                weaponToEquip._rtModel = gObject;
								weaponToEquip._weaponHook = gObject.GetComponentInChildren<WeaponHook> ();
								weaponToEquip._weaponHook.InitDamageColliders (_player);

								//Destroy currentLeftWeapon Model and label it as unequipped
								currentLeftWeapon._equippedHand = AllEnums.PreferredHand.Ambidextrous;
								Destroy (currentLeftWeapon._rtModel);
								currentLeftWeapon._rtModel = null;
								currentLeftWeapon._weaponHook = null;

								//Setup newly equipped weapon
								_player._invControl._currentLeftWeapon = weaponToEquip;
								currentLeftWeapon = weaponToEquip;
								leftSlots [targetIndex] = weaponToEquip._instance;
								_player._invControl.EquipWeapon (currentLeftWeapon, true);
								_eqSlotsUI.UpdateEquipmentSlotV2 (weaponToEquip._instance, AllEnums.ItemType.Weapon, _curEqSlot);
								Debug.Log ("Replacing Currently Equipped Left");
								break;
							}
						}
					} else {
						//slot has weapon in but can easily be replaced
						for (int i = 0; i < unequippedWeapons.Count; i++) {
							//find storage slot item
							if (_wRControl.CompareWorldItem (unequippedWeapons [i]._instance._itemName, _curStorageSlot._ID)) {
								Shepherd.RuntimeWeapon weaponToEquip = (Shepherd.RuntimeWeapon)unequippedWeapons [i];
								weaponToEquip._equippedHand = AllEnums.PreferredHand.Left;
								for (int j = 0; j < leftRTWeapons.Count; j++) {
									//find current slot item
									if (_wRControl.CompareWorldItem (leftRTWeapons [j]._instance._itemName, _curEqSlot._icon._ID)) {
										leftRTWeapons [j]._equippedHand = AllEnums.PreferredHand.Ambidextrous;
										break;
									}
								}
								leftSlots [targetIndex] = weaponToEquip._instance;
								_eqSlotsUI.UpdateEquipmentSlotV2 (weaponToEquip._instance, AllEnums.ItemType.Weapon, _curEqSlot);
								Debug.Log ("Swapping Item in Left Slot");
								break;
							}
						}
					}
				}
			}
			LoadItemFromSlot (_curEqSlot._icon);
			LoadEquipment ();
			_player._actionControl.UpdateActionsWithCurrentItems ();
			_player.UpdateGameUI ();
		}

		private void ClearArmor() {
			List<WorldItem> armorSlots = _player._invControl._runtimeRefs._armor._value; //_player._invControl._currentEquipment._armor._value;
			List<Shepherd.RuntimeArmor> rtArmors = _player._invControl._currentArmorSet;

			CharacterModel playerModel = _player._invControl._testing;

			if (!string.IsNullOrEmpty (_curEqSlot._icon._ID)) {
				//slot is occupied, replace with new armor
				for (int i = 0; i < rtArmors.Count; i++) {
					Shepherd.Armor curArmor = (Shepherd.Armor)rtArmors [i]._instance;
					//find which armor we are about to equip
					if (_curEqSlot._slotEqType == curArmor._armorType && _wRControl.CompareWorldItem (_curEqSlot._icon._ID, rtArmors [i]._instance._itemName)) {
						rtArmors [i]._isEquipped = false;
						rtArmors.RemoveAt (i);
						for (int j = 0; j < armorSlots.Count; j++) {
							if (_wRControl.CompareWorldItem (_curEqSlot._icon._ID, armorSlots [j]._itemName)) {
								armorSlots.RemoveAt (j);
								break;
							}
						}
						_eqSlotsUI.ClearEquipmentSlot (_curEqSlot, AllEnums.ItemType.Armor);
						Debug.Log ("Unequipping Armor at slot");
						break;
					}
				}

				playerModel.LoadEquipmentFromCurrent (armorSlots);
				_player._invControl.UpdatePlayerStats (armorSlots);
			}
		}

		private void SwapArmor() {
			List<WorldItem> armorSlots = _player._invControl._runtimeRefs._armor._value; //_player._invControl._currentEquipment._armor._value;
			List<Shepherd.RuntimeArmor> rtArmors = _player._invControl._currentArmorSet;

			List<RuntimeWorldItem> unequippedArmors = _wRControl._runtimeReferences.FindRuntimeArmorsByType (false, _curEqSlot._slotEqType);
			CharacterModel playerModel = _player._invControl._testing;

			if (string.IsNullOrEmpty (_curEqSlot._icon._ID)) {
				//slot is empty, add in our armor
				for (int i = 0; i < unequippedArmors.Count; i++) {
					//find which armor we are about to equip
					if (_wRControl.CompareWorldItem (_curStorageSlot._ID, unequippedArmors [i]._instance._itemName)) {
						Shepherd.RuntimeArmor armorToEquip = (Shepherd.RuntimeArmor)unequippedArmors [i];
						armorToEquip._isEquipped = true;
                        armorToEquip._itemName = armorToEquip._instance._itemName;
						rtArmors.Add (armorToEquip);
						armorSlots.Add (armorToEquip._instance);
						//since there is only one slot, we don't have to shift things over like we did with weapons and consumables
						_eqSlotsUI.UpdateEquipmentSlotV2 (armorToEquip._instance, AllEnums.ItemType.Armor, _curEqSlot);
						Debug.Log ("Added Item to Armor Slots");
						break;
					}
				}
			} else {
				//slot is occupied, replace with new armor
				for (int i = 0; i < unequippedArmors.Count; i++) {
					//find which armor we are about to equip
					if (_wRControl.CompareWorldItem (_curStorageSlot._ID, unequippedArmors [i]._instance._itemName)) {
						Shepherd.RuntimeArmor armorToEquip = (Shepherd.RuntimeArmor)unequippedArmors [i];
						armorToEquip._isEquipped = true;
                        armorToEquip._itemName = armorToEquip._instance._itemName;
                        for (int j = 0; j < rtArmors.Count; j++) {
							Shepherd.Armor curArmor = (Shepherd.Armor)rtArmors [j]._instance;
							if (_curEqSlot._slotEqType == curArmor._armorType) {
								rtArmors [j]._isEquipped = false;
								armorSlots [j] = armorToEquip._instance;
								rtArmors [j] = armorToEquip;
								_eqSlotsUI.UpdateEquipmentSlotV2 (armorToEquip._instance, AllEnums.ItemType.Armor, _curEqSlot);
								Debug.Log ("Replacing Armor at slot");
								break;
							}
						}
						break;
					}
				}
			}
			LoadItemFromSlot (_curEqSlot._icon);
			playerModel.LoadEquipmentFromCurrent (armorSlots);
			_player._invControl.UpdatePlayerStats (armorSlots);
		}

		private void ClearConsumable() {
			int targetIndex = _curEqSlot._itemPosition;

			List<WorldItem> consumableSlots = _player._invControl._runtimeRefs._consumable._value; // _player._invControl._currentEquipment._consumable._value;
			List<RuntimeConsumable> rtConsumables = _wRControl._runtimeReferences.FindRuntimeConsumables (true);
			Shepherd.RuntimeConsumable currentConsumable = _player._invControl._currentConsumable;

			Shepherd.Consumable defaultEmpty = (Shepherd.Consumable)_player._invControl._emptyItemsDefault;

			List<RuntimeWorldItem> unequippedConsumables = _wRControl._runtimeReferences.FindRTConsumables (false);

			if (!string.IsNullOrEmpty (_curEqSlot._icon._ID)) {
				//there is something equipped in this slot
				if (_wRControl.CompareWorldItem (_curEqSlot._icon._ID, currentConsumable._instance._itemName)) {
					//slot is currently equipped consumable
					if (consumableSlots.Count == 1) {
						//there is only one consumable equipped on this hand, therefore we need to Instantiate an Unarmed Consumable
						for (int i = 0; i < unequippedConsumables.Count; i++) {
							if (_wRControl.CompareWorldItem (defaultEmpty._itemName, unequippedConsumables [i]._instance._itemName)) {
								Shepherd.RuntimeConsumable consumableToEquip = (Shepherd.RuntimeConsumable)unequippedConsumables [i];
								consumableToEquip._isEquipped = true;
                                consumableToEquip._itemName = consumableToEquip._instance._itemName;

                                //instantiate weapon to be equipped to currentConsumable
                                Shepherd.Consumable consumableToInstanitate = (Shepherd.Consumable)consumableToEquip._instance;
								GameObject gObject = Instantiate (consumableToInstanitate._modelPrefab) as GameObject;
								gObject.name = consumableToEquip._instance._itemName;
								consumableToEquip._rtModel = gObject;

								//Destroy currentConsumable Model and label it as unequipped
								currentConsumable._isEquipped = false;
								Destroy (currentConsumable._rtModel);
								currentConsumable._rtModel = null;

								//Setup newly equipped weapon
								_player._invControl._currentConsumable = consumableToEquip;
								currentConsumable = consumableToEquip;
								consumableSlots [targetIndex] = consumableToEquip._instance;
								_player._invControl.EquipConsumable (currentConsumable, _player._biography._mainHand);
								_eqSlotsUI.UpdateEquipmentSlotV2 (consumableToEquip._instance, AllEnums.ItemType.Consumable, _curEqSlot);
								Debug.Log ("Replacing Currently Equipped Item with Consumable Default");
								break;
							}
						}
					} else {
						//there is another weapon equipped on this hand, therefore we will swap to that weapon
						for (int i = 0; i < rtConsumables.Count; i++) {
							if (_wRControl.CompareWorldItem (_curEqSlot._icon._ID, rtConsumables [i]._instance._itemName)) {
								//find first slot's weapon to replace currentConsumable
								for (int j = 0; j < rtConsumables.Count; j++) {
									if (_wRControl.CompareWorldItem (consumableSlots [0]._itemName, rtConsumables [j]._instance._itemName)) {
										//Destroy currentConsumable Model and label it as unequipped
										currentConsumable._isEquipped = false;
										Destroy (currentConsumable._rtModel);
										currentConsumable._rtModel = null;

										//redetermine what now is equipped
										rtConsumables = _wRControl._runtimeReferences.FindRuntimeConsumables (true); 

										//prep which consumable is now going to be equipped
										RuntimeConsumable consumableToEquip = rtConsumables [j];

										//instantiate weapon to be equipped to currentConsumable
										Consumable consumableToInstanitate = (Consumable)consumableToEquip._instance;
										GameObject gObject = Instantiate (consumableToInstanitate._modelPrefab) as GameObject;
										gObject.name = consumableToEquip._instance._itemName;
										consumableToEquip._rtModel = gObject;
                                        consumableToEquip._itemName = consumableToEquip._instance._itemName;
										//Setup newly equipped weapon
										_player._invControl._currentConsumable = consumableToEquip;
										currentConsumable = consumableToEquip;

										_player._invControl.EquipConsumable (currentConsumable, _player._biography._mainHand);
										Debug.Log ("Clearing Item from Consumable Slot, reverting Currently Equipped to 1st Consumable Slot");
										break;
									}
								}
								break;
							}
						}
						//clear UI
						for (int j = 0; j < _eqSlotsUI._consumableSlots.Count; j++) {
							EquipmentSlot eqSlot = _eqSlotsUI._consumableSlots [j];
							if (eqSlot._slotItype == AllEnums.ItemType.Consumable && eqSlot._slotPosition.x >= targetIndex) {
								_eqSlotsUI.ClearEquipmentSlot (eqSlot, AllEnums.ItemType.Consumable);
							}
						}
						consumableSlots.RemoveAt (targetIndex);
					}
				} else {
					//slot has consumable and can be discarded easily
					for (int i = 0; i < rtConsumables.Count; i++) {
						if (_wRControl.CompareWorldItem (_curEqSlot._icon._ID, rtConsumables [i]._instance._itemName)) {
							rtConsumables [i]._isEquipped = false;
							consumableSlots.RemoveAt (targetIndex);
							Debug.Log ("Clearing Item from Consumable Slot");
							break;
						}
					}

					//clear UI
					for (int j = 0; j < _eqSlotsUI._consumableSlots.Count; j++) {
						EquipmentSlot eqSlot = _eqSlotsUI._consumableSlots [j];
						if (eqSlot._slotItype == AllEnums.ItemType.Consumable && eqSlot._slotPosition.x >= targetIndex) {
							_eqSlotsUI.ClearEquipmentSlot (eqSlot, AllEnums.ItemType.Consumable);
						}
					}
				}
			}
			LoadItemFromSlot (_curEqSlot._icon);
			LoadEquipment ();
		}

		private void SwapConsumable() {
			int targetIndex = _curEqSlot._itemPosition;
			List<WorldItem> consumableSlots = _player._invControl._runtimeRefs._consumable._value; //_player._invControl._currentEquipment._consumable._value;
			List<RuntimeConsumable> rtConsumables = _wRControl._runtimeReferences.FindRuntimeConsumables (true);
			Shepherd.RuntimeConsumable currentConsumable = _player._invControl._currentConsumable;

			List<RuntimeWorldItem> unequippedConsumables = _wRControl._runtimeReferences.FindRTConsumables (false);

			if (string.IsNullOrEmpty (_curEqSlot._icon._ID)) {
				//if we only have hands equipped in our slots, we must replace them first
				if (consumableSlots.Count == 1 && _wRControl.CompareWorldItem (consumableSlots [0]._itemName, _player._invControl._emptyItemsDefault._itemName)) {
					//TODO: SIMILAR TO RuntimeReferences UpdateCurrentRuntimeConsumable
					for (int i = 0; i < unequippedConsumables.Count; i++) {
						//find storage slot item
						if (_wRControl.CompareWorldItem(unequippedConsumables [i]._instance._itemName, _curStorageSlot._ID)) {
							Shepherd.RuntimeConsumable consumableToEquip = (Shepherd.RuntimeConsumable)unequippedConsumables [i];
							consumableToEquip._isEquipped = true;
                            consumableToEquip._itemName = consumableToEquip._instance._itemName;

                            //instantiate Consumable to be equipped to currentConsumable
                            Shepherd.Consumable consumableToInstanitate = (Shepherd.Consumable)consumableToEquip._instance;
							GameObject gObject = Instantiate (consumableToInstanitate._modelPrefab) as GameObject;
							gObject.name = consumableToEquip._instance._itemName;
							consumableToEquip._rtModel = gObject;

							//Destroy currentConsumable Model and label it as unequipped
							currentConsumable._isEquipped = false;
							Destroy (currentConsumable._rtModel);
							currentConsumable._rtModel = null;

							//Setup newly equipped consumable
							_player._invControl._currentConsumable = consumableToEquip;
							currentConsumable = consumableToEquip;
							consumableSlots [0] = consumableToEquip._instance;
							_player._invControl.EquipConsumable (currentConsumable, _player._biography._mainHand);
							_eqSlotsUI.UpdateEquipmentSlotV2 (consumableToEquip._instance, AllEnums.ItemType.Consumable, _curEqSlot);
							Debug.Log ("Replacing Currently Equipped Consumable");
							break;
						}
					}
				} else {
					//if slot is empty, therefore adding consumable is easy
					for (int i = 0; i < unequippedConsumables.Count; i++) {
						//find storage slot item
						if (_wRControl.CompareWorldItem(_curStorageSlot._ID, unequippedConsumables [i]._instance._itemName)) {
							Shepherd.RuntimeConsumable consumableToEquip = (Shepherd.RuntimeConsumable)unequippedConsumables [i];
							consumableToEquip._isEquipped = true;
                            consumableToEquip._itemName = consumableToEquip._instance._itemName;
                            consumableSlots.Add (consumableToEquip._instance);
							//_eqSlotsUI.UpdateEquipmentSlotV2 (consumableToEquip._instance, AllEnums.ItemType.Consumable, _curEqSlot);
							Debug.Log ("Adding Item to Consumable Slot");
							break;
						}
					}
				}
			} else {
				//slot is not empty
				if (_wRControl.CompareWorldItem(_curEqSlot._icon._ID, currentConsumable._instance._itemName)) {
					//slot is currently equipped consumable -- need to replace instantiation

					//TODO: SIMILAR TO RuntimeReferences UpdateCurrentRuntimeConsumable
					for (int i = 0; i < unequippedConsumables.Count; i++) {
						//find storage slot item
						if (_wRControl.CompareWorldItem(unequippedConsumables [i]._instance._itemName, _curStorageSlot._ID)) {
							Shepherd.RuntimeConsumable consumableToEquip = (Shepherd.RuntimeConsumable)unequippedConsumables [i];
							consumableToEquip._isEquipped = true;
                            consumableToEquip._itemName = consumableToEquip._instance._itemName;

                            //instantiate Consumable to be equipped to currentConsumable
                            Shepherd.Consumable consumableToInstanitate = (Shepherd.Consumable)consumableToEquip._instance;
							GameObject gObject = Instantiate (consumableToInstanitate._modelPrefab) as GameObject;
							gObject.name = consumableToEquip._instance._itemName;
							consumableToEquip._rtModel = gObject;

							//Destroy currentConsumable Model and label it as unequipped
							currentConsumable._isEquipped = false;
							Destroy (currentConsumable._rtModel);
							currentConsumable._rtModel = null;

							//Setup newly equipped consumable
							_player._invControl._currentConsumable = consumableToEquip;
							currentConsumable = consumableToEquip;
							consumableSlots [targetIndex] = consumableToEquip._instance;
							_player._invControl.EquipConsumable (currentConsumable, _player._biography._mainHand);
							_eqSlotsUI.UpdateEquipmentSlotV2 (consumableToEquip._instance, AllEnums.ItemType.Consumable, _curEqSlot);
							Debug.Log ("Replacing Currently Equipped Consumable");
							break;
						}
					}

				} else {
					//slot is has consumable but can be easily replaced
					for (int i = 0; i < unequippedConsumables.Count; i++) {
						if (_wRControl.CompareWorldItem(_curStorageSlot._ID, unequippedConsumables [i]._instance._itemName)) {
							Shepherd.RuntimeConsumable consumableToEquip = (Shepherd.RuntimeConsumable)unequippedConsumables [i];
							consumableToEquip._isEquipped = true;
                            consumableToEquip._itemName = consumableToEquip._instance._itemName;
                            for (int j = 0; j < rtConsumables.Count; j++) {
								Shepherd.Consumable curConsumable = (Shepherd.Consumable)rtConsumables [j]._instance;
								if (_wRControl.CompareWorldItem(curConsumable._itemName, _curEqSlot._icon._ID)) {
									Debug.Log ("yup");
									rtConsumables [j]._isEquipped = false;
									break;
								}
							}
							consumableSlots [targetIndex] = consumableToEquip._instance;
							_eqSlotsUI.UpdateEquipmentSlotV2 (consumableToEquip._instance, AllEnums.ItemType.Consumable, _curEqSlot);
							Debug.Log ("Swapping Item in Consumable Slot");
							break;
						}
					}
				}
			}
			LoadItemFromSlot (_curEqSlot._icon);
			LoadEquipment ();
			_player.UpdateGameUI ();
		}
	}

	public struct WeaponStatsInArrayV2 {
		public float _luck;
		public float _soundVolume;

		public float _fear;
		public float _weight;
		public float _maxDurability;
		public float _currentDurability;

		public float[] _requirements;
		public float[] _attackDefence;
		public float[] _resistanceChance;
		public float[] _bodyEffects;
		public float[] _healthEffects;

		public float[] _leftArm;
		public float[] _rightArm;

		public void CreateArrays(WorldItemStats fromStats) {
			_requirements = new float[AllEnums.AttributeNumber];
			_requirements [0] = fromStats._strength;
			_requirements [1] = fromStats._endurance;
			_requirements [2] = fromStats._dexterity;
			_requirements [3] = fromStats._intelligence;
			_requirements [4] = fromStats._vitality;
			_requirements [5] = fromStats._perception;
			_requirements [6] = fromStats._courage;
			_requirements [7] = fromStats._luck;

			_attackDefence = new float[AllEnums.AttackDefenceNumber];
			_attackDefence [0] = fromStats._physical;
			_attackDefence [1] = fromStats._finesse;
			_attackDefence [2] = fromStats._magical;
			_attackDefence [3] = fromStats._fire;
			_attackDefence [4] = fromStats._wind;
			_attackDefence [5] = fromStats._light;
			_attackDefence [6] = fromStats._water;
			_attackDefence [7] = fromStats._earth;
			_attackDefence [8] = fromStats._dark;

			_resistanceChance = new float[AllEnums.ResisistancesNumber];
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

		public void CreateHealthEffects(WorldItem fromItem) {
			//this function should only be called when passing in a consumable

			_bodyEffects = new float[AllEnums.BodyNeedsNumber];
			_bodyEffects [0] = fromItem._itemStats._thirst;
			_bodyEffects [1] = fromItem._itemStats._waste;
			_bodyEffects [2] = fromItem._itemStats._hunger;
			_bodyEffects [3] = fromItem._itemStats._sleep;
			_bodyEffects [4] = fromItem._itemStats._pleasure;

			_healthEffects = new float[AllEnums.StatusTypeNumber - 1];
			_healthEffects [0] = fromItem._itemStats._health;
			_healthEffects [1] = fromItem._itemStats._stamina;
			_healthEffects [2] = fromItem._itemStats._baseCourage;
			_healthEffects [3] = fromItem._itemStats._immuneSystem;
		}

		public void CreatePlayerArrays(CharacterBody fromBody, WorldItemStats fromBodyStats) {
			_requirements = new float[AllEnums.AttributeNumber];
			_requirements [0] = fromBodyStats._strength;
			_requirements [1] = fromBodyStats._endurance;
			_requirements [2] = fromBodyStats._dexterity;
			_requirements [3] = fromBodyStats._intelligence;
			_requirements [4] = fromBodyStats._vitality;
			_requirements [5] = fromBodyStats._perception;
			_requirements [6] = fromBodyStats._courage;
			_requirements [7] = fromBodyStats._luck;

			_attackDefence = new float[AllEnums.AttackDefenceNumber];
			_attackDefence [0] = fromBodyStats._physical;
			_attackDefence [1] = fromBodyStats._finesse;
			_attackDefence [2] = fromBodyStats._magical;
			_attackDefence [3] = fromBodyStats._fire;
			_attackDefence [4] = fromBodyStats._wind;
			_attackDefence [5] = fromBodyStats._light;
			_attackDefence [6] = fromBodyStats._water;
			_attackDefence [7] = fromBodyStats._earth;
			_attackDefence [8] = fromBodyStats._dark;

			_resistanceChance = new float[AllEnums.ResisistancesNumber];
			_resistanceChance [0] = fromBodyStats._bleed;
			_resistanceChance [1] = fromBodyStats._poison;
			_resistanceChance [2] = fromBodyStats._heat;
			_resistanceChance [3] = fromBodyStats._disease;
			_resistanceChance [4] = fromBodyStats._dizzy;
			_resistanceChance [5] = fromBodyStats._freeze;

			/*_leftArm = new float[3];
			_leftArm [0] = fromBodyStats._lPower1;
			_leftArm [0] = fromBodyStats._lPower2;
			_leftArm [0] = fromBodyStats._lPower3;

			_rightArm = new float[3];
			_rightArm [0] = fromBodyStats._rPower1;
			_rightArm [0] = fromBodyStats._rPower2;
			_rightArm [0] = fromBodyStats._rPower3;*/
		}
	}
}