using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ConfigFile : ConfigPanel
{
    string[] fileNames;
    public GameObject fileButton;

    override public void Start()
    {
        /*Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 3;*/

        gameObject.AddComponent<GraphicRaycaster>();
        ListFile();
    }

    [ContextMenu("List File")]
    public void ListFile()
    {
        GameObject fileList = GameObject.Find("FileList");
        //GameObject fileButton = fileList.transform.GetChild(0).gameObject;

        foreach(Transform go in fileList.transform)
        {
            Destroy(go.gameObject);
        }


        int index = 0;

        fileNames = Directory.GetFiles(Application.persistentDataPath, "*.txt");
        foreach (string file in fileNames)
        {
            Debug.Log(file);
            GameObject go = Instantiate(fileButton);
            go.transform.SetParent(fileList.transform);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            //go.GetComponent<Button>().onClick.RemoveAllListeners();
            int i = index;
            //go.GetComponent<Button>().onClick.AddListener(() => LoadFile(i));
            go.GetComponentInChildren<TouchManager>().OnClick.AddListener(() => LoadFile(i));
            go.GetComponentInChildren<TouchManager>().OnLongClick.AddListener(() => DeleteFile(i));

            index++;

            Texture2D t = LoadPNG(file.Replace(".txt", ".png") ) ;

            go.GetComponentInChildren<RawImage>().texture = t;

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


    public void LoadFile(int i)
    {
        Debug.Log("Load File:" + fileNames[i]);
        GetComponent<Designer>().LoadFromFile(fileNames[i]);
    }

    public void DeleteFile(int i)
    {
        Debug.Log("Delete File:" + fileNames[i]);
        File.Delete(fileNames[i]);
        File.Delete(fileNames[i].Replace(".txt",".png"));

        ListFile();
    }

    public void Close()
    {
        Destroy(this.gameObject);
    }
}
