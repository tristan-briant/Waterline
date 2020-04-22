using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;

public class ShareFile : MonoBehaviour
{


    void Start()
    {
        string[] filters = { ".waterline" };
        FileBrowser.SetFilters(true, filters);
        FileBrowser.SetDefaultFilter(".waterline");
#if UNITY_STANDALONE
        GameObject go =GameObject.Find("ButtonShareImage");
        if(go!=null) 
            go.SetActive(false);
        go =GameObject.Find("ButtonShareLevel");
        if(go!=null) 
            go.SetActive(false);
#endif
    }

    public void ShareScreenshot()
    {
        StartCoroutine("TakeSSAndShare");
    }

    public void ShareLevel()
    {
        StartCoroutine("ShareDataLevel");
        //FileBrowser.ShowSaveDialog((path) => { OpenAndReadFile(path); }, null, false, "content://", "Select File", "Select");
    }

    public void LoadFile()
    {
        FileBrowser.SetDefaultFilter(".waterline");
        FileBrowser.ShowLoadDialog((path) => { ReadFile(path); },
                        null, false, null, "Select File", "Select");
    }

    public void SaveFile()
    {

        string filename = "LVL" + System.DateTime.Now.ToShortDateString().Replace("/", "-") + "-"
            + System.DateTime.Now.ToLongTimeString().Replace(":", "-");
        //string file = filename + ".waterline";

        //FileBrowser.AddQuickLink("level.waterline", null);
        //FileBrowser.de
        FileBrowser.ShowSaveDialog((path) => { WriteFile(path, filename); }, null, true, null, "Save " + filename);
    }

    void ReadFile(string path)
    {
        if (path != "")
        {
            //byte[] bt = FileBrowserHelpers.ReadBytesFromFile(path);
            //Designer.PGdata = System.Text.Encoding.ASCII.GetString(bt);
            Designer.PGdata = FileBrowserHelpers.ReadTextFromFile(path);
            Designer.LoadFromString();
        }
    }

    void WriteFile(string path, string filename)
    {
        string f;

        f = FileBrowserHelpers.CreateFileInDirectory(path, filename + ".waterline");
        Designer.SaveToString();
        FileBrowserHelpers.WriteTextToFile(f, Designer.PGdata);

        f = FileBrowserHelpers.CreateFileInDirectory(path, filename + ".png");
        byte[] bt = Designer.MakeThumbBytes(512);
        FileBrowserHelpers.WriteBytesToFile(f, bt);
    }

    private IEnumerator ShareDataLevel()
    {
        yield return new WaitForEndOfFrame();

        Designer.SaveToString();
        string str = Designer.PGdata;
        string filePath = Path.Combine(Application.temporaryCachePath, "custom-level.waterline");
        File.WriteAllText(filePath, str);

        new NativeShare().AddFile(filePath).SetSubject("Waterline Custom Level").SetText("Custom level").Share();
    }

    private IEnumerator TakeSSAndShare()
    {
        yield return new WaitForEndOfFrame();

        string filePath = Path.Combine(Application.temporaryCachePath, "Screenshot.png");
        File.WriteAllBytes(filePath, Designer.MakeThumbBytes(1024));
        new NativeShare().AddFile(filePath).SetSubject("Waterline Screenshot").SetText("Screenshot").Share();

        // Share on WhatsApp only, if installed (Android only)
        //if( NativeShare.TargetExists( "com.whatsapp" ) )
        //	new NativeShare().AddFile( filePath ).SetText( "Hello world!" ).SetTarget( "com.whatsapp" ).Share();
    }

}
