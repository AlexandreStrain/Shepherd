using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[System.Serializable]
	public class RuntimeWeapon : RuntimeWorldItem {

		public WeaponHook _weaponHook;
		public AllEnums.PreferredHand _equippedHand; //ambidextrous = none
	}
}