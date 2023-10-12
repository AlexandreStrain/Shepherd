using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd
{
    [CreateAssetMenu(menuName = "Shepherd/World Objects/NPC/BioCard"), Serializable]
    public class BioCard : ScriptableObject
    {

        public string _nameOfCharacter; //Name of NPC who this scorecard belongs to
        [Header("Runtime Statistics")]
        public AllEnums.Gender _gender;
        public AllEnums.Race _race;
        public AllEnums.Origin _origin;
        public AllEnums.Class _class;
        public AllEnums.Faction _faction;
        public AllEnums.Status _status;

        public WorldItemStats _currentStatistics;

        [Header("Dialogue Tree")]

        [Range(-10, 10)]
        public float _verbalAccount = 0f; //How are the current interactions with the player going? updates everytime the player talks to the NPC
        public int _verbalIndex = 0; //How far along in dialogue is the player at
        public int _specialEventIndex = 0; //How many special events have taken place for this character?

        [Header("Player Relations")]
        [Range(-10, 10)]
        public float _relationship = 0f; //How are we getting along with the player? only updates if verbal account reaches max or min value
        [Range(-10, 10)]
        public float _personality = 0f; //How do we talk towards the player? influenced by relationship and current dialogue at any verbalIndex (what are we most likely to say given the circumstances?)
        public bool _isMarkedAsImportant;
        public bool _hasRevealedName; //Did the NPC reveal their name to the player?
    }


    //TODO: BIOCARDS SHOULD ALL BE LIKE THIS, AND NOT SCRIPTABLE
    [Serializable]
    public class BiocardSaveData
    {
        public string _nameOfCharacter; //Name of NPC who this scorecard belongs to

        public AllEnums.Gender _gender;
        public AllEnums.Race _race;
        public AllEnums.Origin _origin;
        public AllEnums.Class _class;
        public AllEnums.Faction _faction;

        public AllEnums.Status _status;

        public WorldItemStats _currentStatistics;

        [Range(-10, 10)]
        public float _verbalAccount = 0f; //How are the current interactions with the player going? updates everytime the player talks to the NPC
        public int _verbalIndex = 0; //How far along in dialogue is the player at
        public int _specialEventIndex = 0; //How many special events have taken place for this character?

        [Range(-10, 10)]
        public float _relationship = 0f; //How are we getting along with the player? only updates if verbal account reaches max or min value
        [Range(-10, 10)]
        public float _personality = 0f; //How do we talk towards the player? influenced by relationship and current dialogue at any verbalIndex (what are we most likely to say given the circumstances?)
        public bool _isMarkedAsImportant;
        public bool _hasRevealedName;

        public void CopyDataFromBiocard(BioCard biocard)
        {
            this._nameOfCharacter = biocard._nameOfCharacter;
            this._verbalAccount = biocard._verbalAccount;
            this._verbalIndex = biocard._verbalIndex;
            this._specialEventIndex = biocard._specialEventIndex;
            this._relationship = biocard._relationship;
            this._personality = biocard._personality;

            this._currentStatistics = biocard._currentStatistics;

            this._gender = biocard._gender;
            this._race = biocard._race;
            this._origin = biocard._origin;
            this._class = biocard._class;
            this._faction = biocard._faction;
            this._status = biocard._status;

            this._isMarkedAsImportant = biocard._isMarkedAsImportant;
            this._hasRevealedName = biocard._hasRevealedName;
    }

        public void PasteDataIntoBiocard(ref BioCard biocard)
        {
            biocard._nameOfCharacter = this._nameOfCharacter;
            biocard._verbalAccount = this._verbalAccount;
            biocard._verbalIndex = this._verbalIndex;
            biocard._specialEventIndex = this._specialEventIndex;
            biocard._relationship = this._relationship;
            biocard._personality = this._personality;

            biocard._currentStatistics = this._currentStatistics;

            biocard._gender = this._gender;
            biocard._race = this._race;
            biocard._origin = this._origin;
            biocard._class = this._class;
            biocard._faction = this._faction;
            biocard._status = this._status;

            biocard._isMarkedAsImportant = this._isMarkedAsImportant;
            biocard._hasRevealedName = this._hasRevealedName;
        }
    }
}