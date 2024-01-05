using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NamedAsset.Editor
{
    [System.Serializable]
    public class AssetSearchView
    {
        [System.Serializable]
        public class AssetInfo
        {
            public string Name;
            public string Path;
            public Object Asset;
            public Texture Preview;
        }
        [System.Serializable]
        public class AssetGroup
        {
            public string Name;
            public List<AssetInfo> Assets = new List<AssetInfo>();
            public bool Foldout = true;
        }

        public List<AssetGroup> Groups = new List<AssetGroup>();
        public Vector2 ScrollPos;
        public string SearchText;
        public string Selected;

        public void RefreshByAssetCollector()
        {
            Groups.Clear();
            foreach (var package in AssetCollector.instance.Packages)
            {
                AssetGroup group = new AssetGroup
                {
                    Name = package.Name
                };
                foreach (var asset in package.Assets)
                {
                    AssetInfo assetInfo = new AssetInfo
                    {
                        Name = asset.Name,
                        Path = asset.Path
                    };
                    group.Assets.Add(assetInfo);
                }
                Groups.Add(group);
            }
        }

        public void OnGUI(System.Action<string> onSelect)
        {
            using(new GUILayout.HorizontalScope())
            {
                SearchText = EditorGUILayout.TextField(SearchText, "SearchTextField");
                if (GUILayout.Button("", "SearchCancelButton"))
                {
                    SearchText = "";
                }
            }
            using(var scroll = new GUILayout.ScrollViewScope(ScrollPos))
            {
                ScrollPos = scroll.scrollPosition;
                foreach (var group in Groups)
                {
                    group.Foldout = EditorGUILayout.Foldout(group.Foldout, group.Name, true);
                    if (!group.Foldout)
                        continue;
                    foreach (var asset in group.Assets)
                    {
                        if (!string.IsNullOrEmpty(SearchText) && asset.Name.IndexOf(SearchText) < 0)
                            continue;
                        if (asset.Asset == null)
                        {
                            asset.Asset = AssetDatabase.LoadMainAssetAtPath(asset.Path);
                        }
                        if (asset.Asset && asset.Preview == null)
                        {
                            asset.Preview = AssetPreview.GetAssetPreview(asset.Asset);
                        }
                        string bg = Selected == asset.Name ? "sv_iconselector_back" : "Box";
                        using (new GUILayout.HorizontalScope(bg))
                        {
                            using (new GUILayout.VerticalScope())
                            {
                                if (GUILayout.Button(asset.Name))
                                {
                                    Selected = asset.Name;
                                    onSelect?.Invoke(asset.Name);
                                }
                                if (asset.Asset)
                                    EditorGUILayout.ObjectField(asset.Asset, asset.Asset.GetType(), false);
                            }
                            if (asset.Preview && GUILayout.Button(asset.Preview, GUILayout.Width(64), GUILayout.Height(64)))
                            {
                                Selected = asset.Name;
                                onSelect?.Invoke(asset.Name);
                            }
                        }
                    }
                }
            }
        }
    }
}
