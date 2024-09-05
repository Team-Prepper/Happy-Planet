using EHTool.LangKit;
using EHTool.UIKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIFieldFind : GUIPopUp
{

    [SerializeField] InputField _id;

    public void Tour() {

        Close();
        UIManager.Instance.OpenGUI<GUIFieldLoader>("FieldLoader").FieldLoad(new TourField(), _id.text, "", () => {
            UIManager.Instance.OpenGUI<GUIFullScreen>("FieldTour");
        });
    }

}
