using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Shepherd {
	public class SpecialEventListener : MonoBehaviour {

		public GameEvent _gameEvent;
		public List<UnityEvent> _relatedResponses = new List<UnityEvent>();

		public void Response(int index) {
			if (index > _relatedResponses.Count-1 || index < 0) {
				Debug.Log ("Invalid Index");
			} else {
				_relatedResponses [index].Invoke ();
			}
		}

		public void Response() {
			if(_relatedResponses.Count != 0) {
				_relatedResponses [0].Invoke ();
			}
		}

		/// <summary>
		/// Override this to override the OnEnableLogic()
		/// </summary>
		public virtual void OnEnableLogic() {
			if (_gameEvent != null) {
				_gameEvent.RegisterSE (this);
			}
		}

		void OnEnable() {
			OnEnableLogic ();
		}

		/// <summary>
		/// Override this to override the OnDisableLogic()
		/// </summary>
		public virtual void OnDisableLogic() {
			if (_gameEvent != null) {
				_gameEvent.UnregisterSE (this);
			}
		}

		void OnDisable() {
			OnDisableLogic ();
		}
	}
}