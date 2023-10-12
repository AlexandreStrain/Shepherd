using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Shepherd
{
    [System.Serializable]
    public class LevelController : MonoBehaviour
    {
        private LockOnController _eManager;
       
        public Banner _playerBanner;
        public bool _spawnFlock;
        public GameObject _flockSpawn;

        public GameObject[] _enemies;

        public LevelSceneAssets[] _levelScenes;

        //_eManager = GetComponent<LockOnController>();

        public void LoadLevelScene(int sceneIndex)
        {
            LevelSceneAssets currentScene = _levelScenes[sceneIndex];
            if (GameSessionController.Instance.currentData != null)
            {
                GameSessionController.Instance._gameTimeOfDay = GameSessionController.Instance.currentData._currentTime;
            } else
            {
                GameSessionController.Instance._gameTimeOfDay = currentScene._currentTime;
            }

            for (int i = 0; i < currentScene._spawnPoints.Length; i++)
            {
                SpawnPoint sPoint = currentScene._spawnPoints[i].GetComponent<SpawnPoint>();
                if (sPoint)
                {
                    sPoint.Init();
                    sPoint.StartSpawning();
                } else
                {
                    Debug.LogWarning("THERE IS AN OBJECT IN SPAWNPOINTS ARRAY THAT DOES NOT HAVE SPAWNPOINT SCRIPT ATTACHED.");
                }
            }

            if (_flockSpawn)
            {
                SpawnPoint flockPoint = _flockSpawn.GetComponent<SpawnPoint>();
                if (flockPoint && _spawnFlock && GameSessionController.Instance._loadFromFile)
                {
                    flockPoint.Init();
                    flockPoint.StartSpawning(GameSessionController.Instance.currentData._allFlock);
                }
                else if (flockPoint && _spawnFlock && !GameSessionController.Instance._loadFromFile)
                {
                    flockPoint.Init();
                    flockPoint.StartSpawning();
                }
            }

            if (GameObject.FindGameObjectWithTag("Banner") != null)
            {
                //Load Up Banner
                _playerBanner = GameObject.FindGameObjectWithTag("Banner").GetComponentInChildren<Banner>();
                if (_playerBanner)
                {
                    _playerBanner.SetupOnGround();
                }
            }
            else
            {
                Debug.LogWarning("THERE IS NO BANNER IN OUR SCENE! SHEEP WILL NOT BE ABLE TO FLOCK PROPERLY");
            }

            //load up interactions
            for (int i = 0; i < currentScene._levelInteractions.Length; i++)
            {
                InteractionHook iHook = currentScene._levelInteractions[i].GetComponentInChildren<InteractionHook>();
                if (iHook)
                {
                    iHook.Setup();
                }
            }
        }

        public void SpawnEnemy()
        {
            for (int i = 0; i < _enemies.Length; i++)
            {
                GameObject gObject = Instantiate(_enemies[i], new Vector3(-17.25f, -6.125f, 2.75f), Quaternion.identity, _eManager.transform) as GameObject;
                //EnemyAIController eState = gObject.GetComponentInChildren<EnemyAIController> ();
                //eState._eStates._lockOnTransform = GameObject.FindGameObjectWithTag ("Player").transform;

                _eManager._enemyTargets.Add(gObject.GetComponentInChildren<LockOnTarget>());
            }
        }

        [System.Serializable]
        public struct LevelSceneAssets
        {
            public string name;
            public AllEnums.Origin _currentWorldLocation;
            public AllEnums.WeatherCondition _currentWeather;
            public float _currentTime;

            public Transform _playerSpawnPosition;

            public GameObject[] _spawnPoints;
            public GameObject[] _levelInteractions;
        }
    }
}