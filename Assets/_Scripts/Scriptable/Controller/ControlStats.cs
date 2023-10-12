using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[CreateAssetMenu(menuName = "Shepherd/Single Instances/Control Stats")]
	public class ControlStats : ScriptableObject {

		[Header("Movement")]
		public float _moveSpeed = 2f; //also counts as max jog speed
		public float _runSpeed = 3.5f;
		public float _crouchSpeed = 1f;
		public float _rotateSpeed = 5f;
		public float _runCycleLegOffset = 0f;
		//test
		public float _pickupRadius; //area around player which they can pickup WorldItems

		[Header("Dodge")]
		public float _rollSpeed = 10f;
		public float _rollMultiplier = 1f;
		public AnimationCurve _rollCurve;
		public float _dodgeStaminaCostPercentage = 0.30f; //for now hardcoded, but this will be affected by endurance

		[Header("Jump")]
		public float _jumpPower = 12f;
		public float _jumpLeapStaminaCost = 10f;
		public float _gravityMultiplier = 2f;
		public float _originalGroundCheckDistance = 0.5f; //original
		public float _groundCheckOffset = 0.7f; //might not be needed -- start raycast down from this distance off ground

		[Header("Combat")]
		public float _parryOffset = 1.4f; //tweak
		public float _backstabOffset = 1.4f; //tweak
		public float _maxKickWindow = 0.15f; //time given to kick
		public float _moveAmountThreshold = 0.2f; //for kicking -- rename!
	}
}