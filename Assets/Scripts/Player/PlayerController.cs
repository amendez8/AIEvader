using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    
	public int playerNumber;
	[Header ("--- General Movement Variables ---")]
	public float moveSpeed;
    public float walkAnimationSpeed;
    public float jumpForce;
	[Tooltip ("This float is communicated to the animator to set the speed of the prepare for jump animation")]
	public float jumpPrepareSpeed;
	[Tooltip ("This float is communicated to the animator to set the speed of the jump animation")]
	public float jumpSpeed;
	[Tooltip ("This float is communicated to the animator to set the speed of the move while airborne animation")]
	public float jumpMovingSpeed;
	[Tooltip ("This float is communicated to the animator to set the speed of the crouch animation")]
	public float crouchSpeed;
	[Tooltip ("This float is communicated to the animator to set the speed of the dodge animation")]
	public float dodgeSpeed;
	[Tooltip ("Single force to be applied on the dodge movement")]
	public float dodgeForce;
	[Tooltip ("Required time interval after a dodge to be able to dodge again")]
	public float dodgeCooldown;

	[HideInInspector]				
	public bool animIsJabbing;
	[HideInInspector]
	public bool animIsClubbing;

	[Header ("--- Attack Variables ---")]
	[Tooltip ("This float is communicated to the animator to set the speed of the jab animation")]
	public float jabSpeed;
	[Tooltip ("Minimum velocity treshold after which the player is considered running and the jab attack will be a dash")]
	public float jabDashTreshold;
	[Tooltip ("Single force to be applied on the dash attack")]
	public float jabDashForce;
	[Tooltip ("Single force to be applied upon being punched, push player back")]
	public float jabStagger;
	[Tooltip ("Single force to be applied upon being hit by weapon, push player back")]
	public float limbStagger;

	[Tooltip ("Speed at witch the club animation prepares for the strike.")]
	public float clubAttackPepare;
	[Tooltip ("Speed for the club attack animation.")]
	public float clubAttackSpeed;
	[Tooltip ("Speed at witch the animation prepares for the strike.")]
	public float throwAttackPepare;
	[Tooltip ("Speed for the animation.")]
	public float throwAttackSpeed;

	[Header ("--- State Variables ---")]
	public LayerMask mWhatIsGround;
	public float kGroundCheckRadius = 0.1f;
	Vector3 direction;

	// Booleans
	bool inputLocked = false;
	// Character controls will need to be locked for small intervals of time, like during a dash jab
	bool movementLocked = false;
	// As above but just for the movement
	bool running;
	bool jumping;
	bool moving;
	bool dodging;
	[HideInInspector]
	public bool crouching;
	bool grounded;
	bool falling;
	bool attackingMelee;
	bool isHit;
	bool tearOffLimb;
	bool throwingLimb;

    public bool isDashing;
	public bool hasWeapon;
    private bool paused;

	// Numerical variables
	private float moveInput = 0.0f;
    private float playerDamage = 0;
	private float dodgeCooldownTimer = 0.0f;		// When <= 0 you may dodge
    private float damageMultiplier = 1f;


	// References
	Rigidbody2D rb;
	Transform myTransform;
	Transform sprites;
	Animator anim;
	Transform groundCheck;
	private Health health;
	public GameObject weaponArm;
	public GameObject weaponLeg;

    [Header("--- Sound Effects ---")]
    public AudioClip jabHit;
    public AudioClip clubHit;
    public AudioClip throwSound;

    Color initialColor;
    public Transform torso;

    float knockbackCtr;
    public float knockbackEffectiveTime;
    bool knockedback;
    public float opponentSpeedKnockbackEffect;
    // Use this for initialization
    void Start ()
	{
		// Setup references
		rb = GetComponent<Rigidbody2D> ();
		anim = GetComponentInChildren<Animator> ();
		myTransform = transform;
		sprites = myTransform.FindChild ("Sprites");
		groundCheck = transform.FindChild ("GroundCheck");
		health = GetComponent<Health> ();
        paused = false;
        initialColor = torso.GetComponent<SpriteRenderer>().color;
        // Setup variables
        if (playerNumber == 1)
        {
            direction = new Vector2(1.0f, 0);
            FaceDirection(new Vector2(1.0f, 0));
        }
        if (playerNumber == 2)
        {
            direction = new Vector2(-1.0f, 0);
            FaceDirection(new Vector2(-1.0f, 0));
        }
	}
	
	// Update is called once per frame
	void Update ()
	{
		CheckGrounded ();
		CheckFalling ();
		ClampToCamera ();

		if (!inputLocked)
			CollectInput ();

		if (attackingMelee) {
			MeleeAttack ();
			attackingMelee = false;
		}
        if (crouching)
        {
            damageMultiplier = 0.5f;
        }
        else
        {
            damageMultiplier = 1f;
        }
        if(playerDamage >= 3)
        {
            health.TakeOffLimb();
            playerDamage = 0;
        }

		// Timer events
		dodgeCooldownTimer -= Time.deltaTime;
	}

	void FixedUpdate ()
	{

		if (tearOffLimb) {
			health.RipOffLimb ();			// Rip off limb returns true when the player succesfully equips a limb
			tearOffLimb = false;
		}

        if (knockedback)
        {
            knockbackCtr += Time.deltaTime;
            if (knockbackCtr > knockbackEffectiveTime) knockedback = false;
            
        }
       
        Move ();
		Crouch ();
		JumpPrepare ();
		ThrowLimb ();
		Dodge ();
	}

	private void CollectInput ()
	{
		if (!movementLocked)
			moveInput = Input.GetAxis ("Horizontal " + playerNumber);

		attackingMelee = Input.GetButtonDown ("Melee Attack " + playerNumber);
		
		jumping = Input.GetButtonDown ("Jump" + playerNumber);

		tearOffLimb = Input.GetButtonDown ("Tear Limb " + playerNumber);

		crouching = (Input.GetAxis ("Crouch " + playerNumber) == 0 ? false : true);

		throwingLimb = Input.GetAxis ("Throw Limb " + playerNumber) == 1.0f ? true : false ;

		dodging = Input.GetButtonDown ("Dodge " + playerNumber);

        if(paused && Input.GetButtonDown("Start" + playerNumber))
        {
			ArrowCanvas arrowCanvas = FindObjectOfType<ArrowCanvas> ();
			arrowCanvas.SetPasedText (false);

            Time.timeScale = 1;
            paused = false;
        }
        else if(!paused && Input.GetButtonDown("Start" + playerNumber))
        {
			ArrowCanvas arrowCanvas = FindObjectOfType<ArrowCanvas> ();
			arrowCanvas.SetPasedText (true);

            Debug.Log(Time.timeScale);
            Time.timeScale = 0;
            paused = true;
        }
	}

	private void Move ()
	{
		// Don't move while holding down crouch button. The crouch method cancels horizontal velocity
		if (!knockedback && !crouching && moveInput != 0.0f) {
            knockedback = false;
            knockbackCtr = 0.0f;
			// Horizontal movement
			moving = true;

			// Floor and ceiling functions are necessary for moveInput with a controller axis that returns a range between -1 and 1 
			if(moveInput < 0)
				direction = new Vector2 (Mathf.Floor(moveInput), 0.0f);
			else
				direction = new Vector2(Mathf.Ceil(moveInput), 0.0f);

            //print("In the move function, the direction vector is: " + direction)
            rb.velocity = new Vector2(moveInput * Time.timeScale * moveSpeed, rb.velocity.y);
            FaceDirection (direction);
		} 
		else
			moving = false;

		// Pass movement speed to animator
		anim.SetFloat ("speed", Mathf.Abs (moveInput) * walkAnimationSpeed);
	}

	// The animator will finish the jump sequence after this method is called
	private void JumpPrepare ()
	{
		if (grounded && jumping) {
			jumping = false;

			// Set animator speed variables and trigger attack type
			anim.SetFloat ("jumpPrepareSpeed", jumpPrepareSpeed);
			anim.SetFloat ("jumpSpeed", jumpSpeed);
			anim.SetFloat ("jumpSpeedMoving", jumpMovingSpeed);
			anim.SetTrigger ("jump");
		}
	}

	// The animator will call this method to apply the physics movement when the prepare animation is done
	public void JumpStart ()
	{
		rb.velocity = new Vector2 (0.0f, jumpForce);
	}


	private void Crouch ()
	{
		if (crouching) {
			//CancelHorizontalVelocity ();

			anim.SetFloat ("crouchSpeed", crouchSpeed);
		}
		anim.SetBool ("crouching", crouching);
	}

    private void MeleeAttack()
    {

        if (!crouching)
        {
            // If the player doesn't have a weapon, jab attack...
            if (!hasWeapon)
            {
                // Read a speed treshold to see if you should do a dash attack
                if (Mathf.Abs(moveInput) >= jabDashTreshold && grounded)
                {
                    // Lock movement inputs
                    movementLocked = true;
                    // Cancel prior horizontal velocity
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    // Using force instead of velocity to add single dash burst
                    isDashing = true;
                }

                // Set animator speed variables and trigger attack type
                anim.SetFloat("jabSpeed", jabSpeed);
                anim.SetTrigger("jab");
                animIsJabbing = true;
            }
            // ... else use weapon as a club
            else {
                // Set animator speed variables and trigger attack type
                anim.SetFloat("clubPrepareSpeed", clubAttackPepare);
                anim.SetFloat("clubSpeed", clubAttackSpeed);
                anim.SetTrigger("club");
                animIsClubbing = true;
            }
        }
    }


	// Starts the throw animation. The once the animation is done it will call InstantiateAndThrowLimb
	private void ThrowLimb ()
	{
		// Don't throw when crouching
		if (hasWeapon && throwingLimb && !crouching) {
            //AudioSource.PlayClipAtPoint(throwSound, transform.position, 30f); THIS WAS CHANGED, THE LIMB HAS AN AUDIOSOURCE SO IT CAN CONTINUOUSLY PLAY THE SFX
            anim.SetFloat ("throwLimbPrepareSpeed", throwAttackPepare);
			anim.SetFloat ("throwLimbSpeed", throwAttackSpeed);
			anim.SetTrigger ("throwLimb");
            hasWeapon = false;
		}
	}

	private void Dodge ()
	{
		if (dodging) {
			// Don't dodge while in the air, falling, or moving
			if (grounded && !moving && dodgeCooldownTimer <= 0) {
				// Add force in oposite horizontal direction to where the player is facing
				rb.AddForce (new Vector2 (-Mathf.Sign (direction.x) * dodgeForce, direction.y));

				// Trigger animator
				anim.SetFloat ("dodgeSpeed", dodgeSpeed);
				anim.SetTrigger ("dodge");

				// Reset timer
				dodgeCooldownTimer = dodgeCooldown;
			} 
			else 
			{
				// Just for debugging
				/*
				Debug.Log("grounded: " + grounded);
				Debug.Log ("falling: " + falling);
				Debug.Log ("moving: " + moving);
				Debug.Log ("dodgeCooldownTimer: " + dodgeCooldownTimer);
				*/
			}

			dodging = false;
		}
	}

	// Called by the animation to finish the attack
    public void InstantiateAndThrowLimb()
    {
        if (weaponArm.activeSelf)
        {
            weaponArm.GetComponent<Throw>().ThrowLimb(direction);
        }
        else if (weaponLeg.activeSelf)
        {
            weaponLeg.GetComponent<Throw>().ThrowLimb(direction);
        }
    }

    private void FaceDirection (Vector2 direction)
	    {
		    if (direction.x != 0.0f) {
			    Quaternion rotation3D = direction.x > 0 ? Quaternion.LookRotation (Vector3.forward) : Quaternion.LookRotation (Vector3.back);
			    sprites.rotation = rotation3D;
		    }
	    }

	private void CheckGrounded ()
	{
		Collider2D[] colliders = Physics2D.OverlapCircleAll (groundCheck.position, kGroundCheckRadius, mWhatIsGround);
		foreach (Collider2D col in colliders) {
			if (col.gameObject != gameObject) {
				grounded = true;
				anim.SetBool ("grounded", grounded);
				return;
			}
		}
		grounded = false;

		anim.SetBool ("grounded", grounded);
	}

	private void CheckFalling ()
	{
		falling = rb.velocity.y < 0.0f;
	}

	private void ClampToCamera ()
	{
		Vector3 position = Camera.main.WorldToViewportPoint (myTransform.position);
		position.x = Mathf.Clamp (position.x, 0.03f, 0.97f);
		// position.y = Mathf.Clamp(position.y, 0.05f, 0.95f);
		myTransform.position = Camera.main.ViewportToWorldPoint (position);
	}

	public void GetHitByJab (GameObject oppositePlayer)
	{
		PlayerController opponent = oppositePlayer.GetComponent<PlayerController> ();
		if (opponent) {
			if (opponent.animIsJabbing && !isHit) { //if the opponent is in jab motion and I have not been hit yet
                StartCoroutine(MakeHitVisible());
                AudioSource.PlayClipAtPoint(jabHit, transform.position, 30.0f);
				isHit = true; //I can't be hit twice by the same jab animation
				playerDamage += damageMultiplier/2;
                rb.velocity = new Vector2(0.0f,0.0f);
                knockedback = true;
                float additionalKnocback = System.Math.Abs(oppositePlayer.GetComponent<Rigidbody2D>().velocity.x) * opponentSpeedKnockbackEffect + 1;
				rb.AddForce (additionalKnocback * jabStagger * (opponent.GetDirection ()), ForceMode2D.Impulse); // push player being hit back, "stagger"
			}
		}  
	}
    

	public void GetHitByClub (GameObject oppositePlayer)
	{
		PlayerController opponent = oppositePlayer.GetComponent<PlayerController> ();
		if (opponent) {
			if (opponent.animIsClubbing && !isHit) {
                StartCoroutine(MakeHitVisible());
                AudioSource.PlayClipAtPoint(clubHit, transform.position, 30.0f);
                rb.velocity = new Vector2(0.0f, 0.0f);
                knockedback = true;
                rb.AddForce (limbStagger * (opponent.GetDirection ()), ForceMode2D.Impulse); // push player being hit back, "stagger"
                StartCoroutine (RemoveLimb (0.5f));
			}
		}
	}

	public void GetHitByThrowingLimb ()
	{
		health.Kill ();
	}

	private IEnumerator RemoveLimb (float seconds)
	{
		isHit = true;
        playerDamage += (3 * damageMultiplier); 
		yield return new WaitForSeconds (seconds);

		isHit = false;
	}

	private void CancelHorizontalVelocity ()
	{
		// Cancel horizontal velocities and stop running animations
		anim.SetFloat ("speed", 0);
		rb.velocity = new Vector2 (0, rb.velocity.y);
	}

	public void SetIsHit (bool hit)
	{
		isHit = hit;
	}

	public bool GetIsHit ()
	{
		return isHit;
	}

	public void SetInputLocked (bool locked)
	{
		inputLocked = locked;
	}

	public void SetMovementLocked (bool locked)
	{
		movementLocked = locked;
	}

	public Animator GetAnimator ()
	{
		return anim;
	}

	public Vector3 GetDirection ()
	{
		return direction;
	}

    IEnumerator MakeHitVisible()
    {

        torso.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
        yield return new WaitForSeconds(0.3f);
        torso.GetComponent<SpriteRenderer>().color = initialColor;
    }

}
