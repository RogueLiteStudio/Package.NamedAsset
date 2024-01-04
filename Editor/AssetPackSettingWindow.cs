using UnityEditor;

namespace NamedAsset.Editor
{
    [EditorWindowTitle(title = "打包资源编辑器")]
    public class AssetPackSettingWindow : EditorWindow
    {
        [MenuItem("Window/打包资源编辑器")]
        public static void ShowWindow()
        {
            GetWindow<AssetPackSettingWindow>();
        }

        public UnityEditor.Editor settingEditor;

        private void OnEnable()
        {
            if (settingEditor == null)
            {
                settingEditor = UnityEditor.Editor.CreateEditor(AssetPackSetting.instance);
            }
        }

        private void OnGUI()
        {
            settingEditor.OnInspectorGUI();
        }


        private void OnDestroy()
        {
            if (settingEditor != null)
            {
                DestroyImmediate(settingEditor);
            }
            AssetPackSetting.instance.Save();
        }
    }
}
