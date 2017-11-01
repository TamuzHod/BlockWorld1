using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolbarBuildButtons : MonoBehaviour {

    // Use this for initialization
    void Start()
    {

        MouseManager mouseManger = GameObject.FindObjectOfType<MouseManager>();

        // populate our button list

        foreach (GameObject shipPart in ShipPartPrefabs)
        {
            GameObject buttonGameObject = Instantiate(BuildButtonPrefab, this.transform);
            Text buttonLabel = buttonGameObject.GetComponentInChildren<Text>();
            buttonLabel.text = shipPart.name;

            Button theButton = buttonGameObject.GetComponent<Button>();

            theButton.onClick.AddListener( () => { mouseManger.ThePrefabToSpawn = shipPart; }  );

        }
    }


    public GameObject BuildButtonPrefab;
    public GameObject[] ShipPartPrefabs;

	// Update is called once per frame
	void Update () {
		
	}
}
