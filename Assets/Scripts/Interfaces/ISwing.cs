using System.Collections.Generic;
using UnityEngine;

public interface ISwing
{
    void StartSwing(float angle, Dictionary<DamageType, float> damage);
}
