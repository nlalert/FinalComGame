using System;
using System.Collections.Generic;

namespace FinalComGame;

public enum AbilityType
{
    Dash,
    Glide,
    Grapple
}

public class AbilityManager
{
    // Dictionary to track which abilities are unlocked
    private Dictionary<AbilityType, bool> _unlockedAbilities;

    public AbilityManager()
    {
        _unlockedAbilities = new Dictionary<AbilityType, bool>();
        
        Reset();
    }

    // Unlock an ability
    public void UnlockAbility(AbilityType abilityType)
    {
        _unlockedAbilities[abilityType] = true;
    }

    // Check if an ability is unlocked
    public bool IsAbilityUnlocked(AbilityType abilityType)
    {
        return _unlockedAbilities.ContainsKey(abilityType) && _unlockedAbilities[abilityType];
    }

    // Reset all abilities to locked state
    public void Reset()
    {
        foreach (AbilityType ability in Enum.GetValues(typeof(AbilityType)))
        {
            _unlockedAbilities[ability] = false;
        }
    }
}