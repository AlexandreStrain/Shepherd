using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	public class InteractionHook : MonoBehaviour {

		public Shepherd.WorldInteraction _myInteraction;

		private CharacterStateController _owner;

		public void InitOwner(CharacterStateController cControl) {
			_owner = cControl;
		}

        public void Setup() {
			if (_myInteraction is Conversation && _owner != null) {
				//Debug.Log ("preparing voice...");
				Shepherd.Conversation test = (Shepherd.Conversation)_myInteraction;
				test._source = _owner;
			} else if (_myInteraction is Environment) {
				Shepherd.Environment test = (Shepherd.Environment)_myInteraction;
				test._worldObject = this.gameObject;
                if (test._toggle)
                {
                    test.Interact(null);
                }

                //Debug.Log ("Prepping to Open...");
            } else if (_myInteraction is Give && _owner != null) {


			} else if (_myInteraction is ToggleInteract) {
				ToggleInteract test = (ToggleInteract)_myInteraction;
				test._source = _owner;
				test._objectToToggle = ((EnemyAIController)_owner._myBrain).GetMyUI ();

			} else if (_myInteraction is Pickup && _owner != null)
            {
                _myInteraction = _owner._interactControl._myActiveInteraction;
            }
		}

		public void Teardown() {
			if (_myInteraction is Conversation && _owner != null) {
				Debug.Log ("quieting voice...");
				Shepherd.Conversation test = (Shepherd.Conversation)_myInteraction;
				test._source = _owner;
			} else if (_myInteraction is Environment) {
				Shepherd.Environment test = (Shepherd.Environment)_myInteraction;
				test._worldObject = this.gameObject;
				Debug.Log ("tearing down Open...");
			} else if (_myInteraction is Pickup) {
				_owner._wRControl._worldInteractions.RemoveInteraction (_myInteraction);
				_myInteraction = null;
			} else if (_myInteraction is Give) {
                
			}
            else if (_myInteraction is Pickup)
            {
                _myInteraction = _owner._interactControl._myActiveInteraction;
            }
        }
	}
}