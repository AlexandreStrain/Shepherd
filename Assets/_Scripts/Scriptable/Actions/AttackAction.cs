using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[CreateAssetMenu(menuName = "Shepherd/World Interactions/Combat/Attack Action")]
	public class AttackAction : ScriptableObject {

		[Header("Attack")]
		public StringVariable _animation;
		public bool _changeAnimationSpeed = false;
		public float _animationSpeed = 1f;

		[Header("Base Action Cost")]
		//depending on how we bring weapon stats into this, this may not be put on the action itself
		public float _staminaCost;
		public float _otherCost;

		[Header("Vulnerabilities")]
		public bool _vulnerableToParry = true;
		public bool _canParryWith = false;
		public bool _vulnerableToBackstab = false;
		public bool _canBackstabWith = false;

		[Header("Override Animations")]
		public bool _overrideDamageAnimation;
		public StringVariable _damageAnimation;
		public bool _overrideKickAnimation;
		public StringVariable _kickAnimation;
	}
}