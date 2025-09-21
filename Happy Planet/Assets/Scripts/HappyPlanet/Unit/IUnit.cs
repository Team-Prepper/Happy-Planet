using UnityEngine;

public interface IUnit {

    public UnitData GetInfor();

    public Vector3 Pos { get; }
    public Vector3 Dir { get; }

    public UnitState UnitState { get; }

    public float LifeSpanRatio { get; }
    public float EarnRatio { get; }

    public void LevelUp(float time, bool isAction = false);
    public void LevelDown(float time, bool isAction = false);

    public void Remove(float time, bool isAction = false);
    public void SetId(int id);

    public bool Exist();

}