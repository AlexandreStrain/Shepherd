using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	public abstract class CharacterStateController : MonoBehaviour {
		/*Variables*/
		[Header("Init")]
		public GameObject _activeModel;
		public ControlStats _myControlStats;
		public InputModule _myBrain;
		public Biography _biography;
		[HideInInspector]
		public Transform _myTransform;

		[Header("Debug")]
		public float _delta;
		public AllEnums.CharacterActionState _currentActionStatus;
		public float _moveAmount;
		public Vector3 _moveDirection;
		protected float _turnAmount; //change? need?
		protected float _rollAmount; //change? need?

		public AllEnums.ActionInputType _prevActionInput;
		public bool _controlColliderCrouch;
		public CharacterStateController _attackedBy;
		[HideInInspector]
		public GameInputs _input;

		protected float _colliderHeight;
		protected Vector3 _colliderCentre;
		protected CapsuleCollider _controllerCollider;

		protected float _airTimer;
		protected float _spellCastTimer;
		protected float _invincibilityTimer;
		protected float _groundCheckDistance;

		public LayerMask _ignoreLayers;
		public LayerMask _ignoreForGroundCheck; //might not be necessary

		[Header("Character")]
		public SkinnedMeshRenderer _bodySkin;
		public CharacterBody _body;
		[HideInInspector]
		public Senses _bodySenses;
		public CharacterStates _states;

		[Header("Inventory")]
		public Shepherd.InventoryController _invControl;
		public GameObject _currentAilment;
		[HideInInspector]
		public WorldResourceController _wRControl;

		[Header("Actions")]
		public ActionController _actionControl;
		public ActionContainer _currentAction;
		public SpellAction _currentSpellAction;
		[HideInInspector]
		public GameObject _currentSpellObject;

		[Header("Interactions")]
		public InteractionController _interactControl;

		[Header("LockOn")]
		public LockOnTarget _lockOnTarget;
		public Transform _lockOnTransform;

		[Header("Delegates")]
		public DelegateCalls.SpellCastStart _spellCastStart;
		public DelegateCalls.SpellCastLoop _spellCastLoop;
		public DelegateCalls.SpellCastStop _spellCastStop;
		public DelegateCalls.SpellCastLoop _spellEffectLoop;
		public DelegateCalls.ItemEffectLoop _itemEffectLoop;

		[Header("Animator and RigidBody")]
		[HideInInspector]
		public Animator _animator;
		[HideInInspector]
		public AnimationHook _animatorHook;
		[HideInInspector]
		public Rigidbody _rigidbody;
		protected List<Rigidbody> _ragdollRigidbody = new List<Rigidbody>();
		protected List<Collider> _ragdollColliders = new List<Collider> ();
			
		/*METHODS*/
		public virtual void Init () {
			_rigidbody = GetComponent<Rigidbody> ();
			//_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
			_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

			//This seems weird
			_animatorHook = _activeModel.GetComponent<AnimationHook>();
			if (_animatorHook == null) {
				_animatorHook = _activeModel.AddComponent<AnimationHook> ();
			}
			//...

			//_body.Init (_biography._bodyStats, _myTransform);
			_body.Init(_biography._baseStatistics, _myTransform);
				
			_controllerCollider = GetComponent<CapsuleCollider> ();
			_colliderCentre = _controllerCollider.center;
			_colliderHeight = _controllerCollider.height;
		}
        public virtual void Tick(float delta) { }
		public virtual void FixedTick (float delta) { }
		public virtual void TakeDamage (CharacterStateController byCharacter, Shepherd.RuntimeWeapon byWeapon) {//AttackAction byWeaponAction, Shepherd.Weapon byWeapon) {
		}
		public virtual void RecieveSpellDamage () {
			//might not be necessary
		}
		public virtual void PauseForMenu() { }
		public virtual void MonitorStats () { }
		public virtual void CheckForParry(Transform fromTarget, CharacterStateController fromCharacter) { }
		public virtual void CheckForBackstab(Transform fromTarget, CharacterStateController fromCharacter) { }
		public virtual void UpdateGameUI() { }

		public virtual void HandleAffliction() { }
		public virtual void HandleDeath() { }
		public virtual void HandleInteractions () { }
        public virtual void HandleTwoHanded() { }
		public virtual void HandleGettingParried (AttackAction byWeaponAction, Weapon byWeapon) { }
		public virtual void HandleGettingBackstabbed (AttackAction byWeaponAction, Weapon byWeapon) { }

		public virtual void PlayAnimation (string targetAnimation, bool isMirrored) { }
		public virtual void PlayInteractAnimation(string targetAnimation, bool isMirrored) { }
		public virtual void StartSpellCasting() { }
		public virtual void StopSpellCasting() { }
		public virtual void EnableRagdoll() { }
		public virtual void EmptySpellCastDelegates() {
			_spellCastStart = null;
			_spellCastLoop = null;
			_spellCastStop = null;
		}
		public virtual bool CheckForGround () {
			return true;
		}
		public virtual bool CheckForAttackInput() {
			return false;
		}

		protected virtual void HandleGroundedMovement () { }
		protected virtual void HandleAirborneMovement () { }
		protected virtual void HandleRotation () { }
		protected virtual void HandleMovementAnimations () { }
		protected virtual void HandleLockOnAnimations(Vector3 moveDirection) { }
		protected virtual void HandleRollingAnimations () { }
		protected virtual void HandleLockonAnimations (Vector3 moveDirection) { }
		protected virtual void HandleKicking () {
			//might not be necessary
		}
		protected virtual void HandleLanding () {
			//might rename
		}
		protected virtual void HandleActions (ActionContainer aContainer) {
			/*
			* Includes:
			* ItemAction, WeaponAction, SpellAction, BlockAction, ParryAction
			*/
		}
		protected virtual void HandleAttackAction(ActionContainer aContainer, Shepherd.AttackAction aAction) { }
		protected virtual void HandleBlockAction(AttackAction bAction, bool isMirrored) { }
		protected virtual void HandleSpellAction(ActionContainer aContainer, Shepherd.SpellAction sAction) { }
		protected virtual void HandleParryAction(AttackAction aAction, bool isMirrored) { }
		protected virtual bool CheckForParryTarget(AttackAction aAction, bool isMirrored) {
			return false;
		}
		protected virtual bool CheckForBackstabTarget(AttackAction aAction, bool isMirrored) {
			return false;
		}
		protected virtual void HandleSpellcasting () { }
		protected virtual void HandleBlocking () { }
		protected virtual void HandleWeaponModelVisibility() { }
		protected virtual void HandleInvincibilityFrames () { }
		protected virtual void SetupAnimator () { 
			if (_activeModel == null) {
				_animator = GetComponentInChildren<Animator> ();
				Debug.LogWarning ("Getting Animator from Child of this GameObject... Might not work!");
				if (_animator == null) {
					Debug.Log ("No Model Found.");
				} else {
					_activeModel = _animator.gameObject;
				}
			}

			if (_animator == null) {
				_animator = _activeModel.GetComponent<Animator> ();
			}

			if (_animator.isHuman) {
				_animator.GetBoneTransform (HumanBodyBones.LeftHand).localScale = Vector3.one;
				_animator.GetBoneTransform (HumanBodyBones.RightHand).localScale = Vector3.one;
			}

			_animator.applyRootMotion = false;
		}
		protected virtual void InitRagdoll() { }
	}
}