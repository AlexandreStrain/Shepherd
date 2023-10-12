using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[CreateAssetMenu(menuName = "Shepherd/User Interface/UI StatInfo")]
	public class UserInterfaceStatInfo : ScriptableObject {

		public List<StringVariable> _allStatInfo = new List<StringVariable>();

		private Dictionary<string, int> _statInfoEncyclopedia = new Dictionary<string, int>();

		public void Init() {
			//#if UNITY_EDITOR
			//_allStatInfo = EditorUtilities.FindAssetByType<StringVariable>();
			//#endif

			_statInfoEncyclopedia.Clear ();

			for (int i = 0; i < _allStatInfo.Count; i++) {
				if (_statInfoEncyclopedia.ContainsKey (_allStatInfo [i].name)) {
					Debug.Log ("Uh Oh, duplicate Stat Info: " + _allStatInfo [i].name);
				} else {
					_statInfoEncyclopedia.Add (_allStatInfo [i].name, i);
				}
			}
		}

		public StringVariable GetStatInfo(string id) {
			StringVariable returnItem = null;
			int index = -1;

			if (_statInfoEncyclopedia.TryGetValue (id, out index)) {
				returnItem = _allStatInfo [index];
			}

			if (index == -1) {
				Debug.Log ("No Stat Info with ID: " + id + " found!");
			}

			return returnItem;
		}
	}
}