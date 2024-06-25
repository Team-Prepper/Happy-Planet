using UnityEngine;

[CreateAssetMenu(fileName = "Data_Unit_", menuName = "ScriptableObjects/UnitInfor", order = 1)]
public class UnitInfor : ScriptableObject {

    [SerializeField] Theme _mainTheme;
    [SerializeField] string _unitNameCode;

    [SerializeField] float _lifeSpan;
    [SerializeField] float _earnTime = 1;

    [SerializeField] IUnit.LevelData[] _levelData;
    [SerializeField] IUnit.LevelData _deathData;

    public string UnitCode { get { return _unitNameCode; } }
    public float LifeSpan { get { return _lifeSpan; } }
    public float EarnTime { get { return _earnTime; } }

    public int GetMaxLevel() {
        return _levelData.Length - 1;
    }

    public IUnit.LevelData GetDeathData() => _deathData;
    public IUnit.LevelData GetLevelData(int level) => _levelData[level];

}