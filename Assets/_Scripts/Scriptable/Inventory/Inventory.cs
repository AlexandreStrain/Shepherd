using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[CreateAssetMenu(menuName = "Shepherd/Single Instances/Inventory")]
	public class Inventory : ScriptableObject {

		public List<WorldItem> _allItems = new List<WorldItem>();

		private Dictionary<string, int> _itemEncyclopedia = new Dictionary<string, int>();

		public void Init() {
			#if UNITY_EDITOR
			_allItems = EditorUtilities.FindAssetByType<WorldItem>();
			#endif

			_itemEncyclopedia.Clear ();

			for (int i = 0; i < _allItems.Count; i++) {
				if (_itemEncyclopedia.ContainsKey (_allItems [i].name)) {
					Debug.Log ("Uh Oh, duplicate World Item: " + _allItems [i].name);
				} else {
					_itemEncyclopedia.Add (_allItems [i].name, i);
				}
			}
		}

		public WorldItem GetItem(string id) {
			WorldItem returnItem = null;
			int index = -1;

			if (_itemEncyclopedia.TryGetValue (id, out index)) {
				returnItem = _allItems [index];
			}

			if (index == -1) {
				Debug.Log ("No World Item with ID: " + id + " found!");
			}

			return returnItem;
		}
	}
}