using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : KeybinableComponent
{

	// Use this for initialization
	void Start () {
        shipRigidbody = this.transform.root.GetComponent <Rigidbody>();
        if(shipRigidbody == null)
        {
            Debug.LogError("No RigidBody?");
        }

        Rigidbody r;
    }

    public float ThrusterStrength = 10f;

    Rigidbody shipRigidbody;

    //runs immidetly befor every tick of physic engine 
    // all none instatneience physic updates shuld go here
    private void FixedUpdate()
    {
        if(!shipRigidbody.isKinematic)
        {
            if (Input.GetKey(keyCode))
            {
                Rigidbody rb = shipRigidbody;

                Vector3 theForce = -this.transform.forward * ThrusterStrength;

                rb.AddForceAtPosition(theForce, this.transform.position);

                SetParticles(true);
            }
            else
            {
                SetParticles(false);
            }
        }
        else
        {
            SetParticles(false);
        }

    }

    void SetParticles(bool enabled)
    {
        ParticleSystem.EmissionModule em = GetComponentInChildren<ParticleSystem>().emission;
        em.enabled = enabled;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
