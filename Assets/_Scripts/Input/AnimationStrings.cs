using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationStrings {
	/*Animator Parameters*/
	public static string IsOnGround = "isOnGround";
	public static string IsBlocking = "isBlocking";
	public static string IsInteracting = "isInteracting";
	public static string IsLockOn = "isLockOn";
	public static string IsTwoHanding = "isTwoHanding";
	public static string IsMirrored = "isMirrored";
	public static string IsRunning = "isRunning";
	public static string IsCrouching = "isCrouching";
	public static string IsSpellCasting = "isSpellCasting";
	public static string IsUsingItem = "isUsingItem";
	public static string IsGivingItem = "isGivingItem";

	public static string AnimationSpeed = "AnimationSpeed";
	public static string Vertical = "Vertical";
	public static string Horizontal = "Horizontal";

	public static string IsInAction = "isInAction";


	/*Animation States*/

	public static string StateBothHandsIdle = "BothHands_Idle";
	public static string StateOneHandIdle = "OneHand_Idle";

	public static string StateEmpty = "Empty";

	public static string StateDodge = "Roll";
	public static string StateLeap = "jump_launch";
	public static string StateLand = "jump_land";

	public static string StateParryAttack = "parry_attack";

	public static string StateGetHit01 = "Get_Hit_01";
	public static string StateGetHit02 = "Get_Hit_02";
	public static string StateGetHit03 = "Get_Hit_03"; //throws person backward

	public static string StateKick01 = "Kick_01";

	public static string StatePunch01 = "Punch_01";

	public static string StateGetBackstabbed = "Get_Backstabbed_01";
	public static string StateGetParried = "Get_Parried_01";
	public static string StateAttackInterrupt = "attack_interrupt";
	//public static string StateAttackOneHand = "OneHand_Attack_01";

	public static string StateChangeItem = "changeItem";
	public static string StatePickupItem = "pick_up";
	public static string StateChangeWeapon = "changeWeapon";
	public static string StateGiveItem = "giveItem";

	public static string StateCannotDoSpell = "Cannot_Do_Spell";


	public static string StateGetUpFaceUp = "GetUp_FaceUp_01";
	public static string StateGetUpFaceDown = "GetUp_FaceDown_01";

	//not sure if needed right now...
	public static string StateEmptyRightHand = "Empty_RightH";
	public static string StateEmptyLeftHand = "Empty_LeftH";
	public static string StateEmptyBothHands = "Empty Both Hands";
	public static string StateEmptyInteractions = "Empty Interactions";
	public static string StateEmptyOverride = "Empty Override";
	public static string LocomotionRunning = "Locomotion Running";
	public static string LocomotionNormal = "Locomotion Normal";

	//Animation suffix
	public static string LeftHandSuffix = "_LeftH";
	public static string RightHandSuffix = "_RightH";
}