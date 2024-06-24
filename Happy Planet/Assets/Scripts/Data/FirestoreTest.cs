using EHTool.UIKit;

public class FirestoreTest : GUIFullScreen {

    FirestoreConnector<int> _test;

    public override void Open()
    {
        _test = new FirestoreConnector<int>();
        _test.Connect("LogData");
    }

    public void Button1()
    {
        _test.UpdateRecordAt(1, 10);
    }

    public void Button2()
    {
        _test.AddRecord(-4);
    }

}
