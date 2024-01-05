using UnityEditor;
using UnityEngine;

namespace NamedAsset.Editor
{
    public class AssetSearchPopup : PopupWindowContent
    {
        public static int ControllId;
        public static string SelectAsset;
        public static bool IsClosed = true;
        public AssetSearchView View = new AssetSearchView();

        private static int s_PopupHash = "AssetSearchPopup".GetHashCode();
        private static int GetControlID(Rect activatorRect)
        {
            return GUIUtility.GetControlID(s_PopupHash, FocusType.Keyboard, activatorRect);
        }

        public static void Popup(Rect position, string val)
        {
            ControllId = GetControlID(position);
            SelectAsset = val;
            var instance = new AssetSearchPopup();
            instance.View.RefreshByAssetCollector();
        }

        public static string GetSelectKey(string val, Rect activatorRect)
        {
            if (GetControlID(activatorRect) == ControllId && IsClosed)
            {
                if (SelectAsset != val)
                {
                    GUI.changed = true;
                    val = SelectAsset;
                }
                ControllId = 0;
                SelectAsset = null;
            }
            return val;
        }

        public override void OnGUI(Rect rect)
        {
            View.OnGUI((asset) =>
            {
                SelectAsset = asset;
                IsClosed = true;
                editorWindow.Close();
            });
        }
    }
}
