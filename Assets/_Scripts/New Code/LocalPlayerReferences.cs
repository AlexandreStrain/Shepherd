using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[System.Serializable]
	public class LocalPlayerReferences {

		[Header("Slots")]
		public CurrentItem _rightHand;
		public CurrentItem _leftHand;
		public CurrentItem _spell;
		public CurrentItem _consumable;

		//test
		public CurrentItem _armor;

		[Header("Events")]
		public GameEvent _updateGameUI;
	}
}