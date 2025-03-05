using UnityEngine;
using EHTool;

class CreateEvent : ILogEvent {
    Vector3 Position;
    Vector3 Dir;
    string UnitCode;

    internal CreateEvent(IUnit data)
    {
        Position = data.Pos;
        Dir = data.Dir;
        UnitCode = data.GetInfor().UnitCode;
    }
    internal CreateEvent(string[] code)
    {
        UnitCode = code[1];

        Position = FieldLogUtils.CostumStringToVector3(code[2], code[3], code[4]);
        Dir = FieldLogUtils.CostumStringToVector3(code[5], code[6], code[7]);
    }

    public override string ToString()
    {
        return string.Format("01/{0}/{1}/{2}", UnitCode,
            FieldLogUtils.CostumVector3ToString(Position),
            FieldLogUtils.CostumVector3ToString(Dir));

    }

    public void Action(IField target, float time, int id, bool isAction = false)
    {
        if (!isAction) {
            SoundManager.Instance.PlaySound("Place", "VFX");
        }
        Unit newUnit = AssetOpener.ImportGameObject("Prefabs/unit").GetComponent<Unit>();

        newUnit.transform.position = Position;
        newUnit.transform.up = Dir;

        newUnit.SetInfor(UnitDataManager.Instance.GetUnitData(UnitCode), time, id, 0, !isAction);

        target.RegisterUnit(id, newUnit);

    }
    public void Undo(IField target, float time, int id)
    {
        SoundManager.Instance.PlaySound("Remove", "VFX");
        target.GetUnit(id)?.Remove(time);
        target.UnregisterUnit(id);

    }

}