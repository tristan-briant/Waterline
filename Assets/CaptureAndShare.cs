using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CaptureAndShare : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("capture");
            StartCoroutine(TakeSSAndShare());
        }
    }

    private IEnumerator TakeSSAndShare()
    {
        yield return new WaitForEndOfFrame();

        /*Texture2D ss = new Texture2D( Screen.width, Screen.height, TextureFormat.RGB24, false );
        ss.ReadPixels( new Rect( 0, 0, Screen.width, Screen.height ), 0, 0 );
        ss.Apply();
        */

        string str = "coucou tout le monde !";

        string filePath = Path.Combine(Application.temporaryCachePath, "shared text.txt");
        //File.WriteAllBytes( filePath, ss.EncodeToPNG() );
        File.WriteAllText(filePath, str);
        // To avoid memory leaks
        //Destroy( ss );

        new NativeShare().AddFile(filePath).SetSubject("Subject goes here").SetText("Hello world!").Share();

        // Share on WhatsApp only, if installed (Android only)
        //if( NativeShare.TargetExists( "com.whatsapp" ) )
        //	new NativeShare().AddFile( filePath ).SetText( "Hello world!" ).SetTarget( "com.whatsapp" ).Share();
    }
}
