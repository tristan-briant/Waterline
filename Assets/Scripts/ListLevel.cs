using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class ListLevel : MonoBehaviour
{
    public List<string> names = new List<string>() ;

    [ContextMenu("List elements")]
    public void ListAll()
    {
        foreach (string s in names)
            Debug.Log(s);
    }

    [ContextMenu("Clean Up Path")]
    public void CleanUpPath()
    {
        for(int i =0; i < names.Count; i++)
        {
            Debug.Log(i);
            names[i] = names[i].Replace("Assets/Resources/Levels","").Replace("\r","");
        }
    }


    private void Start()
    {
        Load();
    }

    [ContextMenu("Load List")]
    public void Load()
    {
        StreamReader reader= new StreamReader("Assets/Resources/Levels/LevelList.txt");

        string text = reader.ReadToEnd();
        reader.Close();

        names = new List<string>( text.Split(new string[] { "\n", "\r" }, System.StringSplitOptions.RemoveEmptyEntries));
    }


    [ContextMenu("Save List")]
    public void Save()
    {
        StreamWriter sr = File.CreateText("Assets/Resources/Levels/LevelList.txt");
        foreach(string s in names)
            sr.WriteLine(s);
        sr.Close();
        sr.Dispose();
    } 
}
