using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EHTool.UIKit {

    public class UIManager : Singleton<UIManager> {
        public IGUIFullScreen NowDisplay { get; private set; }

        IDictionary<string, string> _dic;
        IQueue<IGUIFullScreen> uiStack;

        private GUIMessageBox _msgBox;

        public void OpenFullScreen(IGUIFullScreen newData)
        {
            if (NowDisplay != null)
            {
                uiStack.Enqueue(NowDisplay);
            }

            uiStack.Enqueue(newData);

            IGUIFullScreen tmp = uiStack.Dequeue();

            if (tmp == NowDisplay)
            {
                newData?.SetOff();
                return;
            }

            NowDisplay?.SetOff();
            NowDisplay = newData;
            NowDisplay.SetOn();

        }

        public void CloseFullScreen(IGUIFullScreen closeFullScreen)
        {
            if (NowDisplay != closeFullScreen)
            {
                uiStack.Remove(closeFullScreen);
                return;
            }

            if (uiStack.Count < 1)
                return;

            NowDisplay = uiStack.Dequeue();
            NowDisplay.SetOn();

        }

        protected override void OnCreate()
        {
            NowDisplay = null;
            uiStack = new StablePriorityQueue<IGUIFullScreen>();

            IDictionaryConnector<string, string> connector =
                new JsonDictionaryConnector<string, string>();
            ///new XMLDictionaryReader<string, string>();

            _dic = connector.ReadData("GUIInfor");

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            NowDisplay = null;
            uiStack = new StablePriorityQueue<IGUIFullScreen>();

        }

        public T OpenGUI<T>(string guiName, Action callback = null) where T : Component, IGUI
        {
            string path = Instance._dic[guiName];

            GameObject retGO = AssetOpener.ImportGameObject(path);
            retGO.GetComponent<IGUI>().Open(callback);

            return retGO.GetComponent<T>();
        }

        public void DisplayMessage(string messageContent)
        {
            if (_msgBox == null)
            {
                _msgBox = OpenGUI<GUIMessageBox>("MessageBox");
            }

            _msgBox.SetMessage(messageContent);
        }

    }
}