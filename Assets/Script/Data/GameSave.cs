using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSave
{
    public List<BuildingSave> buildings;
    public List<UnlockSave> unlockedBuildings;
    public ResourceAmount[] currentResources;
    public int year, month, day, hour, minute;
}

[Serializable]
public class BuildingSave
{
    public string buildingDataId;
    public int gridX, gridY;
    public int variantIndex;
}

[Serializable]
public class UnlockSave
{
    public string buildingDataId;
}
