using UnityEngine;

public interface IReloadable
{
   void Reload();
   void CancelReload();
   float CurrentAmmo { get; }
   int MaxAmmo { get; }
}
