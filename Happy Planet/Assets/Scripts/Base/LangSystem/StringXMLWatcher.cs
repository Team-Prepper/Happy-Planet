using UnityEditor;
using UnityEngine;

namespace LangSystem {
    public class StringXMLWatcher : AssetPostprocessor {
        // 에셋이 임포트되거나 변경될 때 호출됩니다.
        // assetPath는 변경된 파일의 경로입니다.
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var assetPath in importedAssets)
            {
                if (!assetPath.Contains("XML/String/")) continue;

                StringManager.Instance.UpdateData();
                Debug.Log(assetPath + " file update");
            }
        }
    }
}