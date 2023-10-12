using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[CreateAssetMenu(menuName = "Shepherd/World Interactions/Give Interaction")]
	public class Give : WorldInteraction {

		public CharacterStateController _source;
		public RuntimeConsumable _itemToGive;
        public EquipmentData _itemsToGive;

		public void Init(CharacterStateController source) {
            _source = source;
		}

		public override void Interact (CharacterStateController audience) {
            if ( (_itemToGive._instance != null || _source._wRControl.GetItem(_itemToGive._instance._itemName) != _source._wRControl.GetItem(_source._invControl._emptyItemsDefault._itemName)) && _itemsToGive == null)
            {
                GiveSingular(audience);
            } else if (_itemToGive._instance == null && _itemsToGive != null && _itemsToGive._data.Count > 0)
            {

                GiveMultiple(audience);
            }
		}

        private void GiveSingular(CharacterStateController audience)
        {
            if (audience._myBrain is InputController)
            { //is Player, or StateController
                if (_source._myBrain is EnemyAIController)
                { //is Player, or StateController
                    if (_source._wRControl.CompareWorldItem(_itemToGive._instance._itemName, _source._invControl._emptyItemsDefault._itemName))
                    {
                        return; //don't hand over your hands!
                    }
                    //RuntimeConsumable itemToGive = 

                    AddToFeed(_itemToGive._instance, true);

                    _source._invControl._currentConsumable._durability--;
                    if (_source._invControl._currentConsumable._durability == 0 || (_source._invControl._currentConsumable._instance._isQuestItem && audience._invControl._equippedItems._data.Contains(_source._invControl._currentConsumable._instance)))
                    {
                        _itemToGive._isEquipped = false;
                        _itemToGive._durability = 1f;
                                               
                        _source._invControl.UnequipConsumable(_itemToGive);
                        //destroy currently equipped
                    }
                    else
                    {
                        Shepherd.Consumable consumable = (Shepherd.Consumable)_source._invControl._currentConsumable._instance;
                        GameObject gObject1 = GameObject.Instantiate(consumable._modelPrefab) as GameObject;
                        gObject1.SetActive(false);
                        gObject1.name = consumable.name;

                        Shepherd.RuntimeConsumable newConsumable = new Shepherd.RuntimeConsumable();
                        newConsumable._rtModel = gObject1;
                        newConsumable._instance = consumable;
                        newConsumable._unbreakable = consumable._isUnbreakable;
                        newConsumable._durability = 1f;
                        newConsumable._isEquipped = true;
                        newConsumable._itemName = consumable._itemName;
                        _itemToGive = newConsumable;
                        //_source._invControl.UnequipConsumable (_itemToGive);
                    }
                    audience._invControl._runtimeRefs.RegisterRuntimeConsumable(_itemToGive);
                    audience.UpdateGameUI();
                    _itemToGive = null;
                    _enabled = false;
                }
            }
            else if (audience._myBrain is EnemyAIController)
            { // is EnemyStateController) {
                if (_source._myBrain is InputController)
                { //is Player, or StateController
                    if (_source._wRControl.CompareWorldItem(_itemToGive._instance._itemName, _source._invControl._emptyItemsDefault._itemName))
                    {
                        return; //don't hand over your hands!
                    }
                    //RuntimeConsumable itemToGive = 

                    AddToFeed(_itemToGive._instance, false);

                    _source._invControl._currentConsumable._durability--;
                    if (_source._invControl._currentConsumable._durability == 0 || (_source._invControl._currentConsumable._instance._isQuestItem && audience._invControl._equippedItems._data.Contains(_source._invControl._currentConsumable._instance)))
                    {
                        _source._invControl.UnequipConsumable(_itemToGive);
                        //destroy currently equipped
                    }
                    else
                    {
                        Shepherd.Consumable consumable = (Shepherd.Consumable)_source._invControl._currentConsumable._instance;
                        GameObject gObject1 = GameObject.Instantiate(consumable._modelPrefab) as GameObject;
                        gObject1.SetActive(false);
                        gObject1.name = consumable.name;

                        Shepherd.RuntimeConsumable newConsumable = new Shepherd.RuntimeConsumable();
                        newConsumable._rtModel = gObject1;
                        newConsumable._instance = consumable;
                        newConsumable._unbreakable = consumable._isUnbreakable;
                        newConsumable._durability = 1f;
                        newConsumable._isEquipped = true;
                        newConsumable._itemName = consumable._itemName;
                        _itemToGive = newConsumable;
                        //_source._invControl.UnequipConsumable (_itemToGive);
                    }
                    audience._invControl.UpdateRuntimeInventoryWithRuntimeItem(_itemToGive);
                    _itemToGive = null;
                    _enabled = false;
                }
            }
        }

        private void GiveMultiple(CharacterStateController audience)
        {
            if (audience._myBrain is InputController)
            { //is Player, or StateController
                if (_source._myBrain is EnemyAIController)
                { //is Player, or StateController
                    if (_itemsToGive._data.Contains(_source._invControl._emptyItemsDefault))
                    {
                        return; //don't hand over your hands!
                    }
                   
                    for (int i = 0; i < _itemsToGive._data.Count; i++)
                    {
                        AddToFeed(_itemsToGive._data[i], true);
                    }
                    audience._wRControl.CopyInventoryFromEquipData(_itemsToGive);
                    audience._invControl.WorldItemsToRuntime(_itemsToGive._data);
                    audience.UpdateGameUI();
                    _itemsToGive = null;
                    _itemToGive = null;
                    _enabled = false;

                   /* _source._invControl._currentConsumable._durability--;
                    if (_source._invControl._currentConsumable._durability == 0 || (_source._invControl._currentConsumable._instance._isQuestItem && audience._invControl._equippedItems._data.Contains(_source._invControl._currentConsumable._instance)))
                    {
                        _itemToGive._equipped = false;
                        _itemToGive._durability = 1f;
                        _source._invControl.UnequipConsumable(_itemToGive);
                        //destroy currently equipped
                    }
                    else
                    {
                        Shepherd.Consumable consumable = (Shepherd.Consumable)_source._invControl._currentConsumable._instance;
                        GameObject gObject1 = GameObject.Instantiate(consumable._modelPrefab) as GameObject;
                        gObject1.SetActive(false);
                        gObject1.name = consumable.name;

                        Shepherd.RuntimeConsumable newConsumable = new Shepherd.RuntimeConsumable();
                        newConsumable._rtModel = gObject1;
                        newConsumable._instance = consumable;
                        newConsumable._unbreakable = consumable._infiniteUse;
                        newConsumable._durability = 1f;
                        newConsumable._equipped = true;
                        _itemToGive = newConsumable;
                        //_source._invControl.UnequipConsumable (_itemToGive);
                    }
                    audience._invControl._runtimeRefs.RegisterRuntimeConsumable(_itemToGive);
                    audience.UpdateGameUI();
                    _itemToGive = null;
                    _enabled = false;*/
                }
            }
            else if (audience._myBrain is EnemyAIController)
            { // is EnemyStateController) {
                if (_source._myBrain is InputController)
                { //is Player, or StateController
                }
            }
        }

        private void AddToFeed(WorldItem rtWorldItem, bool isPlayer)
        {
            UserInterfaceController.Instance.OpenInfoFeed();
            GameObject gObject = Instantiate(UserInterfaceController.Instance._informationFeed._infoGrid._template) as GameObject;
            gObject.transform.SetParent(UserInterfaceController.Instance._informationFeed._infoGrid._grid);
            InfoFeedData message = gObject.GetComponentInChildren<InfoFeedData>();
            message._infoImage.sprite = rtWorldItem._itemHUDIcon;
            message._description.text = rtWorldItem._itemName;
            message._action.text = (isPlayer) ? (AllEnums.InfoFeedType.Acquired).ToString() : (AllEnums.InfoFeedType.Given).ToString();
            message._timeStamp.text = GameSessionController.Instance._dayNightCycle._timeText.text;
            gObject.SetActive(true);
            UserInterfaceController.Instance._informationFeed.AddToInfoFeed(message);           
        }
	}
}