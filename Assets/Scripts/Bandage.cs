using UnityEngine;

public class Bandage : MonoBehaviour, IPickable
{
    public void PickUp(Inventory context)
    {
        context.AddBandage(1);
        Destroy(gameObject);
    }
}
