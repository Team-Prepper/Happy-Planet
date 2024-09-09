using EHTool.UIKit;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Unit : MonoBehaviour, IUnit {


    [SerializeField] UnityEvent _levelUpEvent;
    [SerializeField] UnitInfor _unitInfor;
    [SerializeField] EarnEffect _earnEffect;

    [SerializeField] GameObject _liveZone;
    [SerializeField] GameObject _deathZone;

    public Vector3 Pos { get => transform.position; }
    public Vector3 Dir { get => transform.up; }

    public int Id { get; private set; }

    public float InstantiateTime { get; private set; }

    public int NowLevel { get; private set; }

    public float LifeSpanRatio => LifeSpan / _unitInfor.LifeSpan;

    public float EarnRatio => (GameManager.Instance.Field.SpendTime - _lastEarnTime) / _unitInfor.EarnTime;

    private float LifeSpan => (GameManager.Instance.Field.SpendTime - InstantiateTime);

    private float _lastEarnTime = 0;

    private bool _destroyFlag = false;

    private IUnit.LevelData _earnData;

    public void SetInfor(UnitInfor infor, float instantiateTime, int id = 0, int level = 0, bool isInitial = false)
    {
        _unitInfor = infor;
        InstantiateTime = instantiateTime;

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

    public UnitInfor GetInfor() => _unitInfor;

    IUnit.LevelData GetEarnData(float time) {
        if (GameManager.Instance.Field.CompareTime(time, InstantiateTime + _unitInfor.LifeSpan) > 0)
            return _unitInfor.GetDeathData();

        return _unitInfor.GetLevelData(NowLevel);
    }

    private void LateUpdate()
    {
        if (_destroyFlag) return;

        Earn(GameManager.Instance.Field.SpendTime);
        Loss(GameManager.Instance.Field.SpendTime);

        if (GameManager.Instance.Field.CompareTime(InstantiateTime) < 0) return;

        bool isAlive = GameManager.Instance.Field.CompareTime(InstantiateTime + _unitInfor.LifeSpan) <= 0;

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
        while (GameManager.Instance.Field.CompareTime(time, _lastEarnTime + _unitInfor.EarnTime) >= 0)
        {

            _lastEarnTime += _unitInfor.EarnTime;

            IUnit.LevelData data = GetEarnData(_lastEarnTime);

            GameManager.Instance.Field.AddMoney(data.EarnMoney);
            GameManager.Instance.Field.AddEnergy(data.EarnEnergy);

            _earnEffect.SetEarnData(data.EarnMoney, data.EarnEnergy);
            _earnEffect.EffectOn();
        }
    }

    public void Loss(float time)
    {

        while (GameManager.Instance.Field.CompareTime(time, _lastEarnTime) < 0)
        {

            if (GameManager.Instance.Field.CompareTime(_lastEarnTime, InstantiateTime) <= 0) return;

            IUnit.LevelData data = GetEarnData(_lastEarnTime);

            _lastEarnTime -= _unitInfor.EarnTime;

            GameManager.Instance.Field.AddMoney(-data.EarnMoney);
            GameManager.Instance.Field.AddEnergy(-data.EarnEnergy);

            _earnEffect.SetEarnData(-data.EarnMoney, -data.EarnEnergy);
            _earnEffect.EffectOn();
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
