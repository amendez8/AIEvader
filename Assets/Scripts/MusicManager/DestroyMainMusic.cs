using UnityEngine;
using System.Collections;

public class DestroyMainMusic : MonoBehaviour {

    private MusicManager mainMusic;

	// Use this for initialization
	void Start () {
        mainMusic = GameObject.Find("MusicManager").GetComponent<MusicManager>();
        mainMusic.DetroyMusicManager();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
