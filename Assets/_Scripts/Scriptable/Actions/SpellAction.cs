using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	public class SpellAction : ScriptableObject {

		[Header("Spell")]
		public AllEnums.SpellClass _spellClass;
		public AllEnums.SpellType _spellType;
		public StringVariable _chargeAnimation;
		public StringVariable _castAnimation;
		public bool _changeAnimationSpeed = false;
		public float _animationSpeed = 1f;
		public float _castTime;

		[Header("Base Action Cost")]
		//depending on how we bring weapon stats into this, this may not be put on the action itself
		public float _castCost;
		public float _staminaCost;
		public float _otherCost;

		public bool InitSpell(CharacterStateController caster) {
			if (caster._body._stamina.GetMeter() < _castCost + _staminaCost) {
				return false;
			}

			switch (_spellClass) {
			case AllEnums.SpellClass.Fire:
				//Fire Spells make you more hungry as well as tire you out!
				if (caster._body._hunger.GetMeter () < _castCost + _otherCost) {
					return false;
				}
				//caster._body._hunger.Subtract (_castCost);
				break;
			case AllEnums.SpellClass.Wind:
				break;
			case AllEnums.SpellClass.Light:
				break;
			case AllEnums.SpellClass.Water:
				//Water Spells make you more thirsty as well as tire you out!
				if (caster._body._thirst.GetMeter () < _castCost + _otherCost) {
					return false;
				}
				//caster._body._thirst.Subtract (_castCost);
				break;
			case AllEnums.SpellClass.Earth:
				break;
			case AllEnums.SpellClass.Dark:
				if (caster._body._courage.GetMeter () < _castCost + _otherCost) {
					return false;
				}
				//caster._body._courage.Subtract (_castCost);
				break;
			case AllEnums.SpellClass.Magic:
				break;
			case AllEnums.SpellClass.None:
			default:
				break;
			}

			caster._body._stamina.Subtract (_castCost);
			return true;
		}

		public virtual void CastSpell(CharacterStateController caster) {
			caster._body._stamina.Subtract (_staminaCost);

			switch (_spellClass) {
			case AllEnums.SpellClass.Fire:
				//caster._body._hunger.Subtract (_otherCost);
				if (caster._spellCastLoop != null) {
					//caster._spellCastLoop += caster._body._hunger.Decay;
				}
				break;
			case AllEnums.SpellClass.Wind:
				break;
			case AllEnums.SpellClass.Light:
				break;
			case AllEnums.SpellClass.Water:
				//caster._body._thirst.Subtract (_otherCost);
				if (caster._spellCastLoop != null) {
					//caster._spellCastLoop += caster._body._thirst.Decay;
				}
				break;
			case AllEnums.SpellClass.Earth:
				break;
			case AllEnums.SpellClass.Dark:
				//Dark Spells make you more tired as well as creep you out!
				//caster._body._courage.Subtract (_otherCost);
				if (caster._spellCastLoop != null) {
					//caster._spellCastLoop += caster._body._courage.Decay;
				}
				break;
			case AllEnums.SpellClass.Magic:
				break;
			case AllEnums.SpellClass.None:
			default:
				break;
			}
		}

		public virtual void HandleSpellType(CharacterStateController recipient, float amount) {
			if (amount == 0f) {
				//
			}

			switch (_spellType) {
			case AllEnums.SpellType.Buff:
				break;
			case AllEnums.SpellType.Looping:
				Debug.Log ("Looping... ");
				recipient._body._health.Subtract (recipient._body._health._decay * amount);
				break;
			case AllEnums.SpellType.Projectile:
				Debug.Log ("Projectile...");
				recipient._body._health.Subtract (amount);
				break;
			default:
				break;
			}
		}

		public virtual void UseSpellEffect(CharacterStateController recipient) {
		}
	}
}