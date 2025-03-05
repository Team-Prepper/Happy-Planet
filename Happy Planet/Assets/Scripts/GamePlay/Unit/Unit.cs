using UnityEngine;
using UnityEngine.Events;

public class Unit : MonoBehaviour, IUnit {

    [SerializeField] private UnityEvent _levelUpEvent;
    [SerializeField] private UnitData _unitInfor;
    [SerializeField] private IEarnEffect[] _earnEffect;

    [SerializeField] private GameObject _liveZone;
    [SerializeField] private GameObject _deathZone;

    public Vector3 Pos { get => transform.position; }
    public Vector3 Dir { get => transform.up; }

    public int Id { get; private set; }

    public float InstantiateTime { get; private set; }

    public int NowLevel { get; private set; }

    public float LifeSpanRatio => LifeSpan / _unitInfor.LifeSpan;

    public float EarnRatio => (GameManager.Instance.Field.FieldData.SpendTime - _lastEarnTime) / _unitInfor.EarnTime;

    private float LifeSpan => GameManager.Instance.Field.FieldData.SpendTime - InstantiateTime;

    private float _lastEarnTime = 0;

    private bool _destroyFlag = false;

    private IUnit.LevelData _earnData;

    public void SetInfor(UnitData infor, float instantiateTime, int id = 0, int level = 0, bool isInitial = false)
    {
        _unitInfor = infor;
        InstantiateTime = Mathf.Max(0, instantiateTime);

        if (isInitial) {
            _lastEarnTime = InstantiateTime;
        }
        else
        {
            _lastEarnTime = InstantiateTime + Mathf.Floor(LifeSpan / infor.EarnTime) * infor.EarnTime;
        }

        Id = id;
        NowLevel = level;

        _CreatePrefabAt(infor.GetLevelData(NowLevel).Prefab, _liveZone.transform);
        _CreatePrefabAt(infor.GetDeathData().Prefab, _deathZone.transform);

    }

    public void SetId(int id)
    {
        Id = id;
    }

    public UnitData GetInfor() => _unitInfor;

    private IUnit.LevelData GetEarnData(float time) {
        if (GameManager.Instance.Field.FieldData.CompareTime(time, InstantiateTime + _unitInfor.LifeSpan) > 0)
            return _unitInfor.GetDeathData();

        return _unitInfor.GetLevelData(NowLevel);
    }

    private void LateUpdate()
    {
        if (_destroyFlag) return;

        FieldData fieldData = GameManager.Instance.Field.FieldData;

        Earn(fieldData.SpendTime);
        Loss(fieldData.SpendTime);

        if (fieldData.CompareTime(InstantiateTime) < 0) return;

        bool isAlive = fieldData.CompareTime
            (InstantiateTime + _unitInfor.LifeSpan) <= 0;

        _liveZone.SetActive(isAlive);
        _deathZone.SetActive(!isAlive);

    }

    public void LevelUp(float time, bool isAction = false)
    {
        if (NowLevel >= _unitInfor.GetMaxLevel()) return;

        if (!isAction)
        {
            Earn(time);
            Loss(time);
        }

        NowLevel++;
        _LevelChangeEvent();
    }

    public void LevelDown(float time, bool isAction = false)
    {
        if (NowLevel <= 0) return;

        if (!isAction)
        {
            Earn(time);
            Loss(time);
        }

        NowLevel--;
        _LevelChangeEvent();

    }

    private void _LevelChangeEvent()
    {
        _levelUpEvent.Invoke();
        _CreatePrefabAt(GetInfor().GetLevelData(NowLevel).Prefab, _liveZone.transform);

    }

    public void Remove(float time, bool isAction = false)
    {
        _destroyFlag = true;

        if (!isAction)
        {
            Earn(time);
            Loss(time);
        }

        Destroy(gameObject);
    }
    
    public void Earn(float time)
    {
        while (GameManager.Instance.Field.FieldData.CompareTime(time, _lastEarnTime + _unitInfor.EarnTime) >= 0)
        {

            _lastEarnTime += _unitInfor.EarnTime;

            IUnit.LevelData data = GetEarnData(_lastEarnTime);

            GameManager.Instance.Field.AddMoney(data.EarnMoney);
            GameManager.Instance.Field.AddEnergy(data.EarnEnergy);

            foreach (IEarnEffect effect in _earnEffect)
            {
                effect.EarnEffectOn(data.EarnMoney, data.EarnEnergy);
            }
        }
    }

    public void Loss(float time)
    {

        while (GameManager.Instance.Field.FieldData.CompareTime(time, _lastEarnTime) < 0)
        {

            if (GameManager.Instance.Field.FieldData.CompareTime(_lastEarnTime, InstantiateTime) <= 0) return;

            IUnit.LevelData data = GetEarnData(_lastEarnTime);

            _lastEarnTime -= _unitInfor.EarnTime;

            GameManager.Instance.Field.AddMoney(-data.EarnMoney);
            GameManager.Instance.Field.AddEnergy(-data.EarnEnergy);

            foreach (IEarnEffect effect in _earnEffect)
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