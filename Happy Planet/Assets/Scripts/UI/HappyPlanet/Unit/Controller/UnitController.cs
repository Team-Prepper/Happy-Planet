using UnityEngine;
using EasyH.Unity.UI;

public class UnitController : MonoBehaviour, IUnitController
{
    private IUnit _targetUnit;

    void Start() { 
        _targetUnit = GetComponent<IUnit>();
    }

    public void Initial(UnitData infor)
    { 
        
    }

    public void Interaction()
    {
        UIManager.Instance.OpenGUI<GUIUnitInfor>("UnitInfor").SetUnit(_targetUnit);
    }

}
