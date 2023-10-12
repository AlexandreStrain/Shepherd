using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[CreateAssetMenu(menuName = "Shepherd/World Objects/NPC/Biography"), Serializable]
    public class Biography : ScriptableObject {

		[Header("General Information")]
		public string _name; //NPC full name
		[TextArea(1,5)]
		public string _description;

		public AllEnums.Gender _gender;
		public AllEnums.Race _race;
        public AllEnums.Origin _origin;

		public AllEnums.Class _class;
		public AllEnums.Faction _faction;

		public List<AllEnums.Faction> _enemies = new List<AllEnums.Faction>();
		public List<AllEnums.Faction> _alliances = new List<AllEnums.Faction>();

		[Header("Statistics")]
		public AllEnums.PreferredHand _mainHand;
		public WorldItemStats _baseStatistics;

        [Header("Dialogue")]
        public Sprite _portrait;
		public BioCard _currentBioCard;
		public AudioClip _testVoice;
		public Dialogue[] _allDialogue;
	}
}