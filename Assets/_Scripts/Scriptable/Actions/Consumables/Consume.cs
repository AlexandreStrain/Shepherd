using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[CreateAssetMenu(menuName = "Shepherd/World Interactions/World Item Actions/Consume")]
	public class Consume : ConsumableAction {

		public override void UseItemEffect (CharacterStateController recipient)
		{
            Debug.Log("Consuming... " + _parent._itemName);
			//Body Status
			recipient._body._health.Add(_parent._itemStats._health);
			recipient._body._stamina.Add (_parent._itemStats._stamina);
			recipient._body._courage.Add(_parent._itemStats._baseCourage);
			recipient._body._immuneSystem.Add(_parent._itemStats._immuneSystem);

			//Body Needs
			recipient._body._hunger.Add(_parent._itemStats._hunger);
			recipient._body._thirst.Add(_parent._itemStats._thirst);
			recipient._body._sleep.Add(_parent._itemStats._sleep);
			recipient._body._waste.Add (_parent._itemStats._waste);
			recipient._body._pleasure.Add(_parent._itemStats._pleasure);
		}
	}
}