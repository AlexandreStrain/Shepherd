using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shepherd;

public class BlockCollider : MonoBehaviour {

	private CharacterStateController _owner;

	public void InitOwner (CharacterStateController cControl) {
		_owner = cControl;
	}
		
	void OnTriggerEnter(Collider other) {
		if (_owner != null) {
			//check if our attack was blocked by another character
			CharacterStateController otherCharacter = other.transform.GetComponentInParent<CharacterStateController> ();
			if (otherCharacter != null && otherCharacter != _owner) {
				if (otherCharacter._states._isDamageColliderOn) {
					
					if(!_owner._states._isInvincible && _owner._states._isBlocking) {
						Debug.Log ("Blocked!");
						_owner._body._stamina.Subtract (25f); //TODO: Hardcoded value!!
						_owner._states._isInvincible = true; //not sure why, probably to prevent a double hit...
					}

					if (otherCharacter._currentAction._isMirrored) {
						if (otherCharacter._invControl._currentLeftWeapon != null) {
							otherCharacter._invControl._currentLeftWeapon._durability--;
						}
					} else {
						if (otherCharacter._invControl._currentRightWeapon != null) {
							otherCharacter._invControl._currentRightWeapon._durability--;
						}
					}
				}
			}
			return;
		}
	}
}
