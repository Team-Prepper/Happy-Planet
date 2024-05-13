using UISystem;
using UnityEngine;
using UnityEngine.Events;

public class Unit : MonoBehaviour, IUnit {

    public int NowLevel { get; private set; }

    [SerializeField] UnityEvent _levelUpEvent;
    [SerializeField] UnitInfor _unitInfor;

    float _instantiateTime = 0;
    float _lastEarnTime = 0;

    public float LifeSpanRatio => (GameManager.Instance.SpendTime - _instantiateTime) / _unitInfor.LifeSpan;

    public float EarnRatio => (GameManager.Instance.SpendTime - _lastEarnTime) / _unitInfor.EarnTime;

    public void SetInfor(string itemCode, float instantiateTime, int level)
    {
        _unitInfor = UnitDataManager.Instance.GetUnitData(itemCode);
        _instantiateTime = instantiateTime;
        NowLevel = level;
    }

    private void LateUpdate()
    {
        float lifeSpan = LifeSpanRatio;

        if (lifeSpan > 1f)
            return;

        if (lifeSpan < 0f)
        {
            UIManager.Instance.OpenGUI<GUIUnitRemove>("UnitRemove").SetTarget(this);
            return;
        }

        float earnRatio = EarnRatio;

        if (earnRatio >= 1)
        {
            GameManager.Instance.AddMoney(_unitInfor.GetEarnMoney(NowLevel));
            GameManager.Instance.AddPollution(_unitInfor.GetEarnPollution(NowLevel));

            _lastEarnTime += _unitInfor.EarnTime;

            return;
        }
        if (earnRatio < 0f) {
            _lastEarnTime -= _unitInfor.EarnTime;

            GameManager.Instance.AddMoney(-_unitInfor.GetEarnMoney(NowLevel));
            GameManager.Instance.AddPollution(-_unitInfor.GetEarnPollution(NowLevel));

            return;
        }

        if (GameManager.Instance.SpendTime >= _lastEarnTime)
            return;
        if (GameManager.Instance.SpendTime <= _instantiateTime)
            return;


    }

    public void LevelUp()
    {
        if (NowLevel >= _unitInfor.GetMaxLevel()) return;
        NowLevel++;
        _levelUpEvent.Invoke();

        Debug.Log("LevelUp");
    }

    public void Remove()
    {
        Destroy(gameObject);
    }

    public void AddUnitLevel() {
        // 레벨에 따른 유닛 모양의 변화
    }

    public UnitInfor GetInfor() => _unitInfor;

    public void Selected(Transform parent)
    {
        transform.parent = parent;
        //zone.Set();
    }

    public void Free() {
        transform.parent = null;
    }
}
