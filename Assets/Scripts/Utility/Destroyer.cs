using UnityEngine;
using System.Collections;

public class Destroyer : MonoBehaviour {

	// Anything that comes into contact with this layer is destroyed. Currently only set to collide with thrown arms
	void OnTriggerEnter2D(Collider2D col)
	{
		Destroy(col.gameObject);
	}
}
