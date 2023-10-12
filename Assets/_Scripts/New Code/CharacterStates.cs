using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[System.Serializable]
	public class CharacterStates {

		[Header("Movement")]
		public bool _isGrounded;
		public bool _isRunning;
		public bool _isLockOn;
		public bool _isRolling;
		public bool _isCrouching;
		public bool _isJumping; //regular jump
		public bool _isLeaping; //running jump

		public bool _isAbleToMove;
		public bool _isAbleToAttack;
		public bool _isAbleToRotate;

		[Header("Animator")]
		public bool _isDelayed; //isActing???
		public bool _isIKEnabled;
		public bool _isAnimatorInAction;

		[Header("Combat")]
		public bool _isTwoHanding;
		public bool _isSpellCasting;
		public bool _isDamageColliderOn;
		public bool _isInvincible; //If get hit, have some invincibility frames to recover, also for dodge roll

		public bool _isBlocking;
		public bool _isParrying; //am I in the process of parrying someone else?
		public bool _isOpenToParry; //am I in a situation where I can be parried?
		public bool _isParried; //has someone parried me? am I parried?
		public bool _isAbleToBeParried; //can I be parried?

		public bool _isBackstabbed = false; //checks if we are being stabbed in the back by a weapon
		//new stuff?
		public bool _isOpenToBackstab; //does my attacks leave me open to being backstabbed?
		public bool _isAbleToBeBackstabbed; //can I be backstabbed?

		public bool _canKick; //not sure if needed
		public bool _holdKick; //not sure if needed

		[Header("Item")]
		//same as _useItem? No, this is to say that the player is currently using the item, not if we pressed the key!
		public bool _isUsingItem;
		public bool _isGivingItem;
		public bool _isChangingItem;
		public bool _closeWeapons; //subject to change

		[Header("Status")]
		//not sure what to name...
		public bool _isDead;
		public bool _isInConversation;
	}
}