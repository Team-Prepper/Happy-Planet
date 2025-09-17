using UnityEngine;

[CreateAssetMenu(fileName = "Data_Unit_", menuName = "ScriptableObjects/UnitData", order = 1)]
public class UnitData : ScriptableObject {

    [System.Serializable]
    public class LevelData {
        public Sprite Sprite;
        public GameObject Prefab;
        public int EarnMoney = 0;
        public int EarnEnergy = 0;
        public int RemoveCost = 0;
        public int LevelUpCost = 0;
    }


    //[SerializeField] private Theme _mainTheme;
    [SerializeField] private string _unitNameCode;
    [SerializeField] private string _unitName;

    [SerializeField] private float _lifeSpan;
    [SerializeField] private float _earnTime = 1;

    [SerializeField] private LevelData[] _levelData;
    [SerializeField] private LevelData _deathData;

    public string UnitCode { get { return _unitNameCode; } }
    public string UnitName { get { return _unitName; } }
    public float LifeSpan { get { return _lifeSpan; } }
    public float EarnTime { get { return _earnTime; } }

    public int GetMaxLevel() {
        return _levelData.Length - 1;
    }

    public LevelData GetDeathData() => _deathData;
    public LevelData GetLevelData(int level) => _levelData[level];

}