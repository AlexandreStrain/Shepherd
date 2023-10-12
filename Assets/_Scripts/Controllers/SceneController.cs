using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;
using Shepherd;

public class SceneController : Singleton<SceneController> {

	public GameObject _menuCamera;
	public MyScenes[] _myScenes;
	private Dictionary<string, int> _levelsDictionary = new Dictionary<string, int> ();

	public string _referencesScene = "References";
	public string _startScene = "TestScene1";

	void Init() {
		for (int i = 0; i < _myScenes.Length; i++) {
			if (!_levelsDictionary.ContainsKey (_myScenes [i]._primaryScene)) {
				_levelsDictionary.Add (_myScenes [i]._primaryScene, i);
			}
		}

		StartCoroutine(LoadScene(_referencesScene, LoadSceneMode.Additive));
	}

	public void PressStartGame() {
		StartCoroutine ("StartGameRoutine");
	}

    public void LoadGameScene(string targetScene)
    {
       // if (GameSessionController.Instance._previousScene != GameSessionController.Instance._currentScene)
        //{
         //   SceneManager.UnloadSceneAsync(GameSessionController.Instance._previousScene);
        //}
        int currentScene = GameSessionController.Instance._wResources._levelNames.GetLevelIndex(targetScene);
        StartCoroutine(StartLoadingScene(currentScene));
        if (SceneManager.GetSceneByBuildIndex(GameSessionController.Instance._wResources._levelNames.GetLevelIndex(GameSessionController.Instance._previousScene)).isLoaded)
        {
            Debug.Log("WTF");
            SceneManager.UnloadSceneAsync(GameSessionController.Instance._wResources._levelNames.GetLevelIndex(GameSessionController.Instance._previousScene));
        }
    }

	public void LoadScene(string targetScene) { 
		MyScenes scenes = GetMyScene (targetScene);
		if (scenes._isLoaded) {
			return;
		}

		StartCoroutine (LoadScene (targetScene, LoadSceneMode.Additive, false));
		scenes._isLoaded = true;
	}

	public void UnloadScene(string targetScene) {
		MyScenes scenes = GetMyScene (targetScene);

		if (scenes._isLoaded) {
			scenes._isLoaded = false;
		} else {
			return;
		}

		SceneManager.UnloadSceneAsync (targetScene);
	}

	private int SceneStringToInt(string sceneID) {
		int index = -1;
		_levelsDictionary.TryGetValue (sceneID, out index);
		return index;
	}

	private MyScenes GetMyScene(string iD) {
		int index = SceneStringToInt (iD);
		return _myScenes [index];
	}

	//artifical loading
	IEnumerator StartGameRoutine() {
		UserInterfaceController.Instance._outGameMenus.SetActive (false);
		yield return new WaitForSeconds (0.2f); //artifical loading
		_menuCamera.SetActive (false);

		MyScenes targetScene = GetMyScene (_startScene);
		targetScene._isLoaded = true;

		yield return LoadScene (targetScene._lightData, LoadSceneMode.Additive, true);
		yield return LoadScene (targetScene._primaryScene, LoadSceneMode.Additive, false);

		if (!string.IsNullOrEmpty (targetScene._nextScene)) {
			yield return LoadScene (targetScene._nextScene, LoadSceneMode.Additive, false);
			MyScenes nextScene = GetMyScene (targetScene._nextScene);
			nextScene._isLoaded = true;
		}

		if (!string.IsNullOrEmpty (targetScene._previousScene)) {
			yield return LoadScene (targetScene._previousScene, LoadSceneMode.Additive, false);
			MyScenes prevScene = GetMyScene (targetScene._previousScene);
			prevScene._isLoaded = true;
		}
		yield return new WaitForSeconds (0.5f); //artifical loading

		//GameSessionController.Instance.InitGameSession();
	}

    IEnumerator StartLoadingScene(int targetScene)
    {
        UserInterfaceController.Instance._inGameMenus.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        yield return SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);
        GameSessionController.Instance.Init();
    }

	IEnumerator LoadScene(string targetScene, LoadSceneMode mode, bool isActiveScene = false) {
		yield return SceneManager.LoadSceneAsync (targetScene, mode);
		if (isActiveScene) {
			SceneManager.SetActiveScene (SceneManager.GetSceneByName(targetScene));
		}
	}
}

[System.Serializable]
public class MyScenes {
	public string _primaryScene;
	public string _nextScene;
	public string _previousScene;
	public string _lightData; //???
	public bool _isLoaded;
}