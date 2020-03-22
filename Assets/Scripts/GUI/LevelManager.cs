using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;



public class LevelManager : MonoBehaviour
{

    private static LevelManager instance;

    public int currentLevel;
    public int completedLevel;
    //public int levelMax;
    public float scrollViewHight = 0;
    public bool FirstLaunch = true;
    public bool WithStopper = true;
    public bool hacked = false;
    static public bool designerMode = false;
    public bool designerScene = false;
    public string levelPath;

    public float Volume = 0.5f;
    public string language = "english";
    public string Language
    {
        get
        {
            return language;
        }
        set
        {
            language = value;
        }
    }

    List<string> playgroundName;

    public GameObject text;


    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }

/*
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
*/

        //pluginClass.CallStatic("initialize", context);
/*
        string arguments = intent.Call<string>("getDataString");

        string t = text.GetComponent<Text>().text;
        text.GetComponent<Text>().text = t + arguments;
        //string uri = (new System.Uri(arguments));

        Cursor cursor = context.getContentResolver().query(uri, new String[] { android.provider.MediaStore.Images.ImageColumns.DATA }, null, null, null);
        ContentResolver resolver = getApplicationContext()
        .getContentResolver();


        t = text.GetComponent<Text>().text;
        text.GetComponent<Text>().text = t + uri;

        StartCoroutine(GetText(uri));*/
    }

/*
private string GetPathToImage(Android.Net.Uri uri)
{
    string doc_id = "";
    using (var c1 = ContentResolver.Query (uri, null, null, null, null)) {
        c1.MoveToFirst ();
        String document_id = c1.GetString (0);
        doc_id = document_id.Substring (document_id.LastIndexOf (":") + 1);
    }

    string path = null;

    // The projection contains the columns we want to return in our query.
    string selection = Android.Provider.MediaStore.Images.Media.InterfaceConsts.Id + " =? ";
    using (var cursor = ManagedQuery(Android.Provider.MediaStore.Images.Media.ExternalContentUri, null, selection, new string[] {doc_id}, null))
    {
        if (cursor == null) return path;
        var columnIndex = cursor.GetColumnIndexOrThrow(Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data);
        cursor.MoveToFirst();
        path = cursor.GetString(columnIndex);
    }
    return path;
}
*/

    IEnumerator GetText(String path)
    {
        UnityWebRequest www = UnityWebRequest.Get(path);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;

            string t = text.GetComponent<Text>().text;
            text.GetComponent<Text>().text = t + results.ToString();

        }
    }




    public int LevelMax()
    {
        ListLevel list = GetComponent<ListLevel>();
        return list.names.Count;
    }

    /*public void LoadPlaygroundList()
    {
        string names = Resources.Load<TextAsset>("Levels/PlaygroundList").ToString();
        names = names.Replace("\r", ""); //clean up string 
        playgroundName = new List<string>(names.Split('\n'));
    }*/

    public bool LevelIsCompleted(int i)
    {
        if (i < 1) return true; // level 0 alway completed
        string s = PlayerPrefs.GetString("Level-" + GetPlaygroundName(i));
        if (s == "completed") return true;
        return false;
    }

    public void ResetGame()
    {
        for (int i = 1; i <= LevelMax(); i++)
        {
            PlayerPrefs.SetString("Level-" + GetPlaygroundName(i), "");
        }
    }

    public void LevelCompleted(int i)
    {
        if (i < 1) return;
        PlayerPrefs.SetString("Level-" + GetPlaygroundName(i), "completed");
        PlayerPrefs.Save();
    }


    public string GetPlaygroundName(int level)
    {
        // Return the complete name (with path in Ressources) of the prefab playground
        // level start at 1  

        ListLevel list = GetComponent<ListLevel>();

        if (1 <= level && level <= list.names.Count)
            return list.names[level - 1];
        else
            return "";

    }



}
