using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
    [System.Serializable]
	public class WorldItem : ScriptableObject {

		[Header("General Information")]
		public AllEnums.ItemType _itemType;
        //....
        public string _itemName;
        [TextArea(1, 5)]
        public string _itemDescription;
        public Sprite _itemHUDIcon;
        public Sprite _itemIcon;
        //....

        public bool _isQuestItem; //can never decay
        public bool _isUnbreakable;
		[Header("Statistics")]
		public WorldItemStats _itemStats;
	}

	[System.Serializable]
	public class WorldItemStats {
		[Header("Attributes/Requirements/Side Effects")]
		public float _strength = 0f; //melee, ranged?
		public float _vitality = 0f; //health, resistance and immune system
		public float _endurance = 0f; //stamina, carry weight and poise
		public float _dexterity = 0f; //melee, ranged?
		public float _intelligence = 0f; //magic? or AI?
		public float _courage = 0f; //fear
		public float _perception = 0f; //sight/hearing
		public float _luck = 0f; //because why not


        [Header("Effects")]
        public float _thirst = 0f; //how much thirst does this item reduce?
        public float _waste = 0f; //how much waste does this item give?
        public float _hunger = 0f; //how much hunger does this item reduce?
        public float _sleep = 0f; //how much sleep does this item recover?
        public float _pleasure = 0f; //how much pleasure does this item give?

        public float _health = 0f; //how much health does this item restore?
        public float _stamina = 0f; //how much stamina does this item restore?
        public float _baseCourage = 0f; //how much courage does this item restore?
        public float _immuneSystem = 0f; //how much of the immune system does this item restore?
        public float _poise = 0f; //how much poise does this item give?
        public float _carryWeight = 0f; //how much carry weight does this item add?

        [Header ("Attack/Defence")] //type of attack damage done by weapon
		public float _physical = 0f;
		public float _finesse = 0f;
		public float _magical = 0f; //unknown element
		public float _fire = 0f;
		public float _water = 0f;
		public float _earth = 0f; 
		public float _wind = 0f; //lightning
		public float _light = 0f;
		public float _dark = 0f;

		[Header("Ailments/Resistances")] //type of ailments needed to be immune to
		public float _bleed = 0f;
		public float _poison = 0f;
		public float _heat = 0f;
		public float _freeze = 0f;
		public float _disease = 0f;
		public float _dizzy = 0f;

		[Header("Other")]
		public float _luckDmg = 0f; //because why not
		public float _soundProduction = 0f; //hearing
		public float _fear = 0f; //does this thing cause fear
		public float _weight = 0f;
		public float _maxDurability = 0f;
	}
}