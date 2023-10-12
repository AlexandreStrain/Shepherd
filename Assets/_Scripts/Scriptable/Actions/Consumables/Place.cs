using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[CreateAssetMenu(menuName = "Shepherd/World Interactions/World Item Actions/Place")]
	public class Place : ConsumableAction {

		public GameObject _modelToPlace;

		public override void UseItemEffect (CharacterStateController recipient) {
			if (recipient._invControl._currentConsumable != null) {
                Debug.Log("Placing..." + recipient._invControl._currentConsumable._instance._itemName);
				GameObject gObject = Instantiate (_modelToPlace) as GameObject;
				gObject.name = "Banner";
				gObject.transform.position = new Vector3 (recipient._myTransform.position.x, recipient._myTransform.position.y, recipient._myTransform.position.z);
				Banner newlyPlacedBanner = gObject.GetComponentInChildren<Banner> ();
				newlyPlacedBanner._myEvents.Response (1);
				recipient._animator.SetBool (AnimationStrings.IsUsingItem, false);
				recipient._states._isUsingItem = false;

                //removed placed item as current consumable in recipients hands
                recipient._invControl.UnequipConsumable(recipient._invControl._currentConsumable);
            }
		}
	}
}