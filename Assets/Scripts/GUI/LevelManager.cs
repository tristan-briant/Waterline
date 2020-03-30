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


        /*AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");*/


        /******* get the uri content: ****************/
        AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject context = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

        String dataString = context.Call<AndroidJavaObject>("getIntent").Call<String>("getDataString");

        Debug.LogError("URL Data " + dataString);

        string t = text.GetComponent<Text>().text;
        t += "URL Data :" + dataString;
        text.GetComponent<Text>().text = t;
        /*
                //FileDescriptor fd = inputPFD.getFileDescriptor();
                AndroidJavaObject inputPFD = unityPlayerClass.GetStatic<AndroidJavaObject>("ParcelFileDescriptor");
                inputPFD = unityPlayerClass.GetStatic<AndroidJavaObject>("getContentResolver")
                            .Call<String>("openFileDescriptor",dataString,"r");

                AndroidJavaObject fd = unityPlayerClass.GetStatic<AndroidJavaObject>("FileDescriptor");
                fd = inputPFD

        */
    
        AndroidJavaObject activityObject = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject intent = activityObject.Call<AndroidJavaObject>("getIntent");

        // Now actually get the data. We should be able to get it from the result of AndroidJNI.CallObjectMethod, but I don't now how so just call again
        AndroidJavaObject intentURI = intent.Call<AndroidJavaObject>("getData");

        // Open the URI as an input channel
        AndroidJavaObject contentResolver = activityObject.Call<AndroidJavaObject>("getContentResolver");
        AndroidJavaObject inputStream = contentResolver.Call<AndroidJavaObject>("openInputStream", intentURI);
        //AndroidJavaObject inputChannel = inputStream.Call<AndroidJavaObject>("getChannel");

        byte[] data = inputStream.Call<byte[]>("readAllBytes");
        // Close the streams
        inputStream.Call("close");



        //data = ImportFromIntent();
        t += "\n Data : " + data;
        text.GetComponent<Text>().text = t;



        //StartCoroutine(GetText(dataString));
        /******* ! get the uri content: ****************/
        /*
                AndroidJavaClass intentObj = new AndroidJavaClass("android.content.Intent");
                string ACTION_VIEW = intentObj.GetStatic<string>("ACTION_VIEW");
                int FLAG_ACTIVITY_NEW_TASK = intentObj.GetStatic<int>("FLAG_ACTIVITY_NEW_TASK");
                AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", ACTION_VIEW);

                AndroidJavaObject fileObj = new AndroidJavaObject("java.io.File", apkPath);
                AndroidJavaClass uriObj = new AndroidJavaClass("android.net.Uri");
                AndroidJavaObject uri = uriObj.CallStatic<AndroidJavaObject>("fromFile", fileObj);

                intent.Call<AndroidJavaObject>("setDataAndType", uri, "application/vnd.android.package-archive");
                intent.Call<AndroidJavaObject>("addFlags", FLAG_ACTIVITY_NEW_TASK);
                intent.Call<AndroidJavaObject>("setClassName", "com.android.packageinstaller", "com.android.packageinstaller.PackageInstallerActivity");

                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                currentActivity.Call("startActivity", intent);

                AndroidJavaObject fileObj = new AndroidJavaObject("java.io.File", apkPath);
                AndroidJavaClass fileProvider = new AndroidJavaClass("android.support.v4.content.FileProvider");
                AndroidJavaObject uri = fileProvider.CallStatic<AndroidJavaObject>("getUriForFile", unityContext, authority, fileObj);
        */


        //pluginClass.CallStatic("initialize", context);
        //string arguments = intent.Call<string>("getDataString");

        /*string[] args = System.Environment.GetCommandLineArgs();
        string input = " toto";
        for (int i = 0; i < args.Length; i++)
        {
            
                input = args[i] +" ";
            
        }*/

        /*
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

    private byte[] ImportFromIntent()
    {
        try
        {
            // Get the current activity
            AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activityObject = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

            // Get the current intent
            AndroidJavaObject intent = activityObject.Call<AndroidJavaObject>("getIntent");

            // Get the intent data using AndroidJNI.CallObjectMethod so we can check for null
            IntPtr method_getData = AndroidJNIHelper.GetMethodID(intent.GetRawClass(), "getData", "()Ljava/lang/Object;");
            IntPtr getDataResult = AndroidJNI.CallObjectMethod(intent.GetRawObject(), method_getData, AndroidJNIHelper.CreateJNIArgArray(new object[0]));
            if (getDataResult.ToInt32() != 0)
            {
                // Now actually get the data. We should be able to get it from the result of AndroidJNI.CallObjectMethod, but I don't now how so just call again
                AndroidJavaObject intentURI = intent.Call<AndroidJavaObject>("getData");

                // Open the URI as an input channel
                AndroidJavaObject contentResolver = activityObject.Call<AndroidJavaObject>("getContentResolver");
                AndroidJavaObject inputStream = contentResolver.Call<AndroidJavaObject>("openInputStream", intentURI);
                //AndroidJavaObject inputChannel = inputStream.Call<AndroidJavaObject>("getChannel");

                // Open an output channel
                //AndroidJavaObject outputStream = new AndroidJavaObject("java.io.FileOutputStream", importPath);
                //AndroidJavaObject outputChannel = outputStream.Call<AndroidJavaObject>("getChannel");

                // Copy the file
                //long bytesTransfered = 0;
                //long bytesTotal = inputStream.Call<long>("size");
                /*while (bytesTransfered < bytesTotal && bytesTransfered<100)
                {
                    bytesTransfered += inputChannel.Call<long>("transferTo", bytesTransfered, bytesTotal, outputChannel);
                }*/

                byte[] data = inputStream.Call<byte[]>("readAllBytes");
                // Close the streams
                inputStream.Call("close");
                return data;
                //outputStream.Call("close");
            }
        }
        catch (System.Exception ex)
        {

            // Handle error
        }
        return null;
    }


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
