using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shepherd;

public class DamageCollider : MonoBehaviour {

	public CharacterStateController _owner;

	public void InitOwner(CharacterStateController cControl) {
		_owner = cControl;
		gameObject.layer = 9;
		gameObject.SetActive (false);
	}

	void OnTriggerEnter(Collider other) {
		if (_owner != null) {
			CharacterStateController otherCharacter = other.transform.GetComponentInParent<CharacterStateController> ();
			if (otherCharacter != null && otherCharacter != _owner) {

				otherCharacter.CheckForParry (other.transform, _owner);
				if (otherCharacter._states._isParried) {
					return;
				}
				if (otherCharacter._states._isBlocking) {
					Debug.Log ("block once");
					//otherCharacter._states._isBlocking = false;
				}

				//Debug.Log (_owner._biography._name + " hits " + otherCharacter._biography._name + "!");
				//if player hits enemy
				if (_owner._currentAction != null) {
					Shepherd.RuntimeWeapon byRTWeapon = (_owner._currentAction._isMirrored) ? _owner._invControl._currentLeftWeapon : _owner._invControl._currentRightWeapon;
					//eStates.TakeDamage ((AttackAction)_owner._currentActionSHEP._actions[_owner._actionControl._actionIndex-1]._actionDataObject, byWeaponSHEP._wInstance);
					otherCharacter.TakeDamage (_owner, byRTWeapon);
					otherCharacter._attackedBy = _owner;
				} else {
					otherCharacter.TakeDamage (_owner, null);
				}
			}
		}
	}
}