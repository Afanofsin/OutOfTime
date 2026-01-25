using System.Collections.Generic;
using UnityEngine;

public interface IBoon
{
    List<StatModifier> GetBoonMods();
}
