using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[CreateAssetMenu(menuName="Shepherd/World Interactions/Toggle Interaction")]
	public class ToggleInteract : WorldInteraction {

		public GameObject _objectToToggle;
		public CharacterStateController _source;
		private bool _isOpen = false;

		public override void Interact (CharacterStateController audience) {
			if ((audience._myBrain is InputController) == false) {
				return; //TODO: Temporary, but if you aren't the player, Ignore
			}
			_isOpen = !_isOpen;
			_objectToToggle.SetActive (_isOpen);
			_source._states._isInConversation = _isOpen;

			if (_isOpen) {
				ChangePromptToClose ();
			} else {
				ChangePromptToExamine ();
			}
			base.Interact (audience);
		}
	}
}