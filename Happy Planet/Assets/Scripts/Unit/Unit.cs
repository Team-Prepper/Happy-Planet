using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnitDataManager;

[System.Serializable]
class  LevelData
{
    public int EarnMoney = 0;
    public int EarnPollution = 0;
}

[System.Serializable]
public class UnitInfor {

    [SerializeField] Theme _mainTheme;

    public string UnitCode;

    [SerializeField] LevelData[] _levelData;
    [SerializeField] int _maxLevel;
    [SerializeField] float _lifeSpan;
    [SerializeField] float _earnTime = 1;

    public int NowLevel { get; private set; }

    float _instantiateTime = 0;
    float _lastEarnTime = 0;

    public readonly float bojung = 0.1f;

    public void SetInfor(float InstantiateTime) {
        _instantiateTime = InstantiateTime;
    }

    public bool TryLevelUp()
    {
        if (NowLevel == _levelData.Length - 1)
            return false;

        return true;
    }

    public bool TryEarn() {
        if (GameManager.Instance.SpendTime - _lastEarnTime < _earnTime)
            return false;
        _lastEarnTime += _earnTime;
        return true;
    }

    public bool TryCalcEarn()
    {
        if (GameManager.Instance.SpendTime >= _lastEarnTime)
            return false;
        if (GameManager.Instance.SpendTime <= _instantiateTime)
            return false;
        _lastEarnTime -= 1;
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

    public float LifeSpanRatio() {
        return (GameManager.Instance.SpendTime - _instantiateTime) / _lifeSpan;
    }

    public float EarnRatio()
    {
        return (GameManager.Instance.SpendTime - _lastEarnTime) / _earnTime;

    }


}

public class Unit : MonoBehaviour
{

    [SerializeField] EarnData _earnData;

    [SerializeField] UnityEvent _event;

    [SerializeField] UnitInfor _unitData;

    private void LateUpdate()
    {
        if (_unitData.TryEarn())
        {
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

            return;
        }

    }


    public void Test(TMPro.TextMeshPro _textMeshPro) {
        _textMeshPro.text = _unitData.NowLevel.ToString();
    }

    public void AddUnitLevel() {
        Debug.Log("LevelUp");
        _event.Invoke();
        // 레벨에 따른 유닛 모양의 변화
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
