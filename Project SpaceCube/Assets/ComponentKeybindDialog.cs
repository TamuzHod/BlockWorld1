using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ComponentKeybindDialog : MonoBehaviour {

    KeybinableComponent keybinadableComponent;

    public void OpenDialog(KeybinableComponent keybinadableComponent)
    {
        this.keybinadableComponent = keybinadableComponent;
        gameObject.SetActive(true);

        transform.Find("Keybind").GetComponent<Text>().text = keybinadableComponent.keyCode.ToString(); ;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if( Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }

        //While open listen to key press

       // if( Input.anyKeyDown )
       // {
            // WHICH key was pressed?
            foreach(KeyCode keyCode in Enum.GetValues(typeof(KeyCode) ) )
            {
                if(keyCode != KeyCode.Mouse0 && keyCode != KeyCode.Mouse1 && keyCode != KeyCode.Mouse2 && Input.GetKeyUp( keyCode) )
                {
                    //we found our key

                    keybinadableComponent.keyCode = keyCode;
                    gameObject.SetActive(false);
                    return;

                }
            }
       // }

    }
}
