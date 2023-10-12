using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shepherd;

public class AnimationHook : MonoBehaviour {
	public float _rootMotionMultiplier;

	public bool _useInverseKinematics;
	public bool _killDelta;

	public AvatarIKGoal _currentHand;

	private HandleIK _handleIK;

	private Animator _animator;
	private AnimationCurve _rollCurve;

	private Rigidbody _rigidbody;

	private bool _rolling;
	[HideInInspector]
	public bool _leaping;

	private float _delta;
	private float _rollTime;

	private CharacterStateController _owner;

	public void Init(CharacterStateController character) {
		_owner = character;

		if (_owner != null) {
			_animator = character._animator;
			_rigidbody = character._rigidbody;
			_rollCurve = character._myControlStats._rollCurve;
			_delta = character._delta;
		}

		_handleIK = gameObject.GetComponent<HandleIK> ();
		if (_handleIK != null) {
			_handleIK.Init (_animator);
		}
	}

	public void OpenCanAttack() {
		if (_owner != null) {// && _states._currentAction._comboActionSteps.Count != 0) {
			_owner._states._isAbleToAttack = true;
		}
	}

	public void CloseCanAttack() {
		if (_owner != null) {
			_owner._states._isAbleToAttack = false;
		}
	}

	//TODO: rename this to something better
	public void OpenCanMove() {
		if (_owner != null) {
			_owner._states._isAbleToMove = true;
			//_owner._actionControl._actionIndex = 0;
		}
	}

	/*MAKES COLLIDERS ON WEAPONS WORK WHEN PLAYING AN ATTACK ANIMATION ONLY*/
	public void OpenDamageColliders() {
		if (_owner != null) {
			_owner._states._isDamageColliderOn = true;
			_owner._invControl.OpenAllDamageColliders ();
		}

		OpenParryFlag ();
	}

	public void CloseDamageColliders() {
		if (_owner != null) {
			_owner._states._isDamageColliderOn = false;
			_owner._invControl.CloseAllDamageColliders ();
		}

		CloseParryFlag ();
	}

	/*MAKES COLLIDERS ON WEAPONS WORK WHEN PARRYING AN ATTACK ANIMATION ONLY*/
	public void OpenParryCollider() {
		if (_owner != null) {
			_owner._invControl.OpenParryCollider ();
			_owner._states._isParrying = true;
			//_owner._states._isInvincible = true; //briefly between this point .. might need to change if getting backstabbed
		}
	}

	public void CloseParryCollider() {
		if (_owner != null) {
			_owner._invControl.CloseParryCollider();
			_owner._states._isParrying = false;
		}
	}

	public void OpenBlockCollider() {
		if (_owner != null) {
			_owner._invControl.OpenBlockCollider();
		}
	}

	public void CloseBlockCollider() {
		if (_owner != null) {
			_owner._invControl.CloseBlockCollider();
		}
	}

	public void OpenParryFlag() {
		if (_owner != null) {
			if (_owner._states._isAbleToBeParried) {
				_owner._states._isOpenToParry = true;
			}
		}
	}

	public void CloseParryFlag() {
		if (_owner != null) {
			_owner._states._isOpenToParry = false;
		}
	}

	//related to if we can rotate before an animation (or after, or during!)
	public void OpenRotationControl() {
		if (_owner != null) {
			_owner._states._isAbleToRotate = true;
		}
	}
	public void CloseRotationControl() {
		if (_owner != null) {
			_owner._states._isAbleToRotate = false;
		}
	}
		
	public void InitForRoll() {
		_rolling = true;
		_rollTime = 0f;
	}

	public void CloseRoll() {
		if (!_rolling) {
			return;
		}
		_rootMotionMultiplier = 1f;
		_rollTime = 0f;
		_rolling = false;
	}

	//related to spells -- not sure if needed anymore
	public void CloseParticle() {
		if (_owner != null) {
			//if (_owner._inventoryController._currentSpell != null) {
			//	_owner._inventoryController._currentSpell._currentParticle.SetActive (false);
			//}
		}
	}
	//...

	//TODO: rename...
	public void InitiateThrowForProjectile() {
		if (_owner != null) {
			//_owner.ThrowProjectile ();
			_owner.StartSpellCasting();
		}
	}


	public void InitIKForShield(bool isLeftHand) {
		_handleIK.UpdateIKTargets (((isLeftHand) ? IKSnapShotType.ShieldLeft : IKSnapShotType.ShieldRight), isLeftHand);
	}

	public void InitIKForBreathSpell(bool isLeftHand) {
		_handleIK.UpdateIKTargets (IKSnapShotType.Breath, isLeftHand);
	}


	public void LateTick() {
		if (_handleIK != null) {
			_handleIK.LateTick ();
		}
	}

	public void ConsumeCurrentItem() {
		if (_owner != null) {
			/*if (_owner._inventoryController._currentConsumableItem != null) {
				_owner._inventoryController._currentConsumableItem._itemCount--;
				ItemEffectsController._singleton.UseItemEffect (_owner._inventoryController._currentConsumableItem._instance._consumableEffect, _owner);
				UserInterfaceController._quickSlots.UpdateTextBox (QSlotType.itemPouchActive, _owner._inventoryController._currentConsumableItem._itemCount.ToString ());
			}*/
			if (_owner._invControl._currentConsumable != null) {
				
				Shepherd.Consumable currentInstance = (Shepherd.Consumable)_owner._invControl._currentConsumable._instance;
				currentInstance.SetEffectParent ();
				currentInstance._consumableEffect.UseItemEffect (_owner);

                if (!_owner._invControl._currentConsumable._unbreakable)
                {
                    _owner._invControl._currentConsumable._durability--;
                }
                _owner._states._isUsingItem = false; //Having consumed the item, you are no longer using it
            }
		}
	}

	void OnAnimatorMove() {
		if (_handleIK != null) {
			_handleIK.OnAnimatorMoveTick ((_currentHand == AvatarIKGoal.LeftHand));
		}
		if (_owner == null) {
			return;
		}
		if (_rigidbody == null) {
			return;
		}
		if (_leaping) {
			return;
		}

		if (_owner != null) {
			if (_owner._states._isAnimatorInAction) {
				//return;
				_delta = _owner._delta;
			} else {
				//_delta = _owner._delta;
				return;
			}
		}

		_rigidbody.drag = 0f;

		if (_rootMotionMultiplier == 0) {
			_rootMotionMultiplier = 1f;
		}

		if (!_rolling) {
			Vector3 delta = _animator.deltaPosition; //delta position, whereas _delta is delta time

			if (_killDelta) {
				_killDelta = false;
				delta = Vector3.zero;
			}

			Vector3 velocity = (delta * _rootMotionMultiplier) / _delta; //states._delta

			if (_owner != null && !_owner.CheckForGround()) {
				velocity.y = _rigidbody.velocity.y;
			}

			//velocity.y = Physics.gravity.y;

			if (_delta != 0) {
				_rigidbody.velocity = velocity;
			}
		} else {
			_rollTime += _delta / 0.6f; //TODO: HARD CODED VALUE!

			float zValue = _rollCurve.Evaluate (_rollTime);
			Vector3 velocity1 = Vector3.forward * zValue;
			Vector3 relativePosition = transform.TransformDirection (velocity1);
			Vector3 velocity2 = (relativePosition * _rootMotionMultiplier);// / _states._delta;

			//velocity2.y = Physics.gravity.y;

			if (!_owner.CheckForGround () && !_owner._states._isLeaping) {
				velocity2.y = _rigidbody.velocity.y;
			}

			if (_owner._myBrain is EnemyAIController) {
				EnemyAIController brain = (EnemyAIController)_owner._myBrain;
				brain._agent.velocity = velocity2;
			} else {
				_rigidbody.velocity = velocity2;
			}
		}
	}

	void OnAnimatorIK() {
		if (_handleIK == null) {
			return;
		}

		if (_useInverseKinematics) {
			_handleIK.IKTick (_currentHand, 1f);
		} else {
			if (_handleIK._weight > 0f) {
				_handleIK.IKTick (_currentHand, 0f);
			} else {
				_handleIK._weight = 0f;
			}
		}
	}
}