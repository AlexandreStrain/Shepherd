using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Shepherd;

public class InfoFeed : MonoBehaviour {

	public InfoBoxUI _infoGrid;
	public List<InfoFeedTemplate> _infoFeedList = new List<InfoFeedTemplate> ();
	public float _infoLifeTimer = 5;
	public float _currentTimer;
	public bool _startTimer = false;

    public Text _currentTimeOfDayText;
    public Image _currentTimeOfDayImage;

	public GameObject _infoFeedPanel;

	public void Tick(float delta) {
		if (_currentTimer >= _infoLifeTimer) {
			if (_infoFeedList.Count > 0) {
                UserInterfaceController.Instance._inMenus._weatherReportPanel.CopyOverDataFromInfoFeed(_infoFeedList[0]._data);
                Destroy (_infoFeedList [0]._data.gameObject);
				_infoFeedList.RemoveAt (0);
			}

			_startTimer = (_infoFeedList.Count > 0);
			_currentTimer = 0f;
		}
			
		if (_infoFeedList.Count <= 0) {
			_infoFeedPanel.SetActive (false);
		}

		if (_startTimer) {
			_currentTimer += delta;
		}
	}

	public void ClearFeed() {
		for (int i = 0; i < _infoFeedList.Count; i++) {
            //UserInterfaceController.Instance._inMenus._weatherReportPanel.CopyOverDataFromInfoFeed(_infoFeedList[i]._data);
            Destroy (_infoFeedList [i]._data);
		}
		_infoFeedList.Clear ();
		InfoFeedData[] infoGrid = _infoGrid._grid.GetComponentsInChildren<InfoFeedData> ();
		for (int i = 0; i < infoGrid.Length; i++) {
			Destroy(infoGrid[i].gameObject);
		}
		_currentTimer = 0f;
	}

	public void AddToInfoFeed(InfoFeedData info) {
		info.transform.SetParent (_infoGrid._grid);
		info.transform.localScale = Vector3.one;
		InfoFeedTemplate newInfo;
		newInfo._data = info;
		newInfo._data.gameObject.SetActive (true);
		_infoFeedList.Add (newInfo);
		_startTimer = true;
    }

	[System.Serializable]
	public struct InfoFeedTemplate {
		public InfoFeedData _data;
	}
}