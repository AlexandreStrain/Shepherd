using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	public class SpawnPoint : MonoBehaviour {

		public bool _active = true;
        public bool _triggerEvent = false;
        public bool _revealName = false;

		public float _spawnDeviance = 0f;
		public float _areaOfInfluence = 10f;

		public GameObject _CharacterToSpawn;
		public int _numberOfCharacters = 1;

        public GameObject[] _CharactersToSpawn;
        public bool _loopCharactersToSpawn;
        public bool _randomizeHeight;
        [Range(0, 4)]
        public float maxHeight;
        [Range(0, 4)]
        public float minHeight;

        private int _spawnCharacterIndex;

		public GameObject[] _population;
		public Transform _spawnGroup;

		private WorldResourceController _wRControl;

        public void Init() {
            _population = new GameObject[_numberOfCharacters];
            _wRControl = GameSessionController.Instance._wResources;
            if (_spawnGroup == null)
            {
                _spawnGroup = this.transform;
            }
            minHeight = Mathf.Clamp(minHeight, 0f, 4f);
            maxHeight = Mathf.Clamp(minHeight, 0f, 4f);
            if (minHeight > maxHeight)
            {
                maxHeight = minHeight;
            }
            _CharacterToSpawn.SetActive(true);
        }

        public void StartSpawning(List<BiocardSaveData> data)
        {
            if (_active && _CharacterToSpawn != null)
            {
                for (int j = 0; j < _numberOfCharacters; j++)
                {
                    Vector3 pos = new Vector3((this.transform.position.x + Random.Range(-_spawnDeviance, _spawnDeviance)), _spawnGroup.position.y + 0.5f, (this.transform.position.z + Random.Range(-_spawnDeviance, _spawnDeviance)));

                    if (_CharactersToSpawn.Length > 0 && _numberOfCharacters >= _CharactersToSpawn.Length && _loopCharactersToSpawn)
                    {
                        if (_spawnCharacterIndex >= _CharactersToSpawn.Length)
                        {
                            _spawnCharacterIndex = 0;
                        }
                        _population[j] = (GameObject)Instantiate(_CharactersToSpawn[_spawnCharacterIndex], pos, Quaternion.identity);
                        _spawnCharacterIndex++;
                    }
                    else if (_CharactersToSpawn.Length > 0 && _numberOfCharacters <= _CharactersToSpawn.Length && !_loopCharactersToSpawn)
                    {
                        if (_spawnCharacterIndex >= _CharactersToSpawn.Length)
                        {
                            _spawnCharacterIndex = 0;
                        }
                        _population[j] = (GameObject)Instantiate(_CharactersToSpawn[0], pos, Quaternion.identity);
                        _spawnCharacterIndex++;
                    }
                    else
                    {
                        _population[j] = (GameObject)Instantiate(_CharacterToSpawn, pos, Quaternion.identity);
                    }
                    _population[j].transform.position = pos;

                    _population[j].transform.rotation = Quaternion.LookRotation(new Vector3(Random.Range(-360f, 360f), 0f, Random.Range(-360f, 360f)));

                    _population[j].transform.SetParent(_spawnGroup);

                    EnemyAIController aiControl = _population[j].GetComponent<EnemyAIController>();

                    aiControl._character._biography._name = data[j]._nameOfCharacter;
                    aiControl._character._biography._gender = data[j]._gender;

                    _population[j].name = data[j]._nameOfCharacter;

                    aiControl._character._biography._currentBioCard = ScriptableObject.Instantiate(_wRControl._bioCardTemplate);
                    data[j].PasteDataIntoBiocard(ref aiControl._character._biography._currentBioCard);
                    aiControl._character._biography._currentBioCard._hasRevealedName = (_revealName) ? true : false;

                    aiControl._home = this;
                    int randomStart = Random.Range(100, 250);
                    aiControl._aiPattern._flockCount = randomStart;

                    if (_randomizeHeight)
                    {
                        float randHeight = Random.Range(minHeight, maxHeight);
                        Vector3 height = new Vector3(randHeight, randHeight, randHeight);
                        aiControl._character.gameObject.transform.localScale = height;
                    }

                    aiControl.Init();
                    aiControl._character._body.InitFromStats(data[j]._currentStatistics);

                    if (_triggerEvent)
                    {
                        aiControl._myGameEvent.Raise();
                        aiControl._character._biography._currentBioCard._verbalIndex = 10;
                        Debug.Log("RAISING EVENT>>>>>>> " + aiControl._character._biography._currentBioCard._verbalIndex);
                    }

                    if(GameSessionController.Instance._loadFromFile && aiControl._character._biography._faction == AllEnums.Faction.Saved)
                    {
                        if (!_wRControl._runtimeReferences._flock.Contains(aiControl._character))
                        {
                            aiControl._character._biography._currentBioCard._hasRevealedName = true;
                            Debug.Log(data[j]._nameOfCharacter);
                            aiControl._character.gameObject.SetActive(false);
                            _wRControl._runtimeReferences._flock.Add(aiControl._character);
                        }
                    } else if (aiControl._character._biography._faction == AllEnums.Faction.Saved)
                    {
                        Debug.Log(data[j]._nameOfCharacter + " -- has been added to flock");
                        _wRControl._runtimeReferences._flock.Add(aiControl._character);
                    }
                }
            }
            else if (_active && _CharactersToSpawn.Length > 0)
            {
                for (int j = 0; j < _numberOfCharacters; j++)
                {
                    Vector3 pos = new Vector3((this.transform.position.x + Random.Range(-_spawnDeviance, _spawnDeviance)), _spawnGroup.position.y + 0.5f, (this.transform.position.z + Random.Range(-_spawnDeviance, _spawnDeviance)));
                    _population[j] = (GameObject)Instantiate(_CharacterToSpawn, pos, Quaternion.identity);
                    _population[j].transform.position = pos;

                    _population[j].transform.rotation = Quaternion.LookRotation(new Vector3(Random.Range(-360f, 360f), 0f, Random.Range(-360f, 360f)));

                    _population[j].transform.SetParent(_spawnGroup);

                    EnemyAIController aiControl = _population[j].GetComponent<EnemyAIController>();
                    aiControl._character._biography._currentBioCard = ScriptableObject.Instantiate(_wRControl._bioCardTemplate);
                    data[j].PasteDataIntoBiocard(ref aiControl._character._biography._currentBioCard);

                    aiControl._character._biography._currentBioCard._hasRevealedName = (_revealName) ? true : false;

                    aiControl._home = this;
                    int randomStart = Random.Range(100, 250);
                    aiControl._aiPattern._flockCount = randomStart;

                    aiControl.Init();

                    if (GameSessionController.Instance._loadFromFile && aiControl._character._biography._faction == AllEnums.Faction.Saved)
                    {
                        if (!_wRControl._runtimeReferences._flock.Contains(aiControl._character))
                        {
                            aiControl._character._biography._currentBioCard._hasRevealedName = true;
                            Debug.Log(data[j]._nameOfCharacter);
                            _wRControl._runtimeReferences._flock.Add(aiControl._character);
                        }
                    }
                    else if (aiControl._character._biography._faction == AllEnums.Faction.Saved)
                    {
                        Debug.Log(data[j]._nameOfCharacter + " -- has been added to flock");
                        _wRControl._runtimeReferences._flock.Add(aiControl._character);
                    }
                }
            }
        }

		public void StartSpawning () {
            if (_active && _CharacterToSpawn != null) {
				for (int j = 0; j < _numberOfCharacters; j++) {
					Vector3 pos = new Vector3 ((this.transform.position.x + Random.Range (-_spawnDeviance, _spawnDeviance)), _spawnGroup.position.y + 0.5f, (this.transform.position.z + Random.Range (-_spawnDeviance, _spawnDeviance)));

                    if(_CharactersToSpawn.Length > 0 && _numberOfCharacters >= _CharactersToSpawn.Length && _loopCharactersToSpawn)
                    {
                        if(_spawnCharacterIndex >= _CharactersToSpawn.Length)
                        {
                            _spawnCharacterIndex = 0;
                        }
                        _population[j] = (GameObject)Instantiate(_CharactersToSpawn[_spawnCharacterIndex], pos, Quaternion.identity);
                        _spawnCharacterIndex++;
                    } else if (_CharactersToSpawn.Length > 0 && _numberOfCharacters <= _CharactersToSpawn.Length && !_loopCharactersToSpawn)
                    {
                        if (_spawnCharacterIndex >= _CharactersToSpawn.Length)
                        {
                            _spawnCharacterIndex = 0;
                        }
                        _population[j] = (GameObject)Instantiate(_CharactersToSpawn[0], pos, Quaternion.identity);
                        _spawnCharacterIndex++;
                    } else {
                        _population [j] = (GameObject)Instantiate (_CharacterToSpawn, pos, Quaternion.identity);
                    }
                    _population[j].transform.position = pos;

					_population [j].transform.rotation = Quaternion.LookRotation (new Vector3 (Random.Range (-360f, 360f), 0f, Random.Range (-360f, 360f)));

					_population [j].transform.SetParent (_spawnGroup);

					EnemyAIController aiControl = _population [j].GetComponent<EnemyAIController> ();
                    
                    aiControl._home = this;
					int randomStart = Random.Range (100, 250);
					aiControl._aiPattern._flockCount = randomStart;

					if(aiControl._character._biography._race == AllEnums.Race.Animal) {

						//Randomly selects gender of animal it's going to spawn if unspecified (TODO: if specified)
						int randomGender = Random.Range (0, 101);
						if (randomGender > 49) {
							aiControl._character._biography._gender = AllEnums.Gender.Female;
						} else {
							aiControl._character._biography._gender = AllEnums.Gender.Male;
						}
						string name = _wRControl._worldNames.GetRandomName (aiControl._character._biography._gender);
						aiControl._character._biography._name = name;
						aiControl._character.gameObject.name = name;
					} else
                    {
                        if (_randomizeHeight)
                        {
                            float randHeight = Random.Range(minHeight, maxHeight);
                            Vector3 height = new Vector3(randHeight, randHeight, randHeight);
                            aiControl._character.gameObject.transform.localScale = height;
                        }
                    }

					aiControl.Init ();

                    aiControl._character._biography._currentBioCard._hasRevealedName = (_revealName) ? true : false;
                    if (_triggerEvent)
                    {
                        aiControl._myGameEvent.Raise();
                        aiControl._character._biography._currentBioCard._verbalIndex = 10;
                        Debug.Log("RAISING EVENT>>>>>>> " +aiControl._character._biography._currentBioCard._verbalIndex);
                    }

                    if (GameSessionController.Instance._loadFromFile && aiControl._character._biography._faction == AllEnums.Faction.Saved)
                    {
                        if (!_wRControl._runtimeReferences._flock.Contains(aiControl._character))
                        {
                            aiControl._character._biography._currentBioCard._hasRevealedName = true;
                            _wRControl._runtimeReferences._flock.Add(aiControl._character);
                        }
                    }
                    else if (aiControl._character._biography._faction == AllEnums.Faction.Saved)
                    {
                        Debug.Log(aiControl._character._biography._name + " -- has been added to flock");
                        aiControl._character._biography._currentBioCard._hasRevealedName = true;
                        _wRControl._runtimeReferences._flock.Add(aiControl._character);
                    }
				}
			}
            else if (_active && _CharactersToSpawn.Length > 0)
            {
                for (int j = 0; j < _numberOfCharacters; j++)
                {
                    Vector3 pos = new Vector3((this.transform.position.x + Random.Range(-_spawnDeviance, _spawnDeviance)), _spawnGroup.position.y + 0.5f, (this.transform.position.z + Random.Range(-_spawnDeviance, _spawnDeviance)));
                    _population[j] = (GameObject)Instantiate(_CharacterToSpawn, pos, Quaternion.identity);
                    _population[j].transform.position = pos;

                    _population[j].transform.rotation = Quaternion.LookRotation(new Vector3(Random.Range(-360f, 360f), 0f, Random.Range(-360f, 360f)));

                    _population[j].transform.SetParent(_spawnGroup);

                    EnemyAIController aiControl = _population[j].GetComponent<EnemyAIController>();
                    
                    aiControl._home = this;
                    int randomStart = Random.Range(100, 250);
                    aiControl._aiPattern._flockCount = randomStart;

                    if (aiControl._character._biography._race == AllEnums.Race.Animal)
                    {

                        //Randomly selects gender of animal it's going to spawn if unspecified (TODO: if specified)
                        int randomGender = Random.Range(0, 101);
                        if (randomGender > 49)
                        {
                            aiControl._character._biography._gender = AllEnums.Gender.Female;
                        }
                        else
                        {
                            aiControl._character._biography._gender = AllEnums.Gender.Male;
                        }
                        string name = _wRControl._worldNames.GetRandomName(aiControl._character._biography._gender);
                        aiControl._character._biography._name = name;
                        aiControl._character.gameObject.name = name;
                    }

                    aiControl.Init();
                    aiControl._character._biography._currentBioCard._hasRevealedName = (_revealName) ? true : false;

                    if (GameSessionController.Instance._loadFromFile && aiControl._character._biography._faction == AllEnums.Faction.Saved)
                    {
                        if (!_wRControl._runtimeReferences._flock.Contains(aiControl._character))
                        {
                            aiControl._character._biography._currentBioCard._hasRevealedName = true;
                            _wRControl._runtimeReferences._flock.Add(aiControl._character);
                        }
                    }
                    else if (aiControl._character._biography._faction == AllEnums.Faction.Saved)
                    {
                        Debug.Log(aiControl._character._biography._name + " -- has been added to flock");
                        aiControl._character._biography._currentBioCard._hasRevealedName = true;
                        _wRControl._runtimeReferences._flock.Add(aiControl._character);
                    }
                }
            }
		}
	}
}