using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryCloud : MonoBehaviour
{

    [SerializeField] Cloud _cloud;
    [SerializeField] Transform _createPos;
    [SerializeField] float _createTerm;
    [SerializeField] float _spendTime;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        _spendTime += Time.deltaTime;

        if (_spendTime > _createTerm) {
            _spendTime -= _createTerm;
            Transform temp = Instantiate(_cloud, transform.position, Quaternion.identity).transform;
            temp.parent = transform;
        }
    }
}
