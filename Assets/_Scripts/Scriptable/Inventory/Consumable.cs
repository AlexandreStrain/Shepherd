using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[CreateAssetMenu(menuName = "Shepherd/World Objects/Consumable", order = 3)]
	public class Consumable : WorldItem {
		[Header("Consumable")]
		public StringVariable _useAnimation;
		public ConsumableAction _consumableEffect;
		public GameObject _modelPrefab;
		public int _charges;

		public LeftHandPosition _leftHandPosition;
		public LeftHandPosition _rightHandPosition;

		public void SetEffectParent() { //Init?
			_consumableEffect._parent = this;
		}
	}
}