using UnityEngine;
using System.Collections.Generic;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [Header("Starting Resources")]
    public int startGold = 100;
    public int startWood = 50;
    public int startStone = 20;
    public int startHarvest = 0;

    private Dictionary<ResourceType, int> _resources;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _resources = new Dictionary<ResourceType, int>()
        {
            { ResourceType.Gold,    startGold    },
            { ResourceType.Wood,    startWood    },
            { ResourceType.Stone,   startStone   },
            { ResourceType.Harvest, startHarvest }
        };
    }

    /// <summary>
    /// Renvoie la quantité actuelle de la ressource.
    /// </summary>
    public int Get(ResourceType type)
        => _resources.ContainsKey(type) ? _resources[type] : 0;

    /// <summary>
    /// Ajoute (production) ; crée la clé si nécessaire.
    /// </summary>
    public void Add(ResourceType type, int amount)
    {
        if (!_resources.ContainsKey(type))
            _resources[type] = 0;
        _resources[type] += amount;
        Debug.Log($"[Resources] +{amount} {type} → {_resources[type]}");
    }

    /// <summary>
    /// Vérifie si l’on dispose d’au moins `amount` de la ressource.
    /// </summary>
    public bool Has(ResourceType type, int amount)
    {
        return Get(type) >= amount;
    }

    /// <summary>
    /// Consomme une ressource (alias de Spend), renvoie false si insuffisant.
    /// </summary>
    public bool Consume(ResourceType type, int amount)
    {
        return Spend(type, amount);
    }

    /// <summary>
    /// Dépense réellement : renvoie false si insuffisant.
    /// </summary>
    public bool Spend(ResourceType type, int amount)
    {
        if (Get(type) < amount)
            return false;
        _resources[type] -= amount;
        Debug.Log($"[Resources] –{amount} {type} → {_resources[type]}");
        return true;
    }

    public Dictionary<ResourceType, int> GetRessources()
    {
        return _resources;
    }
}
