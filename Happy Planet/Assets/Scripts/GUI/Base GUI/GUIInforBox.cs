using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UISystem;

public class GUIInforBox : GUIPopUp {

    [SerializeField] TextMeshProUGUI _time;


    public override void Open()
    {
        //UIManager.Instance.InforBox = this;

    }

    
}