using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[CreateAssetMenu(menuName= "Shepherd/Single Instances/Current Item")]
	public class CurrentItem : ScriptableObject {
		public AllEnums.ItemType _type;
		public AllEnums.PreferredHand _onHand; //test.. temp
		public IntVariable _index;
		//rename
		public List<WorldItem> _value = new List<WorldItem>();
		public RuntimeReferences _parent;

		public WorldItem Get() {
			if (_value.Count == 0) {
				return null;
			}

			if (_index.value > _value.Count - 1) {
				_index.value = 0;
			}

			return _value[_index.value];
		}

		public RuntimeWorldItem GetRT() {
			if (_value.Count == 0) {
				return null;
			}

			if (_index.value > _value.Count - 1) {
				_index.value = 0;
			}
			WorldItem item = _value [_index.value];

			List<RuntimeWorldItem> rtItems = null;
			switch (_type) {
			case AllEnums.ItemType.Weapon:
				rtItems = _parent.FindRTWeapons (_onHand);
				break;
			case AllEnums.ItemType.Consumable:
				rtItems = _parent.FindRTConsumables (true);
				break;
			case AllEnums.ItemType.Armor:
			case AllEnums.ItemType.Spell:
			default:
				rtItems = null; //for now..
				break;
			}
				
			for (int i = 0; i < rtItems.Count; i++) {
				if (string.Equals(item._itemName, rtItems[i]._instance._itemName)) {
					return rtItems [i];
				}
			}
			return null;
			//return _value[_index.value];
		}

		public WorldItem GetAt(int index) {
			if (_value.Count == 0 || index > _value.Count - 1) {
				return null;
			}

			return _value[index];
		}

		public void Add(WorldItem thisItem) {
			_value.Add (thisItem);
		}
		public void Remove(WorldItem thisItem) {
			for (int i = 0; i < _value.Count; i++) {
				if (string.Equals (_value[i]._itemName, thisItem._itemName)) {
					Debug.Log ("removing from consumable slots..." + _value[i]._itemName);
					_value.RemoveAt (i);
					break;
				}
			}
		}

		public void Clear() {
			_index.value = 0;
			_value.Clear ();
		}
	}
}