using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[CreateAssetMenu(menuName = "Shepherd/Single Instances/World Names")]
	public class WorldNames : ScriptableObject {

		public TextAsset _femaleNames;
		public TextAsset _maleNames;
		public List<string> _allFemaleNames = new List<string>();
		public List<string> _allMaleNames = new List<string>();
		public Dictionary <string, int> _femaleNamesEncyclopedia = new Dictionary<string, int>();
		public Dictionary<string, int> _maleNamesEncyclopedia = new Dictionary<string, int>();

		public void Init() {
			_allFemaleNames.Clear();
			_allMaleNames.Clear();

			string[] femaleNames = _femaleNames.text.Split('\n');
			for (int i = 0; i < femaleNames.Length; i++) {
				_allFemaleNames.Add(femaleNames[i]);
			}

			string[] maleNames = _maleNames.text.Split('\n');
			for (int i = 0; i < maleNames.Length; i++) {
				_allMaleNames.Add(maleNames[i]);
			}


			_femaleNamesEncyclopedia.Clear ();
			_maleNamesEncyclopedia.Clear ();

			for (int i = 0; i < _allFemaleNames.Count; i++) {
				if (_femaleNamesEncyclopedia.ContainsKey (_allFemaleNames [i])) {
					Debug.Log ("Uh Oh, duplicate Name in Female Names: " + _allFemaleNames [i]);
				} else {
					_femaleNamesEncyclopedia.Add (_allFemaleNames [i], i);
				}
			}

			for (int i = 0; i < _allMaleNames.Count; i++) {
				if (_maleNamesEncyclopedia.ContainsKey (_allMaleNames [i])) {
					Debug.Log ("Uh Oh, duplicate Name in Male Names: " + _allMaleNames [i]);
				} else {
					_maleNamesEncyclopedia.Add (_allMaleNames [i], i);
				}
			}
		}

		public string GetRandomName(AllEnums.Gender gender) {
			int randomNumber = 0;
			switch (gender) {
			case AllEnums.Gender.Male:
				randomNumber = Random.Range (0, _allMaleNames.Count);
				return _allMaleNames [randomNumber];
			case AllEnums.Gender.Female:
				randomNumber = Random.Range (0, _allFemaleNames.Count);
				return _allFemaleNames [randomNumber];
			}
			return null;
		}
	}
}