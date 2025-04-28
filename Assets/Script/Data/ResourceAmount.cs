// Assets/Scripts/ResourceAmount.cs
using UnityEngine;

[System.Serializable]
public struct ResourceAmount
{
    public ResourceType resourceType;  // Gold, Wood, Stone...
    public int amount;        // quantité nécessaire
}
