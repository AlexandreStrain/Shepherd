using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	public class CharacterModel : MonoBehaviour {

		public CharacterModelParts _bodyModel;
		public EquipmentData _equipData;

		public CharacterStateController _parent;
		public bool _isPlayer;

		private void InitArmor() {
			for (int i = 0; i < AllEnums.ArmorTypeNumber; i++) {
				SkinnedMeshRenderer mesh = GetArmorPart ((AllEnums.ArmorType)i);
				if (mesh == null) {
					continue;
				}
				mesh.enabled = false;
			}
		}

		public void LoadEquipmentFromData() {
			if (_equipData == null) {
				return;
			}

			if (_parent != null && _isPlayer) {
				_parent._invControl.SetupInventoryV2 ();
				return;
			} else if (_parent != null && !_isPlayer) {
				//Debug.Log (_parent._biography._name);
				_parent._invControl.SetupInventoryV3 ();
				return;
			}

			InitArmor ();

			for (int i = 0; i < _equipData._data.Count; i++) {
				EquipArmor (_equipData._data [i]);
			}

			//if (_parent != null) {
			//	_parent._inventoryControl._currentEquipment._armor._value = _currentlyEquippedArmor;
			//	_parent._inventoryControl.SetupArmor ();
			//}
		}

		public void LoadEquipmentFromCurrent(List<WorldItem> armorSet) {
			if (armorSet == null) {
				return;
			}

			InitArmor ();

			for (int i = 0; i < armorSet.Count; i++) {
				EquipArmor (armorSet [i]);
			}
		}

		public void EquipArmor(WorldItem item) {
			if (item._itemType != AllEnums.ItemType.Armor) {
				return;
			}

			Armor armor = (Armor)item;
			SkinnedMeshRenderer mesh = GetArmorPart (armor._armorType);
			mesh.sharedMesh = armor._armorMesh;
			mesh.gameObject.SetActive (true);
			mesh.enabled = true;

			mesh.materials = armor._armorMaterials;

			//SkinnedMeshRenderer body = GetBodyPart (armor._armorType);
			//body.enabled = armor._baseBodyEnabled;
		}

		public void UnequipItem(AllEnums.ArmorType type) {
			SkinnedMeshRenderer mesh = GetArmorPart (type);
			//SkinnedMeshRenderer body = GetBodyPart (type);
			if (mesh == null) {
				return;
			}
			mesh.enabled = false;
			mesh.gameObject.SetActive (false);
			//body.enabled = true;
		}

		public void UnequipAll() {
			for (int i = 0; i < _equipData._data.Count; i++) {
				if (_equipData._data [i] is Armor) {
					UnequipItem (((Shepherd.Armor)_equipData._data [i])._armorType);
				}
			}
		}

		public SkinnedMeshRenderer GetArmorPart(AllEnums.ArmorType type) {
			switch (type) {
			case AllEnums.ArmorType.Accessory1:
				return _bodyModel._armorAccessory1;
			case AllEnums.ArmorType.Accessory2:
				return _bodyModel._armorAccessory2;
			case AllEnums.ArmorType.Accessory3:
				return _bodyModel._armorAccessory3;
			case AllEnums.ArmorType.LeftArm:
				return _bodyModel._armorLeftArm;
			case AllEnums.ArmorType.Head:
				return _bodyModel._armorHead;
			case AllEnums.ArmorType.RightArm:
				return _bodyModel._armorRightArm;
			case AllEnums.ArmorType.LeftHand:
				return _bodyModel._armorLeftHand;
			case AllEnums.ArmorType.Torso:
				return _bodyModel._armorTorso;
			case AllEnums.ArmorType.RightHand:
				return _bodyModel._armorRightHand;
			case AllEnums.ArmorType.LeftFoot:
				return _bodyModel._armorLeftFoot;
			case AllEnums.ArmorType.Legs:
				return _bodyModel._armorLegs;
			case AllEnums.ArmorType.RightFoot:
				return _bodyModel._armorRightFoot;
			default:
				return null;
			}
		}

		public SkinnedMeshRenderer GetBodyPart(AllEnums.ArmorType type) {
			switch (type) {
			case AllEnums.ArmorType.Accessory1:
			case AllEnums.ArmorType.Accessory2:
			case AllEnums.ArmorType.Accessory3:
			case AllEnums.ArmorType.LeftArm:
			case AllEnums.ArmorType.Head:
			case AllEnums.ArmorType.RightArm:
			case AllEnums.ArmorType.LeftHand:
			case AllEnums.ArmorType.Torso:
			case AllEnums.ArmorType.RightHand:
			case AllEnums.ArmorType.LeftFoot:
			case AllEnums.ArmorType.Legs:
			case AllEnums.ArmorType.RightFoot:
			default:
				return _bodyModel._body;
			}
		}
	}

	//Couldn't this be scriptable?
	[System.Serializable]
	public class CharacterModelParts {
		//Clothing/Armor Set
		public SkinnedMeshRenderer _armorAccessory1;
		public SkinnedMeshRenderer _armorAccessory2;
		public SkinnedMeshRenderer _armorAccessory3;

		public SkinnedMeshRenderer _armorLeftArm;
		public SkinnedMeshRenderer _armorHead;
		public SkinnedMeshRenderer _armorRightArm;

		public SkinnedMeshRenderer _armorLeftHand;
		public SkinnedMeshRenderer _armorTorso;
		public SkinnedMeshRenderer _armorRightHand;

		public SkinnedMeshRenderer _armorLeftFoot;
		public SkinnedMeshRenderer _armorLegs;
		public SkinnedMeshRenderer _armorRightFoot;

		//Actual Model
		public SkinnedMeshRenderer _body;
	}
}