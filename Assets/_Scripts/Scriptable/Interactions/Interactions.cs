using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[CreateAssetMenu(menuName = "Shepherd/Single Instances/Interactions")]
	public class Interactions : ScriptableObject {

		public List<WorldInteraction> _allInteractions = new List<WorldInteraction>();

		private Dictionary<string, int> _interactionEncyclopedia = new Dictionary<string, int>();

		public void Init() {
			#if UNITY_EDITOR
			_allInteractions = EditorUtilities.FindAssetByType<WorldInteraction>();
			#endif

			_interactionEncyclopedia.Clear ();

			for (int i = 0; i < _allInteractions.Count; i++) {
				if (_interactionEncyclopedia.ContainsKey (_allInteractions [i].name)) {
					Debug.Log ("Uh Oh, duplicate World Interaction: " + _allInteractions [i].name);
				} else {
                    _allInteractions[i].Init();
					_interactionEncyclopedia.Add (_allInteractions [i].name, i);
				}
			}
		}

		public void AddInteraction(WorldInteraction newInteraction) {
			if (_interactionEncyclopedia.ContainsKey (newInteraction.name)) {
				Debug.Log ("Uh Oh, duplicate World Interaction: " + newInteraction.name);
				/*if (newInteraction is ToggleInteract) {
					//TODO: temp fix -- must find anothe way
					newInteraction.name += "I";
					_allInteractions.Add (newInteraction);
					_allInteractions [_allInteractions.Count-1]._enabled = true;
					_interactionEncyclopedia.Add (newInteraction.name, _allInteractions.Count-1);
				}*/
			} else {
				_allInteractions.Add (newInteraction);
				_allInteractions [_allInteractions.Count-1]._enabled = true;
				_interactionEncyclopedia.Add (newInteraction.name, _allInteractions.Count-1);
			}
		}

		public WorldInteraction GetInteraction(string id) {
			WorldInteraction returnInteraction = null;
			int index = -1;

			if (_interactionEncyclopedia.TryGetValue (id, out index)) {
				returnInteraction = _allInteractions [index];
			}

			if (index == -1) {
				Debug.Log ("No World Interaction with ID: " + id + " found!");
			}

			return returnInteraction;
		}

		public void RemoveInteraction(WorldInteraction oldInteraction) {
			if (oldInteraction != null && _interactionEncyclopedia.ContainsKey (oldInteraction.name)) {
				_interactionEncyclopedia.Remove (oldInteraction.name);
				_allInteractions.Remove (oldInteraction);
				Destroy (oldInteraction);
			}
		}
	}
}