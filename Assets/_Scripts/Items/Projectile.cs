using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shepherd;

public class Projectile : MonoBehaviour {

	public float _horizontalSpeed = 15f;
	public float _verticalSpeed = 2f;

	public GameObject _explosionPrefab;

	public Shepherd.ProjectileSpell _parent;

	private CharacterStateController _owner;
	private GameObject _explosion;
	private Rigidbody _rigidbody;

	public float _decay = 5f;

	public void Init() {
		_rigidbody = GetComponent<Rigidbody> ();

		Vector3 targetForce = transform.forward * _horizontalSpeed;
		targetForce += transform.up * _verticalSpeed;
		_rigidbody.AddForce (targetForce, ForceMode.Impulse);
	}

	public void InitOwner(CharacterStateController cControl) {
		_owner = cControl;
		gameObject.layer = 9;
		//gameObject.SetActive (false);
	}

	void OnTriggerEnter(Collider other) {
		if (_owner != null) {
			CharacterStateController otherCharacter = other.transform.GetComponentInParent<CharacterStateController> ();
			if (otherCharacter != null && otherCharacter != _owner) {
				//Debug.Log (_owner._biography._name + " hits " + otherCharacter._biography._name + " with a spell!");
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
				otherCharacter._attackedBy = _owner;
			}
		} else {
			this.gameObject.GetComponent<BoxCollider> ().isTrigger = false;
		}

		//if it hits anything at all, it will go up in a puff of smoke
		GameObject gObject1 = Instantiate (_explosionPrefab, transform.position, transform.rotation) as GameObject;
		gObject1.name = "Projectile Explosion";
		gObject1.transform.SetParent (this.transform);
		_explosion = gObject1;
		Destroy (this.gameObject.GetComponent<BoxCollider> ());
		Destroy (this.gameObject.GetComponent<Rigidbody> ());
		Destroy (this.gameObject.GetComponentInChildren<ParticleSystem> ().gameObject);
		Destroy (this.gameObject.GetComponentInChildren<MeshFilter> ().gameObject);
	}

	void Update() {
		if (_decay <= 0f) {
			Destroy (_explosion);
			Destroy (this.gameObject);
		} else {
			_decay -= Time.deltaTime;
			if (this.gameObject.GetComponentInChildren<Light> () != null) {
				this.gameObject.GetComponentInChildren<Light> ().intensity -= Time.deltaTime / 2f;
			}
		}
	}
}