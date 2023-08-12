using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] float _maxHeight;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localPosition.y > _maxHeight) {
            Destroy(gameObject);
        }
        transform.localPosition += Vector3.up * Time.deltaTime * _speed;
    }
}
