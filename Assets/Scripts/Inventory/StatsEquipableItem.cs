using System;
using System.Collections.Generic;
using RPG.Stats;
using UnityEngine;

namespace RPG.Inventory
{
    [CreateAssetMenu(menuName = ("Inventory/Stats Equipable Item"))]
    public class StatsEquipableItem : EquipableItem, IModifierProvider
    {
        [SerializeField, NonReorderable] private Modifier[] _additiveModifiers = new Modifier[] { };
        [SerializeField, NonReorderable] private Modifier[] _percentageModifiers = new Modifier[] { };

        [Serializable]
        struct Modifier
        {
            public Stat Stat;
            public float Value;
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            foreach (var modifier in _additiveModifiers)
            {
                if (modifier.Stat == stat)
                {
                    yield return modifier.Value;
                }
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            foreach (var modifier in _percentageModifiers)
            {
                if (modifier.Stat == stat)
                {
                    yield return modifier.Value;
                }
            }
        }
    }
}