using EHTool.UIKit;

public class GUITitle : GUIFullScreen {
    public void OpenField() {
        UIManager.Instance.OpenGUI<GUIFieldLoader>("FieldLoader").FieldLoad("temp");
    }
}