using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnit {

    public UnitInfor GetInfor();

    public int NowLevel { get; }
    public float LifeSpanRatio { get; }
    public float EarnRatio { get; }

    public void LevelUp();
    public void Remove();
    

}
