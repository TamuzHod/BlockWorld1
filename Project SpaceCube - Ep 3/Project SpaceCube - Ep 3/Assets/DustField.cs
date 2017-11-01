using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustField : MonoBehaviour {

    public int NumDustMotes = 100;

    public GameObject DustMotePrefab;

    public Transform TheCamera;

    public float CloudRadius = 25;

	// Use this for initialization
	void Start () {

        if(DustMotePrefab == null)
        {
            Debug.LogError("You forgat to link the prfab you dummy");
            return;
        }
		
        if(TheCamera == null)
        {
            TheCamera = Camera.main.transform;

            if(TheCamera == null)
            {
                Debug.LogError("Can't find the camera");
                return;
            }
        }

        MeshRenderer mr = DustMotePrefab.transform.GetComponentInChildren<MeshRenderer>();
        Material matSpaceDust = mr.sharedMaterial;
        matSpaceDust.SetFloat("_FallofDistance", CloudRadius);

        for (int i=0; i < NumDustMotes; i++)
        {
            Vector3 dustMotePosition = TheCamera.position + 
                Random.insideUnitSphere * CloudRadius;
            Instantiate(DustMotePrefab, dustMotePosition, Random.rotation, this.transform); ;
        }

	}
	
	// Update is called once per frame
	void Update () {

        //if any dust mote gets to far from camera 
        // reposition to other side of dust cloud

        float maxDistanceSqured = CloudRadius * CloudRadius;

        for(int i=0; i<this.transform.childCount; i++)
        {
            //is this child too far?
            Transform theChild = this.transform.GetChild(i);
            Vector3 diff = theChild.position - TheCamera.position;

            if(diff.sqrMagnitude > maxDistanceSqured)
            {
                //yes it too far
                //so lets flip the dust motes to other side of camera
                diff = Vector3.ClampMagnitude(diff, CloudRadius);
                Vector3 newPosition = TheCamera.position - diff;
                theChild.position = newPosition;
            }
        }
		
	}
}
