using UnityEngine;
using System.Collections;

public class Spawner1 : MonoBehaviour {

	public GameObject[] playerTemplates;
	public Transform[] spawnPoints;
	public FollowCam cam;
	public LayerMask whatIsGround;
	static public Vector3 raycastOffset = new Vector3(1.0f,0.0f, 0.0f);
	static public int playerHeight = 23;
	public Canvas arrowsCanvas;

	GameObject newPlayer1;
	GameObject newPlayer2;

	PlayerController[] players;
	bool playerDead = false;
	bool initialSpawn = true;
	float deadCtr = 0.0f;

	// Use this for initialization
	void Start () {
		SpawnPlayers();
		players = FindObjectsOfType<PlayerController>();
	}

	// Update is called once per frame
	void Update () {
		CheckIfDead();
	}

	void CheckIfDead()
	{

		for (int i = 0; i < players.Length; i++)
		{
			if (players[i] == null)
			{
				// Show corresponding arrow in canvas (tied to camera)
				ShowArrow ();

				if (deadCtr > 3.0f)
				{
					deadCtr = 0.0f;
					SpawnPlayers();
					players = FindObjectsOfType<PlayerController>();
				}
				deadCtr += Time.deltaTime;
			}
		}
	}

	void ShowArrow()
	{
		PlayerController[] persistingPlayer = FindObjectsOfType<PlayerController>();

		if (persistingPlayer.Length == 1) 
		{
			if (persistingPlayer [0].playerNumber == 1) {
				arrowsCanvas.GetComponent<ArrowCanvas> ().SetRightArrow (true);
				arrowsCanvas.GetComponent<ArrowCanvas> ().SetLeftArrow (false);

			} else 
			{
				arrowsCanvas.GetComponent<ArrowCanvas> ().SetRightArrow (false);
				arrowsCanvas.GetComponent<ArrowCanvas> ().SetLeftArrow (true);
			}

		}
	}

	void SpawnPlayers()
	{
		// Turn off both arrows in canvas
		arrowsCanvas.GetComponent<ArrowCanvas> ().SetRightArrow (false);
		arrowsCanvas.GetComponent<ArrowCanvas> ().SetLeftArrow (false);

		PlayerController[] persistingPlayer = FindObjectsOfType<PlayerController>();

		bool dontSpawnP1 = false;
		bool dontSpawnP2 = false;
		Transform followP1 = null, followP2 = null;
		if (persistingPlayer.Length == 1)
		{
			if (persistingPlayer[0].playerNumber == 1)
			{
				newPlayer1 = persistingPlayer[0].gameObject;
				if (initialSpawn) newPlayer1.transform.position = GetFloorSpawnPoint(1) + new Vector2(0.0f, playerHeight);
				followP1 = newPlayer1.transform;
				dontSpawnP1 = true;
			}
			else if (persistingPlayer[0].playerNumber == 2)
			{
				newPlayer2 = persistingPlayer[0].gameObject;
				if (initialSpawn) newPlayer2.transform.position = GetFloorSpawnPoint(2) + new Vector2(0.0f, playerHeight);
				followP2 = newPlayer2.transform;
				dontSpawnP2 = true;
			}
		}



		if (playerTemplates[0] != null && spawnPoints[0] != null && !dontSpawnP1) {
			newPlayer1 = SpawnPlayer(1);
			followP1 = newPlayer1.transform;
			playerDead = false;
		}
		if (playerTemplates[1] != null && spawnPoints[1] != null && !dontSpawnP2)
		{
			newPlayer2 = SpawnPlayer(2);
			followP2 = newPlayer2.transform;
			playerDead = false;
		}

		cam.Follow(followP1, followP2);
		initialSpawn = false;
	}


	GameObject SpawnPlayer(int playerNum)
	{

		GameObject newPlayer = Instantiate(playerTemplates[playerNum-1], GetFloorSpawnPoint(playerNum) + new Vector2(0.0f, playerHeight), Quaternion.identity) as GameObject;

		//Debug.Log("Player spawned");
		return newPlayer;
	}

	Vector2 GetFloorSpawnPoint(int playerNum)
	{
		bool floorHit = false;
		Vector3 offset = new Vector3();
		if (playerNum == 1) offset = raycastOffset;
		else if (playerNum == 2) offset = -raycastOffset;

		RaycastHit2D hit = new RaycastHit2D();
		for (int i = 0; !floorHit; i++)
		{
			hit = Physics2D.Raycast(spawnPoints[playerNum - 1].position + offset * i, Vector2.down, 1000, whatIsGround);
			floorHit = (hit.collider != null);
		}

		return hit.point;
	}


}