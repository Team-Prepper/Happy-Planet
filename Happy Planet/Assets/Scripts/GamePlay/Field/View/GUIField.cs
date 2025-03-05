using UnityEngine;
using UnityEngine.UI;

public class GUIField : GUIPlanetRotate {

    [SerializeField] private Text _timeText;
    [SerializeField] private Text _dayText;
    [SerializeField] private Text _moneyText;
    [SerializeField] private Text _energyText;

    void TimeSet()
    {
        FieldData fieldData = GameManager.Instance.Field.FieldData;
        int gameTime = Mathf.Max(0, Mathf.RoundToInt(fieldData.SpendTime * 1440));

        _timeText.text = string.Format("{0:D2}:{1:D2}",
            (gameTime / 60) % 24, gameTime % 60);
        _dayText.text = string.Format("Day {0}", fieldData.Day);
    }


    public override void Open()
    {
        base.Open();
        _touchEvent = () =>
        {
            IInteractable target = _GetInteractable();
            if (target == null) return;
            target.Interaction();

        };
        TimeSet();
    }

    // Update is called once per frame
    protected override void Update()
    {
        TimeSet();

        FieldData fieldData = GameManager.Instance.Field.FieldData;

        _moneyText.text = fieldData.Money.ToString();
        _energyText.text = fieldData.Energy.ToString();

        base.Update();

    }
    IInteractable _GetInteractable()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.TryGetComponent(out IInteractable retval))
                return retval;
        }
        
        return null;
    }

}
