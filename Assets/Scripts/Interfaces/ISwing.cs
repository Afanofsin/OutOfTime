using System.Collections.Generic;
using UnityEngine;

public interface ISwing
{ 
    void StartSwing(float angle, IReadOnlyDictionary<DamageType, float> damage, float duration);
}
