using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[CreateAssetMenu(menuName = "Shepherd/Single Instances/Runtime References")]
	public class RuntimeReferences : ScriptableObject {
        [Header("Debug")]
        public bool isFromFile;

        [Header("Slots")]
		public CurrentItem _rightHand;
		public CurrentItem _leftHand;
		public CurrentItem _spell;
		public CurrentItem _consumable;
		public CurrentItem _armor;

		[Header("Events")]
		public GameEvent _updateGameUI;

		[Header("Runtimes")]
		public List<RuntimeWeapon> _runtimeWeapons = new List<RuntimeWeapon>();
		/*
		public List<RuntimeSpells> _runtimeSpells = new List<RuntimeSpells>();
		*/
		public List<RuntimeConsumable> _runtimeConsumables = new List<RuntimeConsumable>();
		//test
		public List<RuntimeArmor> _runtimeArmors = new List<RuntimeArmor>();

		//test
		public AllEnums.FlockAlert _flockAlertSystem;
		public List<CharacterStateController> _flock = new List<CharacterStateController>();
        //...

        //TEST
        public int _prevGold;
        public int _currentGold;
        public int _prevFlockCount;
        public string _locationBefore;
        public string _forecastBefore;

        [SerializeField]
        public Dictionary<Biography, BioCard> _runtimeNetwork = new Dictionary<Biography, BioCard>();

		public void Init() {
			_runtimeWeapons.Clear ();

			//test
			_runtimeConsumables.Clear();
			_runtimeArmors.Clear ();

			//test
			if (_rightHand != null) {
				_rightHand._parent = this;
			}
			if (_leftHand != null) {
				_leftHand._parent = this;
			}
			if (_spell != null) {
				_spell._parent = this;
			}
			if (_consumable != null) {
				_consumable._parent = this;
			}
			if (_armor != null) {
				_armor._parent = this;
			}

			//test
			_flockAlertSystem = AllEnums.FlockAlert.None;
			_flock.Clear();

            //test
            _runtimeNetwork.Clear();
		}

		public void RegisterRuntimeWeapon(Shepherd.RuntimeWeapon rtWeapon) {
			_runtimeWeapons.Add(rtWeapon);
		}

		public void UnregisterRuntimeWeapon(Shepherd.RuntimeWeapon rtWeapon) {
			if(_runtimeWeapons.Contains(rtWeapon)) {
				if(rtWeapon._rtModel) { //_modelInstance
					Destroy(rtWeapon._rtModel); //_weaponModel;
				}
				_runtimeWeapons.Remove(rtWeapon);
			}
		}

		public void RegisterRuntimeConsumable(Shepherd.RuntimeConsumable rtConsumable) {
			_runtimeConsumables.Add(rtConsumable);
		}

		public void UnregisterRuntimeConsumable(Shepherd.RuntimeConsumable rtConsumable) {
			if(_runtimeConsumables.Contains(rtConsumable)) {
				Debug.Log ("inside");
                if (rtConsumable._rtModel)
                { //_modelInstance
                    Debug.Log("destroyed runtime: " + rtConsumable._instance._itemName);
                    Destroy(rtConsumable._rtModel); //_weaponModel;
                }
                _runtimeConsumables.Remove(rtConsumable);
			}
		}

		public void RegisterRuntimeArmor(Shepherd.RuntimeArmor rtArmor) {
			_runtimeArmors.Add(rtArmor);
		}

		public void UnregisterRuntimeArmor(Shepherd.RuntimeArmor rtArmor) {
			if(_runtimeArmors.Contains(rtArmor)) {
				if(rtArmor._rtModel) {
					Destroy(rtArmor._rtModel); //_weaponModel;
				}
				_runtimeArmors.Remove(rtArmor);
			}
		}

		public List<RuntimeConsumable> FindRuntimeConsumables(bool isEquipped) {
			List<Shepherd.RuntimeConsumable> returnValue = new List<Shepherd.RuntimeConsumable> ();
			for (int i = 0; i < _runtimeConsumables.Count; i++) {
				if (_runtimeConsumables [i]._isEquipped == isEquipped) {
					returnValue.Add (_runtimeConsumables [i]);
				}
			}

			return returnValue;
		}
		public List<RuntimeWorldItem> FindRTConsumables(bool isEquipped) {
			List<Shepherd.RuntimeWorldItem> returnValue = new List<Shepherd.RuntimeWorldItem> ();
			for (int i = 0; i < _runtimeConsumables.Count; i++) {
				if (_runtimeConsumables [i]._isEquipped == isEquipped) {
					returnValue.Add (_runtimeConsumables [i]);
				}
			}

			return returnValue;
		}

		public List<RuntimeArmor> FindRuntimeArmors(bool isEquipped) {
			List<Shepherd.RuntimeArmor> returnValue = new List<Shepherd.RuntimeArmor> ();
			for (int i = 0; i < _runtimeArmors.Count; i++) {
				if (_runtimeArmors [i]._isEquipped == isEquipped) {
					returnValue.Add (_runtimeArmors [i]);
				}
			}

			return returnValue;
		}
		public List<RuntimeWorldItem> FindRuntimeArmorsByType(bool isEquipped, AllEnums.ArmorType aType) {
			List<Shepherd.RuntimeWorldItem> returnValue = new List<Shepherd.RuntimeWorldItem> ();
			for (int i = 0; i < _runtimeArmors.Count; i++) {
				if (_runtimeArmors [i]._isEquipped == isEquipped) {
					if (((Shepherd.Armor)_runtimeArmors [i]._instance)._armorType == aType) {
						returnValue.Add (_runtimeArmors [i]);
					}
				}
			}

			return returnValue;
		}

		//TODO: Make only one function
		public List<RuntimeWeapon> FindRuntimeWeapons(AllEnums.PreferredHand onHand) {
			/* LEGEND
			 * All weapons are part of the player's current inventory
			 * Right			= Weapons Equipped in Right Hand
			 * Left				= Weapons Equipped in Left Hand
			 * Ambidextrous 	= Weapons in neither hand
			*/
			List<Shepherd.RuntimeWeapon> returnValue = new List<Shepherd.RuntimeWeapon> ();
			for (int i = 0; i < _runtimeWeapons.Count; i++) {
				if (_runtimeWeapons [i]._equippedHand == onHand) {
					returnValue.Add (_runtimeWeapons [i]);
				}
			}

			return returnValue;
		}

        public List<RuntimeWorldItem> FindRTWeapons(AllEnums.PreferredHand onHand) {
			/* LEGEND
			 * All weapons are part of the player's current inventory
			 * Right			= Weapons Equipped in Right Hand
			 * Left				= Weapons Equipped in Left Hand
			 * Ambidextrous 	= Weapons in neither hand
			*/
			List<Shepherd.RuntimeWorldItem> returnValue = new List<Shepherd.RuntimeWorldItem> ();
			for (int i = 0; i < _runtimeWeapons.Count; i++) {
				if (_runtimeWeapons [i]._equippedHand == onHand) {
					returnValue.Add (_runtimeWeapons [i]);
				}
			}

			return returnValue;
		} 

		public void UpdateCurrentRuntimeConsumable(int index, CharacterStateController toPlayer, ref Shepherd.RuntimeConsumable curRTConsumable) {

            int refIndex = _runtimeConsumables.IndexOf (curRTConsumable);
			if (refIndex == -1) {
				//does not contain current RuntimeConsumable
				Debug.Log("runtime consumables does not contain asked for runtimeconsumable");
				return;
			}

			List<RuntimeConsumable> equippedConsumables = FindRuntimeConsumables (true);

			if (index > equippedConsumables.Count) {
				Debug.Log ("Invalid Index -- this should never run!");
				return;
			}

			Destroy (curRTConsumable._rtModel);
			curRTConsumable._rtModel = null;

			if (curRTConsumable._durability == 0 && !curRTConsumable._unbreakable) {
				_runtimeConsumables.Remove (curRTConsumable);
			}

			//set new currentRuntimeConsumable from RuntimeReferences (retrieved) from index we double checked
			curRTConsumable = equippedConsumables [index];

			//setup new model to instantiate into scene from new currentRuntimeReferences (retrieved from _consumableItem list found in statecontrol)
			//add new model to new currentRuntimeConsumable
			Shepherd.Consumable instance = (Shepherd.Consumable)curRTConsumable._instance;
			GameObject gObject = GameObject.Instantiate (instance._modelPrefab) as GameObject;
			gObject.SetActive (false);
			gObject.name = curRTConsumable._instance.name;

			curRTConsumable._rtModel = gObject;
			curRTConsumable._unbreakable = equippedConsumables [index]._unbreakable;
			curRTConsumable._durability = equippedConsumables [index]._durability; //set new currentRuntimeConsumable stats from runtimeReferences
		}

		public void UpdateCurrentRuntimeWeapon(int index, CharacterStateController toPlayer, ref Shepherd.RuntimeWeapon curRTWeapon) {
			//check if old currentRuntimeWeapon exists in our runtimeReferences for player
			int refIndex = _runtimeWeapons.IndexOf (curRTWeapon);
			if (refIndex == -1) {
				//does not contain current RuntimeWeapon
				Debug.Log ("Invalid Current RuntimeWeapon");
				return;
			}

			//find which RuntimeWeapons are currently equipped by player (specified by hand) and add them to a list
			AllEnums.PreferredHand onHand = curRTWeapon._equippedHand;
			List<Shepherd.RuntimeWeapon> equippedWeapons = FindRuntimeWeapons (onHand);

			//check if the index we passed in matches the list we've retrieved of currently equipped runtimeWeapons
			if (index > equippedWeapons.Count) {
				Debug.Log ("Invalid Index -- this should never run!");
				return;
			}

			//Destroy old model from scene
			Destroy (curRTWeapon._rtModel);
			curRTWeapon._rtModel = null;
			curRTWeapon._weaponHook = null;

			//find runtime by name
			string name = "";
			if (onHand == AllEnums.PreferredHand.Right) {
				name = toPlayer._invControl._runtimeRefs._rightHand._value [index]._itemName; //toPlayer._invControl._currentEquipment._rightHand._value [index]._itemInfo._itemName;
			} else if (onHand == AllEnums.PreferredHand.Left) {
				name = toPlayer._invControl._runtimeRefs._leftHand._value [index]._itemName; //toPlayer._invControl._currentEquipment._leftHand._value [index]._itemInfo._itemName;
			}

			//equippedWeapons isn't sorted, so we have to check manually if the name from index matches up with what we want
			for (int i = 0; i < equippedWeapons.Count; i++) {
				if(  toPlayer._wRControl.CompareWorldItem(name, equippedWeapons[i]._instance._itemName) )  {
					//set new currentRuntimeWeapon from RuntimeReferences (retrieved) from index we double checked
					curRTWeapon = equippedWeapons[i];
					break;
				}
			}

			//setup new model to instantiate into scene
			//(RuntimeWeapons have a string _name used to call model from WorldResourceController)
			Shepherd.Weapon instance = (Shepherd.Weapon)curRTWeapon._instance;
			GameObject gObject = GameObject.Instantiate (instance._modelPrefab) as GameObject;
			gObject.SetActive (false);
			gObject.name = curRTWeapon._instance.name;

			//add new model to new currentRuntimeWeapon from stats pulled from RuntimeReferences
			curRTWeapon._rtModel = gObject;
            curRTWeapon._itemName = curRTWeapon._instance._itemName;
			curRTWeapon._durability = equippedWeapons [index]._durability; //set new currentRuntimeWeapon stats from runtimeReferences
			curRTWeapon._weaponHook = curRTWeapon._rtModel.GetComponentInChildren<WeaponHook> ();
			curRTWeapon._weaponHook.InitDamageColliders (toPlayer);
			curRTWeapon._equippedHand = onHand;
		}
	}
}