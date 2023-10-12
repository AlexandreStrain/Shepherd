using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Utilities;
using System.Linq;

namespace Shepherd {
    public class GameSavesController : Singleton<GameSavesController>
    {
        [SerializeField]
        private List<GameSaveSlotData> _allGameSlots;
        public List<GameSaveSlotData> AllSaveData { get { return _allGameSlots; } }

        public int GameFile1SavesCount;
        public int GameFile2SavesCount;
        public int GameFile3SavesCount;

        public void Init()
        {
            if (!CheckIfDirectoryExists(InputStrings.GameSaveFolder))
            {
                Debug.Log("Creating Place to Save");
                CreateDirectory(InputStrings.GameSaveFolder);
            }

            if (!CheckIfDirectoryExists(InputStrings.GameSaveFolder + InputStrings.GameFile1))
            {
                Debug.Log("Creating a Place for Game 1 Save Files");
                CreateDirectory(InputStrings.GameSaveFolder + InputStrings.GameFile1);
            }

            if (!CheckIfDirectoryExists(InputStrings.GameSaveFolder + InputStrings.GameFile2))
            {
                Debug.Log("Creating a Place for Game 2 Save Files");
                CreateDirectory(InputStrings.GameSaveFolder + InputStrings.GameFile2);
            }

            if (!CheckIfDirectoryExists(InputStrings.GameSaveFolder + InputStrings.GameFile3))
            {
                Debug.Log("Creating a Place for Game 3 Save Files");
                CreateDirectory(InputStrings.GameSaveFolder + InputStrings.GameFile3);
            }

            if(_allGameSlots == null)
            {
                _allGameSlots = new List<GameSaveSlotData>();
            } else
            {
                _allGameSlots.Clear();
            }
           
            InitAllGameFileSaves();
        }

        private void InitAllGameFileSaves()
        {
            string[] fromFile1 = Directory.GetFiles(Application.persistentDataPath + InputStrings.GameSaveFolder + InputStrings.GameFile1);
            string[] fromFile2 = Directory.GetFiles(Application.persistentDataPath + InputStrings.GameSaveFolder + InputStrings.GameFile2);
            string[] fromFile3 = Directory.GetFiles(Application.persistentDataPath + InputStrings.GameSaveFolder + InputStrings.GameFile3);

            string[] allGameFiles = new string[fromFile1.Length + fromFile2.Length + fromFile3.Length];
            fromFile1.CopyTo(allGameFiles, 0);
            fromFile2.CopyTo(allGameFiles, fromFile1.Length);
            fromFile3.CopyTo(allGameFiles, fromFile1.Length + fromFile2.Length);


            //TODO: THIS NEEDS TO BE FOR MOST RECENT SAVE
            GameFile1SavesCount = fromFile1.Length;
            GameFile2SavesCount = fromFile2.Length;
            GameFile3SavesCount = fromFile3.Length;

            BinaryFormatter bFormatter = new BinaryFormatter();

            if(allGameFiles.Length == 0)
            {
                GameSessionController.Instance._firstLaunch = true;
            } else
            {
                GameSessionController.Instance._firstLaunch = false;
            }

            for (int i = 0; i < allGameFiles.Length; i++)
            {
                //Debug.Log(allGameFiles[i]);
                GameSaveSlotData slotData = new GameSaveSlotData();

                FileStream file = File.Open(allGameFiles[i], FileMode.Open);
                JsonUtility.FromJsonOverwrite((string)bFormatter.Deserialize(file), slotData);
                _allGameSlots.Add(slotData);
                file.Close();
            }
        }
        
        public GameSaveSlotData GetCurrentSave(int saveNumber, AllEnums.GameFileState saveFile)
        {
            for (int i = 0; i < _allGameSlots.Count; i++)
            {
                if (_allGameSlots[i]._saveNumber == saveNumber && _allGameSlots[i]._gameFile == saveFile) { 
                    return _allGameSlots[i];
                }
            }
            return null;
        }

        public void LoadGame(GameSaveSlotData slot)
        {
            Debug.Log("From Gave File : " + slot._gameFile + " | From Slot : " + slot._saveNumber);
            for(int i = 0; i < slot._network.Count; i++)
            {
                Debug.Log(slot._network[i]._nameOfCharacter + " is " + slot._network[i]._verbalIndex + " in their dialogue!");
            }
            /* SpawnPoint flockSpawn = GameSessionController.Instance._levelControl._flockSpawn.GetComponent<SpawnPoint>();
             flockSpawn._numberOfCharacters = slot._allFlock.Count;
             GameSessionController.Instance._levelControl._spawnFlock = true;*/

            //GameSessionController.Instance._previousScene = "" + GameSessionController.Instance._currentAct + GameSessionController.Instance._currentChapter + GameSessionController.Instance._currentScene;

            GameSessionController.Instance._currentAct = slot._actNumber;
            GameSessionController.Instance._currentChapter = slot._chapterNumber;
            GameSessionController.Instance._currentScene = slot._sceneNumber;
            GameSessionController.Instance._gameTimeOfDay = 28800f;
            GameSessionController.Instance._sceneControl.LoadGameScene("" + slot._actNumber + slot._chapterNumber + slot._sceneNumber);
            
            /*for(int i = 0; i < slot._allFlock.Count; i++)
            {

            }*/
            Debug.Log("Loading......");
        }

        public void SaveGame(GameSaveSlotData slot)
        {
            BinaryFormatter bFormatter = new BinaryFormatter();
            //first, choose the place to save based off of game file
            if (CheckIfDirectoryExists(InputStrings.GameSaveFolder + "/" + slot._gameFile + (InputStrings.SaveSlot + slot._saveNumber)))
            {
                Debug.Log("Overwriting save in " + slot._gameFile);
                string filePath = InputStrings.GameSaveFolder + "/" + slot._gameFile;

                DeleteFile(filePath + (InputStrings.SaveSlot + slot._saveNumber));

                FileStream file = CreateFile(filePath, (InputStrings.SaveSlot + slot._saveNumber), InputStrings.SaveFormat);
                var json = JsonUtility.ToJson(slot);
                bFormatter.Serialize(file, json);
                file.Close();
            }
            else
            {
                Debug.Log("Saving to " + slot._gameFile);
                string filePath = InputStrings.GameSaveFolder + "/" + slot._gameFile;
                FileStream file = CreateFile(filePath, (InputStrings.SaveSlot + slot._saveNumber), InputStrings.SaveFormat);
                var json = JsonUtility.ToJson(slot);
                bFormatter.Serialize(file, json);
                file.Close();
            }
       

        }

        public void DeleteSaveSlot(GameSaveSlotData slot)
        {
            string filePath = InputStrings.GameSaveFolder + "/" + slot._gameFile + (InputStrings.SaveSlot + slot._saveNumber) + InputStrings.SaveFormat;
            if (CheckIfFileExists(filePath)) {
                Debug.Log("Deleting Save " + slot._saveNumber + " in " + slot._gameFile);
                DeleteFile(filePath);
            } else
            {
                Debug.Log("COULD NOT FIND SAVE SLOT" + slot._saveNumber + " TO DELETE IN " + slot._gameFile);
            }
        }

        public bool CheckIfDirectoryExists(string filePath)
        {
            return Directory.Exists(Application.persistentDataPath + filePath);
        }

        public bool CheckIfFileExists(string filePath)
        {
            return File.Exists(Application.persistentDataPath + filePath);
        }

        public void CreateDirectory(string dirName)
        {
            Directory.CreateDirectory(Application.persistentDataPath + dirName);
        }

        public FileStream CreateFile(string filePath, string fileName, string fileFormat)
        {
            return File.Create(Application.persistentDataPath + filePath + fileName + fileFormat);
        }

        public FileStream OpenFile(string filePath, FileMode mode)
        {
            return File.Open(Application.persistentDataPath + filePath, mode);
        }

        public void DeleteFile(string filePath)
        {
            File.Delete(Application.persistentDataPath + filePath);
        }
    }
}