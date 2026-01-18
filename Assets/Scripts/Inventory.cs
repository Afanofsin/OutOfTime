using UnityEngine;
using Interfaces;

public class Inventory : MonoBehaviour, IKeyHolder
{
    [SerializeField] private bool[] heldKeys = {true, false, true };

    public bool TryGetKey(int i) => heldKeys[i];

}
