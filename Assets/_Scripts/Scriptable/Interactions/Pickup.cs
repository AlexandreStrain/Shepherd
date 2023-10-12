using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[CreateAssetMenu(menuName = "Shepherd/World Interactions/Pickup Interaction")]
	public class Pickup : WorldInteraction {

		public EquipmentData _items;

		public override void Interact (CharacterStateController audience) {
			if (_items != null && _items._data.Count != 0) {
				if (audience._myBrain is InputController) { // is StateController) { or PLayer
					//Update Player's inventory
					StateController sControl = (StateController)audience;
					//sControl._wResourceControl.CopyInventoryFromEquipData (_items);
					sControl._invControl.UpdateRuntimeInventory (_items._data);


					//Update UI -- this should be a call into information feed's script or at the very least UserInterfaceController should handle this
					for (int i = 0; i < _items._data.Count; i++) {
                        AddToFeed(_items._data[i]);
                    }
				} else if (audience._myBrain is EnemyAIController) { // is EnemyStateController) {
					Debug.Log("enemy picked up the item...");
				}
			}
			base.Interact (audience);
		}

       private void AddToFeed(WorldItem item)
        {
            UserInterfaceController.Instance.OpenInfoFeed();
            GameObject gObject = Instantiate(UserInterfaceController.Instance._informationFeed._infoGrid._template) as GameObject;
            gObject.transform.SetParent(UserInterfaceController.Instance._informationFeed._infoGrid._grid);
            InfoFeedData message = gObject.GetComponentInChildren<InfoFeedData>();
            message._infoImage.sprite = item._itemHUDIcon;
            message._description.text = item._itemName;
            message._action.text = (AllEnums.InfoFeedType.Acquired).ToString();
            message._timeStamp.text = GameSessionController.Instance._dayNightCycle._timeText.text;
            gObject.SetActive(true);
            UserInterfaceController.Instance._informationFeed.AddToInfoFeed(message);
        }
	}
}