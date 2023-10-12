using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[System.Serializable]
	public class RuntimeArmor : RuntimeWorldItem {
        /*
         * TODO: based off of what time of entity you are (Human, Humanoid, Animal, Monster, Unknown),
         * there should be a way to setup which model of armor you have at runtime
         */
        public AllEnums.Gender _armorFitsGender;
        public AllEnums.Race _armorFitsRace;
	}
}