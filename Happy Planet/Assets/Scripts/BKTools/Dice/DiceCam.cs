using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceCam : MonoBehaviour
{
    [SerializeField] private GameObject dice;
    private void Update()
    {
        Vector3 dicePos = new Vector3(dice.transform.localPosition.x,15, dice.transform.localPosition.z);
        
        transform.localPosition = dicePos;
    }
}