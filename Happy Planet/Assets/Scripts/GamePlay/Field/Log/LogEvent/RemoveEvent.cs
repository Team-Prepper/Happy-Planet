using UnityEngine;
using EHTool;

class RemoveEvent : ILogEvent {

    Vector3 Position;
    Vector3 Dir;

    string UnitCode;
    int Level;
    float InitialTime;

    internal RemoveEvent(IUnit data)
    {
        Position = data.Pos;
        Dir = data.Dir;
        UnitCode = data.GetInfor().UnitCode;
        Level = data.NowLevel;
        InitialTime = data.InstantiateTime;
    }
    internal RemoveEvent(string[] code)
    {
        UnitCode = code[1];

        Position = FieldLogUtils.CostumStringToVector3(code[2], code[3], code[4]);
        Dir = FieldLogUtils.CostumStringToVector3(code[5], code[6], code[7]);

        Level = int.Parse(code[8]);
        InitialTime = float.Parse(code[9]);
    }
    public override string ToString()
    {
        return string.Format("02/{0}/{1}/{2}/{3}/{4}",
            UnitCode, FieldLogUtils.CostumVector3ToString(Position), FieldLogUtils.CostumVector3ToString(Dir), Level, InitialTime);

    }
    public void Action(IField target, float time, int id, bool isAction = false)
    {
        if (!isAction) {
            SoundManager.Instance.PlaySound("Remove", "VFX");
        }
        target.GetUnit(id)?.Remove(time, isAction);
        target.UnregisterUnit(id);

    }

    public void Undo(IField target, float time, int id)
    {
        Unit newUnit = AssetOpener.ImportGameObject("Prefabs/unit").GetComponent<Unit>();

        SoundManager.Instance.PlaySound("Place", "VFX");

        newUnit.transform.position = Position;
        newUnit.transform.up = Dir;

        newUnit.SetInfor(UnitDataManager.Instance.GetUnitData(UnitCode), InitialTime, id, Level);

        target.RegisterUnit(id, newUnit);

    }
}