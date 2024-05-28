using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EarnEffect : MonoBehaviour, IEffect
{
    [SerializeField] float _showTime = 0.3f;

    [SerializeField] Transform _inforTr;
    [SerializeField] Text _polution;
    [SerializeField] Text _money;

    float _spendTime;

    public void SetEarnData(int polution, int money) { 
        _polution.text = (polution < 0 ? "" : "+") + polution.ToString();
        _money.text = (money < 0 ? "" : "+") + money.ToString();
    }

    public void EffectOn() {
        transform.LookAt(Camera.main.transform);
        _inforTr.up = Camera.main.transform.up;
        _inforTr.position = transform.position;
        _spendTime = 0;
        gameObject.SetActive(true);
        //StopAllCoroutines();
        //StartCoroutine(_Effect());
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

    IEnumerator _Effect() {
        float spendTime = 0;

        while (spendTime < _showTime)
        {
            yield return null;
        }

    }
}
