
using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour
{

	Transform follow;
	Transform other;
	public float smoothness;
	public Transform leftSpawnPoint;
	public Transform rightSpawnPoint;
	float initialY = 71.0f;

	public Transform leftScreenEdge;
	public Transform rightSreenEdge;

	private float cameraHeight;
	private float cameraWidth;

    // Use this for initialization
    void Awake()
    {
        SetLocation();


    }
    void Start ()
	{
		SetLocation ();

		
	}

	// Update is called once per frame
	void Update ()
	{
		Move ();
	}

	public void Move () //follow both players
	{
		Vector2 direction = new Vector2 ();
		//reset y component
		if (follow == null && other == null) {
			transform.position = new Vector3 (transform.position.x, initialY, transform.position.z);
		}
		//if the follow player dies, switch the camera center of interest to the other player
		if (follow == null && other != null) {
			follow = other;
			other = null;
		}

		if (follow != null && other == null) {
			direction = new Vector2 (follow.position.x - GetComponent<Transform> ().position.x, follow.position.y - GetComponent<Transform> ().position.y);
		} else if (follow != null && other != null) {
			Vector2 between = new Vector2 (follow.position.x + (other.position.x - follow.position.x) / 2, follow.position.y + (other.position.y - follow.position.y) / 2);
			direction = new Vector2 (between.x - GetComponent<Transform> ().position.x, between.y - GetComponent<Transform> ().position.y);

		}

        cameraHeight = 2f * GetComponent<Camera>().orthographicSize;
        cameraWidth = cameraHeight * GetComponent<Camera>().aspect;

        float cameraLeftEdge = transform.position.x - (cameraWidth / 2);
		float cameraRightEdge = transform.position.x + (cameraWidth / 2);

		bool hittingLeftEdge = cameraLeftEdge <= leftScreenEdge.position.x;
		bool hittingRightEdge = (cameraRightEdge >= rightSreenEdge.position.x);

		bool performMove = false;

		// See if camera is allowed to move, set performMove flag to true when so
		// If not hitting an edge, move normally.
		if (!(hittingLeftEdge || hittingRightEdge)) {
			performMove = true;
		} 

		// If hitting left screen, only move to the right
		if (hittingLeftEdge && !performMove) {
			//Debug.Log ("Hitting left edge");
			if (direction.x > 0)
				performMove = true;
		}

		// If hitting the right edge of the sreen, only move to the left
		if (hittingRightEdge)
		if ((cameraRightEdge >= rightSreenEdge.position.x)) {
			//Debug.Log ("Hitting right edge");
			if (direction.x < 0)
				performMove = true;
		}
			
		if (performMove)
			GetComponent<Rigidbody2D> ().AddForce (direction * smoothness, ForceMode2D.Impulse);
		
	}

	public void Follow (Transform follow, Transform other)
	{
		this.follow = follow;
		this.other = other;
	}

	public void StopFollowing ()
	{
		this.follow = null;
		this.other = null;
	}

	void SetLocation ()
	{
		PlayerController[] players = FindObjectsOfType<PlayerController> ();
		if (players.Length == 1) {
			if (players [0].playerNumber == 1)
				transform.position = leftSpawnPoint.position;
			else if (players [0].playerNumber == 2)
				transform.position = rightSpawnPoint.position;
		}
	}
}