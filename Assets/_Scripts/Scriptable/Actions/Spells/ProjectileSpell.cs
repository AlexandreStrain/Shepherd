using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[CreateAssetMenu(menuName = "Shepherd/World Interactions/Combat/Spell Action/Projectile")]
	public class ProjectileSpell : SpellAction {

		public GameObject _projectilePrefab;
		public GameObject _effectPrefab;
		//public ParticleHook _particleHook;

		public override void CastSpell(CharacterStateController caster) {
			GameObject gObject = Instantiate (_projectilePrefab);
			gObject.SetActive (true);

			Vector3 targetPos = caster._myTransform.position;
			targetPos += (caster._myTransform.forward)*0.5f;
			targetPos.y += 1.25f;

			Projectile projectile = gObject.GetComponent<Projectile>();
			projectile._parent = this;
			projectile.transform.position = targetPos;
			projectile.transform.rotation = caster._myTransform.rotation;
			projectile.Init ();
			projectile.InitOwner (caster);

			caster.EmptySpellCastDelegates ();

			base.CastSpell (caster);
		}

		public override void UseSpellEffect(CharacterStateController recipient) {
			switch (_spellClass) {
			case AllEnums.SpellClass.Fire:
				Debug.Log ("Catch fire");
				recipient._spellEffectLoop = recipient.HandleAffliction;
				break;
			case AllEnums.SpellClass.Wind:
				break;
			case AllEnums.SpellClass.Light:
				break;
			case AllEnums.SpellClass.Water:
				Debug.Log ("Get Soaked");
				recipient._spellEffectLoop = recipient.HandleAffliction;
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