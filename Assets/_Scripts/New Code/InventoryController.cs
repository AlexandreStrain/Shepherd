using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[System.Serializable]
	public class InventoryController {
		public RuntimeWeapon _currentRightWeapon;
		public RuntimeWeapon _currentLeftWeapon;
		//test
		public RuntimeConsumable _currentConsumable;
		public List<RuntimeArmor> _currentArmorSet = new List<RuntimeArmor>();
        //...

        //debug
        List<RuntimeConsumable> unequippedItems;

        // Test
        public EquipmentData _equippedItems;

		public WorldItem _emptyHandsDefault; //for weapons
		public WorldItem _emptyItemsDefault; //for consumables

		public CharacterModel _testing; //temp workaround

		//test
		public RuntimeReferences _runtimeRefs;
		//...

		//testing
		//subject to change
		public GameObject _parryCollider;
		public GameObject _spellBreathCollider;
		public GameObject _blockCollider;
		//...

		//test
		private WorldResourceController _wResourceControl;
		private CharacterStateController _stateCtrl;

		public void PreInit(bool isPlayer) {
			if (_testing != null) {
				_testing._isPlayer = isPlayer;
			}
		}

		public void Init(WorldResourceController rControl, CharacterStateController sControl) {
			_wResourceControl = rControl;
			_stateCtrl = sControl;

			//temp?
			InitColliders(sControl);

			//test
			if (_testing != null) {
				_testing._parent = sControl;
				_stateCtrl._bodySkin = _testing._bodyModel._body;

				if (_testing._isPlayer) {
					_runtimeRefs = _wResourceControl._runtimeReferences;
				} else if(!_testing._isPlayer && _runtimeRefs == null){
					_runtimeRefs = ScriptableObject.CreateInstance<RuntimeReferences> ();
					_runtimeRefs.name = _stateCtrl._biography._name + " Runtime References";
					_runtimeRefs.Init ();
					_stateCtrl._invControl._testing.LoadEquipmentFromData ();
				} else if (!_testing._isPlayer && _runtimeRefs != null)
                {
                    _runtimeRefs.Init();
                    _stateCtrl._invControl._testing.LoadEquipmentFromData();
                }
			}
			//...
		}

		public void InitColliders(CharacterStateController cControl) {
			ParryCollider parryCollider = _parryCollider.GetComponent<ParryCollider> ();
			parryCollider.InitOwner (cControl);
			BlockCollider blockCollider = _blockCollider.GetComponent<BlockCollider> ();
			blockCollider.InitOwner (cControl);
			SpellBreathCollider spellBreathCollider = _spellBreathCollider.GetComponent<SpellBreathCollider> ();
			spellBreathCollider.InitOwner (cControl);

			CloseParryCollider ();
			CloseBlockCollider ();
			CloseSpellBreathCollider ();
		}

		public void DestroyColliders() {
			GameObject.Destroy (_parryCollider);
			GameObject.Destroy (_blockCollider);
			GameObject.Destroy (_spellBreathCollider);
		}

		public void UpdateRuntimeInventory(List<WorldItem> newItems) {
			for (int i = 0; i < newItems.Count; i++) {
				WorldItem newItem;
				switch (newItems [i]._itemType) {
				case AllEnums.ItemType.Weapon:
					newItem = GameObject.Instantiate (newItems [i]);
					Shepherd.Weapon weapon = (Shepherd.Weapon)newItem;

					Shepherd.RuntimeWeapon newRuntimeWeapon = new Shepherd.RuntimeWeapon ();
					newRuntimeWeapon._instance = weapon;
					newRuntimeWeapon._durability = weapon._itemStats._maxDurability;
					newRuntimeWeapon._equippedHand = AllEnums.PreferredHand.Ambidextrous;

					if (_testing._isPlayer) {
						_wResourceControl._runtimeReferences.RegisterRuntimeWeapon (newRuntimeWeapon);
					}
					_equippedItems._data.Add (weapon);
					break;
				case AllEnums.ItemType.Consumable:
					newItem = GameObject.Instantiate (newItems [i]);
					Shepherd.Consumable consumable = (Shepherd.Consumable)newItem;

					Shepherd.RuntimeConsumable newConsumable = new Shepherd.RuntimeConsumable ();
					newConsumable._instance = consumable;
					newConsumable._unbreakable = consumable._isUnbreakable;
					newConsumable._durability = consumable._charges;
					newConsumable._isEquipped = false;

					if (_testing._isPlayer) {
						_wResourceControl._runtimeReferences.RegisterRuntimeConsumable (newConsumable);
					}
					_equippedItems._data.Add (consumable);
					break;
				case AllEnums.ItemType.Armor:
					newItem = GameObject.Instantiate (newItems [i]);
					Shepherd.Armor armor = (Shepherd.Armor)newItem;

					Shepherd.RuntimeArmor newArmor = new Shepherd.RuntimeArmor ();
					newArmor._instance = armor;
					newArmor._unbreakable = armor._isUnbreakable;
					newArmor._durability = armor._itemStats._maxDurability;
					newArmor._isEquipped = false;

					if (_testing._isPlayer) {
						_wResourceControl._runtimeReferences.RegisterRuntimeArmor (newArmor);
					}
					_equippedItems._data.Add (armor);
					break;
				}
			}
		}

		public void UpdateRuntimeInventoryWithRuntimeItem(RuntimeWorldItem newItem) {
			switch (newItem._instance._itemType) {
			case AllEnums.ItemType.Weapon:
				break;
			case AllEnums.ItemType.Armor:
				break;
			case AllEnums.ItemType.Consumable:
				Debug.Log ("initializing exchanged item: " + _stateCtrl._biography._name);
                if (_wResourceControl.CompareWorldItem(_currentConsumable._instance._itemName, _emptyItemsDefault._itemName) && newItem is RuntimeConsumable) {
					GameObject.Destroy (_currentConsumable._rtModel); //removing hands

					RuntimeConsumable newRTConsumable = (RuntimeConsumable)newItem;
					newRTConsumable._isEquipped = true;
					Shepherd.Consumable consumable = (Shepherd.Consumable)newItem._instance;
					GameObject gObject = GameObject.Instantiate (consumable._modelPrefab) as GameObject;
					gObject.SetActive (false);
					gObject.name = consumable.name;
					newRTConsumable._rtModel = gObject;
                    newRTConsumable._itemName = newItem._instance._itemName;

					_runtimeRefs.RegisterRuntimeConsumable (newRTConsumable);
					_currentConsumable = newRTConsumable;
					//_equippedItems._data.Add (newItem._instance);
					EquipConsumable (_currentConsumable, _stateCtrl._biography._mainHand);
					_currentConsumable._rtModel.SetActive (false); //temp hide
				} else if (_wResourceControl.CompareWorldItem(_currentConsumable._instance._itemName, newItem._instance._itemName) && newItem is RuntimeConsumable) {
					GameObject.Destroy (newItem._rtModel);
					_currentConsumable._durability++;
				} else {
					Debug.Log ("adding new item to runtime refs");
					RuntimeConsumable newRTConsumable = (RuntimeConsumable)newItem;
					newRTConsumable._isEquipped = true;
					newRTConsumable._instance = (Shepherd.Consumable)newItem._instance;
                        newRTConsumable._itemName = newItem._instance._itemName;
                        _runtimeRefs.RegisterRuntimeConsumable (newRTConsumable);
				}
				break;
			case AllEnums.ItemType.Spell:
				break;
			}
		}

		//rename, as this is for humans that are not the player
		public void SetupInventoryV3() {
			List<WorldItem> armors = new List<WorldItem> ();
			bool firstRightWeapon = true;
			bool firstLeftWeapon = true;

			GameObject gObject;

			for (int i = 0; i < _equippedItems._data.Count; i++) {
				//Debug.Log ("ITEM " + i + " : " + _equippedItems._data [i]);
				switch (_equippedItems._data [i]._itemType) {
				case AllEnums.ItemType.Weapon:
					Shepherd.Weapon weapon = (Shepherd.Weapon)_equippedItems._data [i];
					gObject = GameObject.Instantiate (weapon._modelPrefab) as GameObject;
					gObject.SetActive (false);
					gObject.name = weapon.name;

					Shepherd.RuntimeWeapon newRuntimeWeapon = new Shepherd.RuntimeWeapon ();
					newRuntimeWeapon._instance = weapon;
                        newRuntimeWeapon._itemName = newRuntimeWeapon._instance._itemName;
					newRuntimeWeapon._rtModel = gObject;
					newRuntimeWeapon._weaponHook = gObject.GetComponentInChildren<WeaponHook> ();
					newRuntimeWeapon._weaponHook.InitDamageColliders (_stateCtrl);
					newRuntimeWeapon._durability = weapon._itemStats._maxDurability;

					if (firstLeftWeapon && firstRightWeapon) {
						int hand = Random.Range (0, 100);
						if (hand > 49) {
							newRuntimeWeapon._equippedHand = AllEnums.PreferredHand.Left;
							firstLeftWeapon = false;
							_currentLeftWeapon = newRuntimeWeapon;
						} else {
							newRuntimeWeapon._equippedHand = AllEnums.PreferredHand.Right;
							firstRightWeapon = false;
							_currentRightWeapon = newRuntimeWeapon;
						}
					} else {
						if (firstLeftWeapon) {
							newRuntimeWeapon._equippedHand = AllEnums.PreferredHand.Left;
							firstLeftWeapon = false;
							_currentLeftWeapon = newRuntimeWeapon;
						}
						if (firstRightWeapon) {
							newRuntimeWeapon._equippedHand = AllEnums.PreferredHand.Right;
							firstRightWeapon = false;
							_currentRightWeapon = newRuntimeWeapon;
						}
					}
					_runtimeRefs.RegisterRuntimeWeapon (newRuntimeWeapon);
					//TODO: If the enemy has more than one weapon equipped in any of their hands, their rtModel's should not be instantiated until they equip them
					break;
				case AllEnums.ItemType.Consumable:
					Shepherd.Consumable consumable = (Shepherd.Consumable)_equippedItems._data [i];
					gObject = GameObject.Instantiate (consumable._modelPrefab) as GameObject;
					gObject.SetActive (false);
					gObject.name = consumable.name;

					Shepherd.RuntimeConsumable newConsumable = new Shepherd.RuntimeConsumable ();
					newConsumable._instance = consumable;
                        newConsumable._itemName = newConsumable._instance._itemName;
					newConsumable._rtModel = gObject;
					newConsumable._unbreakable = consumable._isUnbreakable;
					newConsumable._durability = consumable._charges;
					newConsumable._isEquipped = false;

					_runtimeRefs.RegisterRuntimeConsumable (newConsumable);
					GameObject.Destroy (newConsumable._rtModel);
					break;
				case AllEnums.ItemType.Armor:
					Shepherd.Armor armor = (Shepherd.Armor)_equippedItems._data [i];
					//gObject = GameObject.Instantiate (consumable._modelPrefab) as GameObject;
					//gObject.SetActive (false);
					//gObject.name = consumable.name;


					Shepherd.RuntimeArmor newArmor = new Shepherd.RuntimeArmor ();
					newArmor._instance = armor;
					//newArmor._rtModel = gObject;
					newArmor._unbreakable = armor._isUnbreakable;
					newArmor._durability = armor._itemStats._maxDurability;
					newArmor._isEquipped = true;
                        newArmor._itemName = armor._itemName;
					_runtimeRefs.RegisterRuntimeArmor (newArmor);
					_currentArmorSet.Add (newArmor);
					armors.Add (armor);
					break;
				}
			}
				
			Shepherd.Weapon unarmedRight = (Shepherd.Weapon)_emptyHandsDefault;
			gObject = GameObject.Instantiate (unarmedRight._modelPrefab) as GameObject;
			gObject.SetActive (false);
			gObject.name = unarmedRight.name;

			Shepherd.RuntimeWeapon newRuntimeUnarmedRight = new Shepherd.RuntimeWeapon ();
			newRuntimeUnarmedRight._instance = _emptyHandsDefault;
            newRuntimeUnarmedRight._itemName = _emptyHandsDefault._itemName;
			newRuntimeUnarmedRight._rtModel = gObject;
			newRuntimeUnarmedRight._weaponHook = gObject.GetComponentInChildren<WeaponHook> ();
			newRuntimeUnarmedRight._weaponHook.InitDamageColliders (_stateCtrl);
			newRuntimeUnarmedRight._durability = unarmedRight._itemStats._maxDurability;
			newRuntimeUnarmedRight._equippedHand = AllEnums.PreferredHand.Right;
			newRuntimeUnarmedRight._unbreakable = true;
			_runtimeRefs.RegisterRuntimeWeapon (newRuntimeUnarmedRight);

			Shepherd.Weapon unarmedLeft = (Shepherd.Weapon)_emptyHandsDefault;
			gObject = GameObject.Instantiate (unarmedRight._modelPrefab) as GameObject;
			gObject.SetActive (false);
			gObject.name = unarmedLeft.name;

			Shepherd.RuntimeWeapon newRuntimeUnarmedLeft = new Shepherd.RuntimeWeapon ();
			newRuntimeUnarmedLeft._instance = _emptyHandsDefault;
            newRuntimeUnarmedLeft._itemName = _emptyHandsDefault._itemName;
			newRuntimeUnarmedLeft._rtModel = gObject;
			newRuntimeUnarmedLeft._weaponHook = gObject.GetComponentInChildren<WeaponHook> ();
			newRuntimeUnarmedLeft._weaponHook.InitDamageColliders (_stateCtrl);
			newRuntimeUnarmedLeft._durability = unarmedRight._itemStats._maxDurability;
			newRuntimeUnarmedLeft._equippedHand = AllEnums.PreferredHand.Left;
			newRuntimeUnarmedLeft._unbreakable = true;
			_runtimeRefs.RegisterRuntimeWeapon (newRuntimeUnarmedLeft);

			Shepherd.Consumable emptyHands = (Shepherd.Consumable)_emptyItemsDefault;
			gObject = GameObject.Instantiate (emptyHands._modelPrefab) as GameObject;
			gObject.SetActive (false);
			gObject.name = emptyHands.name;


			Shepherd.RuntimeConsumable newRuntimeHands = new Shepherd.RuntimeConsumable ();
			newRuntimeHands._instance = _emptyItemsDefault;
            newRuntimeHands._itemName = _emptyItemsDefault._itemName;
			newRuntimeHands._rtModel = gObject;
			newRuntimeHands._durability = emptyHands._itemStats._maxDurability;
			newRuntimeHands._isEquipped = true;
			newRuntimeHands._unbreakable = true;
			_runtimeRefs.RegisterRuntimeConsumable (newRuntimeHands);


			if (_currentRightWeapon._instance == null) {
				_currentRightWeapon = newRuntimeUnarmedRight;
				if (_stateCtrl._animator.isHuman) {
					_currentRightWeapon._rtModel.transform.parent = _stateCtrl._animator.GetBoneTransform (HumanBodyBones.RightHand);
				}
			} else {
				if (_stateCtrl._animator.isHuman) {
					newRuntimeUnarmedRight._rtModel.transform.SetParent(_stateCtrl._animator.GetBoneTransform (HumanBodyBones.RightHand));
				} else {
					newRuntimeUnarmedRight._rtModel.transform.SetParent(_stateCtrl._myTransform);
				}
			}

			if (_currentLeftWeapon._instance == null) {
				_currentLeftWeapon = newRuntimeUnarmedLeft;
				if (_stateCtrl._animator.isHuman) {
					_currentLeftWeapon._rtModel.transform.parent = _stateCtrl._animator.GetBoneTransform (HumanBodyBones.LeftHand);
				}
			} else {
				if (_stateCtrl._animator.isHuman) {
					newRuntimeUnarmedLeft._rtModel.transform.SetParent(_stateCtrl._animator.GetBoneTransform (HumanBodyBones.LeftHand));
				} else {
					newRuntimeUnarmedLeft._rtModel.transform.SetParent(_stateCtrl._myTransform);
				}
			}

			EquipWeapon (_currentRightWeapon, false);
			EquipWeapon (_currentLeftWeapon, true);

			//if (_runtimeRefs.FindRTConsumables (false).Count == 0) {
				_currentConsumable = newRuntimeHands;
			/*} else {
				_currentConsumable = (Shepherd.RuntimeConsumable)_runtimeRefs.FindRTConsumables (false) [0];
				_currentConsumable._equipped = true;

				if (_stateCtrl._animator.isHuman) {
					if (_stateCtrl._biography._mainHand == AllEnums.PreferredHand.Left) {
						newRuntimeHands._rtModel.transform.SetParent (_stateCtrl._animator.GetBoneTransform(HumanBodyBones.LeftHand));
					} else {
						newRuntimeHands._rtModel.transform.SetParent (_stateCtrl._animator.GetBoneTransform(HumanBodyBones.RightHand));
					}
				} else {
					newRuntimeHands._rtModel.transform.SetParent (_stateCtrl._myTransform);
					newRuntimeHands._rtModel.transform.localPosition = Vector3.zero;
				}
			}*/
			EquipConsumable (_currentConsumable, _stateCtrl._biography._mainHand);

			_currentArmorSet = _runtimeRefs.FindRuntimeArmors (true);
			if (_testing != null) {
				_testing.LoadEquipmentFromCurrent (_equippedItems._data);
			}

			UpdatePlayerStats (armors);
			UpdatePlayerCarryWeight (_equippedItems._data);
			_stateCtrl._actionControl.UpdateActionsWithCurrentItems ();
		}

		public void SetupInventoryV2() {
            if (!_equippedItems._data.Contains(_emptyHandsDefault))
            {
                _equippedItems._data.Add(_emptyHandsDefault); //one for left hand
                _equippedItems._data.Add(_emptyHandsDefault); //one for right hand
            }
            if (!_equippedItems._data.Contains(_emptyItemsDefault))
            {
                _equippedItems._data.Add(_emptyItemsDefault);
            }

            if (!_runtimeRefs.isFromFile)
            {
                WorldItemsToRuntime(_equippedItems._data);
            }
            else
            {
                GameObject gObject;
                List<RuntimeWeapon> rtWeaponsLeft = _wResourceControl._runtimeReferences.FindRuntimeWeapons(AllEnums.PreferredHand.Left);

                for (int j = 0; j < rtWeaponsLeft.Count; j++)
                {
                    if (_runtimeRefs._leftHand._value.Contains(rtWeaponsLeft[j]._instance))
                    {
                        rtWeaponsLeft[j]._isEquipped = true;
                        rtWeaponsLeft[j]._equippedHand = AllEnums.PreferredHand.Left;
                    }
                    else
                    {
                        rtWeaponsLeft[j]._isEquipped = false;
                        rtWeaponsLeft[j]._equippedHand = AllEnums.PreferredHand.Ambidextrous;
                    }
                }
                if (_wResourceControl._runtimeReferences.FindRuntimeWeapons(AllEnums.PreferredHand.Left).Count > 0)
                {
                    _currentLeftWeapon = _wResourceControl._runtimeReferences.FindRuntimeWeapons(AllEnums.PreferredHand.Left)[0];
                    _currentLeftWeapon._instance = _wResourceControl._runtimeReferences.FindRuntimeWeapons(AllEnums.PreferredHand.Left)[0]._instance;
                    gObject = GameObject.Instantiate(((Weapon)_currentLeftWeapon._instance)._modelPrefab) as GameObject;
                    gObject.SetActive(false);
                    gObject.name = _currentLeftWeapon._itemName;
                    _currentLeftWeapon._rtModel = gObject;
                    _currentLeftWeapon._weaponHook = _currentLeftWeapon._rtModel.GetComponentInChildren<WeaponHook>();
                    _currentLeftWeapon._weaponHook.InitDamageColliders(_stateCtrl);
                    _currentLeftWeapon._equippedHand = AllEnums.PreferredHand.Left;
                    _currentLeftWeapon._isEquipped = true;

                    // EquipWeapon(_currentRightWeapon, true);
                    //CycleWeapons(true);
                }

                List<RuntimeWeapon> rtWeaponsRight = _wResourceControl._runtimeReferences.FindRuntimeWeapons(AllEnums.PreferredHand.Right);

                for (int j = 0; j < rtWeaponsRight.Count; j++)
                {
                    if (_runtimeRefs._rightHand._value.Contains(rtWeaponsRight[j]._instance))
                    {
                        rtWeaponsRight[j]._isEquipped = true;
                        rtWeaponsRight[j]._equippedHand = AllEnums.PreferredHand.Right;
                    }
                    else
                    {
                        rtWeaponsRight[j]._isEquipped = false;
                        rtWeaponsRight[j]._equippedHand = AllEnums.PreferredHand.Ambidextrous;
                    }
                }

                if (_wResourceControl._runtimeReferences.FindRuntimeWeapons(AllEnums.PreferredHand.Right).Count > 0)
                {
                    _currentRightWeapon = _wResourceControl._runtimeReferences.FindRuntimeWeapons(AllEnums.PreferredHand.Right)[0];

                    _currentRightWeapon._instance = _wResourceControl._runtimeReferences.FindRuntimeWeapons(AllEnums.PreferredHand.Right)[0]._instance;
                    gObject = GameObject.Instantiate(((Weapon)_currentRightWeapon._instance)._modelPrefab) as GameObject;
                    gObject.SetActive(false);
                    gObject.name = _currentRightWeapon._itemName;
                    _currentRightWeapon._rtModel = gObject;
                    _currentRightWeapon._weaponHook = _currentRightWeapon._rtModel.GetComponentInChildren<WeaponHook>();
                    _currentRightWeapon._weaponHook.InitDamageColliders(_stateCtrl);
                    _currentRightWeapon._equippedHand = AllEnums.PreferredHand.Right;
                    _currentRightWeapon._isEquipped = true;

                    //EquipWeapon(_currentRightWeapon, false);
                    //CycleWeapons(false);
                }

                if (_wResourceControl._runtimeReferences.FindRuntimeConsumables(true).Count > 0)
                {
                    _currentConsumable = _wResourceControl._runtimeReferences.FindRuntimeConsumables(true)[0];
                    _currentConsumable._instance = _wResourceControl._runtimeReferences.FindRTConsumables(true)[0]._instance;
                    gObject = GameObject.Instantiate(((Consumable)_currentConsumable._instance)._modelPrefab) as GameObject;
                    gObject.SetActive(false);
                    gObject.name = _currentConsumable._itemName;
                    _currentConsumable._rtModel = gObject;
                    _currentConsumable._equippedHand = _stateCtrl._biography._mainHand;

                    EquipConsumable(_currentConsumable, _stateCtrl._biography._mainHand);
                }


                _currentArmorSet = _wResourceControl._runtimeReferences.FindRuntimeArmors(true);
            }


            if (_runtimeRefs._rightHand._value.Contains(null)) { //temp fix to having nothing in hands on startup (since items created are at runtime)
				_runtimeRefs._rightHand._value.Clear();
				_runtimeRefs._rightHand._value.Add(_emptyHandsDefault);
			}

            //right hand
            if (_runtimeRefs._rightHand._value.Count != 0)
            {
                if (_currentRightWeapon == null)
                {
                    _runtimeRefs._rightHand._index.value = 0;
                    for (int i = 0; i < _runtimeRefs._rightHand._value.Count; i++)
                    {
                        Weapon currentWeapon = (Weapon)_runtimeRefs._rightHand._value[i];
                        List<RuntimeWeapon> rtWeapons = _wResourceControl._runtimeReferences.FindRuntimeWeapons(AllEnums.PreferredHand.Ambidextrous);
                        for (int j = 0; j < rtWeapons.Count; j++)
                        {
                            if (_wResourceControl.CompareWorldItem(rtWeapons[j]._instance._itemName, currentWeapon._itemName))
                            {
                                GameObject gObject = GameObject.Instantiate(currentWeapon._modelPrefab) as GameObject;
                                gObject.SetActive(false);
                                gObject.name = currentWeapon.name;
                                rtWeapons[j]._itemName = currentWeapon._itemName;
                                rtWeapons[j]._rtModel = gObject;
                                rtWeapons[j]._weaponHook = rtWeapons[j]._rtModel.GetComponentInChildren<WeaponHook>();
                                rtWeapons[j]._weaponHook.InitDamageColliders(_stateCtrl);
                                
                                if (i == 0)
                                {
                                    _currentRightWeapon = rtWeapons[j];
                                    _currentRightWeapon._isEquipped = true;
                                    rtWeapons[j]._equippedHand = AllEnums.PreferredHand.Right;
                                }
                                else
                                {
                                    rtWeapons[j]._equippedHand = AllEnums.PreferredHand.Ambidextrous;
                                    GameObject.Destroy(rtWeapons[j]._rtModel);
                                }
                                break;
                            }
                        }
                    }
                }
            }
            else
            {

                Weapon currentWeapon = (Weapon)_emptyHandsDefault;
                List<RuntimeWeapon> rtWeapons = _wResourceControl._runtimeReferences.FindRuntimeWeapons(AllEnums.PreferredHand.Ambidextrous);
                for (int j = 0; j < rtWeapons.Count; j++)
                {
                    if (_wResourceControl.CompareWorldItem(rtWeapons[j]._instance._itemName, currentWeapon._itemName))
                    {
                        GameObject gObject = GameObject.Instantiate(currentWeapon._modelPrefab) as GameObject;
                        gObject.SetActive(false);
                        gObject.name = currentWeapon.name;
                        rtWeapons[j]._itemName = currentWeapon._itemName;
                        rtWeapons[j]._rtModel = gObject;
                        rtWeapons[j]._weaponHook = rtWeapons[j]._rtModel.GetComponentInChildren<WeaponHook>();
                        rtWeapons[j]._weaponHook.InitDamageColliders(_stateCtrl);
                        rtWeapons[j]._equippedHand = AllEnums.PreferredHand.Right;

                        _currentRightWeapon = rtWeapons[j];
                        _currentRightWeapon._isEquipped = true;
                        _currentRightWeapon._itemName = rtWeapons[j]._itemName;
                        break;
                    }
                }
            }
			EquipWeapon (_currentRightWeapon, false);


			if(_runtimeRefs._leftHand._value.Contains(null)) { //temp fix to having nothing in hands on startup (since items created are at runtime)
				_runtimeRefs._leftHand._value.Clear();
                _runtimeRefs._leftHand._value.Add(_emptyHandsDefault);
			}

            //left hand
            if (_runtimeRefs._leftHand._value.Count != 0)
            {
                if (_currentLeftWeapon == null)
                {
                    _runtimeRefs._leftHand._index.value = 0;
                    for (int i = 0; i < _runtimeRefs._leftHand._value.Count; i++)
                    {
                        Weapon currentWeapon = (Weapon)_runtimeRefs._leftHand._value[i];
                        List<RuntimeWeapon> rtWeapons = _wResourceControl._runtimeReferences.FindRuntimeWeapons(AllEnums.PreferredHand.Ambidextrous);
                        for (int j = 0; j < rtWeapons.Count; j++)
                        {
                            if (_wResourceControl.CompareWorldItem(rtWeapons[j]._instance.name, currentWeapon.name))
                            {
                                GameObject gObject = GameObject.Instantiate(currentWeapon._modelPrefab) as GameObject;
                                gObject.SetActive(false);
                                gObject.name = currentWeapon.name;
                                rtWeapons[j]._itemName = currentWeapon._itemName;
                                rtWeapons[j]._rtModel = gObject;
                                rtWeapons[j]._weaponHook = rtWeapons[j]._rtModel.GetComponentInChildren<WeaponHook>();
                                rtWeapons[j]._weaponHook.InitDamageColliders(_stateCtrl);
                                

                                if (i == 0)
                                {
                                    _currentLeftWeapon = rtWeapons[j];
                                    _currentLeftWeapon._isEquipped = true;
                                    rtWeapons[j]._equippedHand = AllEnums.PreferredHand.Left;
                                }
                                else
                                {
                                    rtWeapons[j]._equippedHand = AllEnums.PreferredHand.Ambidextrous;
                                    GameObject.Destroy(rtWeapons[j]._rtModel);
                                }
                                break;
                            }
                        }
                    }
                }
            }
            else
            {

                Shepherd.Weapon currentWeapon = (Shepherd.Weapon)_emptyHandsDefault;
                List<Shepherd.RuntimeWeapon> rtWeapons = _wResourceControl._runtimeReferences.FindRuntimeWeapons(AllEnums.PreferredHand.Ambidextrous);
                for (int j = 0; j < rtWeapons.Count; j++)
                {
                    if (_wResourceControl.CompareWorldItem(rtWeapons[j]._instance.name, currentWeapon.name))
                    {
                        GameObject gObject = GameObject.Instantiate(currentWeapon._modelPrefab) as GameObject;
                        gObject.SetActive(false);
                        gObject.name = currentWeapon.name;
                        rtWeapons[j]._itemName = currentWeapon._itemName;
                        rtWeapons[j]._rtModel = gObject;
                        rtWeapons[j]._weaponHook = rtWeapons[j]._rtModel.GetComponentInChildren<WeaponHook>();
                        rtWeapons[j]._weaponHook.InitDamageColliders(_stateCtrl);
                        rtWeapons[j]._equippedHand = AllEnums.PreferredHand.Left;

                        _currentLeftWeapon = rtWeapons[j];
                        _currentLeftWeapon._isEquipped = true;
                        _currentLeftWeapon._itemName = rtWeapons[j]._itemName;

                        break;
                    }
                }

            }
			EquipWeapon (_currentLeftWeapon, true);

			//consumables
		
			if(_runtimeRefs._consumable._value.Contains(null) || _runtimeRefs._consumable._value.Count == 0) { //temp fix to having nothing in hands on startup (since items created are at runtime)
				_runtimeRefs._consumable._value.Clear();
				_runtimeRefs._consumable._value.Add(_emptyItemsDefault);
                Debug.Log("this gets run");
			}

			if (_runtimeRefs._consumable._value.Count != 0) {
				_runtimeRefs._consumable._index.value = 0;
				for (int i = 0; i < _runtimeRefs._consumable._value.Count; i++) {
					Shepherd.Consumable currentConsumable = (Shepherd.Consumable)_runtimeRefs._consumable._value [i];
					List<Shepherd.RuntimeConsumable> rtConsumables = _wResourceControl._runtimeReferences.FindRuntimeConsumables (false);
					for (int j = 0; j < rtConsumables.Count; j++) {
						if (_wResourceControl.CompareWorldItem( rtConsumables [j]._instance.name, currentConsumable.name )) {
							GameObject gObject = GameObject.Instantiate (currentConsumable._modelPrefab) as GameObject;
							gObject.SetActive (false);
							gObject.name = currentConsumable.name;
                            rtConsumables[j]._itemName = currentConsumable._itemName;
							rtConsumables [j]._rtModel = gObject;
							rtConsumables [j]._isEquipped = true;

							if (i == 0) {
								_currentConsumable = rtConsumables [j];
							} else {
								GameObject.Destroy (rtConsumables [j]._rtModel);
							}
							break;
						}
					}
				}
			}
			EquipConsumable (_currentConsumable, _stateCtrl._biography._mainHand);



			if(_runtimeRefs._armor._value.Contains(null)) { //temp fix to having nothing in hands on startup (since items created are at runtime)
				_runtimeRefs._armor._value.Clear();
				if (GameSessionController.Instance._equippedSet == 0) {
					//equips default male set
					//_runtimeRefs._armor._value = new List<WorldItem>(GameSessionController.Instance._startingSet1._data);
				} else {
					//equips default female set
					//_runtimeRefs._armor._value = new List<WorldItem>(GameSessionController.Instance._startingSet2._data);
				}
			}

			//armor
			if (_runtimeRefs._armor._value.Count != 0) {
				_runtimeRefs._armor._index.value = 0;
				for (int i = 0; i < _runtimeRefs._armor._value.Count; i++) {
					Shepherd.Armor currentArmorPart = (Shepherd.Armor)_runtimeRefs._armor._value [i];
					List<Shepherd.RuntimeArmor> rtArmors = _wResourceControl._runtimeReferences.FindRuntimeArmors (false);
					for (int j = 0; j < rtArmors.Count; j++) {

						if (_wResourceControl.CompareWorldItem(rtArmors [j]._instance.name, currentArmorPart.name)) {
							//GameObject gObject = GameObject.Instantiate (currentArmorPart._modelPrefab) as GameObject;
							//gObject.SetActive (false);
							//gObject.name = currentArmorPart.name;

							//rtArmors [j]._aModel = gObject;
							rtArmors [j]._isEquipped = true;
                            rtArmors[j]._itemName = currentArmorPart._itemName;
							_currentArmorSet.Add (rtArmors [j]);
							break;
						} else {
							rtArmors [j]._isEquipped = false;
						}
					}
				}
			}
			_testing.LoadEquipmentFromCurrent (_runtimeRefs._armor._value);
			UpdatePlayerStats (_runtimeRefs._armor._value);
			UpdatePlayerCarryWeight (_equippedItems._data);

			_stateCtrl._actionControl.UpdateActionsWithCurrentItems ();
			_stateCtrl.UpdateGameUI ();
		}


		public void SetupInventory() {
			if (_runtimeRefs._rightHand._value.Count != 0) {
				WeaponToRuntime (_runtimeRefs._rightHand._value, false, ref _currentRightWeapon);
				EquipWeapon (_currentRightWeapon, false);
			} else {
				if (_emptyHandsDefault != null) {
					WeaponToRuntime (_emptyHandsDefault, ref _currentRightWeapon);
					EquipWeapon (_currentRightWeapon, false);
				}
			}
				
			if (_runtimeRefs._leftHand._value.Count != 0) {
				WeaponToRuntime (_runtimeRefs._leftHand._value, true, ref _currentLeftWeapon);
				EquipWeapon (_currentLeftWeapon, true);
			} else {
				if (_emptyHandsDefault != null) {
					WeaponToRuntime (_emptyHandsDefault, ref _currentLeftWeapon);
					EquipWeapon (_currentLeftWeapon, true);
				}
			}

			InitAllDamageColliders ();
			CloseAllDamageColliders ();

			if (_runtimeRefs._consumable._value.Count != 0) {
				ConsumableToRuntime(_runtimeRefs._consumable._value, ref _currentConsumable);
				EquipConsumable (_currentConsumable, _stateCtrl._biography._mainHand);
			}
			//...

			_stateCtrl._actionControl.UpdateActionsWithCurrentItems ();
			_stateCtrl.UpdateGameUI ();
		}

		public void UpdatePlayerStats(List<WorldItem> armors) {
            //TODO: Clean this up somehow

            //reset stats to normal
            if (_stateCtrl._biography._currentBioCard == null)
            {
                return;
            }

			_stateCtrl._biography._currentBioCard._currentStatistics._physical = _stateCtrl._biography._baseStatistics._physical;
            _stateCtrl._biography._currentBioCard._currentStatistics._finesse = _stateCtrl._biography._baseStatistics._finesse;
			_stateCtrl._biography._currentBioCard._currentStatistics._magical = _stateCtrl._biography._baseStatistics._magical;
			_stateCtrl._biography._currentBioCard._currentStatistics._fire =  _stateCtrl._biography._baseStatistics._fire;
			_stateCtrl._biography._currentBioCard._currentStatistics._wind = _stateCtrl._biography._baseStatistics._wind;
			_stateCtrl._biography._currentBioCard._currentStatistics._light = _stateCtrl._biography._baseStatistics._light;
			_stateCtrl._biography._currentBioCard._currentStatistics._water = _stateCtrl._biography._baseStatistics._water;
            _stateCtrl._biography._currentBioCard._currentStatistics._earth = _stateCtrl._biography._baseStatistics._earth;
            _stateCtrl._biography._currentBioCard._currentStatistics._dark = _stateCtrl._biography._baseStatistics._dark;

			_stateCtrl._biography._currentBioCard._currentStatistics._bleed = _stateCtrl._biography._baseStatistics._bleed;
			_stateCtrl._biography._currentBioCard._currentStatistics._poison = _stateCtrl._biography._baseStatistics._poison;
            _stateCtrl._biography._currentBioCard._currentStatistics._heat = _stateCtrl._biography._baseStatistics._heat;
            _stateCtrl._biography._currentBioCard._currentStatistics._disease = _stateCtrl._biography._baseStatistics._disease;
            _stateCtrl._biography._currentBioCard._currentStatistics._dizzy = _stateCtrl._biography._baseStatistics._dizzy;
            _stateCtrl._biography._currentBioCard._currentStatistics._freeze = _stateCtrl._biography._baseStatistics._freeze;

			//get stats from armor
			for (int i = 0; i < armors.Count; i++) {
				Armor armor = (Armor)armors [i];
				_stateCtrl._biography._currentBioCard._currentStatistics._physical += armor._itemStats._physical;
				_stateCtrl._biography._currentBioCard._currentStatistics._finesse += armor._itemStats._finesse;
				_stateCtrl._biography._currentBioCard._currentStatistics._magical += armor._itemStats._magical;
				_stateCtrl._biography._currentBioCard._currentStatistics._fire += armor._itemStats._fire;
				_stateCtrl._biography._currentBioCard._currentStatistics._wind += armor._itemStats._wind;
				_stateCtrl._biography._currentBioCard._currentStatistics._light += armor._itemStats._light;
				_stateCtrl._biography._currentBioCard._currentStatistics._water += armor._itemStats._water;
				_stateCtrl._biography._currentBioCard._currentStatistics._earth += armor._itemStats._earth;
				_stateCtrl._biography._currentBioCard._currentStatistics._dark += armor._itemStats._dark;

				_stateCtrl._biography._currentBioCard._currentStatistics._bleed += armor._itemStats._bleed;
				_stateCtrl._biography._currentBioCard._currentStatistics._poison += armor._itemStats._poison;
				_stateCtrl._biography._currentBioCard._currentStatistics._heat += armor._itemStats._heat;
				_stateCtrl._biography._currentBioCard._currentStatistics._disease += armor._itemStats._disease;
				_stateCtrl._biography._currentBioCard._currentStatistics._dizzy += armor._itemStats._dizzy;
				_stateCtrl._biography._currentBioCard._currentStatistics._freeze += armor._itemStats._freeze;
			}
		}
		public void UpdatePlayerCarryWeight(List<WorldItem> items) {
			for (int i = 0; i < items.Count; i++) {
                _stateCtrl._body._carryWeight.Add(items [i]._itemStats._weight);
			}
		}
			
		public void EquipConsumable(Shepherd.RuntimeConsumable rtConsumable, AllEnums.PreferredHand mainHand) {
            if(rtConsumable == null)
            {
                return; // if null, don't equip
            }

			Vector3 position = Vector3.zero;
			Vector3 eulers = Vector3.zero;
			Vector3 scale = Vector3.one;
			Transform parent = null;

			Shepherd.Consumable instance = (Shepherd.Consumable)rtConsumable._instance;


            //TODO: Instead of equipping from main hand preferences, it should already be accounted for in our runtime consumable before calling this function

			switch (mainHand) {
			case AllEnums.PreferredHand.Left:
				position = instance._leftHandPosition._position;
				eulers = instance._leftHandPosition._eulers;

                    if (_stateCtrl._animator.isHuman)
                    {
                        parent = _stateCtrl._animator.GetBoneTransform(HumanBodyBones.LeftHand);
                    }
                    else
                    {
                        parent = _stateCtrl._myTransform;
                    }
                 //   parent = _stateCtrl._animator.GetBoneTransform (HumanBodyBones.LeftHand);
				break;
			case AllEnums.PreferredHand.Right:
				position = rtConsumable._rtModel.transform.position;
				eulers = rtConsumable._rtModel.transform.eulerAngles;
                    if (_stateCtrl._animator.isHuman)
                    {
                        parent = _stateCtrl._animator.GetBoneTransform(HumanBodyBones.RightHand);
                    }
                    else
                    {
                        parent = _stateCtrl._myTransform;
                    }
                    //parent = _stateCtrl._animator.GetBoneTransform (HumanBodyBones.RightHand);
                    break;
			case AllEnums.PreferredHand.Ambidextrous:
                    int handRoll = Random.Range(0, 100);
                    if (handRoll > 49)
                    {
                        rtConsumable._equippedHand = AllEnums.PreferredHand.Left;
                        //equip left hand -- because why not
                        position = instance._leftHandPosition._position;
                        eulers = instance._leftHandPosition._eulers;
                        if (_stateCtrl._animator.isHuman)
                        {
                            parent = _stateCtrl._animator.GetBoneTransform(HumanBodyBones.LeftHand);
                        }
                        else
                        {
                            parent = _stateCtrl._myTransform;
                        }
                    }
                    else
                    {
                        //equip right hand -- because why not
                        rtConsumable._equippedHand = AllEnums.PreferredHand.Right;
                        position = instance._rightHandPosition._position;
                        eulers = instance._rightHandPosition._eulers;
                        if (_stateCtrl._animator.isHuman)
                        {
                            parent = _stateCtrl._animator.GetBoneTransform(HumanBodyBones.RightHand);
                        }
                        else
                        {
                            parent = _stateCtrl._myTransform;
                        }
                    }
                    break;
			}

			rtConsumable._rtModel.transform.parent = parent;
			rtConsumable._rtModel.transform.localPosition = position;
			rtConsumable._rtModel.transform.localEulerAngles = eulers;
			rtConsumable._rtModel.transform.localScale = scale;

			rtConsumable._rtModel.SetActive (true);
		}

		public void UnequipConsumable(Shepherd.RuntimeConsumable rtConsumable) {
			//assuming this is currently in the players hand
			if (_currentConsumable == rtConsumable) {
				if (_runtimeRefs._consumable != null) {
					_runtimeRefs.UnregisterRuntimeConsumable (rtConsumable);
					_runtimeRefs._consumable.Remove (rtConsumable._instance);
                    if (_testing._isPlayer)
                    {
                        _equippedItems._data.Remove(rtConsumable._instance); //removes from equipped items in PlayerEquipmentData ?
                    }
                    //if there is no other consumable item equipped -- must revert to hands
					if (_runtimeRefs._consumable._value.Count == 0 && _runtimeRefs._consumable._index.value == 0 && !_runtimeRefs._consumable._value.Contains(_emptyItemsDefault)) {
						Debug.Log ("reverting back to hands");

                        unequippedItems = _runtimeRefs.FindRuntimeConsumables(false);
                        for (int i = 0; i < unequippedItems.Count; i++)
                        {
                            if (_wResourceControl.CompareWorldItem(unequippedItems[i]._instance._itemName, _emptyItemsDefault._itemName))
                            {
                                Debug.Log("found hands in runtime consumables, creating runtime version");

                                //instantiate weapon to be equipped to currentConsumable
                                Consumable consumableToInstanitate = (Consumable)unequippedItems[i]._instance;
                                GameObject gObject = GameObject.Instantiate(consumableToInstanitate._modelPrefab) as GameObject;
                                gObject.name = unequippedItems[i]._instance._itemName;
                                unequippedItems[i]._rtModel = gObject;
                               
                                unequippedItems[i]._isEquipped = true;
                                unequippedItems[i]._itemName = unequippedItems[i]._instance._itemName;
                                _currentConsumable = unequippedItems[i];
                                break;
                            }
                        }
                        _runtimeRefs._consumable.Add(_emptyItemsDefault);
                        _runtimeRefs._runtimeConsumables = unequippedItems;                        
					}
                    else
                    {
                        //else there are other equipped consumable items, switch to one of them
                        Debug.Log("reverting to another equipped consumable item...");
                        List<RuntimeConsumable> equippedItems = _runtimeRefs.FindRuntimeConsumables(true);
                        for (int i = 0; i < equippedItems.Count; i++)
                        {
                            if (!_wResourceControl.CompareWorldItem(equippedItems[i]._instance._itemName, rtConsumable._instance._itemName))
                            {
                                //instantiate weapon to be equipped to currentConsumable
                                Consumable consumableToInstanitate = (Consumable)equippedItems[i]._instance;
                                GameObject gObject = GameObject.Instantiate(consumableToInstanitate._modelPrefab) as GameObject;
                                gObject.name = equippedItems[i]._instance._itemName;
                                equippedItems[i]._rtModel = gObject;
                                equippedItems[i]._itemName = equippedItems[i]._instance._itemName;
                                _currentConsumable = equippedItems[i];
                                _runtimeRefs._consumable._index.value = 0; //reset to 0
                                break;
                            }
                        }                        
                    }
                }
                else
                {
                    //you're an animal and you don't have a current item thingy TODO
                    _runtimeRefs.UnregisterRuntimeConsumable(rtConsumable);
                    unequippedItems = _runtimeRefs.FindRuntimeConsumables(true);
                    for (int i = 0; i < unequippedItems.Count; i++)
                    {
                        if (_wResourceControl.CompareWorldItem(unequippedItems[i]._instance._itemName, _emptyItemsDefault._itemName))
                        {
                            Debug.Log("found hands in runtime consumables, creating runtime version");

                            //instantiate weapon to be equipped to currentConsumable
                            Consumable consumableToInstanitate = (Consumable)unequippedItems[i]._instance;
                            GameObject gObject = GameObject.Instantiate(consumableToInstanitate._modelPrefab) as GameObject;
                            gObject.name = unequippedItems[i]._instance._itemName;
                            unequippedItems[i]._rtModel = gObject;

                            unequippedItems[i]._isEquipped = true;
                            unequippedItems[i]._itemName = unequippedItems[i]._instance._itemName;
                            _currentConsumable = unequippedItems[i];
                            break;
                        }
                    }
                    return;
                }
            }
            EquipConsumable(_currentConsumable, _stateCtrl._biography._mainHand);

            if (_testing._isPlayer)
            {
                CycleConsumables();
            } else
            {
                _stateCtrl.PlayInteractAnimation(AnimationStrings.StateChangeItem, false);
            }
            _stateCtrl.UpdateGameUI();
        }

		public void EquipWeapon(Shepherd.RuntimeWeapon rtWeapon, bool isLeftHand) {
			Vector3 position = Vector3.zero;
			Vector3 eulers = Vector3.zero;
			Vector3 scale = Vector3.one;
			Transform parent = null;

			Shepherd.Weapon instance = (Shepherd.Weapon)rtWeapon._instance;

			if (isLeftHand) {
				position = instance._leftHandPosition._position;
				eulers = instance._leftHandPosition._eulers;

				if (_stateCtrl._animator.isHuman) {
					parent = _stateCtrl._animator.GetBoneTransform (HumanBodyBones.LeftHand);
				} else {
					parent = _stateCtrl._myTransform;
				}
			} else {
				position = rtWeapon._rtModel.transform.position;
				eulers = rtWeapon._rtModel.transform.eulerAngles;
				if (_stateCtrl._animator.isHuman) {
					parent = _stateCtrl._animator.GetBoneTransform (HumanBodyBones.RightHand);
				} else {
					parent = _stateCtrl._myTransform;
				}
			}

			rtWeapon._rtModel.transform.parent = parent;
			rtWeapon._rtModel.transform.localPosition = position;
			rtWeapon._rtModel.transform.localEulerAngles = eulers;
			//rtWeapon._rtModel.transform.localScale = scale;

			rtWeapon._rtModel.SetActive (true);
		}

		public void WeaponToRuntime(Object wObject, ref Shepherd.RuntimeWeapon slot) {
			Shepherd.Weapon weapon = (Shepherd.Weapon)wObject;
			GameObject gObject = GameObject.Instantiate (weapon._modelPrefab) as GameObject;
			gObject.SetActive (false);
			gObject.name = weapon.name;

			Shepherd.RuntimeWeapon newRuntimeWeapon = new Shepherd.RuntimeWeapon ();
			newRuntimeWeapon._rtModel = gObject;
			newRuntimeWeapon._instance = weapon;
            newRuntimeWeapon._itemName = weapon._itemName;
			newRuntimeWeapon._durability = weapon._itemStats._maxDurability;
			newRuntimeWeapon._weaponHook = newRuntimeWeapon._rtModel.GetComponentInChildren<WeaponHook> ();
			newRuntimeWeapon._weaponHook.InitDamageColliders (_stateCtrl);

			slot = newRuntimeWeapon;
			//not even using....
			//_wResourceControl._runtimeReferences.RegisterRuntimeWeapon (newRuntimeWeapon);
		}
		//test
		public void WeaponToRuntime(List<WorldItem> weapons, bool isLeft, ref RuntimeWeapon slot) {
			for (int i = 0; i < weapons.Count; i++) {
				Shepherd.Weapon weapon = (Shepherd.Weapon)weapons [i];
				GameObject gObject = GameObject.Instantiate (weapon._modelPrefab) as GameObject;
				gObject.SetActive (false);
				gObject.name = weapon.name;

				Shepherd.RuntimeWeapon newRuntimeWeapon = new Shepherd.RuntimeWeapon ();
				newRuntimeWeapon._rtModel = gObject;
				newRuntimeWeapon._instance = weapon;
                newRuntimeWeapon._itemName = weapon._itemName;
				newRuntimeWeapon._durability = weapon._itemStats._maxDurability;
				newRuntimeWeapon._weaponHook = newRuntimeWeapon._rtModel.GetComponentInChildren<WeaponHook> ();
				newRuntimeWeapon._weaponHook.InitDamageColliders (_stateCtrl);
				newRuntimeWeapon._equippedHand = (isLeft) ? AllEnums.PreferredHand.Left : AllEnums.PreferredHand.Right;
                newRuntimeWeapon._isEquipped = true;

				if (i == 0) {
					slot = newRuntimeWeapon;
				} else {
					GameObject.Destroy (gObject);
				}

				_wResourceControl._runtimeReferences.RegisterRuntimeWeapon (newRuntimeWeapon);
				_equippedItems._data.Add (weapons [i]);
			}
		}

		//test
		public void ConsumableToRuntime(Object cObject, ref Shepherd.RuntimeConsumable slot) {
			Shepherd.Consumable consumable = (Shepherd.Consumable)cObject;
			GameObject gObject = GameObject.Instantiate (consumable._modelPrefab) as GameObject;
			gObject.SetActive (false);
			gObject.name = consumable.name;

			Shepherd.RuntimeConsumable newConsumable = new Shepherd.RuntimeConsumable ();
			newConsumable._rtModel = gObject;
			newConsumable._instance = consumable;
            newConsumable._itemName = consumable._itemName;
			newConsumable._unbreakable = consumable._isUnbreakable;
			newConsumable._durability = consumable._charges;
			newConsumable._isEquipped = true;

			slot = newConsumable;
			//not even using....
			_wResourceControl._runtimeReferences.RegisterRuntimeConsumable (newConsumable);
		}
		public void ConsumableToRuntime(List<WorldItem> consumables, ref Shepherd.RuntimeConsumable slot) {
			for (int i = 0; i < consumables.Count; i++) {
				Shepherd.Consumable consumable = (Shepherd.Consumable)consumables[i];
				GameObject gObject = GameObject.Instantiate (consumable._modelPrefab) as GameObject;
				gObject.SetActive (false);
				gObject.name = consumable.name;

				Shepherd.RuntimeConsumable newConsumable = new Shepherd.RuntimeConsumable ();
				newConsumable._rtModel = gObject;
				newConsumable._instance = consumable;
                newConsumable._itemName = consumable._itemName;
                newConsumable._unbreakable = consumable._isUnbreakable;
				newConsumable._durability = consumable._charges;
				newConsumable._isEquipped = true;

				if (i == 0) {
					slot = newConsumable;
				} else {
					GameObject.Destroy (gObject);
				}

				_wResourceControl._runtimeReferences.RegisterRuntimeConsumable (newConsumable);
				_equippedItems._data.Add (consumables [i]);
			}
		}

		public void ArmorToRuntime(List<WorldItem> armors, ref List<Shepherd.RuntimeArmor> slots) {
			for (int i = 0; i < armors.Count; i++) {
				Shepherd.Armor armor = (Shepherd.Armor)armors [i];
				//GameObject gObject = GameObject.Instantiate (armor._modelPrefab) as GameObject;
				//gObject.SetActive (false);
				//gObject.name = armor.name;

				Shepherd.RuntimeArmor newArmor = new Shepherd.RuntimeArmor ();
				//newArmor._aModel = gObject;
				newArmor._instance = armor;
                newArmor._itemName = armor._itemName;
				newArmor._unbreakable = armor._isUnbreakable;
				newArmor._durability = armor._itemStats._maxDurability;
				newArmor._isEquipped = true;

				if (!slots.Contains (newArmor)) {
					slots.Add (newArmor);
				}

				_wResourceControl._runtimeReferences.RegisterRuntimeArmor (newArmor);
			}
		}

		public void WorldItemsToRuntime(List<WorldItem> items) {
			for (int i = 0; i < items.Count; i++) {
				switch (items [i]._itemType) {
				case AllEnums.ItemType.Weapon:
					Shepherd.Weapon weapon = (Shepherd.Weapon)items [i];

					Shepherd.RuntimeWeapon newRuntimeWeapon = new Shepherd.RuntimeWeapon ();
					newRuntimeWeapon._instance = weapon;
					newRuntimeWeapon._durability = weapon._itemStats._maxDurability;
					newRuntimeWeapon._equippedHand = AllEnums.PreferredHand.Ambidextrous;
                        newRuntimeWeapon._isEquipped = false;

					if (_testing != null && _testing._isPlayer) {
						_wResourceControl._runtimeReferences.RegisterRuntimeWeapon (newRuntimeWeapon);
					}
					break;
				case AllEnums.ItemType.Consumable:
					Shepherd.Consumable consumable = (Shepherd.Consumable)items [i];

					Shepherd.RuntimeConsumable newConsumable = new Shepherd.RuntimeConsumable ();
					newConsumable._instance = consumable;
					newConsumable._unbreakable = consumable._isUnbreakable;
					newConsumable._durability = consumable._charges;
					newConsumable._isEquipped = false;

					if (_testing != null && _testing._isPlayer) {
						_wResourceControl._runtimeReferences.RegisterRuntimeConsumable (newConsumable);
					}
					break;
				case AllEnums.ItemType.Armor:
					Shepherd.Armor armor = (Shepherd.Armor)items [i];

					Shepherd.RuntimeArmor newArmor = new Shepherd.RuntimeArmor ();
					newArmor._instance = armor;
					newArmor._unbreakable = armor._isUnbreakable;
					newArmor._durability = armor._itemStats._maxDurability;
					newArmor._isEquipped = false;

					if (_testing != null && _testing._isPlayer) {
						_wResourceControl._runtimeReferences.RegisterRuntimeArmor (newArmor);
					}
					break;
				}
			}
		}

		public void CycleWeapons(bool isLeftHand) {
			if (isLeftHand) {
				if (_runtimeRefs._leftHand._value.Count == 0) {
					return;
				}
					
				if (_runtimeRefs._leftHand._index.value < (_runtimeRefs._leftHand._value.Count - 1)) {
					_runtimeRefs._leftHand._index.value++;
				} else {
					_runtimeRefs._leftHand._index.value = 0;
				}
				_wResourceControl._runtimeReferences.UpdateCurrentRuntimeWeapon (_runtimeRefs._leftHand._index.value, _stateCtrl, ref _currentLeftWeapon);
				EquipWeapon (_currentLeftWeapon, true);
			} else {
				if (_runtimeRefs._rightHand._value.Count == 0) {
					return;
				}

				if(_runtimeRefs._rightHand._index.value < (_runtimeRefs._rightHand._value.Count - 1)) {
					_runtimeRefs._rightHand._index.value++;
				} else {
					_runtimeRefs._rightHand._index.value = 0;
				}

				_wResourceControl._runtimeReferences.UpdateCurrentRuntimeWeapon (_runtimeRefs._rightHand._index.value, _stateCtrl, ref _currentRightWeapon);
				EquipWeapon (_currentRightWeapon, false);
			}
			CloseSpellBreathCollider ();
			CloseAllDamageColliders ();
			CloseBlockCollider ();

			_stateCtrl.PlayInteractAnimation (AnimationStrings.StateChangeWeapon, isLeftHand);
			_stateCtrl._actionControl.UpdateActionsWithCurrentItems ();
		}

		public void CycleConsumables() {
			if (_runtimeRefs._consumable == null || _runtimeRefs._consumable._value.Count == 0) {
				return;
			}
				
			if (_runtimeRefs._consumable._index.value < (_runtimeRefs._consumable._value.Count - 1)) {
				_runtimeRefs._consumable._index.value++;
			} else {
				_runtimeRefs._consumable._index.value = 0;
			}
			_wResourceControl._runtimeReferences.UpdateCurrentRuntimeConsumable (_runtimeRefs._consumable._index.value, _stateCtrl, ref _currentConsumable);
			EquipConsumable (_currentConsumable, _stateCtrl._biography._mainHand);
			_stateCtrl.PlayInteractAnimation (AnimationStrings.StateChangeItem, false);
		}

		public void CycleConsumablesToSlot(int numSlot) {
			if (_runtimeRefs._consumable._value.Count == 0) {
				return;
			}

			if (numSlot > (_runtimeRefs._consumable._value.Count - 1)) {
				_runtimeRefs._consumable._index.value = 0;
			} else {
				_runtimeRefs._consumable._index.value = numSlot;
			}
				
			_wResourceControl._runtimeReferences.UpdateCurrentRuntimeConsumable (_runtimeRefs._consumable._index.value, _stateCtrl, ref _currentConsumable);
			EquipConsumable (_currentConsumable, _stateCtrl._biography._mainHand);
			_stateCtrl.PlayInteractAnimation (AnimationStrings.StateChangeItem, false);
		}
		//...

		public void InitAllDamageColliders() {
			if (_currentRightWeapon != null) {
				_currentRightWeapon._weaponHook.InitDamageColliders (_stateCtrl);
			}

			if (_currentLeftWeapon != null) {
				_currentLeftWeapon._weaponHook.InitDamageColliders (_stateCtrl);
			}

			/*
			 * for(int i = 0; i < _currentArmorSet.Count; i++) {
			 * 		_currentArmorSet[i]._weaponHook.InitDamageColliders(_stateCtrl);
			 * }
			*/
		}
		public void OpenAllDamageColliders() {
			if (_currentRightWeapon != null && _currentRightWeapon._weaponHook != null) {
				_currentRightWeapon._weaponHook.OpenDamageColliders ();
			}

			if (_currentLeftWeapon != null && _currentRightWeapon._weaponHook != null) {
				_currentLeftWeapon._weaponHook.OpenDamageColliders ();
			}

			/*
			 * for(int i = 0; i < _currentArmorSet.Count; i++) {
			 * 		_currentArmorSet[i]._weaponHook.OpenDamageColliders();
			 * }
			*/
		}
		public void CloseAllDamageColliders() {
			if (_currentRightWeapon != null && _currentRightWeapon._weaponHook != null) {
				_currentRightWeapon._weaponHook.CloseDamageColliders ();
			}

			if (_currentLeftWeapon != null  && _currentRightWeapon._weaponHook != null) {
				_currentLeftWeapon._weaponHook.CloseDamageColliders ();
			}

			/*
			 * for(int i = 0; i < _currentArmorSet.Count; i++) {
			 * 		_currentArmorSet[i]._weaponHook.CloseDamageColliders();
			 * }
			*/
		}
			
		public void OpenParryCollider() {
			_parryCollider.SetActive (true);
		}
		public void CloseParryCollider() {
			_parryCollider.SetActive (false);
		}
			
		public void OpenBlockCollider() {
			_blockCollider.SetActive (true);
		}
		public void CloseBlockCollider() {
			_blockCollider.SetActive (false);
		}

		public void OpenSpellBreathCollider() {
			_spellBreathCollider.SetActive (true);
		}
		public void CloseSpellBreathCollider() {
			_spellBreathCollider.SetActive (false);
			_spellBreathCollider.GetComponent<SpellBreathCollider> ()._parent = null;
		}
	}
}