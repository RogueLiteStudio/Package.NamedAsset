using UnityEditor;
using UnityEngine;

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
        public Vector2 scrollPos;
        private void OnEnable()
        {
            if (settingEditor == null)
            {
                settingEditor = UnityEditor.Editor.CreateEditor(AssetPackSetting.instance);
            }
        }

        private void OnGUI()
        {
            using(var scroll = new GUILayout.ScrollViewScope(scrollPos))
            {
                scrollPos = scroll.scrollPosition;
                settingEditor.OnInspectorGUI();
            }
            if (GUILayout.Button("Build AssetBundle"))
            {
                AssetPackSetting.instance.Build();
            }
            GUILayout.Space(10);
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
