using UnityEngine;
using UnityEngine.UI;

public class TextEarnEffect : EarnEffectBase
{
    [SerializeField] float _showTime = 0.3f;
    [SerializeField] GameObject _energyGroup;
    [SerializeField] GameObject _moneyGroup;


    [SerializeField] Transform _inforTr;
    [SerializeField] Text _energy;
    [SerializeField] Text _money;

    [SerializeField] float _spendTime;

    public override void EarnEffectOn(int money, int energy)
    {
        _money.text = (money < 0 ? "" : "+") + money.ToString();
        _energy.text = (energy < 0 ? "" : "+") + energy.ToString();

        _moneyGroup.SetActive(money != 0);
        _energyGroup.SetActive(energy != 0);

        EffectOn();
    }

    public void EffectOn() {
        transform.LookAt(Camera.main.transform);
        _inforTr.position = transform.position;
        _spendTime = 0;
        gameObject.SetActive(true);
    }

    void Start()
    {
        transform.forward = Vector3.back;
        float upFactor = Vector3.Dot(Vector3.up, Camera.main.transform.up);
        _inforTr.up = new Vector3(-Mathf.Sqrt(1 - upFactor * upFactor), upFactor);
    }

    void Update() {

        if (_spendTime >= _showTime)
        {
            gameObject.SetActive(false);
            return;
        }

        _inforTr.Translate(Vector3.up * Time.deltaTime);
        _spendTime += Time.deltaTime;
        transform.LookAt(Camera.main.transform);

    }
}
