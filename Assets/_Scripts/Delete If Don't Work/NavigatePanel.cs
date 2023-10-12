using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Shepherd {
	[System.Serializable]
	public class NavigatePanel {

		public Button _leftNavigateButton;
		public Text _leftButtonText;
		public Text _leftNavigateText;

		public Button _rightNavigateButton;
		public Text _rightButtonText;
		public Text _rightNavigateText;

		public Text _navigateTitle;
		public Image _navigateBanner;
		public Button[] _middleNavigateButtons;
		public Image[] _middleNavigateImages;

		public Text _navigateHelpText;
		public GameObject _navigateHelpTemplate;
		public Transform _navigateHelpGrid;


		public void SetupPanel(AllEnums.InventoryUIState currentState) {
			_navigateTitle.text = currentState.ToString ();
			_navigateHelpText.text = "Select an Equipment Slot...";
			if ((int)currentState == 1) {
				_leftNavigateText.text = ((AllEnums.InventoryUIState)(AllEnums.InventoryUINumber - 1)).ToString ();
			} else {
				_leftNavigateText.text = ((AllEnums.InventoryUIState)((int)currentState - 1)).ToString ();
			}
			if((int)currentState == 5) {
				_rightNavigateText.text = ((AllEnums.InventoryUIState)(1)).ToString();
			} else {
				_rightNavigateText.text = (currentState + 1).ToString();
			}
			for (int i = 0; i < _middleNavigateImages.Length; i++) {
				if (i == (int)currentState) {
					_middleNavigateImages [i].fillCenter = true;
				} else {
					_middleNavigateImages [i].fillCenter = false;
				}
			}
		}
	}
}