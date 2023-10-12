using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[CreateAssetMenu(menuName = "Shepherd/World Interactions/Combat/Spell Action/Breath")]
	public class BreathSpell : SpellAction {

		public GameObject _particlePrefab;
		public GameObject _effectPrefab;

		public override void CastSpell(CharacterStateController caster) {
			GameObject gObject = Instantiate (_particlePrefab);
			if (caster._currentSpellObject != null) {
				Destroy (caster._currentSpellObject);
			}
			caster._currentSpellObject = gObject;

			Vector3 targetPos = caster._myTransform.position;
			targetPos += (caster._myTransform.forward) * 0.5f;
			targetPos.y += 1.25f;

			gObject.transform.SetParent (caster._myTransform);
			gObject.transform.position = targetPos;
			gObject.transform.rotation = caster._myTransform.rotation;
			ParticleHook pHook = gObject.GetComponent<ParticleHook> ();

			caster._states._isIKEnabled = true;
			SpellBreathCollider sbCollider = caster._invControl._spellBreathCollider.GetComponent<SpellBreathCollider> ();
			sbCollider._parent = this;

			caster._spellCastStart = caster._invControl.OpenSpellBreathCollider;
			caster._spellCastLoop = pHook.EmitParticle;
			caster._spellCastLoop += caster._body._stamina.Decay;

			caster._spellCastStop = caster._invControl.CloseSpellBreathCollider;
			caster._spellCastStop += pHook.Cleanup;
			base.CastSpell(caster);
		}
			
		public override void UseSpellEffect(CharacterStateController recipient) {
			switch (_spellClass) {
			case AllEnums.SpellClass.Fire:
				if (recipient._spellEffectLoop == null) {
					recipient._spellEffectLoop = recipient.HandleAffliction;
				}
				break;
			case AllEnums.SpellClass.Wind:
				break;
			case AllEnums.SpellClass.Light:
				break;
			case AllEnums.SpellClass.Water:
				Debug.Log ("Get Soaked");
				if (recipient._spellEffectLoop == null) {
					recipient._spellEffectLoop = recipient.HandleAffliction;
				}
				break;
			case AllEnums.SpellClass.Earth:
				break;
			case AllEnums.SpellClass.Dark:

				break;
			case AllEnums.SpellClass.Magic:
				break;
			case AllEnums.SpellClass.None:
			default:
				break;
			}
		}
	}
}
