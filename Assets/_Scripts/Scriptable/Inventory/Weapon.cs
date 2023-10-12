using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[CreateAssetMenu(menuName = "Shepherd/World Objects/Weapon", order = 2)]
	public class Weapon : WorldItem {
		[Header("Weapon")]
		public GameObject _modelPrefab;

		public StringVariable _oneHandIdle;
		public StringVariable _twoHandIdle;

		public ActionContainer[] _oneHandActions;
		public ActionContainer[] _twoHandActions;

		public LeftHandPosition _leftHandPosition;

		public ActionContainer GetActionContainer(AllEnums.ActionInputType fromInput, bool twoHanding = false) {
			if (twoHanding) {
				for (int i = 0; i < _twoHandActions.Length; i++) {
					if (_twoHandActions [i]._actionInputType == fromInput) {
						return _twoHandActions [i];
					}
				}
			} else {
				for (int i = 0; i < _oneHandActions.Length; i++) {
					if (_oneHandActions [i]._actionInputType == fromInput) {
						return _oneHandActions [i];
					}
				}
			}
			return null;
		}

		public Action[] GetActions(AllEnums.ActionInputType fromInput, bool twoHanding = false) {
			ActionContainer actionContainer = GetActionContainer (fromInput, twoHanding);
			if (actionContainer == null) {
				return null;
			}
			return actionContainer._actions;
		}
	}
}