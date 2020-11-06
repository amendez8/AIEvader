using UnityEngine;
using System.Collections;

public class BonesPile : MonoBehaviour
{
    private Rigidbody2D rb;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DestroyBonePile(BonesPile pile)
    {
        Destroy(pile);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        //Debug.Log("Bone pile physical collision");
        // Freeze on real collision. On the layer collision matrix the BonesPile layer should not collide with player
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log("Bone pile trigger collision");
        // For the BonesPileTrigger layer where the collider should be a trigger
        GameObject playerGameobject = col.transform.root.gameObject;
        PlayerController player = playerGameobject.GetComponent<PlayerController>();
        Health health = playerGameobject.GetComponent<Health>();

        if (player)
        {
            if (player.hasWeapon == true && health.GetNbOfLimbs() == 3)
            {
                return;
            }
            else
            {
                Destroy(transform.parent.gameObject);
                health.PutBackLimb();
            }

        }

    }
}
