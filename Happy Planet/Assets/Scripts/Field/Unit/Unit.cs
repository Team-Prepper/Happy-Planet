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

    public void SetInfor(UnitInfor infor, float instantiateTime, int id = 0, int level = 0)
    {
        _unitInfor = infor;
        InstantiateTime = instantiateTime;

        int temp = Mathf.RoundToInt(1 / infor.EarnTime);
        _lastEarnTime = InstantiateTime + Mathf.Floor(LifeSpan / infor.EarnTime) * infor.EarnTime;

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

    private void Update()
    {
        if (_destroyFlag) return;
        if (GameManager.Instance.Field.CompareTime(InstantiateTime) < 0) return;

        bool isAlive = GameManager.Instance.Field.CompareTime(InstantiateTime + _unitInfor.LifeSpan) <= 0;

        _liveZone.SetActive(isAlive);
        _deathZone.SetActive(!isAlive);

        _earnData = isAlive ? _unitInfor.GetLevelData(NowLevel) : _unitInfor.GetDeathData();

        Earn();
        Loss();

    }

    public void LevelUp()
    {
        if (NowLevel >= _unitInfor.GetMaxLevel()) return;

        NowLevel++;
        _LevelChangeEvent();
    }

    public void LevelDown()
    {
        if (NowLevel <= 0) return;

        NowLevel--;
        _LevelChangeEvent();

    }

    private void _LevelChangeEvent()
    {
        _levelUpEvent.Invoke();
        _CreatePrefabAt(GetInfor().GetLevelData(NowLevel).Prefab, _liveZone.transform);

    }

    public void Remove()
    {
        Earn();
        Loss();

        _destroyFlag = true;

        Destroy(gameObject);
    }
    
    public void Earn()
    {
        while (GameManager.Instance.Field.CompareTime(_lastEarnTime + _unitInfor.EarnTime) >= 0)
        {
            GameManager.Instance.Field.AddMoney(_earnData.EarnMoney);
            GameManager.Instance.Field.AddEnergy(_earnData.EarnEnergy);

            _lastEarnTime += _unitInfor.EarnTime;

            _earnEffect.SetEarnData(_earnData.EarnEnergy, _earnData.EarnMoney);
            _earnEffect.EffectOn();
        }
    }

    public void Loss()
    {
        if (GameManager.Instance.Field.CompareTime(InstantiateTime) < 0) return;

        while (GameManager.Instance.Field.CompareTime(_lastEarnTime) < 0)
        {
            _lastEarnTime -= _unitInfor.EarnTime;

            GameManager.Instance.Field.AddMoney(-_earnData.EarnMoney);
            GameManager.Instance.Field.AddEnergy(-_earnData.EarnEnergy);

            _earnEffect.SetEarnData(-_earnData.EarnEnergy, -_earnData.EarnMoney);
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
