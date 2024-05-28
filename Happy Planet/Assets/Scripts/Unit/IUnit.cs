using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnit {

    public UnitInfor GetInfor();

    public Vector3 Pos { get; }
    public Vector3 Dir { get; }

    public int Id { get; }

    public int NowLevel { get; }
    public float LifeSpanRatio { get; }
    public float EarnRatio { get; }

    public void LevelUp();
    public void LevelDown();
    public void SetLevel(int level);

    public void Remove();
    public void SetId(int id);

    public bool Exist();

}
