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

    public float InstantiateTime { get; private set; }
    public int NowLevel { get; private set; }

    float _lastEarnTime = 0;

    public float LifeSpanRatio => (GameManager.Instance.SpendTime - InstantiateTime) / _unitInfor.LifeSpan;

    public float EarnRatio => (GameManager.Instance.SpendTime - _lastEarnTime) / _unitInfor.EarnTime;

    public void SetInfor(UnitInfor infor, float instantiateTime, int level)
    {
        _unitInfor = infor;
        InstantiateTime = instantiateTime;
        _lastEarnTime = GameManager.Instance.SpendTime - (GameManager.Instance.SpendTime - instantiateTime - Mathf.Floor(GameManager.Instance.SpendTime - instantiateTime));

        NowLevel = level;

        _CreatePrefabAt(infor.GetPrefab(NowLevel), _liveZone.transform);
        _CreatePrefabAt(infor.GetDeathPrefab(), _deathZone.transform);
    }

    private void LateUpdate()
    {

        if (LifeSpanRatio > 1f)
        {
            _liveZone.SetActive(false);
            _deathZone.SetActive(true);
            return;
        }

        if (LifeSpanRatio < 0f)
        {
            //UIManager.Instance.OpenGUI<GUIUnitRemove>("UnitRemove").SetTarget(this);
            return;
        }

        _deathZone.SetActive(false);
        _liveZone.SetActive(true);

        if (EarnRatio >= 1)
        {
            GameManager.Instance.AddMoney(_unitInfor.GetEarnMoney(NowLevel));
            GameManager.Instance.AddPollution(_unitInfor.GetEarnPollution(NowLevel));

            _lastEarnTime += _unitInfor.EarnTime;

            _earnEffect.SetEarnData(_unitInfor.GetEarnPollution(NowLevel), _unitInfor.GetEarnMoney(NowLevel));
            _earnEffect.EffectOn();

            return;
        }
        if (EarnRatio < 0f) {
            _lastEarnTime -= _unitInfor.EarnTime;

            GameManager.Instance.AddMoney(-_unitInfor.GetEarnMoney(NowLevel));
            GameManager.Instance.AddPollution(-_unitInfor.GetEarnPollution(NowLevel));

            _earnEffect.SetEarnData(-_unitInfor.GetEarnPollution(NowLevel), -_unitInfor.GetEarnMoney(NowLevel));
            _earnEffect.EffectOn();
            return;
        }


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

    private void _CreatePrefabAt(GameObject prefab, Transform parent) {

        while (parent.childCount > 0)
        {
            Destroy(parent.GetChild(0).gameObject);
        }

        Transform tr = Instantiate(prefab, parent).transform;

        tr.localPosition = Vector3.zero;
        tr.up = transform.up;
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
