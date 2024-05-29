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

    float _lastEarnTime = 0;

    bool _destroyFlag = false;

    float LifeSpan => (GameManager.Instance.SpendTime - InstantiateTime);

    public float LifeSpanRatio => LifeSpan / _unitInfor.LifeSpan;

    public float EarnRatio => (GameManager.Instance.SpendTime - _lastEarnTime) / _unitInfor.EarnTime;

    public void SetInfor(UnitInfor infor, float instantiateTime, int id = 0)
    {
        _unitInfor = infor;
        InstantiateTime = instantiateTime;

        int temp = Mathf.RoundToInt(1 / infor.EarnTime);
        _lastEarnTime = InstantiateTime + Mathf.Round(LifeSpan / infor.EarnTime) * infor.EarnTime;

        Id = id;
        NowLevel = 0;

        _CreatePrefabAt(infor.GetPrefab(NowLevel), _liveZone.transform);
        _CreatePrefabAt(infor.GetDeathPrefab(), _deathZone.transform);
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
        _CreatePrefabAt(GetInfor().GetPrefab(NowLevel), _liveZone.transform);

    }

    public void SetLevel(int level)
    {
        NowLevel = level;

        Debug.Log(level);

        _levelUpEvent.Invoke();
        _CreatePrefabAt(GetInfor().GetPrefab(NowLevel), _liveZone.transform);
    }

    public void Remove()
    {
        _destroyFlag = true;
        Destroy(gameObject);
    }


    private void _Earn()
    {

        GameManager.Instance.AddMoney(_unitInfor.GetEarnMoney(NowLevel));
        GameManager.Instance.AddPollution(_unitInfor.GetEarnPollution(NowLevel));

        _lastEarnTime += _unitInfor.EarnTime;

        _earnEffect.SetEarnData(_unitInfor.GetEarnPollution(NowLevel), _unitInfor.GetEarnMoney(NowLevel));
        _earnEffect.EffectOn();
    }

    private void _Loss()
    {

        _lastEarnTime -= _unitInfor.EarnTime;

        GameManager.Instance.AddMoney(-_unitInfor.GetEarnMoney(NowLevel));
        GameManager.Instance.AddPollution(-_unitInfor.GetEarnPollution(NowLevel));

        _earnEffect.SetEarnData(-_unitInfor.GetEarnPollution(NowLevel), -_unitInfor.GetEarnMoney(NowLevel));
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
