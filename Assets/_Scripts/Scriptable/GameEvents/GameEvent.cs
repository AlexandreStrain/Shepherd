using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[CreateAssetMenu(menuName = "Shepherd/GameEvent")]
	public class GameEvent : ScriptableObject {

		private List<GameEventListener> _gEListeners = new List<GameEventListener>();
		private List<SpecialEventListener> _sEListeners = new List<SpecialEventListener> ();

		public void Register(GameEventListener listener) {
			if(!_gEListeners.Contains(listener)) {
				_gEListeners.Add (listener);
			}
		}

		public void Unregister(GameEventListener listener) {
			if (_gEListeners.Contains (listener)) {
				_gEListeners.Remove (listener);
			}
		}

		public void Raise() {
			for (int i = 0; i < _gEListeners.Count; i++) {
				_gEListeners [i].Response ();
			}
		}

		public void RegisterSE(SpecialEventListener listener) {
			if(!_sEListeners.Contains(listener)) {
				_sEListeners.Add (listener);
			}
		}

		public void UnregisterSE(SpecialEventListener listener) {
			if (_sEListeners.Contains (listener)) {
				_sEListeners.Remove (listener);
			}
		}

		public bool HasGameEventListeners() {
			return _gEListeners.Count > 0;
		}

		public bool HasSpecialGameEventListeners() {
			return _sEListeners.Count > 0;
		}

		public void RaiseResponse(int index) {
			for (int i = 0; i < _sEListeners.Count; i++) {
				//if (index >= 0 && index < _sEListeners [i]._relatedResponses.Count) {
					_sEListeners [i].Response (index);
				//}
			}
		}
	}
}