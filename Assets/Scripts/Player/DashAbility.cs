using UnityEngine;
using System.Collections;

public class DashAbility : MonoBehaviour
{
    public enum DashState
    {
        Ready,
        Dashing,
        Cooldown
    }

    public DashState dashState;
    public float dashTimer;
    public float maxDash;

    public Vector2 savedVelocity;

    private Rigidbody2D rb;
    private PlayerController player;

    // Use this for initialization
    void Start ()
    {
        player = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if (player.isDashing)
	    {
            switch (dashState)
            {
                case DashState.Ready:
                    //savedVelocity = rb.velocity;
                    rb.velocity = new Vector2(Mathf.Sign(player.GetDirection().x) * savedVelocity.x * 3f, 0);
                    dashState = DashState.Dashing;
                    break;
                case DashState.Dashing:
                    dashTimer += Time.deltaTime * 3;
                    if (dashTimer >= maxDash)
                    {
                        dashTimer = maxDash;
                        rb.velocity = savedVelocity;
                        dashState = DashState.Cooldown;
                    }
                    break;
                case DashState.Cooldown:
                    dashTimer -= Time.deltaTime;
                    if (dashTimer <= 0)
                    {
                        dashTimer = 0;
                        dashState = DashState.Ready;
                        player.isDashing = false;
                    }
                    break;
            }
        }
        
    }
}
