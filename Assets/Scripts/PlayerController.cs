using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Controller controller;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        int ID = 0;
        if (!NetworkCommunication.Connect("188.194.223.76", 15565, 1, ref ID))
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