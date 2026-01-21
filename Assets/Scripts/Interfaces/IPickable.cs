using UnityEngine;

public interface IPickable
{ 
    Sprite PickableSprite { get; }
    void PickUp(Inventory context);
}
