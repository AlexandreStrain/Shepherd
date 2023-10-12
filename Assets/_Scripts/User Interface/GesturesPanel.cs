using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GesturesPanel : MonoBehaviour {

	public List<GestureContainer> _gesturesList = new List<GestureContainer>();
	private Dictionary<string, int> _gestureDictionary = new Dictionary<string, int>();

	public GameObject _gesturesGrid;
	public GameObject _gestureIconTemplate;
	public RectTransform _gestureSelector;

	public int _gesturesIndex;
	//dont like this...
	public string _currentGestureAnimation;
	public bool _closeWeapons; //you can also choose which weapon is closed during the animation with another boolean
    public bool _initialized;

	public void CreateGesturesUI() {
		for (int i = 0; i < _gesturesList.Count; i++) {
			GameObject gObject = Instantiate (_gestureIconTemplate) as GameObject;
			gObject.transform.SetParent (_gesturesGrid.transform);
			gObject.transform.localScale = Vector3.one;
			gObject.SetActive (true);
			GesturesIconBase gIcon = gObject.GetComponentInChildren<GesturesIconBase> ();
			gIcon._icon.sprite = _gesturesList [i]._gestureIcon;
			gIcon._ID = _gesturesList [i]._targetAnimation;
			gIcon._gestureName.text = _gesturesList [i]._targetAnimation;
			_gesturesList [i]._iconBase = gIcon;
		}
		_gesturesGrid.SetActive (false);
		_gestureSelector.gameObject.SetActive (false);
		_gesturesIndex = _gesturesList.Count;
		SelectGesture (true);
	}

	public void SelectGesture(bool positive) {
		if (positive) {
			_gesturesIndex++;
		} else {
			_gesturesIndex--;
		}

		if (_gesturesIndex < 0) {
			_gesturesIndex = _gesturesList.Count - 1;
		} 
		if (_gesturesIndex > _gesturesList.Count - 1) {
			_gesturesIndex = 0;
		}

		GesturesIconBase gIBase = _gesturesList [_gesturesIndex]._iconBase;
		_gestureSelector.transform.SetParent (gIBase.transform);
		_gestureSelector.anchoredPosition = Vector2.zero;

		_currentGestureAnimation =_gesturesList [_gesturesIndex]._targetAnimation;
		_closeWeapons = _gesturesList [_gesturesIndex]._closeWeapons;
	}

	public void HandleGesturesMenu(bool isOpen) {
		if (isOpen) {
			if (!_gesturesGrid.activeInHierarchy) {
				_gesturesGrid.SetActive (true);
				_gestureSelector.gameObject.SetActive (true);
			}
		} else {
			if (_gesturesGrid.activeInHierarchy) {
				_gesturesGrid.SetActive (false);
				_gestureSelector.gameObject.SetActive (false);
			}
		}
	}

	public GestureContainer GetGesture (string gID) {
		int index = -1;
		if(_gestureDictionary.TryGetValue(gID, out index)) {
			return _gesturesList[index];
		}
		return null;
	}

	public void Init() {
		//UserInterfaceController._inventoryUI._gesturesPanel = this;
		for (int i = 0; i < _gesturesList.Count; i++) {
			if (_gestureDictionary.ContainsKey (_gesturesList [i]._targetAnimation)) {
				Debug.Log (_gesturesList [i]._targetAnimation + " is a duplicate");
			} else {
				_gestureDictionary.Add (_gesturesList [i]._targetAnimation, i);
			}
		}
        _initialized = true;

    }
}

[System.Serializable]
public class GestureContainer {

	public Sprite _gestureIcon;
	public string _targetAnimation;
	public GesturesIconBase _iconBase;
	public bool _closeWeapons;
}