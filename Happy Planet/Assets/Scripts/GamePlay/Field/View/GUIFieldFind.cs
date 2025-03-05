using EHTool.UIKit;
using UnityEngine;
using UnityEngine.UI;

public class GUIFieldFind : GUIPopUp
{

    [SerializeField] InputField _id;

    public void MoveOtherField(string fieldName) {

        IGUIFullScreen parent = UIManager.Instance.NowDisplay;

        UIManager.Instance.OpenGUI<GUIFieldLoader>("FieldLoader").FieldLoad(
            new PlaygroundField(),
            GameManager.Instance.Auth.GetUserId(),
            fieldName, 
            () => {
                parent.Close();
                UIManager.Instance.OpenGUI<GUIFullScreen>("Field");
            });

    }

    public void Tour()
    {
        Close();

        UIManager.Instance.OpenGUI<GUIFieldLoader>("FieldLoader").FieldLoad(new TourField(),
            _id.text,
            "",
            () => {
            UIManager.Instance.OpenGUI<GUIFullScreen>("FieldTour");
        });

    }

}
