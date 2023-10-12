using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
    [System.Serializable]
	public class RuntimeWorldItem {

		//test
		public WorldItem _instance;
        public string _itemName;
		//...

		public GameObject _rtModel; //Runtime Model

        public bool _isEquipped;
		
		public float _durability; //also known as amount or charges.
        public bool _unbreakable;
    }
}