using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[System.Serializable]
	public class RuntimeConsumable : RuntimeWorldItem {
        public AllEnums.PreferredHand _equippedHand; //ambidextrous = none
    }
}