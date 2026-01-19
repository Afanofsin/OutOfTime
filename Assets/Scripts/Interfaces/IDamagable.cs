using System.Collections.Generic;

namespace Interfaces
{
    public interface IDamageable
    {
        void TakeDamage(Dictionary<DamageType, float> amount);
    }
}
