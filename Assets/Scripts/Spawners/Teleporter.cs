using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Teleporter : MonoBehaviour {

    public int teleporterNumber;
    
    static string[] stages = { "leftFinal", "left2", "left1", "middle", "right1", "right2", "rightFinal"};

    PlayerController[] players;

    Scene scene;

    void Start()
    {
        scene = SceneManager.GetActiveScene();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            Application.LoadLevel("leftFinal");   
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            Application.LoadLevel("left2");
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            Application.LoadLevel("left1");
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            Application.LoadLevel("middle");
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            Application.LoadLevel("right1");
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            Application.LoadLevel("right2");
        }
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            Application.LoadLevel("rightFinal");
        }
    }

    private void ProgressToNextStage(PlayerController player)
    {
        if (player.playerNumber == 1 && teleporterNumber > 0 || player.playerNumber == 2 && teleporterNumber < 0)
        {
            int currentStage = FindCurrentStageIndex();
            DontDestroyOnLoad(player);
            currentStage += teleporterNumber;
            LoadNextStage(stages[currentStage]);
        }
    }

    private int FindCurrentStageIndex()
    {
        for(int i =0; i< stages.Length; i++)
        {
            if (scene.name.Equals(stages[i], System.StringComparison.InvariantCultureIgnoreCase))
            {
                return i;
            }
        }
        return 1000;
    }

    private void LoadNextStage(string stageName)
    {
        Application.LoadLevel(stageName);
    }


    void OnTriggerEnter2D (Collider2D collider)
    {
        GameObject obj = collider.gameObject;
        if (obj.GetComponent<PlayerCollisionDetector>())
        {
            players = FindObjectsOfType<PlayerController>();
            Debug.Log(players.Length);
            if (players.Length == 1)
            {
                ProgressToNextStage(players[0]);
                gameObject.SetActive(false); //to prevent spawning through multiple levels
            }

        }
    }
}
