using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[CreateAssetMenu(menuName = "Shepherd/Single Instances/Resource Controller")]
	public class WorldResourceController : ScriptableObject {

		[Header("Debug")]
		public Inventory _worldInventory;
		public Interactions _worldInteractions;
        public Population _worldPopulation;
        public LevelNames _levelNames;

		public BioCard _bioCardTemplate;
		//public Dialogue _worldDialogue;
		public GameInputController _gInput;

		//test
		public WorldNames _worldNames;
		//...

		public RuntimeReferences _runtimeReferences;

		public EquipmentData _playerInventory; //runtime refs?

		public void Init() {
			_playerInventory = Resources.Load ("PlayerEquipmentData") as EquipmentData;
			_worldInventory = Resources.Load ("WorldInventory") as Inventory;
			_runtimeReferences = Resources.Load ("PlayerRuntimeReferences") as RuntimeReferences;

			//test
			_worldInteractions = Resources.Load("WorldInteractions") as Interactions;

			_worldNames = Resources.Load ("WorldNames") as WorldNames;

            _worldPopulation = Resources.Load("WorldPopulation") as Population;
            _levelNames = Resources.Load("LevelNames") as LevelNames;

			//test2
			_gInput = Resources.Load("GameInputController") as GameInputController;
            _gInput.Init ();

			_worldInventory.Init ();
			_runtimeReferences.Init ();

			//test
			_worldInteractions.Init ();
            _worldPopulation.Init();
            _levelNames.Init();

			_worldNames.Init ();
		}

		//TEMP
		public void CopyInventoryFromEquipData(EquipmentData eqData) {
			for (int i = 0; i < eqData._data.Count; i++) {
				AddItemToInventory (eqData._data [i]);
			}
		}

		public void InitPlayerInventory() {
            _playerInventory._data.Clear ();
            _runtimeReferences._rightHand._value.Clear();
            _runtimeReferences._leftHand._value.Clear();
            _runtimeReferences._consumable._value.Clear();
            _runtimeReferences._armor._value.Clear();
            _runtimeReferences._spell._value.Clear();

            //_runtimeReferences._rightHand._value.Add(GetItem("Unarmed"));
            //_runtimeReferences._leftHand._value.Add(GetItem("Unarmed"));
            //_runtimeReferences._consumable._value.Add(GetItem("Hands"));
            _runtimeReferences._armor._value.Add(GetItem("[L] Old Boot"));
            _runtimeReferences._armor._value.Add(GetItem("[R] Old Boot"));
            _runtimeReferences._armor._value.Add(GetItem("[M] Old Shirt"));
            _runtimeReferences._armor._value.Add(GetItem("[M] Old Pants"));
        }

        public void InitPlayerFromFile(GameSaveSlotData data)
        {
            Debug.Log("Loading Player From file...");
            _playerInventory._data.Clear();
            _runtimeReferences.Init();
            _runtimeReferences._rightHand._value.Clear();
            _runtimeReferences._leftHand._value.Clear();
            _runtimeReferences._consumable._value.Clear();
            _runtimeReferences._armor._value.Clear();
            _runtimeReferences._spell._value.Clear();
            _runtimeReferences.isFromFile = true;
            //CURRENT EQUIPMENT
            for (int i = 0; i < data._currentRightHand.Count; i++)
            {
                
                _runtimeReferences._rightHand.Add(GetItem(data._currentRightHand[i]));
            }
            if(_runtimeReferences._rightHand._value.Count == 0){
                _runtimeReferences._rightHand._value.Add(GetItem("Unarmed"));
            }
            for (int i = 0; i < data._currentLeftHand.Count; i++)
            {
                _runtimeReferences._leftHand.Add(GetItem(data._currentLeftHand[i]));
                
            }
            if (_runtimeReferences._leftHand._value.Count == 0)
            {
                _runtimeReferences._leftHand._value.Add(GetItem("Unarmed"));
            }
            for (int i = 0; i < data._currentConsumables.Count; i++)
            {
                _runtimeReferences._consumable.Add(GetItem(data._currentConsumables[i]));
               
            }

            for (int i = 0; i < data._currentArmor.Count; i++)
            {
                _runtimeReferences._armor.Add(GetItem(data._currentArmor[i]));
               
            }
            //RUNTIME EQUIPMENT
            for(int i = 0; i < data._allWeapons.Count; i++)
            {             
                _playerInventory._data.Add(GetItem(data._allWeapons[i]._itemName));
                _runtimeReferences._runtimeWeapons.Add(data._allWeapons[i]);
            }
            for (int i = 0; i < data._allConsumables.Count; i++)
            {                
                _playerInventory._data.Add(GetItem(data._allConsumables[i]._itemName));
                _runtimeReferences._runtimeConsumables.Add(data._allConsumables[i]);
            }
            for (int i = 0; i < data._allArmor.Count; i++)
            {
                _playerInventory._data.Add(GetItem(data._allArmor[i]._itemName));
                _runtimeReferences._runtimeArmors.Add(data._allArmor[i]);
            }

            for(int i = 0; i <_playerInventory._data.Count; i++)
            {
                Debug.Log($"THIS IS TO SEE: {_playerInventory._data[i]._itemStats}");
            }

        }

		public void AddItemToInventory(string id) {
			WorldItem desiredItem = GetItem (id);
			AddItemToInventory (desiredItem);
		}
		public void AddItemToInventory(WorldItem wItem) {
			if (wItem == null) {
				return;
			}
			WorldItem newItem = Instantiate (wItem);
			newItem.name = wItem._itemName;
			//Debug.Log (newItem.name);
			_playerInventory._data.Add (newItem);
		}
		//...

		public WorldItem GetItem(string id) {
			return _worldInventory.GetItem (id);
		}

		public WorldInteraction GetInteraction(string id) {
			WorldInteraction test = _worldInteractions.GetInteraction (id);
			return test;
			//return _worldInteractions.GetInteraction (id);
		}

		public List<WorldItem> GetAllItemsOfType(AllEnums.ItemType type) {
			List<WorldItem> returnValue = new List<WorldItem> ();
			for (int i = 0; i < _worldInventory._allItems.Count; i++) {
				if (_worldInventory._allItems [i]._itemType == type) {
					returnValue.Add (_worldInventory._allItems [i]);
				}
			}
			return returnValue;
		}
		public List<WorldItem> GetAllItemsOfTypeFromList(AllEnums.ItemType type, List<WorldItem> fromList) {
			List<WorldItem> returnValue = new List<WorldItem> ();
			for (int i = 0; i < fromList.Count; i++) {
				if (fromList [i]._itemType == type) {
					returnValue.Add (fromList [i]);
				}
			}
			return returnValue;
		}
		public List<WorldItem> GetAllArmorsOfTypeFromList(AllEnums.ArmorType type, List<WorldItem> fromList) {
			List<WorldItem> returnValue = new List<WorldItem> ();
			for (int i = 0; i < fromList.Count; i++) {
				if (fromList [i] is Shepherd.Armor) {
					if(((Shepherd.Armor)fromList[i])._armorType == type) {
						returnValue.Add (fromList [i]);
					}
				}
			}
			return returnValue;
		}

		public Weapon GetWeapon(string id) {
			WorldItem item = GetItem (id);
			return (Weapon)item;
		}

		public Armor GetArmor(string id) {
			WorldItem item = GetItem (id);
			return (Armor)item;
		}

		public Consumable GetConsumable(string id) {
			WorldItem item = GetItem (id);
			return (Consumable)item;
		}

		public void DestroyRTWeapon(RuntimeWeapon thisRTWeapon) {
			Debug.Log (thisRTWeapon._instance.name + " Is Destroyed!");
			_runtimeReferences.UnregisterRuntimeWeapon (thisRTWeapon);
			Destroy(thisRTWeapon._rtModel);
		}

		public void DestroyRTConsumable(RuntimeConsumable thisRTConsumable) {
            Debug.Log(thisRTConsumable._instance.name + " Is Destroyed!");
            _runtimeReferences.UnregisterRuntimeConsumable (thisRTConsumable);
			Destroy(thisRTConsumable._rtModel);
		}

        public bool CompareWorldItem(string WorldItem1, string WorldItem2)
        {
            return _worldInventory.GetItem(WorldItem1) == _worldInventory.GetItem(WorldItem2);
        }
	}
}