using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Shepherd {
	public class DayNightCycle : MonoBehaviour {

		public static DayNightCycle _singleton;
        [SerializeField]
		public TimeSpan _currentTime;
		public Transform _sunTransform;
		public Transform _moonTransform;

		public Sprite _sunSprite;
		public Light _sunLight;

		public Sprite _moonSprite;
		public Light _moonLight;


		public Text _timeText;
		public Image _timeOfDayImage; //affected by weather;
		public int _days; //not necessary

		public float _sunIntensity;
		public float _moonIntensity;
		public Color _fogDay = Color.gray;
		public Color _fogNight = Color.black;
		public Material _skyBox;

		public int _timeSpeed; //60 is every second
		public bool _pauseTime;
		public bool _setTime;
		public AllEnums.TimeOfDay _setToTime;

		private const int _dayInSeconds = 86400;
        public int DayInSeconds { get { return _dayInSeconds; } }
		private const int _dayInMinutes = 1440;
		private const int _dayInHours = 24;
        public int DayInHours { get { return _dayInHours; } }
		private const string _dayTime = "AM";
		private const string _nightTime = "PM";
		

		// Update is called once per frame
		void Update () {
            if (!GameSessionController.Instance._successfulLaunch)
            {
                return;
            }

            if (!_pauseTime && !GameSessionController.Instance._endOfDay) {
                if (!_setTime) {
                    ChangeTime();
                } else {
                    _setTime = false;
                    if ((int)_setToTime > 0) {
                        GameSessionController.Instance._gameTimeOfDay = (_dayInSeconds / _dayInHours) * (int)_setToTime;
                    } else {
                        GameSessionController.Instance._gameTimeOfDay = 0;
                    }
                }
            }
		}

		public void ChangeTime() {
            GameSessionController.Instance._gameTimeOfDay += Time.deltaTime * _timeSpeed;
			if (GameSessionController.Instance._gameTimeOfDay > _dayInSeconds) {
				_days++;
                GameSessionController.Instance._gameTimeOfDay = 0f;
			}
			_currentTime = TimeSpan.FromSeconds (GameSessionController.Instance._gameTimeOfDay);
			string[] tempTime = _currentTime.ToString().Split(":"[0]);
			_timeText.text = tempTime [0] + ":" + tempTime [1];

			_sunTransform.rotation = Quaternion.Euler (new Vector3 (((GameSessionController.Instance._gameTimeOfDay - (_dayInSeconds/4)) / _dayInSeconds * 360f), 0f, 0f));
			_moonTransform.rotation = Quaternion.Euler (new Vector3 (((GameSessionController.Instance._gameTimeOfDay - (_dayInSeconds/4)) / _dayInSeconds * 360f)+180f, 0f, 0f));
			if (GameSessionController.Instance._gameTimeOfDay <= (_dayInSeconds / 2f)) {
				RenderSettings.fogColor = Color.Lerp (_fogNight, _fogDay, _sunIntensity);
				_sunIntensity = 1f - ((_dayInSeconds / 2f) - GameSessionController.Instance._gameTimeOfDay) / (_dayInSeconds / 2f);
				_timeText.text += " " + _dayTime;

				_moonIntensity = (1f - (GameSessionController.Instance._gameTimeOfDay - (_dayInSeconds / 2f)) / (_dayInSeconds / 2f)) - 1f;
			} else {
				RenderSettings.fogColor = Color.Lerp (_fogDay, _fogNight, _moonIntensity);
				_sunIntensity = 1f - (GameSessionController.Instance._gameTimeOfDay - (_dayInSeconds / 2f)) / (_dayInSeconds / 2f);
				_timeText.text += " " + _nightTime;

				_moonIntensity = (1f - ((_dayInSeconds / 2f) - GameSessionController.Instance._gameTimeOfDay) / (_dayInSeconds / 2f)) - 1f;
			}
			//RenderSettings.fog = true;
			//RenderSettings.fogColor = Color.Lerp (_fogNight, _fogDay, _sunIntensity * _sunIntensity);
			//RenderSettings.fogDensity = 

			if (GameSessionController.Instance._gameTimeOfDay <= (_dayInSeconds / 4f) || GameSessionController.Instance._gameTimeOfDay >= (_dayInSeconds / 4f) * 3f) {
				RenderSettings.fog = true;
				float atmosphere = 1f + _sunIntensity;
				atmosphere = Mathf.Clamp(atmosphere, 1f, 1.65f); 
				RenderSettings.skybox.SetFloat ("_AtmosphereThickness", atmosphere);
				RenderSettings.ambientIntensity = Mathf.Lerp(1f, 2f, Time.deltaTime);
				//RenderSettings.ambientIntensity = Mathf.Lerp(0.5f, 1f, Time.deltaTime);
				_timeOfDayImage.sprite = _moonSprite;
                GameSessionController.Instance._MorningOrNight = "Night";
			} else {
				RenderSettings.fog = false;
				float atmosphere = (1f - _sunIntensity) + 1f;
				atmosphere = Mathf.Clamp(atmosphere, 1f, 1.65f); 
				RenderSettings.skybox.SetFloat ("_AtmosphereThickness",  atmosphere);
				RenderSettings.ambientIntensity = Mathf.Lerp(2f, 1f, Time.deltaTime);
				//RenderSettings.ambientIntensity = Mathf.Lerp(1f, 0.5f, Time.deltaTime);
				_timeOfDayImage.sprite = _sunSprite;
                GameSessionController.Instance._MorningOrNight = "Day";
            }


			_sunLight.intensity = _sunIntensity;
			_moonLight.intensity = _moonIntensity;
		}
	}
}