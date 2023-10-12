using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[System.Serializable]
	public class InteractionController {

		//public List<Shepherd.WorldInteraction> _interactions = new List<Shepherd.WorldInteraction>();
		public List<Shepherd.InteractionHook> _interactions = new List<Shepherd.InteractionHook>();
		public int _index; //might not be needed

		public GameObject _interactCollider;
		public Shepherd.WorldInteraction _myActiveInteraction;
		[HideInInspector]
		public InteractionHook _interactHook;

		//testing
		private float _interactRadius;
		private Transform _parent;
		private Interactions _wInteractions;
		private CharacterStateController _owner;

		public float _searchTimer; //in future private
		public float _searchDelay = 0.5f; //could be based off of perception

		public void Init(CharacterStateController sCtrl) {
			_interactRadius = sCtrl._myControlStats._pickupRadius;
			_parent = sCtrl._myTransform;
			_wInteractions = sCtrl._wRControl._worldInteractions;

			_owner = sCtrl;

			if (_interactCollider != null) {
				_interactHook = _interactCollider.GetComponentInChildren<InteractionHook> ();
				_interactHook.InitOwner (sCtrl);
				if (_myActiveInteraction != null) {
					_interactHook._myInteraction = _myActiveInteraction;
					_interactHook.Setup ();
					OpenInteractCollider ();
				} else {
					CloseInteractCollider ();
				}
			}
		}

		public void EnableActiveInteraction() {
			if (_myActiveInteraction != null) {
				_myActiveInteraction._enabled = true;
				OpenInteractCollider ();
			}
		}
		public void DisableActiveInteraction() {
			if (_myActiveInteraction != null) {
				_myActiveInteraction._enabled = false;
				CloseInteractCollider ();
			}
		}

		public void OpenInteractCollider() {
			_interactCollider.SetActive (true);
		}
		public void CloseInteractCollider() {
			_interactCollider.SetActive (false);
			_interactHook.Teardown ();
		}

		public WorldInteraction GetInteraction() {
			return _wInteractions.GetInteraction (_interactions [_index]._myInteraction.name);
			//return _interactions [_index]._myInteraction; //could this be Interactions GetInteraction to get this from dictionary instead?
		}

		public void Tick(float delta, List<Transform> thingsNearby) {
			if (_searchTimer < _searchDelay) {
				_searchTimer += delta;
				return;
			}
			_searchTimer = 0f;

			_interactions.Clear (); //reset available interactions

			//List<Transform> thingsNearby = _bodySenses.GetAudibleObjects ();
			for (int i = 0; i < thingsNearby.Count; i++) {
				try {
					InteractionHook interaction = thingsNearby [i].GetComponentInChildren<InteractionHook> ();
					if (interaction == this._interactHook) {
						continue; //ignore the interactions I give out
					}
					float distance = Vector3.Distance (thingsNearby [i].position, _parent.position);
					if (interaction != null && interaction._myInteraction != null) {
						if (!_interactions.Contains (interaction) && interaction._myInteraction._enabled && distance < _interactRadius) {
							//Debug.Log (interaction._myInteraction.name);
							_interactions.Add (interaction);
						}
					}
				} catch {
					continue;
				}
			}
		}
	}
}