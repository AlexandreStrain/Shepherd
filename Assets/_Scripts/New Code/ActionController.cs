using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[System.Serializable]
	public class ActionController {
		
		public int _actionIndex;
		public ActionContainer[] _actionSlots;

		private CharacterStateController _sControl;

		public void Init(CharacterStateController sControl) {
			_sControl = sControl;

			_actionIndex = 0;

			_actionSlots = new ActionContainer[AllEnums.ActionInputTypeNumber];
			for (int i = 0; i < _actionSlots.Length; i++) {
				ActionContainer a = new ActionContainer ();
				a._actionInputType = (AllEnums.ActionInputType)i;
				_actionSlots [i] = a;
			}
		}
			
		public void UpdateActionsWithCurrentItems() {
			//EmptyAllSlots(); ???

			bool twoHandRight = _sControl._input._dRightHold;
			bool twoHandLeft = _sControl._input._dLeftHold;

			Shepherd.RuntimeWeapon rItem = _sControl._invControl._currentRightWeapon;
			Shepherd.RuntimeWeapon lItem = _sControl._invControl._currentLeftWeapon;

			Shepherd.Weapon rWeapon = (rItem == null) ? null : (Shepherd.Weapon)rItem._instance;
			Shepherd.Weapon lWeapon = (lItem == null) ? null : (Shepherd.Weapon)lItem._instance;

			if (rItem == null && lItem == null) {
				return; //then we are unarmed, and have unarmed actions already when we did EmptyAllSlots()
			}

			if ((twoHandRight == true && twoHandLeft == false) && rItem != null) {
				//Two Hand Right Weapon
				if (lItem != null) {
					lItem._rtModel.SetActive (false);
				}
				rItem._rtModel.SetActive (true);

				SetupMoveset(rWeapon, false, true);

				if (rWeapon._twoHandIdle != null) {
					_sControl._animator.CrossFade (rWeapon._twoHandIdle._variable, 0.1f);
				} else {
					_sControl._animator.CrossFade (AnimationStrings.StateEmptyBothHands, 0.1f);
				}
			} else if ((twoHandRight == false && twoHandLeft == true) && lItem != null) {
				//Two Hand Left Weapon
				if (rItem != null) {
					rItem._rtModel.SetActive (false);
				}
				lItem._rtModel.SetActive (true);

				SetupMoveset(lWeapon, true, true);

				if (lWeapon._twoHandIdle != null) {
					_sControl._animator.CrossFade (lWeapon._twoHandIdle._variable, 0.1f);
				} else {
					_sControl._animator.CrossFade (AnimationStrings.StateEmptyBothHands, 0.1f);
				}
			} else if (lItem == null && rItem != null) {
				//ONLY Right Weapon
				rItem._rtModel.SetActive(true);

				SetupMoveset (rWeapon);

				string targetAnimation = rWeapon._oneHandIdle._variable + AnimationStrings.RightHandSuffix;
				_sControl._animator.Play (targetAnimation);
			} else if (rItem == null && lItem != null) {
				//ONLY Left Weapon
				lItem._rtModel.SetActive(true);
				SetupMoveset (lWeapon, true);

				string targetAnimation = lWeapon._oneHandIdle._variable + AnimationStrings.LeftHandSuffix;
				_sControl._animator.Play (targetAnimation);
			} else {
				//Dual Wield!
				rItem._rtModel.SetActive(true);
				lItem._rtModel.SetActive(true);

				ActionContainer rWeak = _sControl._actionControl.GetActionContainer (AllEnums.ActionInputType.RightWeak);
				rWeak._actions = rWeapon.GetActions (AllEnums.ActionInputType.RightWeak, false);
				rWeak._isMirrored = false;

				ActionContainer rStrong = _sControl._actionControl.GetActionContainer (AllEnums.ActionInputType.RightStrong);
				rStrong._actions = rWeapon.GetActions (AllEnums.ActionInputType.RightStrong, false);
				rStrong._isMirrored = false;

				ActionContainer lWeak = _sControl._actionControl.GetActionContainer (AllEnums.ActionInputType.LeftWeak);
				lWeak._actions = lWeapon.GetActions (AllEnums.ActionInputType.RightWeak, false);
				lWeak._isMirrored = true;

				ActionContainer lStrong = _sControl._actionControl.GetActionContainer (AllEnums.ActionInputType.LeftStrong);
				lStrong._actions = lWeapon.GetActions (AllEnums.ActionInputType.RightStrong, false);
				lStrong._isMirrored = true;

				string targetAnimation = rWeapon._oneHandIdle._variable + AnimationStrings.RightHandSuffix;
				_sControl._animator.Play (targetAnimation);
				targetAnimation = lWeapon._oneHandIdle._variable + AnimationStrings.LeftHandSuffix;
				_sControl._animator.Play (targetAnimation);
			}

		}

		public ActionContainer GetActionContainer(AllEnums.ActionInputType t) {
			for (int i = 0; i < _actionSlots.Length; i++) {
				if (_actionSlots [i]._actionInputType == t) {
					return _actionSlots [i];
				}
			}
			return null;
		}

		//test
		public Shepherd.Weapon GetWeaponFromAction(bool isLeft) {
			if (isLeft) {
				return (Shepherd.Weapon)_sControl._invControl._currentLeftWeapon._instance;
			} else {
				return (Shepherd.Weapon)_sControl._invControl._currentRightWeapon._instance;
			}
		}
		//...

		private void SetupMoveset(Weapon fromWeapon, bool isLeftHand = false, bool isTwoHanding = false) {
			for (int i = 0; i < AllEnums.ActionInputTypeNumber; i++) {
				ActionContainer aContainer = _sControl._actionControl.GetActionContainer ((AllEnums.ActionInputType)i);
				aContainer._actions = fromWeapon.GetActions ((AllEnums.ActionInputType)i, isTwoHanding);
				aContainer._isMirrored = isLeftHand;
			}
		}
	}


	[System.Serializable]
	public class ActionContainer {
		public AllEnums.ActionInputType _actionInputType;
		public Action[] _actions;
		public bool _isMirrored;
	}

	[System.Serializable]
	public class Action {
		public AllEnums.ActionType _actionType;
		public Object _actionDataObject;
	}
}