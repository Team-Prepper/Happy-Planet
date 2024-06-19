using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EHTool.UIKit {

    public class UIManager : Singleton<UIManager> {

        class GUIData {
            internal string name;
            internal string path;

            internal void Read(XmlNode node)
            {
                name = node.Attributes["name"].Value;
                path = node.Attributes["path"].Value;
            }
        }

        public IGUIFullScreen NowDisplay { get; private set; }

        IDictionary<string, GUIData> _dic;
        IList<IGUIFullScreen> uiStack;

        private GUIMessageBox _msgBox;

        public void EnrollmentGUI(IGUIFullScreen newData)
        {

            if (NowDisplay == null)
            {
                NowDisplay = newData;
                return;

            }

            NowDisplay.SetOff();
            uiStack.Add(NowDisplay);
            uiStack.Add(newData);

            Pop();

        }

        public void Pop()
        {
            if (uiStack.Count < 1)
                return;

            NowDisplay = uiStack[uiStack.Count - 1];
            uiStack.RemoveAt(uiStack.Count - 1);
            NowDisplay.SetOn();

        }

        protected override void OnCreate()
        {
            NowDisplay = null;
            uiStack = new List<IGUIFullScreen>();

            _dic = new Dictionary<string, GUIData>();
            XmlDocument xmlDoc = AssetOpener.ReadXML("GUIInfor");

            XmlNodeList nodes = xmlDoc.SelectNodes("List/Element");

            for (int i = 0; i < nodes.Count; i++)
            {
                GUIData guiData = new GUIData();
                guiData.Read(nodes[i]);

                _dic.Add(guiData.name, guiData);
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            NowDisplay = null;
            uiStack = new List<IGUIFullScreen>();

        }

        public T OpenGUI<T>(string guiName)
        {

            string path = Instance._dic[guiName].path;

            GameObject retGO = AssetOpener.ImportGameObject(path);
            retGO.GetComponent<IGUI>().Open();

            return retGO.GetComponent<T>();
        }

        public void DisplayMessage(string messageContent)
        {
            if (_msgBox == null) _msgBox = OpenGUI<GUIMessageBox>("MessageBox");
            else _msgBox.SetOn();

            _msgBox.SetMessage(messageContent);
        }

    }
}
