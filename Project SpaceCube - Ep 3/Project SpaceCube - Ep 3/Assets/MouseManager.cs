using UnityEngine;
using System.Collections;


public class MouseManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ShipRigidbody = ShipRoot.GetComponent<Rigidbody>();
    }

    public GameObject ThePrefabToSpawn;

    public LayerMask SnapPointLayerMask;

    public ComponentKeybindDialog ComponentKeybindingDialog;

    public GameObject ShipRoot;

    Rigidbody ShipRigidbody;



    // Update is called once per frame
    void Update () {
	
        // Was the mouse pressed down this frame?

        if(Input.GetMouseButtonDown(0))
        {
            checkLeftClick();
        }
        if (Input.GetMouseButtonDown(1))
        {
            checkRightClick();
        }



    }

    Collider doRayCast()
    {
        Camera theCamera = Camera.main;

        Ray ray = theCamera.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo))
        {
            return hitInfo.collider;
        }
        return null;
    }

    void checkRightClick()
    {
        Collider theCollider = doRayCast();

        if (theCollider == null)
        {
            return;
        }

        //We have right-clicked on something. Does it have a keybinadable component?
        // we need to check the parent of the colider given how prefabs are assembeled

        GameObject theParent = FindShipPart(theCollider);

        if(theParent == null)
        {
            return;
        }

        KeybinableComponent kc = theParent.gameObject.GetComponent<KeybinableComponent>();

        if(kc == null)
        {
            // This object isn't keybinadable
            return;

        }

        // if we get to this point we got to something keybindable

        ComponentKeybindingDialog.OpenDialog(kc);
    }

    GameObject FindShipPart(Collider collider)
    {
        Transform curr = collider.transform;

        while(curr != null)
        {
            if(curr.gameObject.tag == "ShipPart")
            {
                return curr.gameObject;
            }

            curr = curr.parent;
        }

        return null;
    }

    void checkLeftClick()
    {

        // Yes, the left mouse button was pressed this frame
        // Is the mouse over a cube

        Collider theCollider = doRayCast();

        if(theCollider == null)
        {
            return;
        }
       
        int maskForThisHitObject = 1 << theCollider.gameObject.layer;


        if ((maskForThisHitObject & SnapPointLayerMask) != 0)
        {
            // Now let's spawn a new object

            Vector3 spawnSpot = theCollider.transform.position;

            Quaternion spawnRotation = theCollider.transform.rotation;

            GameObject go = (GameObject)Instantiate(ThePrefabToSpawn, spawnSpot, spawnRotation);
            go.transform.SetParent(theCollider.transform, true);

            Mesh mesh = go.GetComponentInChildren<MeshFilter>().sharedMesh;
            //float volume = mesh.bounds.size.x * mesh.bounds.size.y * mesh.bounds.size.z;
            Vector3 scale = go.transform.GetChild(0).transform.lossyScale;
            //float volume = mesh.bounds.size.x * mesh.bounds.size.y * mesh.bounds.size.z;
            float volume = VolumeOfMesh(mesh);
            //Debug.LogError("volume = " + volume + " scale = " + scale.x * scale.y * scale.z);
            float mass = ShipRigidbody.mass += volume * scale.x * scale.y * scale.z;

            // Disable render on snap point if something is attached to

            if (theCollider.GetComponent<Renderer>() != null)
            {
                theCollider.GetComponent<Renderer>().enabled = false;
            }
            if (theCollider.GetComponent<Collider>() != null)
            {
                theCollider.GetComponent<Collider>().enabled = false;
            }

        }

    }

    public float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float v321 = p3.x * p2.y * p1.z;
        float v231 = p2.x * p3.y * p1.z;
        float v312 = p3.x * p1.y * p2.z;
        float v132 = p1.x * p3.y * p2.z;
        float v213 = p2.x * p1.y * p3.z;
        float v123 = p1.x * p2.y * p3.z;
        return (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
    }

    public float VolumeOfMesh(Mesh mesh)
    {
        float volume = 0;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            Vector3 p1 = vertices[triangles[i + 0]];
            Vector3 p2 = vertices[triangles[i + 1]];
            Vector3 p3 = vertices[triangles[i + 2]];
            volume += SignedVolumeOfTriangle(p1, p2, p3);
        }
        return Mathf.Abs(volume);
    }

    void RemovePart(GameObject go)
    {
        //This is just an example may not work

        // make sure to re-enable the sanp point we are attached to

        go.transform.GetComponent<Renderer>().enabled = false;
        go.transform.GetComponent<Collider>().enabled = false;

        Destroy(go);


    }

    public void SetMode_Edit()
    {

        // show snappoint modes
        SetSanpPointEnabled(ShipRoot.transform, true);

        //Unlock camera controll
        Camera.main.transform.parent.SetParent(null);


    }

    public void SetMode_Flight()
    {
        //hide all snappoint nodes
        SetSanpPointEnabled(ShipRoot.transform, false);

        //Tell camera to lock to ship rout
        Camera.main.transform.parent.SetParent(ShipRoot.transform);
        Camera.main.transform.parent.localPosition = Vector3.zero;


    }

    void SetSanpPointEnabled(Transform t, bool setToActive)
    {
        int maskForThisHitObject = 1 << t.gameObject.layer;

        if ((maskForThisHitObject & SnapPointLayerMask) != 0)
        {
            //This is a snap point
            if (setToActive)
            {
                //Alawys activate -- just in case
                t.gameObject.SetActive(true);
            }
            else
            {
                if (t.childCount == 0)
                {
                    t.gameObject.SetActive(false);
                    return;

                }
            }
        }

        //Loop through all of this objects children

        for (int i = 0; i < t.childCount; i++)
        {
            SetSanpPointEnabled(t.GetChild(i), setToActive);
        }
    }



}
