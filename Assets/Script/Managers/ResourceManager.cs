using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [Header("Starting Resources")]
    public int startGold = 100;
    public int startWood = 50;
    public int startStone = 20;
    public int startHarvest = 0;

    // Ressources en cours : c'est _resources qui est l'état source
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
    /// Dépense réellement : renvoie false si insuffisant.
    /// </summary>
    public bool Spend(ResourceType type, int amount)
    {
        if (Get(type) < amount) return false;
        _resources[type] -= amount;
        Debug.Log($"[Resources] –{amount} {type} → {_resources[type]}");
        return true;
    }

    public bool Has(ResourceType type, int amount) => Get(type) >= amount;
    public bool Consume(ResourceType type, int amount) => Spend(type, amount);

    // --------------------------------------------------------------------------------
    // Méthodes de save/load
    // --------------------------------------------------------------------------------

    /// <summary>
    /// Appelé par SaveManager.SaveGame() :
    /// retourne pour chaque ResourceType son montant actuel.
    /// </summary>
    public ResourceAmount[] GetCurrentAmounts()
    {
        // On parcourt _resources, pas un dictionnaire vide !
        return _resources
            .Select(kv => new ResourceAmount
            {
                resourceType = kv.Key,
                amount = kv.Value
            })
            .ToArray();
    }

    /// <summary>
    /// Appelé par SaveManager.LoadGame() :
    /// réinitialise les montants selon le tableau chargé.
    /// </summary>
    public void SetAmounts(ResourceAmount[] arr)
    {
        // On vide et on remet chaque valeur dans _resources
        _resources.Clear();
        foreach (var r in arr)
        {
            _resources[r.resourceType] = r.amount;
        }

        // Mets à jour ton UI ici si besoin, par exemple :
        // UIManager.Instance.RefreshResourceDisplay();
    }
}
