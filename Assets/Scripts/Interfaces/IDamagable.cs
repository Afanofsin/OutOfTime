using System.Collections.Generic;

namespace Interfaces
{
    public interface IDamageable
    {
        void TakeDamage(IReadOnlyDictionary<DamageType, float> damage);
    }
}
