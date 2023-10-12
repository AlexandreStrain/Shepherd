using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shepherd;

public class ParryCollider : MonoBehaviour {

	private CharacterStateController _owner;

	public void InitOwner(CharacterStateController cControl) {
		_owner = cControl;
	}
		
	void OnTriggerEnter(Collider other) {
		if (_owner != null) {
			CharacterStateController otherCharacter = other.transform.GetComponentInParent<CharacterStateController> ();
			if (otherCharacter != null && otherCharacter != _owner) {
				if (otherCharacter._states._isAbleToBeParried && !otherCharacter._states._isInvincible) {
					otherCharacter.CheckForParry (transform.root, _owner);
				}
			}

			return;
		}
	}
}