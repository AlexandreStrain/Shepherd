using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Shepherd {
	public class UpdateNpcUI : GameEventListener {

		public Transform _parentCanvas;
		public GameObject _targetCursor;
		public GameObject _nameTag;
		public Text _nameText;
		public CharacterStateController _character;

		public GameObject _healthPanel;
		public float _visualizerSpeed = 1.5f;
		public Slider _healthSlider;
		public Slider _healthVisualizer;
		public Slider _courageSlider;

		public GameObject _bodyNeedsPanel;
		public Text _thirstText;
		public Text _hungerText;
		public Text _pleasureText;

		private WorldResourceController _wRControl;

		public void SetCharacter(CharacterStateController toCharacter) {
			_character = toCharacter;

			_healthSlider.maxValue = toCharacter._body._health.GetMaximum();
			_healthVisualizer.maxValue = toCharacter._body._health.GetMaximum();
			_courageSlider.maxValue = toCharacter._body._courage.GetMaximum();

			switch (toCharacter._biography._faction) {
			case AllEnums.Faction.None: //0
				_bodyNeedsPanel.SetActive (false);
				break;
			case AllEnums.Faction.Citizen: //1
				_bodyNeedsPanel.SetActive (false);
				break;
			case AllEnums.Faction.Saved: //2
				break;
			case AllEnums.Faction.Wild: //3
				_bodyNeedsPanel.SetActive (false);
				break;
			case AllEnums.Faction.Fallen: //4
				_bodyNeedsPanel.SetActive (false);
				break;
			}
			_parentCanvas = GameObject.FindGameObjectWithTag ("WorldCanvas").transform;
			this.gameObject.transform.SetParent (_parentCanvas);

			//test
			_wRControl = Resources.Load ("WorldResourceControl") as WorldResourceController;

		}

		public override void Response() {
            if (_character._biography._faction == AllEnums.Faction.Saved)
            {
                if (_nameTag.activeInHierarchy)
                {
                    if (_character._biography._currentBioCard._hasRevealedName)
                    {
                        _nameText.text = _character._biography._name;
                        _nameText.text += (_character._biography._gender == AllEnums.Gender.Male) ? " [M]" : " [F]";
                    }
                    else
                    {
                        _nameText.text = "???";
                    }
                }


                if (_bodyNeedsPanel.activeInHierarchy)
                {
                    _thirstText.text = _character._body._thirst.GetMeter().ToString();
                    _hungerText.text = _character._body._hunger.GetMeter().ToString();
                    _pleasureText.text = _character._body._pleasure.GetMeter().ToString();
                }
            }

           

			if (_healthPanel.activeInHierarchy) {
				_healthVisualizer.value = Mathf.Lerp (_healthVisualizer.value, _character._body._health.GetMeter (), _character._delta * _visualizerSpeed);
				_healthSlider.value = Mathf.Lerp(_healthSlider.value, _character._body._health.GetMeter (), _character._delta * _visualizerSpeed * 2f);

				_courageSlider.value =  Mathf.Lerp(_courageSlider.value, _character._body._courage.GetMeter (), _character._delta * _visualizerSpeed);

				if (_character._biography._faction == AllEnums.Faction.Saved) {
					if (_character._attackedBy != null) {
						_wRControl._runtimeReferences._flockAlertSystem = AllEnums.FlockAlert.Danger;
					} else {
						if (_wRControl._runtimeReferences._flockAlertSystem == AllEnums.FlockAlert.Danger) {
							_wRControl._runtimeReferences._flockAlertSystem = AllEnums.FlockAlert.None;
						}
					}
				} 
			}
			
			base.Response ();
		}

		void OnDestroy() {
			_wRControl._runtimeReferences._flockAlertSystem = AllEnums.FlockAlert.None;
		}
	}
}