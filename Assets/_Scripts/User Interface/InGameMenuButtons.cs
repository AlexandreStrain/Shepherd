using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Shepherd;

public class InGameMenuButtons : MonoBehaviour
{
    public InGameMenus _inGameMenu;

    public void NavigateToInMenu(int state)
    {
        _inGameMenu.NavigateInGameMenus(state);
    }

    public void SelectAllInFlockOverview()
    {
        for (int i = 0; i < _inGameMenu._flockOverviewPanel._allFlockUI.Count; i++)
        {
            _inGameMenu._flockOverviewPanel._allFlockUI[i].SetActive(true);
        }
        _inGameMenu._flockOverviewPanel._allFlockScrollbar.value = 1;
        _inGameMenu._flockOverviewPanel._allFlockScrollbar.onValueChanged.Invoke(_inGameMenu._flockOverviewPanel._allFlockScrollbar.value);
    }

    public void SelectStarredFlockOverview()
    {
        for (int i = 0; i < _inGameMenu._flockOverviewPanel._allFlockUI.Count; i++)
        {
            EquipmentSlot eqSlot = _inGameMenu._flockOverviewPanel._allFlockUI[i].GetComponent<EquipmentSlot>();
            bool important = GameSessionController.Instance._wResources._runtimeReferences._flock[eqSlot._itemPosition]._biography._currentBioCard._isMarkedAsImportant;
            if (important)
            {
                _inGameMenu._flockOverviewPanel._allFlockUI[i].SetActive(true);
            } else
            {
                _inGameMenu._flockOverviewPanel._allFlockUI[i].SetActive(false);
            }
        }
        _inGameMenu._flockOverviewPanel._allFlockScrollbar.value = 1;
        _inGameMenu._flockOverviewPanel._allFlockScrollbar.onValueChanged.Invoke(_inGameMenu._flockOverviewPanel._allFlockScrollbar.value);
    }

    public void SelectGenderFlockOverview(bool isMale)
    {
        AllEnums.Gender gender = (isMale) ? AllEnums.Gender.Male : AllEnums.Gender.Female;
        for (int i = 0; i < _inGameMenu._flockOverviewPanel._allFlockUI.Count; i++)
        {
            EquipmentSlot eqSlot = _inGameMenu._flockOverviewPanel._allFlockUI[i].GetComponent<EquipmentSlot>();
            Biography sheepBio = GameSessionController.Instance._wResources._runtimeReferences._flock[eqSlot._itemPosition]._biography;
            if (sheepBio._gender == gender)
            {
                _inGameMenu._flockOverviewPanel._allFlockUI[i].SetActive(true);
            }
            else
            {
                _inGameMenu._flockOverviewPanel._allFlockUI[i].SetActive(false);
            }
        }
        _inGameMenu._flockOverviewPanel._allFlockScrollbar.value = 1;
        _inGameMenu._flockOverviewPanel._allFlockScrollbar.onValueChanged.Invoke(_inGameMenu._flockOverviewPanel._allFlockScrollbar.value);
    }

    public void MarkImportant()
    {
        Biography bio = null;
        List<CharacterStateController> allFlock = GameSessionController.Instance._wResources._runtimeReferences._flock;
        for (int i = 0; i < allFlock.Count; i++)
        {
            if (string.Equals(allFlock[i]._biography._name, _inGameMenu._flockOverviewPanel._name.text))
            {
                bio = allFlock[i]._biography;
            }
        }
        if(bio != null)
        {
            bio._currentBioCard._isMarkedAsImportant = _inGameMenu._flockOverviewPanel._starSheep.isOn;
        }
    }

    public void SetupRenameSheep()
    {
        _inGameMenu._flockOverviewPanel._isRenaming = true;
        _inGameMenu._flockOverviewPanel._renameSheep.gameObject.SetActive(true);
        _inGameMenu._flockOverviewPanel._renameSheep.text = "";
    }

    public void RenameSheep()
    {
        _inGameMenu._flockOverviewPanel._renameSheep.gameObject.SetActive(false);
        string newName = _inGameMenu._flockOverviewPanel._renameSheep.text;
        if (newName.Length == 0)
        {
            _inGameMenu._flockOverviewPanel._isRenaming = false;
            return;
        }

        Biography oldBio = null;
        Biography checkBio = null;
        List<CharacterStateController> allFlock = GameSessionController.Instance._wResources._runtimeReferences._flock;
        for (int i = 0; i < allFlock.Count; i++)
        {
            if (string.Equals(allFlock[i]._biography._name, _inGameMenu._flockOverviewPanel._name.text)) {
                oldBio = allFlock[i]._biography;
            }
            if (string.Equals(allFlock[i]._biography._name, newName)) {
                checkBio = allFlock[i]._biography;
            }
        }

        //check to see if name exists
        if (checkBio != null)
        {
            Debug.LogWarning("Name already exists");
        }
        else if (oldBio != null && checkBio != oldBio)
        {
            oldBio._name = newName;
            oldBio._currentBioCard._nameOfCharacter = newName;
        }

        
        _inGameMenu._flockOverviewPanel._isRenaming = false;
        _inGameMenu.LoadEquipment();
        _inGameMenu.UpdateSheepSlot();
    }
}
