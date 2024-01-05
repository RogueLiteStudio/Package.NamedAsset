using UnityEditor;
using UnityEngine;

namespace NamedAsset.Editor
{
    public static class AssetSearchGUI
    {
        public static string Pop(Rect position, string val)
        {
            Rect btnRect = position;
            position.width -= 30;
            btnRect.x = position.xMax;
            btnRect.width = 30;

            val = EditorGUI.TextField(position, val);
            if (GUI.Button(btnRect, EditorGUIUtility.TrIconContent("d_SearchOverlay")))
            {
                AssetSearchPopup.Popup(position, val);
            }

            return AssetSearchPopup.GetSelectKey(val, position);
        }

        public static string LayoutPop(string val, params GUILayoutOption[] options)
        {
            var position = EditorGUILayout.GetControlRect(false, 18f, EditorStyles.popup, options);
            return Pop(position, val);
        }
    }
}
