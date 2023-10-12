using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd
{
    [CreateAssetMenu(menuName = "Shepherd/Single Instances/Level Names")]
    public class LevelNames : ScriptableObject
    {

        public List<StringVariable> _allLevelNames = new List<StringVariable>();

        private Dictionary<string, int> _levelEncyclopedia = new Dictionary<string, int>();

        public void Init()
        {
#if UNITY_EDITOR
            _allLevelNames.Clear();
            //_allLevelNames = EditorUtilities.FindAssetByType<StringVariable>();
            StringVariable[] allLevelNames = Resources.LoadAll<StringVariable>("Level Names");
            for(int i = 0; i < allLevelNames.Length; i++)
            {
                _allLevelNames.Add(allLevelNames[i]);
            }
#endif

            _levelEncyclopedia.Clear();

            for (int i = 0; i < _allLevelNames.Count; i++)
            {
                if (_levelEncyclopedia.ContainsKey(_allLevelNames[i].name))
                {
                    Debug.Log("Uh Oh, duplicate Level Name: " + _allLevelNames[i].name + " Found! \\n Follow 7 Acts, 5 Chapters, 3 Scenes Rule");
                }
                else
                {
                    _levelEncyclopedia.Add(_allLevelNames[i].name, i);
                }
            }
        }

        public string GetLevelName(string id)
        {
            StringVariable returnItem = null;
            int index = -1;

            if (_levelEncyclopedia.TryGetValue(id, out index))
            {
                returnItem = _allLevelNames[index];
            }

            if (index == -1)
            {
                Debug.Log("No Level with ID: " + id + " found!\\n Make sure it follows ActChapterScene (i.e 000 = Prologue Chapter 1 Scene 1)");
            }

            return returnItem._variable;
        }

        public int GetLevelIndex(string id)
        {
            int index = -1;

            if (_levelEncyclopedia.TryGetValue(id, out index))
            {
                return index;
            }

            if (index == -1)
            {
                Debug.Log("No Level with ID: " + id + " found!\\n Make sure it follows ActChapterScene (i.e 000 = Prologue Chapter 1 Scene 1)");
                index = 1;
            }

            return index;
        }
    }
}
