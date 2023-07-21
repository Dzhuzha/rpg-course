using System;
using System.Collections.Generic;
using RPG.Inventory;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = ("Inventory/Drop Library"), fileName = "DropLibrary", order = 3)]
public class DropLibrary : ScriptableObject
{
    [SerializeField, NonReorderable] private int[] _dropChance;
    [SerializeField, NonReorderable] private int[] _minDrops;
    [SerializeField, NonReorderable] private int[] _maxDrops;
    [SerializeField, NonReorderable] private DropConfig[] _potentialDrops = new DropConfig[]{};

    [Serializable]
    private class DropConfig
    {
        public InventoryItem Item;
        [NonReorderable] public int[] RelativeChance;
        [NonReorderable] public int[] MinQuantity;
        [NonReorderable] public int[] MaxQuantity;

        public int GetRandomQuantity(int level)
        {
            if (!Item.IsStackable)
            {
                return 1;
            }
            
            int min = GetByLevel(MinQuantity, level);
            int max = GetByLevel(MaxQuantity, level);
            
            return Random.Range(min, max + 1);
        }
    }
    
    public struct Dropped
    {
        public InventoryItem Item;
        public int Quantity;
    }

    public IEnumerable<Dropped> GetRandomDrops(int level)
    {
        if (!ShouldRandomDrop(level))
        {
            yield break;
        }

        int cycles = GetRandomNumberOfDrops(level);

        for (int i = 0; i < cycles; i++)
        {
            yield return GetRandomDrop(level);
        }
    }

    private bool ShouldRandomDrop(int level)
    {
        return Random.Range(0, 100) < GetByLevel(_dropChance, level);
    }

    private int GetRandomNumberOfDrops(int level)
    {
        int min = GetByLevel(_minDrops, level);
        int max = GetByLevel(_maxDrops, level);
        return Random.Range(min, max);
    }

    private Dropped GetRandomDrop(int level)
    {
        DropConfig drop = SelectRandomDrop(level);
        var result = new Dropped();
        
        result.Item = drop.Item;
        result.Quantity = drop.GetRandomQuantity(level);
        return result;
    }

    private DropConfig SelectRandomDrop(int level)
    {
        int totalChance = GetTotalChance(level);
        int randomRoll = Random.Range(0, totalChance);
        int finalChance = 0;

        foreach (var drop in _potentialDrops)
        {
            finalChance += GetByLevel(drop.RelativeChance, level);

            if (finalChance > randomRoll)
            {
                return drop;
            }
        }

        return null;
    }

    private int GetTotalChance(int level)
    {
        int totalChance = 0;

        foreach (var drop in _potentialDrops)
        {
            totalChance += GetByLevel(drop.RelativeChance, level);
        }

        return totalChance;
    }

    static T GetByLevel<T>(T[] values, int level)
    {
        if (values.Length == 0)
        {
            return default;
        }

        if (level > values.Length)
        {
            return values[values.Length - 1];
        }

        if (level <= 0)
        {
            return default;
        }

        return values[level - 1];
    }
}