using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
            Debug.Log("Destroyed Duplicate Music Manager");
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void DetroyMusicManager()
    {
        Destroy(gameObject);
    }
}
