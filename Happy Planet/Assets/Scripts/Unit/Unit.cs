using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnitDataManager;

[System.Serializable]
class  LevelData
{
    public int NeedToLevelUp;

    public int EarnMoney;
    public int EarnPollution;
}

[System.Serializable]
public class UnitInfor {

    [SerializeField] Theme _mainTheme;

    public string UnitCode;

    [SerializeField] LevelData[] _levelData;
    [SerializeField] int _maxLevel;
    public int NowLevel { get; private set; }

    float _instantiateTime = 0;
    float _lastEarn = 0;

    public readonly float bojung = 0.1f;

    public void SetInfor(float InstantiateTime) {
        _instantiateTime = InstantiateTime;
    }

    public bool TryLevelUp()
    {
        if (NowLevel == _levelData.Length - 1)
            return false;
        if (CalcExp() < 1)
            return false;

        NowLevel++;
        return true;
    }

    public bool TryEarn() {
        if (GameManager.Instance.SpendTime - _lastEarn < 1)
            return false;
        _lastEarn += 1;
        return true;
    }

    public bool TryCalcEarn()
    {
        if (GameManager.Instance.SpendTime >= _lastEarn)
            return false;
        if (GameManager.Instance.SpendTime <= _instantiateTime)
            return false;
        _lastEarn -= 1;
        return true;

    }

    public bool TryLevelDown() {

        if (NowLevel == 0)
            return false;
        if (CalcExp(NowLevel - 1) > 1)
            return false;

        NowLevel--;
        return true;
    }

    public bool NeedRemove() {
        if (GameManager.Instance.SpendTime < _instantiateTime - bojung) {
            return true;
        }
        return false;
    }

    public int GetEarnMoney() {
        return _levelData[NowLevel].EarnMoney;
    }
    public int GetEarnPollution()
    {
        return _levelData[NowLevel].EarnPollution;
    }

    public float CalcExp() {
        return CalcExp(NowLevel);
    }

    public float CalcExp(int level) {
        float compareTarget = 0;
        if (level != 0) compareTarget = _levelData[level - 1].NeedToLevelUp;

        return (GameManager.Instance.SpendTime - _instantiateTime - compareTarget) / (_levelData[level].NeedToLevelUp - compareTarget);
    }

}

public class Unit : MonoBehaviour
{

    [SerializeField] EarnData _earnData;

    [SerializeField] UnityEvent _event;

    [SerializeField] UnitInfor _unitData;

    [SerializeField] GameObject[] _appeal;

    private void LateUpdate()
    {


        if (_unitData.TryEarn())
        {
            if (_unitData.TryLevelUp())
            {
                AddUnitLevel();
            }

            GameManager.Instance.AddMoney(_unitData.GetEarnMoney());
            GameManager.Instance.AddPollution(_unitData.GetEarnPollution());

            return;
        }

        if (_unitData.NeedRemove())
        {
            // 삭제 물어보는 팝업 띄우기
            return;
        }

        if (_unitData.TryCalcEarn())
        {

            GameManager.Instance.AddMoney(-_unitData.GetEarnMoney());
            GameManager.Instance.AddPollution(-_unitData.GetEarnPollution());

            if (_unitData.TryLevelDown())
            {
                DecreaseUnitLevel();
            }

            return;
        }

    }


    public void Test(TMPro.TextMeshPro _textMeshPro) {
        _textMeshPro.text = _unitData.NowLevel.ToString();
    }

    public void AddUnitLevel() {

        _appeal[_unitData.NowLevel].SetActive(true);
        _appeal[_unitData.NowLevel - 1].SetActive(false);
        Debug.Log("LevelUp");
        _event.Invoke();
        // 레벨에 따른 유닛 모양의 변화
    }


    public void DecreaseUnitLevel()
    {
        _appeal[_unitData.NowLevel].SetActive(true);
        _appeal[_unitData.NowLevel + 1].SetActive(false);
        Debug.Log("LevelDecrease");
        _event.Invoke();

    }

    public UnitInfor GetData()
    {
        return _unitData;
    }

    public void Selected(Transform parent)
    {
        transform.parent = parent;
        //zone.Set();
    }

    public void Free() {
        transform.parent = null;
        //zone.Clear();
    }


}
