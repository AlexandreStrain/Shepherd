using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Shepherd {
	public class UpdateSlotFromItem : GameEventListener {

		public Image _icon;
		public Text _text;
		public CurrentItem _targetItem;

		public override void Response() {
			RuntimeWorldItem item = _targetItem.GetRT ();
			if (item == null) {
				_icon.enabled = false;
				_icon.sprite = null;
				if (_text != null) {
					_text.text = "";
				}
			} else {
				_icon.sprite = item._instance._itemHUDIcon;
				_icon.enabled = true;
				if (_text != null) {
					_text.text = item._durability.ToString();
				}
			}

			base.Response ();
		}
	}
}