using UnityEngine;
using System.Collections;

public class TrollCollider : MonoBehaviour {

    public AudioClip laugh;
    public GameObject necromancer;

    private int instantiateCounter = 0;
    private float endGame = 0.0f;

    void OnTriggerEnter2D(Collider2D col)
    {
        PlayerController player = col.transform.root.gameObject.GetComponent<PlayerController>();
        if (player)
        {
            endGame += Time.deltaTime;
            if (endGame >= 0.4 && instantiateCounter == 0)
            {
                Instantiate(necromancer, transform.position, Quaternion.identity);
                AudioSource.PlayClipAtPoint(laugh, transform.position);
                instantiateCounter++;
            }

            if (endGame >= 0.6)
            {
                Application.LoadLevel("Main Menu");
            }
            Debug.Log(endGame);
        }
    }	
}
