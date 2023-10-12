using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	public class ConsumableAction : ScriptableObject {
		public Consumable _parent;

		public virtual void UseItemEffect(CharacterStateController recipient) {
		}
	}
}