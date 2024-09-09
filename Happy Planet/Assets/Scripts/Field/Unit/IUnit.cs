using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnit {

    [System.Serializable]
    class LevelData {
        public Sprite Sprite;
        public GameObject Prefab;
        public int EarnMoney = 0;
        public int EarnEnergy = 0;
        public int RemoveCost = 0;
        public int LevelUpCost = 0;
    }

    public UnitInfor GetInfor();

    public Vector3 Pos { get; }
    public Vector3 Dir { get; }

    public int Id { get; }
    public float InstantiateTime { get; }
    public int NowLevel { get; }
    public float LifeSpanRatio { get; }
    public float EarnRatio { get; }

    public void LevelUp(float time, bool isAction = false);
    public void LevelDown(float time, bool isAction = false);

    public void Remove(float time, bool isAction = false);
    public void SetId(int id);

    public bool Exist();

}
