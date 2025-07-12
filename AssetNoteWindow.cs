using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class AssetNoteWindow : Editor
{
    static string previousSelectedAssetPath = "";
    static string selectedAssetPath = "";

    static AssetNoteWindow()
    {
        // 安全监听资源选择变化
        Selection.selectionChanged += OnSelectionChanged;
    }

    private static void OnSelectionChanged()
    {
        if (Selection.activeObject == null) return;

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (!string.IsNullOrEmpty(path) &&
            (AssetDatabase.IsValidFolder(path) || System.IO.File.Exists(path)) &&
            path != selectedAssetPath)
        {
            previousSelectedAssetPath = selectedAssetPath;
            selectedAssetPath = path;
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
            // 只有在新的资源合法时才加载备注
            OnSelectionChanged();
            LoadNote();
        }

        public override void OnInspectorGUI()
        {
            string path = AssetDatabase.GetAssetPath(target);
            if (!string.IsNullOrEmpty(path) &&
                (AssetDatabase.IsValidFolder(path) || System.IO.File.Exists(path)))
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
            if (isFolder) EditorGUI.EndDisabledGroup(); // Unity Bug workaround

            EditorGUILayout.LabelField("Notes", EditorStyles.boldLabel);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(100));
            assetNote = EditorGUILayout.TextArea(assetNote, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Save Note"))
            {
                SaveSelectedNote();
            }

            if (isFolder) EditorGUI.BeginDisabledGroup(true);
        }

        private void LoadNote()
        {
            if (string.IsNullOrEmpty(selectedAssetPath)) return;

            var importer = AssetImporter.GetAtPath(selectedAssetPath);
            if (importer != null)
            {
                assetNote = importer.userData;
                lastSavedNote = assetNote;
            }
        }

        private void SaveSelectedNote()
        {
            if (string.IsNullOrEmpty(selectedAssetPath)) return;

            var importer = AssetImporter.GetAtPath(selectedAssetPath);
            if (importer != null)
            {
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

