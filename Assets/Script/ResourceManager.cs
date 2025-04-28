// Assets/Scripts/ResourceManager.cs
using UnityEngine;
using System.Collections.Generic;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    // Valeurs initiales exposées dans l’Inspector
    [Header("Starting Resources")]
    public int startGold = 100;
    public int startWood = 50;
    public int startStone = 20;

    private Dictionary<ResourceType, int> _resources;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _resources = new Dictionary<ResourceType, int>()
        {
            { ResourceType.Gold,  startGold },
            { ResourceType.Wood,  startWood },
            { ResourceType.Stone, startStone }
        };
    }

    public int Get(ResourceType type)
        => _resources.ContainsKey(type) ? _resources[type] : 0;

    public void Add(ResourceType type, int amount)
    {
        if (!_resources.ContainsKey(type)) _resources[type] = 0;
        _resources[type] += amount;
        Debug.Log($"[Resources] +{amount} {type} → {_resources[type]}");
    }

    /// <summary>
    /// Tentative de dépense : renvoie false si insuffisant.
    /// </summary>
    public bool Spend(ResourceType type, int amount)
    {
        if (Get(type) < amount) return false;
        _resources[type] -= amount;
        Debug.Log($"[Resources] –{amount} {type} → {_resources[type]}");
        return true;
    }
}
