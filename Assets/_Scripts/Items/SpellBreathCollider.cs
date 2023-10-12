using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shepherd;

public class SpellBreathCollider : MonoBehaviour {

	public Shepherd.BreathSpell _parent;

	private CharacterStateController _owner;

	public void InitOwner(CharacterStateController cControl) {
		_owner = cControl;
	}

	void OnTriggerStay(Collider other) {
		if (_owner != null) {
			CharacterStateController otherCharacter = other.transform.GetComponentInParent<CharacterStateController> ();
			if (otherCharacter != null && otherCharacter != _owner) {
				//if character hits target
				if (_owner._currentAction != null) {
					Shepherd.RuntimeWeapon byRTWeapon = (_owner._currentAction._isMirrored) ? _owner._invControl._currentLeftWeapon : _owner._invControl._currentRightWeapon;
					otherCharacter.TakeDamage (_owner, byRTWeapon);
				} else {
					otherCharacter.TakeDamage (_owner, null);
				}


				if (otherCharacter._body._immuneSystem.GetMeter () <= 0) {
					if (otherCharacter._currentAilment != null) {
						Destroy (otherCharacter._currentAilment); //for now... destroy the previous ailment
					}
					//TODO: CLEAN THIS UP
					GameObject gObject = Instantiate (_parent._effectPrefab, transform.position, transform.rotation) as GameObject;
					gObject.name = _parent._spellClass.ToString () + " Effect";
					gObject.transform.SetParent (otherCharacter._myTransform);
					gObject.SetActive (true);

					ParticleSystem.ShapeModule shape = gObject.GetComponentInChildren<ParticleSystem> ().shape;
					shape.skinnedMeshRenderer = otherCharacter._bodySkin;
					otherCharacter._currentAilment = gObject;

					_parent.UseSpellEffect (otherCharacter);
				}
			}
		}
	}
}