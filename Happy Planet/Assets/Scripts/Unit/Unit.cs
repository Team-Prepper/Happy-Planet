using UISystem;
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
    public float EarnRatio => (GameManager.Instance.SpendTime - _lastEarnTime) / _unitInfor.EarnTime;

    private float LifeSpan => (GameManager.Instance.SpendTime - InstantiateTime);

    private float _lastEarnTime = 0;

    private bool _destroyFlag = false;

    private IUnit.LevelData _earnData;


    public void SetInfor(UnitInfor infor, float instantiateTime, int id = 0)
    {
        _unitInfor = infor;
        InstantiateTime = instantiateTime;

        int temp = Mathf.RoundToInt(1 / infor.EarnTime);
        _lastEarnTime = InstantiateTime + Mathf.Floor(LifeSpan / infor.EarnTime) * infor.EarnTime;

        Id = id;
        NowLevel = 0;

        _CreatePrefabAt(infor.GetLevelData(NowLevel).Prefab, _liveZone.transform);
        _CreatePrefabAt(infor.GetDeathData().Prefab, _deathZone.transform);
    }

    public void SetId(int id)
    {
        Id = id;
    }

    public UnitInfor GetInfor() => _unitInfor;

    private void LateUpdate()
    {
        if (LifeSpanRatio < 0f) return;

        _liveZone.SetActive(LifeSpanRatio <= 1f);
        _deathZone.SetActive(LifeSpanRatio > 1f);

        _earnData = LifeSpanRatio <= 1 ? _unitInfor.GetLevelData(NowLevel) : _unitInfor.GetDeathData();

        if (EarnRatio >= 1)
        {
            _Earn();
            return;
        }
        if (EarnRatio >= 0f) return;

        _Loss();

    }

    public void LevelUp()
    {
        if (NowLevel >= _unitInfor.GetMaxLevel()) return;

        if (EarnRatio >= 1)
        {
            _Earn();
        }

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

    public void SetLevel(int level)
    {
        NowLevel = level;

        _levelUpEvent.Invoke();
        _CreatePrefabAt(GetInfor().GetLevelData(NowLevel).Prefab, _liveZone.transform);
    }

    public void Remove()
    {
        if (EarnRatio >= 1)
        {
            _Earn();
        }
        _destroyFlag = true;
        Destroy(gameObject);
    }


    private void _Earn()
    {

        GameManager.Instance.AddMoney(_earnData.EarnMoney);
        GameManager.Instance.AddPollution(_earnData.EarnPollution);

        _lastEarnTime += _unitInfor.EarnTime;

        _earnEffect.SetEarnData(_earnData.EarnMoney, _earnData.EarnMoney);
        _earnEffect.EffectOn();
    }

    private void _Loss()
    {

        _lastEarnTime -= _unitInfor.EarnTime;

        GameManager.Instance.AddMoney(-_earnData.EarnMoney);
        GameManager.Instance.AddPollution(-_earnData.EarnPollution);

        _earnEffect.SetEarnData(-_earnData.EarnMoney, -_earnData.EarnMoney);
        _earnEffect.EffectOn();

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
