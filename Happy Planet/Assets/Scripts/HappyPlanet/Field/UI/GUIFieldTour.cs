using UnityEngine;
using UnityEngine.UI;
using EasyH.Unity.UI;

public class GUIFieldTour : GUIPlanetRotate {

    [SerializeField] private Text _timeText;
    [SerializeField] private Text _dayText;

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
        TimeSet();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        TimeSet();

    }

    public override void Close() {

        UIManager.Instance.OpenGUI<GUIFieldLoader>("FieldLoader").FieldClose();

        base.Close();

    }

}
