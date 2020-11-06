using UnityEngine;
using System.Collections;

// For comunication between the animator controller and the PlayerController, since they have to be in separate objects
public class AuxAnimationCommunicator : MonoBehaviour {

	PlayerController playerController;
    Health playerHealth;

	void Start()
	{
        // Setup references
		playerController = GetComponentInParent<PlayerController> ();
        playerHealth = GetComponentInParent<Health>();
    }

	// Using integer notation for input since the animator function called doesn't support bool
	public void SetInputLocked(int locked)
	{
		if(locked == 0)
			playerController.SetInputLocked (false);
		else if(locked==1)
			playerController.SetInputLocked (true);
	}

	// Using integer notation for input since the animator function called doesn't support bool
	public void SetMovementLocked(int locked)
	{
		if(locked == 0)
			playerController.SetMovementLocked (false);
		else if(locked==1)
			playerController.SetMovementLocked (true);
	}

    public void DisableIsJabbing()
    {
        playerController.animIsJabbing = false;
    }

	public void JumpStart()
	{
		playerController.JumpStart ();
	}

	public void InstantiateAndThrowLimb()
	{
		playerController.InstantiateAndThrowLimb ();
	}

    public void DisableIsClubbing()
    {
        playerController.animIsClubbing = false;
    }

    public void SwapArms()
    {
        playerHealth.SwapArms();
    }

    public void SwapLegs()
    {
        playerHealth.SwapLegs();
    }
}
