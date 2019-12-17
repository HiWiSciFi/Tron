using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Controller controller;

    private void Start()
    {
        PlayerSettings.Load();

        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;

        int ID = 0;
        if (!NetworkCommunication.Connect("localhost", 4444, 1, ref ID))
        {
            Debug.LogError("Connection Failed");
        }

        NetworkCommunication.localPlayer = controller;
    }

    private void Update()
    {
        if (NetworkCommunication.DataAvailable)
            NetworkCommunication.ReadData();
    }
}