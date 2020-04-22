using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaptureAndShare : MonoBehaviour
{
    public void ShareLevel(){
        StartCoroutine("ShareDataLevel");
    }

    public void ShareSS(){
        StartCoroutine("TakeSSAndShare");
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
        File.WriteAllBytes( filePath, Designer.MakeThumbBytes() );
        //Destroy( ss );

        new NativeShare().AddFile(filePath).SetSubject("Waterline Screeshot").SetText("Screeshot").Share();

        // Share on WhatsApp only, if installed (Android only)
        //if( NativeShare.TargetExists( "com.whatsapp" ) )
        //	new NativeShare().AddFile( filePath ).SetText( "Hello world!" ).SetTarget( "com.whatsapp" ).Share();
    }
}
