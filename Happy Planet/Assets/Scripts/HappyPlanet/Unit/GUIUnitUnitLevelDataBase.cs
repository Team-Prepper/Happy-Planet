using UnityEngine;

public abstract class GUIUnitUnitLevelDataBase : MonoBehaviour
{
    public void SetUnitLevelData(UnitData data, int level)
    {
        SetGUI(data.GetLevelData(level));
    }

    protected abstract void SetGUI(UnitData.LevelData data);

}