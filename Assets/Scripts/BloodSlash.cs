using System.Collections.Generic;
using UnityEngine;

public class BloodSlash : SkillBase
{
    [SerializeField] private SwingBase _bloodSlash;
    [SerializeField] private Dictionary<DamageType, float> _damage;

    public override void Perform()
    {
        var swing = Instantiate(_bloodSlash, PlayerController.Instance.CurrentPlayer.transform) as ISwing;
        
        swing!.StartSwing(PlayerController.Instance.GetAngle(), _damage, 0.75f);
    }
}
