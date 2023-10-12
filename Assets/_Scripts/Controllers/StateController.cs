using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shepherd;

public class StateController : CharacterStateController {
	//test
	//if ambidextrous, this is rolls to see which hand the consumable item appears in when using
	private int _handRoll;

	[Header("Stats")]
	private float _kickTimer;

	private bool _blockAnimation; //decides what hand to block with
	private string _blockIdleAnimaton; //resets the hand blocking back to idle

	public override void Init() {
		_myTransform = this.transform;
		SetupAnimator ();

		
		//_body.Init ();
		base.Init ();

		_animatorHook.Init (this);

		gameObject.layer = 8;
		_ignoreLayers = ~(1 << 9) & ~(1 << 4); //1 bitshift 8
		_ignoreForGroundCheck = ~(1 << 9) & ~(1 << 4);//~(1 << 9 | 1 << 10);

		_animator.SetBool (AnimationStrings.IsOnGround, true); //_states._isGrounded (initialize as true to start)
		_states._isAnimatorInAction = false;
		_animator.SetBool (AnimationStrings.IsInAction, false);

		_actionControl.Init (this);
		_invControl.Init (_wRControl, this);
		_interactControl.Init (this);

		InitRagdoll ();

		if (_biography != null) {
            Biography newInstance = ScriptableObject.Instantiate (_biography);
			_biography = newInstance; //new to this instance

			if (_biography._currentBioCard == null) {
				BioCard newBioCard = ScriptableObject.Instantiate (_wRControl._bioCardTemplate);
                //newBioCard._nameOfCharacter = _biography._name;
				_biography._currentBioCard = newBioCard;
                _biography._currentBioCard._nameOfCharacter = _biography._name;
                _biography._currentBioCard._status = AllEnums.Status.Normal;
                _biography._currentBioCard._faction = _biography._faction;
                _biography._currentBioCard._gender = _biography._gender;
                _biography._currentBioCard._class = _biography._class;
                _biography._currentBioCard._origin = _biography._origin;
                _biography._currentBioCard._race = _biography._race;
            }
		}

		//_inventoryControl.SetupInventory ();
	}

	protected override void InitRagdoll() {
		Rigidbody[] rigs = GetComponentsInChildren<Rigidbody>();
		for (int i = 0; i < rigs.Length; i++) {
			if (rigs [i] != _rigidbody) {
				rigs [i].gameObject.layer = 10;

				rigs [i].isKinematic = true;
				_ragdollRigidbody.Add (rigs [i]);

				Collider collider = rigs [i].GetComponent<Collider> ();
				collider.isTrigger = true;
				_ragdollColliders.Add (collider);
			}
		}
	}

	public override void Tick(float delta) {
		_delta = delta;
		//test
		_body.Tick(delta);
		//...

		_states._isGrounded = CheckForGround ();
		_animator.SetBool (AnimationStrings.IsOnGround, _states._isGrounded);
		_states._isAnimatorInAction = _animator.GetBool (AnimationStrings.IsInAction);
		_states._isDelayed = _animator.GetBool (AnimationStrings.IsInteracting);

		if (_spellEffectLoop != null) {
			_spellEffectLoop ();
		}
		if (_itemEffectLoop != null) {
			_itemEffectLoop ();
		}

		switch (_currentActionStatus) {
		case AllEnums.CharacterActionState.Moving:
			//test
			HandleBlocking ();
			//..

			bool attack = CheckForAttackInput ();
			if (!attack) {
				_actionControl._actionIndex = 0;
				_animator.applyRootMotion = false;
			}
			break;
		case AllEnums.CharacterActionState.Airborne:
			//nothing here... yet
			break;
		case AllEnums.CharacterActionState.Interacting:
			if (_states._isSpellCasting && !_states._isUsingItem  && !_states._isGivingItem && !_states._isChangingItem) {
				if (_currentSpellAction != null && _spellCastTimer >= _currentSpellAction._castTime) {
					PlaySavedSpellAction ();
				} else {
					_spellCastTimer += _delta;
				}
				break;
			}

			if (!_states._isDelayed) {
				_currentActionStatus = AllEnums.CharacterActionState.Moving;
			}
			break;
		case AllEnums.CharacterActionState.OverrideActions:
			//_states._isAnimatorInAction = _animator.GetBool (AnimationStrings.IsInAction);
			if (_states._isAbleToAttack) {
				CheckForAttackInput ();
			}

			if (!_states._isAnimatorInAction) {
				_currentActionStatus = AllEnums.CharacterActionState.Moving;
			}
			return;
		}

		HandleInvincibilityFrames ();

		HandleEnableInverseKinematics ();

		_interactControl.Tick (_delta, _body.GetAudibleObjects());

		//if (_states._isAnimatorInAction) {
		//if (!_states._isAnimatorInAction) {
		if (!_states._isAnimatorInAction && !_states._isDelayed) {
			_states._isAbleToAttack = true;
			_states._isAbleToMove = true;
		} else if (!_states._isAnimatorInAction && _states._isDelayed) {
			_states._isAbleToAttack = false;
			_states._isAbleToMove = true;
		}

		//if below is true, then an animation is playing
		//if (!_states._isAnimatorInAction && !_states._isAbleToMove && !_states._isAbleToAttack) {
		if (_states._isAnimatorInAction && !_states._isAbleToAttack) {
			return;
		}

		/*if (_states._isAbleToMove && !_states._isAnimatorInAction) {
			if (_moveAmount > 0.3f) {
				_animator.SetBool (AnimationStrings.IsInAction, false);
				_animator.Play (AnimationStrings.StateEmptyOverride);
				_states._isAnimatorInAction = false;
			}
		}*/

		HandleKicking ();

		if (_states._isAbleToMove) {
			HandleItemAction ();
		}

		_animator.SetBool (AnimationStrings.IsLockOn, _states._isLockOn);

		if (!_states._isLockOn) {
			HandleMovementAnimations ();
		} else {
			HandleLockOnAnimations (_moveDirection);
		}
		_animatorHook._useInverseKinematics = _states._isIKEnabled;

		HandleSpellcasting();

		HandleRollingAnimations ();
	}

	public override void FixedTick(float delta) {
		_delta = delta;

		//_states._isBlocking = false; //???
		HandleWeaponModelVisibility (); //not sure this goes here..
			

		_states._isAnimatorInAction = _animator.GetBool (AnimationStrings.IsInAction);

		//if true, then you are playing an animation but not yet committed to the combo or attack
		//so you can aim your attack a bit before it goes off!!
		if (_states._isAbleToRotate) {
			HandleRotation ();
		}

		if (_states._isParried) {
			_states._isParried = _states._isAnimatorInAction; //_attackedBy != null && _states._isAnimatorInAction;
		} else if (_states._isBackstabbed) {
			_states._isBackstabbed = _states._isAnimatorInAction; //_attackedBy != null && _states._isAnimatorInAction;
		} else if (_attackedBy != null && (!_states._isParried && !_states._isBackstabbed)) {
			//_attackedBy = null;
		} 

		/*if (_attackedBy != null) {
			Debug.Log ("True");
			if (_interactControl._myActiveInteraction is Conversation) {
				Debug.Log ("VERY TRUE");
				DialogueController._singleton.CloseDialogue ();
				DialogueController._singleton.GetComponent<AudioSource> ().Stop ();
			}
		}*/

		if (_states._isGrounded) {
			HandleRotation ();
			HandleGroundedMovement ();
		} else {
			HandleAirborneMovement ();
		}
	}

	protected override void HandleActions (ActionContainer aContainer) {
		_currentAction = aContainer;
		AllEnums.ActionType aType = aContainer._actions [_actionControl._actionIndex]._actionType;

		switch (aType) {
		case AllEnums.ActionType.Attack:
			AttackAction attackAction = (AttackAction)aContainer._actions [_actionControl._actionIndex]._actionDataObject;
			HandleAttackAction (aContainer, attackAction);
			_currentSpellAction = null;
			break;
		case AllEnums.ActionType.Block:
			AttackAction blockAction = (AttackAction)aContainer._actions [_actionControl._actionIndex]._actionDataObject;
			HandleBlockAction (blockAction, aContainer._isMirrored);
			_currentSpellAction = null;
			break;
		case AllEnums.ActionType.Spell:
			Shepherd.SpellAction spellAction = (Shepherd.SpellAction)aContainer._actions [_actionControl._actionIndex]._actionDataObject;
			HandleSpellAction (aContainer, spellAction);
			break;
		case AllEnums.ActionType.Parry:
			AttackAction parryAction = (AttackAction)aContainer._actions[_actionControl._actionIndex]._actionDataObject;
			HandleParryAction(parryAction, aContainer._isMirrored);
			_currentSpellAction = null;
			break;
		}
	}

	protected override void HandleAttackAction(ActionContainer aContainer, Shepherd.AttackAction aAction) {
		if (CheckForParryTarget (aAction, aContainer._isMirrored)) {
			return;
		}
		if (CheckForBackstabTarget (aAction, aContainer._isMirrored)) {
			return;
		}

		if (_body._stamina.GetMeter () > aAction._staminaCost) {
			_body._stamina.Subtract (aAction._staminaCost);
			//other cost
			PlayAnimation (aAction._animation._variable, aContainer._isMirrored);
			_currentActionStatus = AllEnums.CharacterActionState.OverrideActions;
			_animator.applyRootMotion = true;
			_animator.SetBool (AnimationStrings.IsInAction, true);
			_actionControl._actionIndex++;
		}
	}
		
	protected override void HandleSpellAction(ActionContainer aContainer, Shepherd.SpellAction sAction) {
		if (_states._isAnimatorInAction || _states._isSpellCasting || _states._isBlocking || _states._isCrouching || _states._isGivingItem) {
			return;
		}

		if (sAction.InitSpell (this)) {
			string targetAnimation = sAction._chargeAnimation._variable;
			targetAnimation += (aContainer._isMirrored) ? AnimationStrings.LeftHandSuffix : AnimationStrings.RightHandSuffix;
			Shepherd.RuntimeWeapon rtWeapon = (aContainer._isMirrored) ? _invControl._currentLeftWeapon : _invControl._currentRightWeapon;
			rtWeapon._durability--;
			_animator.SetBool (AnimationStrings.IsMirrored, aContainer._isMirrored);

			_states._isSpellCasting = true;
			_animator.SetBool (AnimationStrings.IsSpellCasting, true);
			if (sAction._changeAnimationSpeed) {
				_animator.SetFloat (AnimationStrings.AnimationSpeed, sAction._animationSpeed);
			}
			_animator.CrossFade (targetAnimation, 0.1f);
			_states._isDelayed = true;
			_currentSpellAction = sAction;

			_currentActionStatus = AllEnums.CharacterActionState.Interacting;

			_spellCastTimer = 0f;
		} else {
			_invControl.CloseSpellBreathCollider ();
			EmptySpellCastDelegates ();
			_states._isSpellCasting = false;
			_animator.SetBool (AnimationStrings.IsSpellCasting, false);
			PlayAnimation (AnimationStrings.StateCannotDoSpell, aContainer._isMirrored);
		}
	}

	//rename!!!
	private void PlaySavedSpellAction() {
		if (_currentSpellAction._castAnimation != null) {
			_animator.SetBool (AnimationStrings.IsSpellCasting, false);
			PlayAnimation (_currentSpellAction._castAnimation._variable, _currentAction._isMirrored);
			_currentActionStatus = AllEnums.CharacterActionState.OverrideActions;
		} else {
			StartSpellCasting ();
		}
	}

	//rename?
	public override void StartSpellCasting() {
		if (_currentSpellAction is ProjectileSpell) {
			ProjectileSpell proj = (ProjectileSpell)_currentSpellAction;
			proj.CastSpell (this);
		} else if (_currentSpellAction is BreathSpell) {
			BreathSpell breath = (BreathSpell)_currentSpellAction;
			_animatorHook.InitIKForBreathSpell (_currentAction._isMirrored);
			_animatorHook._currentHand = (_currentAction._isMirrored) ? AvatarIKGoal.LeftHand : AvatarIKGoal.RightHand;
			if (_spellCastStart == null && _spellCastLoop == null && _spellCastStop == null) {
				breath.CastSpell (this);
			}
		}
	}

	protected override void HandleBlockAction(AttackAction bAction, bool isMirrored) {
		if (_states._isAnimatorInAction) {
			return;
		}
		_states._isBlocking = true;
		_states._isIKEnabled = true;
		_animatorHook._currentHand = (isMirrored) ? AvatarIKGoal.LeftHand : AvatarIKGoal.RightHand;
		_animatorHook.InitIKForShield (isMirrored);

		_animator.SetBool (AnimationStrings.IsMirrored, isMirrored);

		//test
		Shepherd.Weapon test = _actionControl.GetWeaponFromAction (isMirrored);

		if (!_blockAnimation) {
			//_blockIdleAnimaton = (_states._isTwoHanding) ? bAction._twoHandIdleAnimation._variable : bAction._oneHandIdleAnimation._variable;
			_blockIdleAnimaton = (_states._isTwoHanding) ? test._oneHandIdle._variable : test._twoHandIdle._variable;

			_blockIdleAnimaton += (isMirrored) ? AnimationStrings.LeftHandSuffix : AnimationStrings.RightHandSuffix;

			string targetAnimation = bAction._animation._variable;
			targetAnimation += (isMirrored) ? AnimationStrings.LeftHandSuffix : AnimationStrings.RightHandSuffix;
			_animator.CrossFade (targetAnimation, 0.1f); //hardcoded animation!!
			_blockAnimation = true;

			//test
			_animatorHook.OpenBlockCollider();
		}
	}

	protected override void HandleParryAction(AttackAction aAction, bool isMirrored) {
		if (_body._stamina.GetMeter () < aAction._staminaCost || _states._isCrouching || _states._isLeaping || _states._isGivingItem) {
			return;
		}

		string targetAnimation = null;
		targetAnimation = aAction._animation._variable;

		if (string.IsNullOrEmpty (targetAnimation)) {
			return;
		}

		//not sure what this does
		_states._isAbleToBeParried = aAction._vulnerableToParry;

		float attackSpeed = 1f;
		if (aAction._changeAnimationSpeed) {
			attackSpeed = aAction._animationSpeed;
			if (attackSpeed == 0) {
				attackSpeed = 1f;
			}
		}

		_animator.SetFloat (AnimationStrings.AnimationSpeed, attackSpeed);
		PlayAnimation (targetAnimation, isMirrored);

		_body._stamina.Subtract (aAction._staminaCost);
	}

	protected override void HandleInvincibilityFrames() {
		if (_states._isInvincible) {
			_invincibilityTimer += _delta;
			_invControl.CloseAllDamageColliders ();
			if (_invincibilityTimer > 0.5f) { //TODO: Hardcoded value!!
				_states._isInvincible = false;
				_invincibilityTimer = 0f;
			}
		}
	}

	protected override void HandleWeaponModelVisibility() {
		if (_states._closeWeapons) {
			_states._closeWeapons = _animator.GetBool (AnimationStrings.IsInAction) && !_states._isDelayed;

			if (_invControl._currentRightWeapon != null) {
				_invControl._currentRightWeapon._rtModel.SetActive (false);
			}
			if (_invControl._currentLeftWeapon != null) {
				_invControl._currentLeftWeapon._rtModel.SetActive (false);
			}
			if (_invControl._currentConsumable != null) {
				if (_invControl._currentConsumable._rtModel != null) {
					_invControl._currentConsumable._rtModel.SetActive (false);
				}
			}
			return;
		}

		if (!_states._isDelayed) {
			if (_states._isUsingItem) {
				//Debug.Log ("Delay: " + _states._isDelayed);
				_states._isUsingItem = _animator.GetBool (AnimationStrings.IsInteracting) && _animator.GetBool (AnimationStrings.IsUsingItem);// _states._isDelayed;
				//Debug.Log ("Using Item: " + _states._isUsingItem);
			}

			if (_states._isChangingItem) {
				_states._isChangingItem = _animator.GetBool (AnimationStrings.IsInteracting) || _states._isDelayed;
			}
			//return; //?
		}

		if (_states._isChangingItem && !_states._isUsingItem && !_states._isGivingItem) {
			if (_invControl._currentRightWeapon != null && _input._dRightPress) {
				_invControl._currentRightWeapon._rtModel.SetActive (false);
			}
			if (_invControl._currentLeftWeapon != null && _input._dLeftPress) {
				_invControl._currentLeftWeapon._rtModel.SetActive (false);
			}
				
			if (_invControl._currentConsumable != null) {
				if (_invControl._currentConsumable._rtModel != null) {
					_invControl._currentConsumable._rtModel.SetActive (false);
				}
			}
		} else if (!_states._isChangingItem && _states._isUsingItem && !_states._isGivingItem) {
			if (_invControl._currentLeftWeapon != null && _biography._mainHand == AllEnums.PreferredHand.Left) {
				_invControl._currentLeftWeapon._rtModel.SetActive (!_states._isUsingItem);
			} else if (_invControl._currentRightWeapon != null && _biography._mainHand == AllEnums.PreferredHand.Right) {
				_invControl._currentRightWeapon._rtModel.SetActive (!_states._isUsingItem);
			} else if (_biography._mainHand == AllEnums.PreferredHand.Ambidextrous) {
                _handRoll = Random.Range(0, 100);
				if (_handRoll > 49) {
					if (_invControl._currentRightWeapon != null) {
						_invControl._currentRightWeapon._rtModel.SetActive (!_states._isUsingItem);
					}
				} else {
					if (_invControl._currentLeftWeapon != null) {
						_invControl._currentLeftWeapon._rtModel.SetActive (!_states._isUsingItem);
					}
				}
			}

			if (_invControl._currentConsumable != null) {
				if (_invControl._currentConsumable._rtModel != null) {
					_invControl._currentConsumable._rtModel.SetActive (_states._isUsingItem);//(_animator.GetBool (AnimationStrings.IsInteracting));
				}
			}
		} else if (!_states._isChangingItem && !_states._isUsingItem && _states._isGivingItem) {
			_interactControl.OpenInteractCollider ();
			if (_invControl._currentLeftWeapon != null && _biography._mainHand == AllEnums.PreferredHand.Left) {
				_invControl._currentLeftWeapon._rtModel.SetActive (!_states._isGivingItem);
			} else if (_invControl._currentRightWeapon != null && _biography._mainHand == AllEnums.PreferredHand.Right) {
				_invControl._currentRightWeapon._rtModel.SetActive (!_states._isGivingItem);
			} else if (_biography._mainHand == AllEnums.PreferredHand.Ambidextrous) {
                _handRoll = Random.Range(0, 100);
                if (_handRoll > 49) {
					if (_invControl._currentRightWeapon != null) {
						_invControl._currentRightWeapon._rtModel.SetActive (!_states._isGivingItem);
					}
				} else {
					if (_invControl._currentLeftWeapon != null) {
						_invControl._currentLeftWeapon._rtModel.SetActive (!_states._isGivingItem);
					}
				}
			}

			if (_invControl._currentConsumable != null) {
				if (_invControl._currentConsumable._rtModel != null) {
					_invControl._currentConsumable._rtModel.SetActive (_animator.GetBool (AnimationStrings.IsInteracting));
				}
			}
		} else {
			if (_interactControl._myActiveInteraction == null || _interactControl._myActiveInteraction._enabled == false) {
				_interactControl.CloseInteractCollider ();
			}
			if (_invControl._currentRightWeapon._rtModel != null && !_input._dRightPress && !_input._dLeftHold) {
				_invControl._currentRightWeapon._rtModel.SetActive (true);
			}
			if (_invControl._currentLeftWeapon._rtModel != null && !_input._dLeftPress && !_input._dRightHold) {
				_invControl._currentLeftWeapon._rtModel.SetActive (true);
			}
				
			if (_invControl._currentConsumable != null) {
				if (_invControl._currentConsumable._rtModel != null) {
					_invControl._currentConsumable._rtModel.SetActive (false);
				}
			}
		}
	}

	private void HandleEnableInverseKinematics() {
		if (_states._isBlocking == false && _states._isSpellCasting == false) {
			_states._isIKEnabled = false;
			_animatorHook._useInverseKinematics = false;
		}
	}

	//UPDATE HUD AND BODY CONDITION
	public override void MonitorStats() {
        UpdateStatistics();
		if (_body._health.GetMeter () <= 0) {
			if (!_states._isDead) {
				_states._isDead = true;
				HandleDeath ();
				return;
			}
		}

		if (_attackedBy != null)
        {
            _body._underDuress = true;
        } else
        {
            _body._underDuress = false;
        }
			
		if (_body._immuneSystem.GetMeter () >= _body._immuneSystem.GetMaximum()) {
			_spellEffectLoop = null;
			if (_currentAilment != null) {
				Destroy (_currentAilment);
				_currentAilment = null;
			}
		}
			
		if (_states._isAbleToMove && _states._isRunning && !_body._stamina.IsEmpty ()) {
			if(!_states._isUsingItem && !_states._isGivingItem && _states._isGrounded && _moveAmount > 0f) {
				_body._stamina.Subtract (_body._stamina._decay + _delta);
			}
		} else {
			_states._isRunning = false;
			_animator.SetBool (AnimationStrings.IsRunning, _states._isRunning);
		}

		if (_states._isSpellCasting || _states._isRolling || !_states._isGrounded || _states._isBlocking || _states._isAnimatorInAction) {
			return;
		}
		_body._stamina.Regen ();

		//test
		//HARD CODED VALUES ---- SUBJECT TO CHANGE
		if (_body._hunger.GetMeter () < 50f) {
			if (!_body._myNeeds.Contains(_body._hunger)) {
				_body._myNeeds.Enqueue (_body._hunger);
				_biography._currentBioCard._status = AllEnums.Status.Hungry;
			}
		}

		if (_body._thirst.GetMeter () < 50f) {
			if (!_body._myNeeds.Contains(_body._thirst)) {
				_body._myNeeds.Enqueue (_body._thirst);
				_biography._currentBioCard._status = AllEnums.Status.Thirsty;
			}
		}

		if (_body._sleep.GetMeter () < 50f) {
			if (!_body._myNeeds.Contains(_body._sleep)) {
				_body._myNeeds.Enqueue (_body._sleep);
				_biography._currentBioCard._status = AllEnums.Status.Sleeping;
			}
		}

		if (_body._waste.GetMeter () >= _body._waste.GetMaximum()) {
			if (!_body._myNeeds.Contains(_body._waste)) {
				_body._myNeeds.Enqueue (_body._waste);
				_biography._currentBioCard._status = AllEnums.Status.Waste;
			}
		}

		if (_body._pleasure.GetMeter () < 50f) {
			if (!_body._myNeeds.Contains(_body._pleasure)) {
				_body._myNeeds.Enqueue (_body._pleasure);
				_biography._currentBioCard._status = AllEnums.Status.Unhappy;
			}
		}
		//...
	}

    private void UpdateStatistics()
    {
        _biography._currentBioCard._currentStatistics._thirst = _body._thirst.GetMeter();
        _biography._currentBioCard._currentStatistics._hunger = _body._hunger.GetMeter();
        _biography._currentBioCard._currentStatistics._waste = _body._waste.GetMeter();
        _biography._currentBioCard._currentStatistics._sleep = _body._sleep.GetMeter();
        _biography._currentBioCard._currentStatistics._pleasure = _body._pleasure.GetMeter();

        _biography._currentBioCard._currentStatistics._health = _body._health.GetMeter();
        _biography._currentBioCard._currentStatistics._stamina = _body._stamina.GetMeter();
        _biography._currentBioCard._currentStatistics._baseCourage = _body._courage.GetMeter();
        _biography._currentBioCard._currentStatistics._immuneSystem = _body._immuneSystem.GetMeter();
        _biography._currentBioCard._currentStatistics._poise = _body._poise.GetMeter();
        _biography._currentBioCard._currentStatistics._carryWeight = _body._carryWeight.GetMeter();
    }
	//.....

	public override void TakeDamage (CharacterStateController byCharacter, RuntimeWeapon byWeapon) {
		//Debug.Log ("Ouch");
		if (!_states._isInvincible) {
			if (byWeapon == null || byCharacter._currentAction._actions.Length == 0) {
				_body._health.Subtract (_body._health.GetMaximum());
				Debug.Log ("No weapon to take damage from!");
				return;
			}

			Object byWeaponAction;
			if (byCharacter._currentAction._actions.Length == 1) {
				byWeaponAction = byCharacter._currentAction._actions [0]._actionDataObject;
			} else {
				byWeaponAction = byCharacter._currentAction._actions [byCharacter._actionControl._actionIndex - 1]._actionDataObject;
			}
				
			if (byWeaponAction is AttackAction && _currentSpellAction == null) {
				AttackAction aAction = (AttackAction)byCharacter._currentAction._actions [byCharacter._actionControl._actionIndex - 1]._actionDataObject;
				if (_states._isAbleToMove) { // || _body._poise.GetMeter() > 100f) {
					if (aAction._overrideDamageAnimation) {
						_animator.Play (aAction._damageAnimation._variable);
					} else {
						//TODO: hard coded value!!
						int randomInteger = Random.Range (0, 100);
						string targetAnimation = (randomInteger > 50) ? AnimationStrings.StateGetHit01 : AnimationStrings.StateGetHit02;
						_animator.Play (targetAnimation);
					}
				}
			} else {
				int randomInteger = Random.Range (0, 100);
				string targetAnimation = (randomInteger > 50) ? AnimationStrings.StateGetHit01 : AnimationStrings.StateGetHit03;
				_animator.Play (targetAnimation);
			}
				
			float damageReceived;
			if (byCharacter._states._isTwoHanding) {
				damageReceived = StatsCalculator.CalculateBaseDamage (byWeapon._instance._itemStats, _biography._currentBioCard._currentStatistics, 1.5f);
			} else {
				damageReceived = StatsCalculator.CalculateBaseDamage (byWeapon._instance._itemStats, _biography._currentBioCard._currentStatistics);
			}
			//Debug.Log(damageReceived);
			if (byCharacter._currentSpellAction == null) {
				_body._health.Subtract (damageReceived);
			} else {
				byCharacter._currentSpellAction.HandleSpellType (this, damageReceived);
			}

			float immuneSystemDamage = StatsCalculator.CalculateImmunityDamage (byWeapon._instance._itemStats, _biography._currentBioCard._currentStatistics);
			_body._immuneSystem.Subtract (immuneSystemDamage);

			byWeapon._durability--;

			//subject to change (poise damage, if damage recieved knocks me off balance)
			//_body._poise.Add(damageReceived);
			//Debug.Log("Damage Dealt to " + this + ": " + damageReceived + "\nPoise is at: " + _body._poise.GetMeter());

			_states._isInvincible = true;
			_states._isDelayed = true;
			_states._isAbleToMove = false;
			_states._isAnimatorInAction = true; //false
			_animator.applyRootMotion = true;
			_animator.SetBool (AnimationStrings.IsInAction, true);
		}
	}

	public void RecieveDamage(EnemyAIAttacks fromAttack, WorldItem byWeapon) {
		if (!_states._isInvincible) {
			//float damageReceived = StatsCalculator.CalculateBaseDamage(byWeapon._weaponStats, _body);
			float damageReceived;
			if(byWeapon != null) {
				damageReceived = StatsCalculator.CalculateBaseDamage(byWeapon._itemStats, _biography._currentBioCard._currentStatistics);
			} else {
				damageReceived = 5f;
			}
			//_currentHealth -= damageReceived;
			_body._health.Subtract(damageReceived);


			//subject to change (poise damage, if damage recieved knocks me off balance)
			//_body._poise.Add(damageReceived);

			string targetAnimation;

			//if (_isMoving) { // || _body._poise.GetMeter() > 100f) {
			//if (fromAttack._hasReactAnimation) {
			//	targetAnimation = fromAttack._reactAnimation;
			//} else {
				//hard coded value!!
				int randomInteger = Random.Range (0, 100);
				targetAnimation = (randomInteger > 50) ? AnimationStrings.StateGetHit01 : AnimationStrings.StateGetHit02;
				//_animator.Play (targetAnimation);
			//}
			//}
			//...
			//Debug.Log("Damage Dealt to " + this + ": " + damageReceived + "\nPoise is at: " + _body._poise.GetMeter());
			_animator.applyRootMotion = true;

			_states._isInvincible = true;

			PlayAnimation (targetAnimation, false);
		}
	}

	public void HandleItemAction() {
		if (_states._isAnimatorInAction || _states._isDelayed || _states._isChangingItem) {
			return;
		}
			
		//test
		RuntimeConsumable cSlot = _invControl._currentConsumable;
		Consumable consumable = (Consumable)_invControl._currentConsumable._instance;

		if (cSlot == null) {
			return;
		}

		if (cSlot._durability < 1 && !cSlot._unbreakable) {
			Debug.Log ("Last One, so I'm destroying it: " + cSlot._instance._itemName);
			_invControl.UnequipConsumable (cSlot);
			return;
		}

        //Check if animation needs to be mirrored (mirrored meaning in left hand)
        bool isMirrored = false;
        if (cSlot._equippedHand == AllEnums.PreferredHand.Left)
        {
            isMirrored = true;
        }
        else if (cSlot._equippedHand == AllEnums.PreferredHand.Ambidextrous)
        {
            Debug.Log("WARNING -- THIS SHOULD NOT BE HAPPENING");
        }


        if (_states._isGivingItem && !_states._isUsingItem && !_animator.GetBool(AnimationStrings.IsGivingItem)) {
            Debug.Log(_biography._name + " is Giving Item..." + _invControl._currentConsumable._instance._itemName);
            if(_wRControl.CompareWorldItem(_invControl._currentConsumable._instance._itemName, _invControl._emptyItemsDefault._itemName))
            {
                if (_invControl._testing._isPlayer)
                {
                    return; //don't hand over your hands
                }
            }

			
			Give giveInteract = (Give)_wRControl._worldInteractions.GetInteraction(_biography._name + "'s Offering");
			if (giveInteract == null) {
				Debug.Log ("WHOOPS -- Didn't create a give interaction for current entity");
                return;
			} else {
               
                giveInteract._enabled = true;
                giveInteract._itemToGive = _invControl._currentConsumable;
                giveInteract._source = this;

				_interactControl._myActiveInteraction = giveInteract;
				_interactControl._interactHook._myInteraction = giveInteract;
				_interactControl._interactHook.Setup ();

				_states._isUsingItem = false;
				_states._isGivingItem = true;
				_states._isChangingItem = false;
			}

		} else if (!_states._isGivingItem && _states._isUsingItem && !_animator.GetBool(AnimationStrings.IsUsingItem)) {
			Debug.Log ("Using Item... " + _biography._name);
			string targetAnimation = "";
			if (consumable._useAnimation != null) {
				targetAnimation = consumable._useAnimation._variable;
				if (string.IsNullOrEmpty (targetAnimation)) {
					PlayAnimation ("well", isMirrored);
					return;
				}
			} else {
				PlayAnimation ("well", isMirrored);
				return;
			}

			if(_biography._mainHand == AllEnums.PreferredHand.Ambidextrous) {
				_invControl.EquipConsumable (cSlot, AllEnums.PreferredHand.Ambidextrous);
			}

			_animator.SetBool (AnimationStrings.IsUsingItem, true);

			//_states._isUsingItem = true;
			_states._isGivingItem = false;
			_states._isChangingItem = false;
			PlayInteractAnimation (targetAnimation, isMirrored);
			_currentActionStatus = AllEnums.CharacterActionState.Interacting;
		}
	}
		
	public override void HandleTwoHanded() {
		if (_input._dRightHold || _input._dLeftHold) {
			if (!_states._isTwoHanding) {
				_states._isTwoHanding = true;
				_states._isDelayed = true;
				if (_input._dRightHold) {
					_animator.SetBool (AnimationStrings.IsMirrored, false);
					//PlayAnimation (AnimationStrings.StateChangeItem, false);
					PlayInteractAnimation (AnimationStrings.StateChangeItem, false);
				} else {
					_animator.SetBool (AnimationStrings.IsMirrored, true);
					//PlayAnimation (AnimationStrings.StateChangeItem, true);
					PlayInteractAnimation (AnimationStrings.StateChangeItem, true);
				}

				_currentActionStatus = AllEnums.CharacterActionState.Interacting;
				_actionControl.UpdateActionsWithCurrentItems ();
			}
		} else {
			_states._isTwoHanding = false;
			_states._isDelayed = true;
			if (_input._dRightPress) {
				_animator.SetBool (AnimationStrings.IsMirrored, false);
				PlayInteractAnimation (AnimationStrings.StateChangeWeapon, false);
				_animator.CrossFade(AnimationStrings.StateEmptyBothHands, 0.1f);
			} else {
				_animator.SetBool (AnimationStrings.IsMirrored, true);
				PlayInteractAnimation (AnimationStrings.StateChangeWeapon, true);
				_animator.CrossFade(AnimationStrings.StateEmptyBothHands, 0.1f);
			}

			_currentActionStatus = AllEnums.CharacterActionState.Interacting;
			_actionControl.UpdateActionsWithCurrentItems ();
		}
	}

	//TODO: Both Functions below are similar... we could make them into one function!
	public override void HandleGettingParried(AttackAction byWeaponAction, Weapon byWeapon) {
		float damageReceived = StatsCalculator.CalculateBaseDamage(byWeapon._itemStats, _biography._currentBioCard._currentStatistics);

		_body._health.Subtract(damageReceived);
		_states._isParried = true;
		_states._isDelayed = true; //not sure if needed...
		_states._isInvincible = true;
		_animator.applyRootMotion = true;
		PlayAnimation (AnimationStrings.StateGetParried, false);
	}
	public override void HandleGettingBackstabbed(AttackAction byWeaponAction, Weapon byWeapon) {
		float damageReceived = StatsCalculator.CalculateBaseDamage(byWeapon._itemStats, _biography._currentBioCard._currentStatistics);// byWeapon._backstabMultiplier);

		_body._health.Subtract(damageReceived);
		_states._isBackstabbed = true;
		_states._isDelayed = true; //again, not sure if needed
		_states._isInvincible = true;
		_animator.applyRootMotion = true;
		PlayAnimation (AnimationStrings.StateGetBackstabbed, false);
	}

	//TESTING...
	protected override void HandleGroundedMovement() {
		CheckForJumping ();

		//if below is true, then an animation is playing
		//if (!_states._isAnimatorInAction && !_states._isAbleToMove && !_states._isAbleToAttack) {
		if (_states._isAnimatorInAction && !_states._isAbleToAttack) {
			return;
		}
			
		_animator.applyRootMotion = false;

		if (_states._isUsingItem || _states._isGivingItem || _states._isSpellCasting || _states._isChangingItem) {
			_states._isRunning = false;
			_moveAmount = Mathf.Clamp (_moveAmount, 0f, 0.5f); //reduces movement while using item
		}

        ScaleCapsuleForCrouching();

		float targetSpeed;
		if (_states._isRunning && !_body._stamina.IsEmpty()) {
			targetSpeed = _myControlStats._runSpeed;
			_states._isLockOn = false;
		} else if (_states._isCrouching) {
			targetSpeed = _myControlStats._crouchSpeed;
		} else {
			targetSpeed = _myControlStats._moveSpeed;
		}
			
		if (_states._isGrounded && _states._isAbleToMove) {
			//_turnAmount = Mathf.Atan2(_moveDirection.x, _moveDirection.z);
			_rigidbody.velocity = _moveDirection * (targetSpeed * _moveAmount);
		}
	}

	//test
	private void ScaleCapsuleForCrouching() {
		if (_states._isGrounded && _states._isCrouching) {
			if (_controlColliderCrouch) {
				_states._isAbleToAttack = false;
				return;
			}
			_controllerCollider.height = _controllerCollider.height / 1.5f; //2f
			_controllerCollider.center = _controllerCollider.center / 1.5f; //2f
			_controlColliderCrouch = true;
		} else {
            return;
			Ray crouchRay = new Ray (_rigidbody.position + Vector3.up * _controllerCollider.radius * 0.5f, Vector3.up);
			float crouchRayLength = _colliderHeight - _controllerCollider.radius * 0.5f;
			if (Physics.SphereCast (crouchRay, _controllerCollider.radius * 0.5f, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore)) {
				_states._isCrouching = true;
				return;
			}
			_controllerCollider.height = _colliderHeight;
			_controllerCollider.center = _colliderCentre;
			_controlColliderCrouch = false;
		}
	}
	//...

	private void CheckForJumping() {
		if (_input._jumpButton && !_states._isCrouching && _animator.GetCurrentAnimatorStateInfo (0).IsName (AnimationStrings.LocomotionRunning)) {
			if (_body._stamina.GetMeter () < _myControlStats._jumpLeapStaminaCost) {
				return;
			}
			//leap!
			_rigidbody.velocity = new Vector3 (_rigidbody.velocity.x, _myControlStats._jumpPower, (_rigidbody.velocity.z * 1.5f));
			_states._isGrounded = false;
			PlayAnimation (AnimationStrings.StateLeap, false);
			_animatorHook._leaping = true;
			_states._isLeaping = true;
			_body._stamina.Subtract (_myControlStats._jumpLeapStaminaCost);
			_groundCheckDistance = 0.3f; //change
		} else if (_input._jumpButton && !_states._isCrouching && _animator.GetCurrentAnimatorStateInfo (0).IsName (AnimationStrings.LocomotionNormal)) {
			if (_body._stamina.GetMeter () < _myControlStats._jumpLeapStaminaCost) {
				return;
			}
			//jump!
			if (!_states._isAbleToAttack) {
				_rigidbody.velocity = new Vector3 (_rigidbody.velocity.x, _myControlStats._jumpPower, (_rigidbody.velocity.z*1.45f));
			} else {
				_rigidbody.velocity = new Vector3 (_rigidbody.velocity.x, _myControlStats._jumpPower, _rigidbody.velocity.z);
			}
			_states._isGrounded = false;
			_states._isJumping = true;
			_body._stamina.Subtract (_myControlStats._jumpLeapStaminaCost);
			_groundCheckDistance = 0.3f;
		}  else {
			_states._isJumping = false;
			if (_airTimer < 0.6f && !_states._isLeaping) {
				_airTimer = 0f;
			} else {
				HandleLanding ();
			}
		}
	}

	protected override void HandleLanding() {
		//second half eliminates the ability to spam jump and roll once meter is below stamina cost for roll ---v
		if (_moveAmount == 0 || (_body._stamina.GetMeter() < _body._stamina.GetMaximum() * _myControlStats._dodgeStaminaCostPercentage)) {
			PlayAnimation (AnimationStrings.StateLand, false);
		} else {
			if (_moveDirection == Vector3.zero) {
				_moveDirection = transform.forward;
			}
			_states._isRolling = true;
			Quaternion targetRotation = Quaternion.LookRotation (_moveDirection);
			transform.rotation = targetRotation;
		}
		_states._isAnimatorInAction = false;
		_animator.SetBool (AnimationStrings.IsInAction, false);
		_states._isBlocking = false;
		_animatorHook._leaping = false;
		_states._isLeaping = false;
		_airTimer = 0f;
	}

	protected override void HandleAirborneMovement() {
		_airTimer += _delta;

		_input._vertical = 0f;
		Vector3 extraGravityForce = (Physics.gravity * _myControlStats._gravityMultiplier) - Physics.gravity;
		_rigidbody.AddForce (extraGravityForce);

		_groundCheckDistance = (_rigidbody.velocity.y < 0f) ? _myControlStats._originalGroundCheckDistance : 0.3f;
	}
	//...

	//rename or find another way -- used in spell effects for dark shield
	public void AffectBlocking() {
		_states._isBlocking = true;
	}
	public void StopAffectBlocking() {
		_states._isBlocking = false;
	}

	public override void PlayAnimation(string targetAnimation, bool isMirrored) {
		/*if (_states._isDelayed) {
			_states._isAnimatorInAction = false;
		} else {
			_states._isAnimatorInAction = true;
		}*/
        if(targetAnimation == null)
        {
            return;
        }

		_states._isAnimatorInAction = true;

		_states._isAbleToMove = false;
		_states._isAbleToAttack = false;
		_states._isBlocking = false;
		_states._isSpellCasting = false;
		_states._isJumping = false;
		_states._canKick = false;

		_animator.SetBool (AnimationStrings.IsInAction, _states._isAnimatorInAction);
		_animator.SetBool (AnimationStrings.IsMirrored, isMirrored); //if ambidextrous
		_animator.CrossFade (targetAnimation, 0.1f);
	}

	public override void PlayInteractAnimation(string targetAnimation, bool isMirrored) {
		_states._isDelayed = true;

		_states._isAbleToMove = true;
		_states._isAbleToAttack = false;
		_states._isBlocking = false;
		_states._isSpellCasting = false;
		_states._isJumping = false;
		_states._canKick = false;

		_animator.SetBool (AnimationStrings.IsInteracting, _states._isDelayed);
		_animator.SetBool (AnimationStrings.IsMirrored, isMirrored); //if ambidextrous
		_animator.CrossFade (targetAnimation, 0.1f);
	}

	public override void HandleInteractions() {
		//could all of this be handled in the scriptable object?
		Shepherd.WorldInteraction wInteract = _interactControl._interactions [_interactControl._index]._myInteraction;
		if (wInteract == null || wInteract._enabled == false) {
			return;
		}
        // !_invControl._testing._isPlayer   ----- add this to above so that pesky npcs don't steal things they aren't supposed to

		if (wInteract._prompt == AllEnums.PromptType.Listen || wInteract._prompt == AllEnums.PromptType.Continue || wInteract._prompt == AllEnums.PromptType.Skip) {
            wInteract.Interact (this);
			return;
		}
			
		if (!wInteract._repeatable) {
			wInteract._enabled = false;
			//may or may not be necessary, as the icontainer can be a part of an npc which we can feed dialogue through or a tree which regenerates its resources
			//Destroy (iContainer); 
		}

		Vector3 targetDirection = _interactControl._interactions [_interactControl._index].transform.position - _myTransform.position;
		SnapToDirection (targetDirection);

		if (wInteract._specialEvent != null) {
			//do special event
			Debug.Log ("EVENT TIME BAYBEE: " + wInteract._specialEvent.name);
			if (wInteract._specialEvent.HasGameEventListeners()) {
				wInteract._specialEvent.Raise ();
			}

			if (wInteract._specialEvent.HasSpecialGameEventListeners ()) {
				wInteract._specialEvent.RaiseResponse (0); //TODO: Hardcoded value!!
			}
			//wInteract._specialEvent.Raise ();
		}
			
		wInteract.Interact (this); //might need to pass variables in depending on interaction type
		if (wInteract._animation != null) {
			PlayAnimation (wInteract._animation._variable, false); //if ambidextrous, right or left handed...
		}
	}

	public void SnapToDirection(Vector3 targetDirection) {
		targetDirection.Normalize ();
		targetDirection.y = 0f;
		if (targetDirection == Vector3.zero) {
			targetDirection = transform.forward;
		}
		Quaternion target = Quaternion.LookRotation (targetDirection);
		//Quaternion targetRotation = Quaternion.Slerp (transform.rotation, target, _delta * _myControlStats._rotateSpeed); //might not work
		transform.rotation = target;
	}

	public override bool CheckForGround() {
		bool rayTouchGround = false;

		if (!_states._isGrounded) {

			RaycastHit hitInfo;

		#if UNITY_EDITOR
			Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * _groundCheckDistance));
		#endif
			// 0.1f is a small offset to start the ray from inside the character
			// it is also good to note that the transform position in the sample assets is at the base of the character
			if (Physics.Raycast (transform.position + (Vector3.up * 0.4f), Vector3.down, out hitInfo, _groundCheckDistance)) {
				rayTouchGround = true;
				_animator.applyRootMotion = true;
			} else {
				rayTouchGround = false;
				_animator.applyRootMotion = false;
			}
		} else {
			Vector3 origin = transform.position + (Vector3.up * _groundCheckDistance);

			//TESTING...
			Vector3 rightFoot = new Vector3(transform.position.x + 0.15f, transform.position.y, transform.position.z + 0.05f);
			rightFoot = rightFoot + (Vector3.up * _groundCheckDistance);
			Vector3 leftFoot = new Vector3 (transform.position.x - 0.15f, transform.position.y, transform.position.z + 0.05f);
			leftFoot = leftFoot + (Vector3.up * _groundCheckDistance);
			//...

			Vector3 direction = Vector3.down;
			float distance = _groundCheckDistance + 0.3f; //TODO: Hardcoded value!! 0.3f
			RaycastHit hit;
			RaycastHit hitL;
			RaycastHit hitR;
			#if UNITY_EDITOR
			Debug.DrawRay (origin, direction * distance);
			Debug.DrawRay (rightFoot, direction * distance);
			Debug.DrawRay (leftFoot, direction * distance);
			#endif
			if (Physics.Raycast (origin, direction, out hit, distance, _ignoreForGroundCheck)) {//_ignoreLayers)) {
				rayTouchGround = true;
				Vector3 targetPosition = hit.point;
				transform.position = targetPosition;
			} else if (Physics.Raycast (rightFoot, direction, out hitR, distance, _ignoreLayers) || Physics.Raycast (leftFoot, direction, out hitL, distance, _ignoreLayers)) {
				rayTouchGround = true;
			}
		}
		return rayTouchGround;
	}

	protected override void SetupAnimator() {
		base.SetupAnimator ();
	}

	protected override void HandleSpellcasting() {
		_animator.SetBool (AnimationStrings.IsSpellCasting, _states._isSpellCasting);

		if (_states._isAnimatorInAction || _states._isJumping) {
			return;
		}
		if (_currentSpellAction == null) {
			return;
		}

		if (_spellCastStart == null && _spellCastLoop == null && _spellCastStop == null) {
			return;
		}

		if (_spellCastStart != null) {
			_spellCastStart ();
		}
			
		if (!_input.CheckAttackButtons() || _states._isCrouching) {
			StopSpellCasting ();
			return;
		}
			
		if (_body._stamina.IsEmpty ()) {
			_invControl.CloseSpellBreathCollider ();
			_states._isIKEnabled = false;
			_animatorHook._useInverseKinematics = false;
			_animator.SetBool (AnimationStrings.IsSpellCasting, false);
			PlayAnimation (AnimationStrings.StateCannotDoSpell, _currentAction._isMirrored);
			if (_spellCastStop != null) {
				_spellCastStop ();
			}
			EmptySpellCastDelegates ();
			_currentActionStatus = AllEnums.CharacterActionState.OverrideActions;
			return;
		}

		//_states._isIKEnabled = true;
		_animatorHook._currentHand = (_currentAction._isMirrored) ? AvatarIKGoal.LeftHand : AvatarIKGoal.RightHand;

		if (_spellCastLoop != null) {
			_spellCastLoop ();
			return;
		}
	}

	public override void StopSpellCasting() {
		_invControl.CloseSpellBreathCollider ();
		_states._isIKEnabled = false;
		_animatorHook._useInverseKinematics = false;
		_animator.SetBool (AnimationStrings.IsSpellCasting, false);
		if (_spellCastStop != null) {
			_spellCastStop ();
		}
		EmptySpellCastDelegates ();
		_states._isDelayed = true;
		PlayInteractAnimation (((_currentAction._isMirrored) ? AnimationStrings.StateEmptyLeftHand : AnimationStrings.StateEmptyRightHand), _currentAction._isMirrored);
		_spellCastTimer = 0f;
		_currentActionStatus = AllEnums.CharacterActionState.Interacting;
	}

	//BLOCKING
	protected override void HandleBlocking() {
		//subject to change
		//_animator.SetBool(AnimationStrings.IsBlocking, _isBlocking);
		if (!_states._isBlocking) {
			if (_blockAnimation) {
				_animator.CrossFade (_blockIdleAnimaton, 0.1f);
				_blockAnimation = false;

				//test
				_animatorHook.CloseBlockCollider();
			}
		} else {
			
		}
	}

	protected override void HandleRotation() {
		/*
		Vector3 targetDirection = (!_isLockOn) ? _moveDirection : _lockOnTransform.position - transform.position;
		*/
		if (_states._isAnimatorInAction && !_states._isAbleToRotate) {
			return;
		}

		Vector3 targetDirection;
		if (!_states._isLockOn) {
			targetDirection = _moveDirection;
		} else {
			if (_lockOnTransform != null) {
				targetDirection = _lockOnTransform.position - transform.position;
			} else {
				targetDirection = _moveDirection;
			}
		}
			
		targetDirection.y = 0f;
		if (targetDirection == Vector3.zero) {
			targetDirection = transform.forward;
		}
		Quaternion targetRotate = Quaternion.LookRotation (targetDirection);
		Quaternion targetRotation = Quaternion.Slerp (transform.rotation, targetRotate, _delta * _moveAmount * _myControlStats._rotateSpeed);
		transform.rotation = targetRotation;
	}

	protected override void HandleMovementAnimations() {
		if (_states._isUsingItem || _states._isGivingItem || _body._stamina.IsEmpty() || _states._isChangingItem) {
			_animator.SetBool (AnimationStrings.IsRunning, false);
		} else {
			_animator.SetBool (AnimationStrings.IsRunning, _states._isRunning);
		}
		_animator.SetBool (AnimationStrings.IsCrouching, _states._isCrouching);
		_animator.SetBool (AnimationStrings.IsGivingItem, _states._isGivingItem);

		_animator.SetFloat (AnimationStrings.Vertical, Mathf.Clamp01(_moveAmount), 0.1f, _delta);
		//resetting horizontal so that the character isn't leaning strangely
		_animator.SetFloat (AnimationStrings.Horizontal, 0f, 0.1f, _delta);
		//_animator.SetFloat (AnimationStrings.Horizontal, _turnAmount, 0.1f, _delta);

		if (!_states._isGrounded) {
			_animator.SetFloat ("Jump", _rigidbody.velocity.y);
		}

		// calculate which leg is behind, so as to leave that leg trailing in the jump animation
		// (This code is reliant on the specific run cycle offset in our animations,
		// and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
		float runCycle = Mathf.Repeat(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime + _myControlStats._runCycleLegOffset, 1);
		float jumpLeg = (runCycle < 0.5f ? 1 : -1) * _moveAmount;
		if (_states._isGrounded) {
			_animator.SetFloat("JumpLeg", jumpLeg);
		}
	}

	protected override void HandleRollingAnimations() {
		if (!_states._isRolling || _states._isAnimatorInAction || _states._isUsingItem || _states._isChangingItem) {
			return;
		}

		if (!_states._isGrounded || _body._stamina.IsEmpty()) {
			_states._isCrouching = true;
			return;
		}
			
		if (_states._isSpellCasting) {
			StopSpellCasting ();
			_currentSpellAction = null;
			_states._isAnimatorInAction = false;
			_animator.SetBool (AnimationStrings.IsInAction, false);
			_currentActionStatus = AllEnums.CharacterActionState.Moving;
			return;
		}

		//test
		Vector3 relativeDirection = _myTransform.InverseTransformDirection(_moveDirection); //_input._moveDirection
		float vertical = relativeDirection.z;
		float horizontal = relativeDirection.x;

		_rollAmount = _myControlStats._rollSpeed; //don't understand this

		if (relativeDirection == Vector3.zero) { //take a step back
			_moveDirection = -_myTransform.forward;
		}

		_states._isLeaping = false;
		_states._isInvincible = true;
		_body._stamina.Subtract (_body._stamina.GetMaximum() * _myControlStats._dodgeStaminaCostPercentage);

		_animator.SetFloat (AnimationStrings.Vertical, vertical);
		_animator.SetFloat (AnimationStrings.Horizontal, horizontal);
		PlayAnimation (AnimationStrings.StateDodge, false);
	}

	protected override void HandleKicking() {

		if (!_states._holdKick) {
			if (_moveAmount > _myControlStats._moveAmountThreshold) {
				_kickTimer += _delta;
				if (_kickTimer < _myControlStats._maxKickWindow) { //kick timer max time hardcoded!!
					_states._canKick = true;
				} else {
					_kickTimer = _myControlStats._maxKickWindow;
					_states._holdKick = true;
					_states._canKick = false;
				}
			} else {
				//_canKick = false;
				//_kickTimer = 0f;
				_kickTimer -= _delta;
				if (_kickTimer < 0f) {
					_kickTimer = 0f;
					//_holdKick = false;
					_states._canKick = false;
				}
			}
		} else {
			if (_moveAmount < _myControlStats._moveAmountThreshold) {
				_kickTimer -= _delta;
				if (_kickTimer < 0f) {
					_kickTimer = 0f;
					_states._holdKick = false;
					_states._canKick = false;
				}
			}
		}
	}

	protected override void HandleLockOnAnimations(Vector3 moveDirection) {
		Vector3 relativeDirection = transform.InverseTransformDirection (moveDirection);
		float horizontal = relativeDirection.x;
		float vertical = relativeDirection.z;

		_animator.SetFloat (AnimationStrings.Vertical, vertical, 0.1f, _delta);
		_animator.SetFloat (AnimationStrings.Horizontal, horizontal, 0.1f, _delta);

		_animator.SetBool (AnimationStrings.IsCrouching, _states._isCrouching);
		_animator.SetBool (AnimationStrings.IsGivingItem, _states._isGivingItem);

		if (!_states._isGrounded) {
			_animator.SetFloat ("Jump", _rigidbody.velocity.y);
		}

		// calculate which leg is behind, so as to leave that leg trailing in the jump animation
		// (This code is reliant on the specific run cycle offset in our animations,
		// and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
		float runCycle = Mathf.Repeat(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime + _myControlStats._runCycleLegOffset, 1);
		float jumpLeg = (runCycle < 0.5f ? 1 : -1) * _moveAmount;
		if (_states._isGrounded) {
			_animator.SetFloat("JumpLeg", jumpLeg);
		}
	}

	//TESTING Immune System 
	public override void HandleAffliction() {
		if (_currentAilment == null) {
			return;
		}

		if (_body._immuneSystem.GetMeter() < _body._immuneSystem.GetMaximum()) {
			_currentAilment.transform.parent = _bodySkin.transform;
			_currentAilment.transform.localPosition = Vector3.up;
			_currentAilment.GetComponentInChildren<ParticleSystem> ().Play ();
			_currentAilment.GetComponentInChildren<ParticleSystem> ().Emit (1);
			Light ailmentLight = _currentAilment.GetComponentInChildren<Light> ();
			if (ailmentLight != null) {
				ailmentLight.gameObject.SetActive (true);
				ailmentLight.intensity -= _body._immuneSystem._regen / 10f; //Time.deltaTime / 2f;
			}
			_body._health.Decay(); //TODO: for now, but current ailment should be depending on poison, bleed etc
		}
	}

	public override void HandleDeath () {
		//this.gameObject.layer = 11;
		_input._horizontal = 0f;
		_input._vertical = 0f;
		_states._isDead = true;
		_states._isParried = false;
		_states._isDelayed = false;
		_states._isOpenToParry = false;
		_states._isBackstabbed = false;
		_states._isAbleToMove = false;

		EmptySpellCastDelegates ();
		StopSpellCasting ();
		_currentSpellAction = null;
		_currentAction = null;
		if (_currentSpellObject != null) {
			Destroy (_currentSpellObject);
		}

		_invControl.CloseAllDamageColliders();

		if (_ragdollRigidbody.Count != 0) {
			//EnableRagdoll ();

			//temp
			Vector3 d = new Vector3(Random.Range(-5f,5f), Random.Range(0,5f), Random.Range(-5f,5f));
			for (int i = 0; i < _ragdollRigidbody.Count; i++) {
				_ragdollRigidbody [i].isKinematic = false;
				_ragdollColliders [i].isTrigger = false;
				//test
				_ragdollRigidbody [i].AddForce(-_myTransform.forward+d, ForceMode.Impulse);
			}

			Collider controlCollider = _rigidbody.gameObject.GetComponent<Collider> ();
			controlCollider.enabled = false;
			Destroy(controlCollider);
			_rigidbody.isKinematic = true; //??

			//LockOnController._singleton._enemyTargets.Remove (_lockOnTarget);
			Destroy (_lockOnTarget);
			Destroy (_animator);
			Destroy (_animatorHook);
			Destroy (_bodySenses);
			Destroy (_currentAilment);
			//Destroy (this);
			//...

		} else {
            PlayAnimation ("well", false);
            Debug.Log("I DIED");



            //_animator.Play ("death1"); //hardcoded for spider, will produce error if not spider

            //_animator.transform.parent = null;
            //this.gameObject.SetActive (false);

            //Destroy (this.gameObject);
            /*
			 Collider controlCollider = _eRigidbody.gameObject.GetComponent<Collider> ();
			 controlCollider.enabled = false;
			 _eRigidbody.isKinematic = true; //??
			 StartCoroutine ("CloseAnimator");
			*/
        }
	}
		
	//check if we've pushed an attack button
	public override bool CheckForAttackInput() {
		if (!_states._isAbleToAttack) {
			//test
			_states._isBlocking = false;
			_actionControl._actionIndex = 0;
			//...
			return false;
		}

		ActionContainer iA = null;

		if (_input._weakRightButton) {
			iA = _actionControl.GetActionContainer (AllEnums.ActionInputType.RightWeak);
			if (iA._actions != null) {
				if (_actionControl._actionIndex > iA._actions.Length-1) {
					_actionControl._actionIndex = 0;
				}
				if (iA._actions [_actionControl._actionIndex]._actionDataObject != null) {
					HandleActions (iA);
					_prevActionInput = AllEnums.ActionInputType.RightWeak;
					return true;
				}
			}
		}

		if (_input._strongRightButton) {
			iA = _actionControl.GetActionContainer (AllEnums.ActionInputType.RightStrong);
			if (iA._actions != null) {
				if (_actionControl._actionIndex > iA._actions.Length - 1) {
					_actionControl._actionIndex = 0;
				}
				if (iA._actions [_actionControl._actionIndex]._actionDataObject != null) {
					HandleActions (iA);
					_prevActionInput = AllEnums.ActionInputType.RightStrong;
					return true;
				}
			}
		}

		if (_input._weakLeftButton) {
			iA = _actionControl.GetActionContainer (AllEnums.ActionInputType.LeftWeak);
			if (iA._actions != null) {
				if (_actionControl._actionIndex > iA._actions.Length-1) {
					_actionControl._actionIndex = 0;
				}
				if (iA._actions [_actionControl._actionIndex]._actionDataObject != null) {
					HandleActions (iA);
					_prevActionInput = AllEnums.ActionInputType.LeftWeak;
					return true;
				}
			}
		}

		if (_input._strongLeftButton) {
			iA = _actionControl.GetActionContainer (AllEnums.ActionInputType.LeftStrong);
			if (iA._actions != null) {
				if (_actionControl._actionIndex > iA._actions.Length-1) {
					_actionControl._actionIndex = 0;
				}
				if (iA._actions [_actionControl._actionIndex]._actionDataObject != null) {
					HandleActions (iA);
					_prevActionInput = AllEnums.ActionInputType.LeftStrong;
					return true;
				}
			}
		}
		//test
		_states._isBlocking = false;
		_actionControl._actionIndex = 0;
		//...
		return false;
	}

	//check if we backstabbed a target
	protected override bool CheckForBackstabTarget(AttackAction aAction, bool isMirrored) {
		if (!aAction._canBackstabWith) {
			return false;
		}

		if (_body._stamina.GetMeter () < aAction._staminaCost) {
			return false;
		}

		CharacterStateController backstabTarget = null;

		Vector3 origin = transform.position;
		origin.y += 1f; //???
		Vector3 rayDir = transform.forward;
		RaycastHit hit;
		if(Physics.Raycast(origin, rayDir, out hit, 1f, _ignoreLayers)) {
			backstabTarget = hit.transform.GetComponentInParent<CharacterStateController> ();
		}

		if (backstabTarget != null) {
			backstabTarget.CheckForBackstab (_myTransform, this);
			if (!backstabTarget._states._isBackstabbed) {
				return false;
			}

			Vector3 faceDirection = transform.position - backstabTarget.transform.position;
			faceDirection.Normalize ();
			faceDirection.y = 0f;
			float angle = Vector3.Angle (backstabTarget.transform.forward, faceDirection);
			//we are in range and we are at the proper angle to make a backstab on a target for more damage
			if (angle > 150f) {
				Vector3 targetPosition = faceDirection * _myControlStats._backstabOffset;
				targetPosition += backstabTarget.transform.position;
				transform.position = targetPosition;

				backstabTarget.transform.rotation = transform.rotation;

				Shepherd.RuntimeWeapon byRTWeapon = (isMirrored) ? _invControl._currentLeftWeapon : _invControl._currentRightWeapon;
				Shepherd.Weapon byWeapon = (Shepherd.Weapon)byRTWeapon._instance;
				backstabTarget.HandleGettingBackstabbed (aAction, byWeapon);

				PlayAnimation (AnimationStrings.StateParryAttack, isMirrored);

				_lockOnTarget = null;

				_body._stamina.Subtract (aAction._staminaCost);
				//_body.something.subtract(wSlot._otherCost);

				return true;
			}
		}
		return false;
	}

	//check if we parried a target
	protected override bool CheckForParryTarget(AttackAction aAction, bool isMirrored) {
		if (!aAction._canParryWith) {
			return false;
		}

		if (_body._stamina.GetMeter () < aAction._staminaCost) {
			return false;
		}

		CharacterStateController parriedTarget = null;

		Vector3 origin = transform.position;
		origin.y += 1f; //???
		Vector3 rayDir = transform.forward;
		RaycastHit hit;
		if(Physics.Raycast(origin, rayDir, out hit, 3f, _ignoreLayers)) {
			parriedTarget = hit.transform.GetComponentInParent<CharacterStateController> ();
		}
		//								v-- seems redundant
		//if ((parriedTarget != null && parriedTarget._parriedBy != null) && parriedTarget._states._isParried) {
		if (parriedTarget != null && parriedTarget._states._isParried) {
			/*float distanceFromTarget = Vector3.Distance (parriedTarget.transform.position, transform.position);
			if (distanceFromTarget < 3f) {
				
			}*/
			Vector3 faceDirection = parriedTarget.transform.position - transform.position;
			faceDirection.Normalize ();
			faceDirection.y = 0f;
			float angle = Vector3.Angle (transform.forward, faceDirection);
			//we are in range and we are at the proper angle to make an attack on a parry target for more damage
			if (angle < 60f) {
				Vector3 targetPosition = -faceDirection * _myControlStats._parryOffset;
				targetPosition += parriedTarget.transform.position;
				transform.position = targetPosition;

				if (faceDirection == Vector3.zero) {
					faceDirection = -parriedTarget.transform.forward;
				}
				Quaternion eRotation = Quaternion.LookRotation (-faceDirection);
				Quaternion pRotation = Quaternion.LookRotation (faceDirection);

				parriedTarget.transform.rotation = eRotation;
				transform.rotation = pRotation;

				Shepherd.RuntimeWeapon byRTWeapon = (isMirrored) ? _invControl._currentLeftWeapon : _invControl._currentRightWeapon;
				Shepherd.Weapon byWeapon = (Shepherd.Weapon)byRTWeapon._instance;
				parriedTarget.HandleGettingParried (aAction, byWeapon);

				PlayAnimation (AnimationStrings.StateParryAttack, isMirrored);

				_lockOnTarget = null;

				_body._stamina.Subtract (aAction._staminaCost);
				//_body.something.subtract(wSlot._otherCost);
				return true;
			}
		}
		return false;
	}

	//Check if we've been parried
	public override void CheckForParry (Transform fromTarget, CharacterStateController fromCharacter) {
		if ((_states._isAbleToBeParried || !_states._isInvincible) && _states._isOpenToParry) {
			if (!fromCharacter._states._isParrying) {
				return;
			}

			Vector3 direction = transform.position - fromTarget.position;
			direction.Normalize ();
			float dotProduct = Vector3.Dot (fromTarget.forward, direction);
			if (dotProduct < 0f) {
				return;
			}
			_invControl.CloseAllDamageColliders ();
			fromCharacter._animatorHook.CloseParryCollider();

			_states._isParried = true;
			_states._isOpenToParry = false;
			_animatorHook.CloseParryCollider (); //not sure if needed, especially if you are the one being parried...
			_states._isInvincible = true;
			PlayAnimation(AnimationStrings.StateAttackInterrupt, _currentAction._isMirrored);
			_animator.applyRootMotion = true;

			//fromCharacter now has a parried target -- Us!
			_attackedBy = fromCharacter;
		}
	}

	//Check if we've been backstabbed
	public override void CheckForBackstab (Transform fromTarget, CharacterStateController fromCharacter) {
		if ((_states._isAbleToBeBackstabbed || !_states._isInvincible) && _states._isOpenToBackstab) {
			if (!fromCharacter._states._isAbleToAttack) {
				return;
			}

			_invControl.CloseAllDamageColliders ();
			fromCharacter._animatorHook.CloseDamageColliders();

			_states._isBackstabbed = true;
			_states._isOpenToBackstab = false;
			_states._isAbleToMove = false;
			_states._isOpenToParry = false;
			_states._isInvincible = true;
			_animator.applyRootMotion = true;

			//fromCharacter now has a backstabbed target -- Us!
			_attackedBy = fromCharacter;
		}
	}

	public override void EnableRagdoll() {
		for (int i = 0; i < _ragdollRigidbody.Count; i++) {
			_ragdollRigidbody [i].isKinematic = false;
			_ragdollColliders [i].isTrigger = false;
		}
		Collider controlCollider = _rigidbody.gameObject.GetComponent<Collider> ();
		controlCollider.enabled = false;
		_rigidbody.isKinematic = true; //??

		StartCoroutine ("CloseAnimator");
	}

	IEnumerator CloseAnimator() {
		yield return new WaitForEndOfFrame ();
		_animator.enabled = false;
		this.enabled = false; //???
	}

	public override void UpdateGameUI() {
        if(_invControl._runtimeRefs._updateGameUI != null)
        {
            _invControl._runtimeRefs._updateGameUI.Raise();
        }
		/*if (_wRControl._runtimeReferences._updateGameUI != null) {
			_wRControl._runtimeReferences._updateGameUI.Raise ();
		}*/
	}
}