using UnityEngine;
using UnityEngine.Events;

namespace Shepherd {
	public class GameEventListener : MonoBehaviour {
		
		public GameEvent _gameEvent;
		public UnityEvent _response;

		public virtual void Response() {
			_response.Invoke ();
		}

		/// <summary>
		/// Override this to override the OnEnableLogic()
		/// </summary>
		public virtual void OnEnableLogic() {
			if (_gameEvent != null) {
				_gameEvent.Register (this);
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
				_gameEvent.Unregister (this);
			}
		}

		void OnDisable() {
			OnDisableLogic ();
		}
	}
}