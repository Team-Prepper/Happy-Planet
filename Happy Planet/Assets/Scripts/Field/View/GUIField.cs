using UnityEngine;
using UnityEngine.UI;

public class GUIField : GUIPlanetRotate {

    [SerializeField] Text _timeText;
    [SerializeField] Text _dayText;
    [SerializeField] Text _moneyText;
    [SerializeField] Text _energyText;
    void TimeSet()
    {
        int gameTime = Mathf.Max(0, Mathf.RoundToInt(GameManager.Instance.Field.SpendTime * 1440));

        _timeText.text = string.Format("{0:D2}:{1:D2}", (gameTime / 60) % 24, gameTime % 60);
        _dayText.text = string.Format("Day {0}", GameManager.Instance.Field.Day);
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
        int gameTime = Mathf.Max(0, Mathf.RoundToInt(GameManager.Instance.Field.SpendTime * 1440));

        TimeSet();

        _moneyText.text = GameManager.Instance.Field.Money.ToString();
        _energyText.text = GameManager.Instance.Field.Energy.ToString();

        base.Update();

    }
    IInteractable _GetInteractable()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.TryGetComponent(out IInteractable retval))
                return retval;
        }
        return null;
    }

}
