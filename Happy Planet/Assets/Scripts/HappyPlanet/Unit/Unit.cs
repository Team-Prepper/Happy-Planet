using UnityEngine;
using UnityEngine.Events;

public class Unit : MonoBehaviour, IUnit {

    [SerializeField] private UnityEvent _levelUpEvent;
    [SerializeField] private UnitData _unitData;
    [SerializeField] private EarnEffectBase[] _earnEffect;

    [SerializeField] private GameObject _liveZone;
    [SerializeField] private GameObject _deathZone;

    [SerializeField] private UnitEventEffectBase _createEffect;
    [SerializeField] private UnitEventEffectBase _removeEffect;
    [SerializeField] private UnitEventEffectBase _levelUpEffect;
    [SerializeField] private UnitEventEffectBase _levelDownEffect;

    public Vector3 Pos => transform.position;
    public Vector3 Dir => transform.up;
    public UnitState UnitState { get; set; } = new UnitState();

    public float LifeSpanRatio => LifeSpan / _unitData.LifeSpan;

    public float EarnRatio => (GameManager.Instance.Field.FieldData.SpendTime - _lastEarnTime) / _unitData.EarnTime;

    private float LifeSpan => GameManager.Instance.Field.FieldData.SpendTime - UnitState.InstantiateTime;

    private float _lastEarnTime = 0;

    private bool _destroyFlag = false;

    public void SetInfor(UnitData infor, float instantiateTime, int id = 0, int level = 0, bool isInitial = false)
    {
        _unitData = infor;
        UnitState.InstantiateTime = Mathf.Max(0, instantiateTime);

        if (isInitial)
        {
            _lastEarnTime = UnitState.InstantiateTime;
            _createEffect?.EffectPlay();
        }
        else
        {
            _lastEarnTime = UnitState.InstantiateTime + Mathf.Floor(LifeSpan / infor.EarnTime) * infor.EarnTime;
        }

        UnitState.Id = id;
        UnitState.Level = level;

        _CreatePrefabAt(infor.GetLevelData(UnitState.Level).Prefab, _liveZone.transform);
        _CreatePrefabAt(infor.GetDeathData().Prefab, _deathZone.transform);

    }

    public void SetId(int id)
    {
        UnitState.Id = id;
    }

    public UnitData GetInfor() => _unitData;

    private UnitData.LevelData GetEarnData(float time) {
        if (GameManager.Instance.Field.FieldData.CompareTime(time, UnitState.InstantiateTime + _unitData.LifeSpan) > 0)
            return _unitData.GetDeathData();

        return _unitData.GetLevelData(UnitState.Level);
    }

    private void LateUpdate()
    {
        if (_destroyFlag) return;

        FieldData fieldData = GameManager.Instance.Field.FieldData;

        Earn(fieldData.SpendTime);
        Loss(fieldData.SpendTime);

        if (fieldData.CompareTime(UnitState.InstantiateTime) < 0) return;

        bool isAlive = fieldData.CompareTime
            (UnitState.InstantiateTime + _unitData.LifeSpan) <= 0;

        _liveZone.SetActive(isAlive);
        _deathZone.SetActive(!isAlive);

    }

    public void LevelUp(float time, bool isAction = false)
    {
        if (UnitState.Level >= _unitData.GetMaxLevel()) return;

        if (!isAction)
        {
            _levelUpEffect?.EffectPlay();
            Earn(time);
            Loss(time);
        }

        UnitState.Level++;
        _LevelChangeEvent();

    }

    public void LevelDown(float time, bool isAction = false)
    {
        if (UnitState.Level <= 0) return;


        if (!isAction)
        {
            _levelDownEffect?.EffectPlay();
            Earn(time);
            Loss(time);
        }

        UnitState.Level--;
        _LevelChangeEvent();

    }

    private void _LevelChangeEvent()
    {
        _levelUpEvent.Invoke();
        _CreatePrefabAt(GetInfor().GetLevelData(UnitState.Level).Prefab, _liveZone.transform);

    }

    public void Remove(float time, bool isAction = false)
    {
        _destroyFlag = true;

        if (!isAction)
        {
            _removeEffect?.EffectPlay();
            Earn(time);
            Loss(time);
        }

        Destroy(gameObject);
    }
    
    public void Earn(float time)
    {
        while (GameManager.Instance.Field.FieldData.CompareTime(time, _lastEarnTime + _unitData.EarnTime) >= 0)
        {

            _lastEarnTime += _unitData.EarnTime;

            UnitData.LevelData data = GetEarnData(_lastEarnTime);

            GameManager.Instance.Field.AddMoney(data.EarnMoney);
            GameManager.Instance.Field.AddEnergy(data.EarnEnergy);

            foreach (EarnEffectBase effect in _earnEffect)
            {
                effect.EarnEffectOn(data.EarnMoney, data.EarnEnergy);
            }
        }
    }

    public void Loss(float time)
    {

        while (GameManager.Instance.Field.FieldData.CompareTime(time, _lastEarnTime) < 0)
        {

            if (GameManager.Instance.Field.FieldData.CompareTime(_lastEarnTime, UnitState.InstantiateTime) <= 0) return;

            UnitData.LevelData data = GetEarnData(_lastEarnTime);

            _lastEarnTime -= _unitData.EarnTime;

            GameManager.Instance.Field.AddMoney(-data.EarnMoney);
            GameManager.Instance.Field.AddEnergy(-data.EarnEnergy);

            foreach (EarnEffectBase effect in _earnEffect)
            {
                effect.EarnEffectOn(-data.EarnMoney, -data.EarnEnergy);
            }
        }

    }

    private void _CreatePrefabAt(GameObject prefab, Transform parent)
    {

        for (int i = 0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }

        Transform tr = Instantiate(prefab, parent).transform;

        tr.localPosition = Vector3.zero;
        tr.up = transform.up;
    }

    public bool Exist()
    {
        return !_destroyFlag;
    }

}