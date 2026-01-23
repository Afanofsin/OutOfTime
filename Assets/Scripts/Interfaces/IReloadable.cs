using UnityEngine;

public interface IReloadable
{
   void Reload();
   void CancelReload();
   bool IsReloading { get; }
   static float CurrentAmmo { get; }
   int MaxAmmo { get; }
}
