using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour {

	public InventorySlotTemplate _icon;

	public AllEnums.ArmorType _slotEqType;
	public AllEnums.ItemType _slotItype;

	//not sure about this string
	public string _slotName;

	public Vector2 _slotPosition;
	//not sure about this integer
	public int _itemPosition;

	public void Init(Shepherd.InGameMenus ui) {
		_icon = GetComponent<InventorySlotTemplate> ();
		ui._eqSlotsUI.AddSlotOnList (this);
	}
}