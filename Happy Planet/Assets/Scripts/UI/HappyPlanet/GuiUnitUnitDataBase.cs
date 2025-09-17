using UnityEngine;

public abstract class GUIUnitUnitDataBase : MonoBehaviour
{
    public void SetUnit(UnitData data)
    {
        SetGUI(data);
    }

    public abstract void SetGUI(UnitData data);
}