using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class VolumeManager : MonoBehaviour {

    LevelManager LVM;

    void Awake()
    {
        LVM = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
    }

    // Use this for initialization
    void Start () {
        AudioListener.volume=LVM.Volume;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
