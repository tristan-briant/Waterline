using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using UnityEngine.SceneManagement;


[CustomEditor(typeof(ListLevel))]
public class LevelDataEditor : Editor
{
    private ReorderableList list;

    const float height = 100;

    private void OnEnable()
    {
        list = new ReorderableList(serializedObject, serializedObject.FindProperty("names"))
        {
            elementHeightCallback = (x) => { return height; }
        };

        list.drawElementCallback =
        (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, 150, EditorGUIUtility.singleLineHeight),
                element, GUIContent.none);


            string path = element.stringValue.Replace(".txt", "");

            Texture texture = Resources.Load<Texture>("Levels/" + path);

            if (texture)
                EditorGUI.DrawPreviewTexture(
                    new Rect(rect.x + 160, rect.y, height - 5, height - 5), texture);

            if (GUI.Button(new Rect(rect.x + 260, rect.y, 50, 2*EditorGUIUtility.singleLineHeight), "Load"))
            {
                LevelManager LVM = FindObjectOfType<LevelManager>();
                LVM.designerScene = true;
                LVM.currentLevel = index + 1;
                
                SceneManager.LoadScene("LevelScene");
            }

            if (GUI.Button(new Rect(rect.x + 300, rect.y + EditorGUIUtility.singleLineHeight + 30, 
                40, EditorGUIUtility.singleLineHeight), "Save"))
            {
                Designer designer = FindObjectOfType<Designer>();
                designer.SaveToResources(element.stringValue);
            }


        };


        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Levels names");
        };


    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();

        DrawDefaultInspector();

        if (GUILayout.Button("Save List"))
         {
            (serializedObject.targetObject as ListLevel).Save();
         }
    }



    public static Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }

}
