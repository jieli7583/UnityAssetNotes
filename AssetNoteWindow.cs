// 改进：在 Notes 区域添加手动保存按钮，避免丢失内容

using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class AssetNoteEditor : Editor
{
    static string previousSelectedAssetPath = "";
    static string selectedAssetPath = "";

    static AssetNoteEditor()
    {
        // 监听选择变化
        Selection.selectionChanged += OnSelectionChanged;
    }

    private static void OnSelectionChanged()
    {
        string selectedObjectPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (!string.IsNullOrEmpty(selectedObjectPath) && (AssetDatabase.IsValidFolder(selectedObjectPath) || System.IO.File.Exists(selectedObjectPath)) && selectedObjectPath != selectedAssetPath)
        {
            previousSelectedAssetPath = selectedAssetPath;
            selectedAssetPath = selectedObjectPath;
        }
        else
        {
            selectedAssetPath = "";
        }
    }

    [CustomEditor(typeof(UnityEngine.Object), true)]
    public class AssetInspector : Editor
    {
        private static string assetNote = "";
        private string lastSavedNote = "";
        private Vector2 scrollPosition = Vector2.zero;

        private void OnEnable()
        {
            SaveSelectedNote();
            OnSelectionChanged();
            LoadNote();
        }

        public override void OnInspectorGUI()
        {
            string path = AssetDatabase.GetAssetPath(target);
            if (!string.IsNullOrEmpty(path) && (AssetDatabase.IsValidFolder(path) || System.IO.File.Exists(path)))
            {
                base.OnInspectorGUI();
                bool isFolder = target.GetType() == typeof(DefaultAsset);
                AssetInspectorGUI(isFolder);
            }
            else
            {
                base.OnInspectorGUI();
            }
        }

        private void AssetInspectorGUI(bool isFolder)
        {
            if (isFolder) EditorGUI.EndDisabledGroup();

            EditorGUILayout.LabelField("Notes", EditorStyles.boldLabel);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(100));
            assetNote = EditorGUILayout.TextArea(assetNote, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("💾 Save Note"))
            {
                SaveSelectedNote();
            }

            if (isFolder) EditorGUI.BeginDisabledGroup(true);
        }

        public void LoadNote()
        {
            if (!string.IsNullOrEmpty(selectedAssetPath))
            {
                AssetImporter importer = AssetImporter.GetAtPath(selectedAssetPath);
                assetNote = importer.userData;
                lastSavedNote = assetNote;
            }
        }

        private void SaveSelectedNote()
        {
            if (!string.IsNullOrEmpty(selectedAssetPath))
            {
                AssetImporter importer = AssetImporter.GetAtPath(selectedAssetPath);
                importer.userData = assetNote;
                lastSavedNote = assetNote;
                EditorUtility.SetDirty(importer);
                AssetDatabase.WriteImportSettingsIfDirty(selectedAssetPath);
                AssetDatabase.SaveAssets();
            }
        }
    }

    [CustomEditor(typeof(MonoScript))] public class ScriptInspector : AssetInspector { }
    [CustomEditor(typeof(TextAsset))] public class TextInspector : AssetInspector { }
    [CustomEditor(typeof(Material))] public class MaterialInspector : AssetInspector { }
}
