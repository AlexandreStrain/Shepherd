using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shepherd;

public class WeaponHook : MonoBehaviour {

	public GameObject[] _damageColliders;

	public void OpenDamageColliders() {
		for (int i = 0; i < _damageColliders.Length; i++) {
			_damageColliders [i].SetActive (true);
		}
	}

	public void CloseDamageColliders() {
		for (int i = 0; i < _damageColliders.Length; i++) {
			_damageColliders [i].SetActive (false);
		}
	}

	public void InitDamageColliders(CharacterStateController sController) { //StateController
		for (int i = 0; i < _damageColliders.Length; i++) {
			//_damageColliders [i].GetComponent<DamageCollider> ().InitPlayer (sController);
			_damageColliders [i].GetComponent<DamageCollider> ().InitOwner (sController);
		}
	}
}