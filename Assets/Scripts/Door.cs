using System.Collections;
using Interfaces;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private int requiredKey;
    private IKeyHolder _cachedKeyHolder;
    
    public void Interact(GameObject interactor)
    {
        if (CanInteract(interactor))
        {
            gameObject.SetActive(false);
        }
    }
    
    public bool CanInteract(GameObject interactor)
    {
        if (_cachedKeyHolder == null && !interactor.TryGetComponent(out _cachedKeyHolder))
        {
            return false;
        }
        
        return _cachedKeyHolder.TryGetKey(requiredKey);
    }
}
