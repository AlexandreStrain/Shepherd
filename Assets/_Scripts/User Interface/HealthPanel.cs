using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Shepherd;

public class HealthPanel : MonoBehaviour {

	public float _minHealthSliderBarSize;
	public float _maxHealthSliderBarSize;
	public float _minStaminaSliderBarSize;
	public float _maxStaminaSliderBarSize;

	public float _visualizerSpeed = 2f;

	public Slider _healthSlider;
	public Slider _healthVisualizer;
	public Slider _staminaSlider;
	public Slider _staminaVisualizer;

	//Not sure which one of these will be visible
	public Slider _courageSlider;
	public Slider _immuneSystemSlider;
	public Slider _poiseSlider;

	public Text _playerFlockPopulation;
	public Image _playerFlockStatusImage;

	public Sprite _blankImage;

	private float _delta;
	//private float _staringHealthPanelSize = 300f; //hardcoded value!!
	//private float _maxHealthPanelSize = 600f; //hardcoded value!!

	public Image[] _playerStatusEffectsImageList;

	public Sprite[] _flockAlertSprites;

	public enum StatusSlider { Health, Stamina, Courage, Poise, Immune };

	public void Init (CharacterBody player) {

		InitGrowingBars (_healthSlider, _minHealthSliderBarSize, _maxHealthSliderBarSize, player._health.GetMaximum(), player._health.GetMeter ());
		InitGrowingBars (_healthVisualizer, _minHealthSliderBarSize, _maxHealthSliderBarSize, player._health.GetMaximum(), player._health.GetMeter ());
		InitGrowingBars (_staminaSlider, _minStaminaSliderBarSize, _maxStaminaSliderBarSize, player._stamina.GetMaximum(), player._stamina.GetMeter ());
		InitGrowingBars (_staminaVisualizer, _minStaminaSliderBarSize, _maxStaminaSliderBarSize, player._stamina.GetMaximum(), player._stamina.GetMeter ());

		UpdateBars (_courageSlider, player._courage.GetMeter (), player._courage.GetMaximum());
		UpdateBars (_immuneSystemSlider, player._immuneSystem.GetMeter (), player._immuneSystem.GetMaximum());
		UpdateBars (_poiseSlider, player._poise.GetMeter (), player._poise.GetMaximum());

		_playerFlockStatusImage.sprite = _blankImage;
		//_playerFlockStatusImage.CrossFadeAlpha (0f, 0f, false);

		_playerStatusEffectsImageList [0].sprite = _blankImage;
		_playerStatusEffectsImageList [0].CrossFadeAlpha (0f, 0f, false);
		_playerStatusEffectsImageList [1].sprite = _blankImage;
		_playerStatusEffectsImageList [1].CrossFadeAlpha (0f, 0f, false);
	}

	public void Tick(float delta, CharacterStateController player) {
		_delta = delta;
		//UpdateBars (_healthSlider, player._health.GetMeter ());

		_healthVisualizer.value = Mathf.Lerp (_healthVisualizer.value, player._body._health.GetMeter (), delta * _visualizerSpeed);
		UpdateBarsLerp (_healthSlider, player._body._health.GetMeter (), _delta);

		UpdateBarsLerp (_staminaSlider, player._body._stamina.GetMeter (), _delta);
		_staminaVisualizer.value = Mathf.Lerp (_staminaVisualizer.value, player._body._stamina.GetMeter (), delta * _visualizerSpeed);

		UpdateBars (_courageSlider, player._body._courage.GetMeter ());
		UpdateBars (_immuneSystemSlider, player._body._immuneSystem.GetMeter ());
		UpdateBars (_poiseSlider, player._body._poise.GetMeter ());

		_playerFlockPopulation.text = player._invControl._runtimeRefs._flock.Count.ToString ();
		switch (player._wRControl._runtimeReferences._flockAlertSystem) {
		case AllEnums.FlockAlert.None:
			_playerFlockStatusImage.sprite = _blankImage;
			break;
		case AllEnums.FlockAlert.Warning:
			_playerFlockStatusImage.sprite = _flockAlertSprites [0];
			break;
		case AllEnums.FlockAlert.Danger:
			_playerFlockStatusImage.sprite = _flockAlertSprites [1];
			break;
		case AllEnums.FlockAlert.Missing:
			_playerFlockStatusImage.sprite = _flockAlertSprites [2];
			break;
		}
	}
		
	private void UpdateBars(Slider sType, float currentValue, float maxValue = -1f) {
		if (maxValue != -1f) {
			sType.maxValue = maxValue;
		}
		sType.value = currentValue;
	}

	private void UpdateBarsLerp(Slider sType, float currentValue, float delta) {
		sType.value = Mathf.Lerp(sType.value, currentValue, delta * _visualizerSpeed * 4f);
	}
		
	private void InitGrowingBars(Slider sType, float minBarSize, float maxBarSize, float maxValue, float currentValue) {
		RectTransform barWidth = sType.GetComponent<RectTransform> ();

		if (maxValue > minBarSize && maxValue <= maxBarSize) {
			barWidth.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, maxValue);
		} else if (maxValue > maxBarSize) {
			barWidth.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, maxBarSize);
		} else {
			barWidth.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, minBarSize);
		}
		sType.maxValue = maxValue;
		sType.value = currentValue;
	}

	private bool IsFlockEndangered() {
		
		return false;
	}
}