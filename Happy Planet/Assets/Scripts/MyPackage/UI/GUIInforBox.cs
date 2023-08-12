using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIInforBox : GUIPopUp {

    [SerializeField] TextMeshProUGUI _time;


    protected override void Open()
    {
        UIManager.Instance.InforBox = this;

    }

    
}
