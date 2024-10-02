using UnityEngine;

public class Item : MonoBehaviour {

    private string _itemName;
    protected string ItemName {
        get {
            return _itemName;
        }
        set {
            _itemName = value;
        }
    }


    protected virtual void Awake() {
    }
}