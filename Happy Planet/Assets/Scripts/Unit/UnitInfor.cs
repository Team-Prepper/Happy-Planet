using UnityEngine;


[System.Serializable]
class LevelData {
    public Sprite Sprite;
    public GameObject Prefab;
    public int EarnMoney = 0;
    public int EarnPollution = 0;
}

[CreateAssetMenu(fileName = "Data_Unit_", menuName = "ScriptableObjects/UnitInfor", order = 1)]
public class UnitInfor : ScriptableObject {

    [SerializeField] Theme _mainTheme;
    [SerializeField] string _unitNameCode;

    [SerializeField] float _lifeSpan;
    [SerializeField] float _earnTime = 1;

    [SerializeField] LevelData[] _levelData;

    public string UnitCode { get { return _unitNameCode; } }
    public float LifeSpan { get { return _lifeSpan; } }
    public float EarnTime { get { return _earnTime; } }

    public int GetMaxLevel() {
        return _levelData.Length - 1;
    }

    public Sprite GetSprite(int level) => _levelData[level].Sprite;
    public GameObject GetPrefab(int level) => _levelData[level].Prefab;
    public int GetEarnMoney(int level) => _levelData[level].EarnMoney;
    public int GetEarnPollution(int level) => _levelData[level].EarnPollution;

}