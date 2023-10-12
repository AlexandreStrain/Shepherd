using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	//[CreateAssetMenu(menuName = "Shepherd/World Interactions/Effects/Spell Effect")]
	public class SpellEffect : ScriptableObject {

		public AllEnums.SpellClass _spellClass;
		public AllEnums.SpellType _spellType;

		//public abstract void UseSpellEffect (CharacterStateController caster, CharacterStateController recipient);
		public virtual void UseSpellEffect (StateController caster, CharacterStateController recipient) {
		}
	}
}