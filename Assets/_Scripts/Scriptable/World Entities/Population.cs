using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd
{
    [CreateAssetMenu(menuName = "Shepherd/Single Instances/Population")]
    public class Population : ScriptableObject
    {
        public List<Biography> _allBiographies = new List<Biography>();

        private Dictionary<string, int> _biographyEncyclopedia = new Dictionary<string, int>();

        public void Init()
        {
            #if UNITY_EDITOR
              _allBiographies = EditorUtilities.FindAssetByType<Biography>();
            #endif

            _biographyEncyclopedia.Clear();

            for (int i = 0; i < _allBiographies.Count; i++)
            {
                if (_biographyEncyclopedia.ContainsKey(_allBiographies[i].name))
                {
                    Debug.Log("Uh Oh, duplicate World Item: " + _allBiographies[i].name);
                }
                else
                {
                    _biographyEncyclopedia.Add(_allBiographies[i].name, i);
                }
            }
        }

        public Biography GetBiography(string id)
        {
            Biography returnItem = null;
            int index = -1;

            if (_biographyEncyclopedia.TryGetValue(id, out index))
            {
                returnItem = _allBiographies[index];
            }

            if (index == -1)
            {
                Debug.Log("No Biography with Name: " + id + " found!");
            }

            return returnItem;
        }
    }
}