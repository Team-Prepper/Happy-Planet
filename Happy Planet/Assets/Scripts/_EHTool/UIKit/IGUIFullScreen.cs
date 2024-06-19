namespace EHTool.UIKit {
    public interface IGUIFullScreen : IGUI {
        public void AddPopUp(IGUIPopUp popUp);
        public void PopPopUp();
        public void AddPanel(IGUIPanel panel);
        public void ClosePanel();
    }
}