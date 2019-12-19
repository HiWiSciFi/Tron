using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public List<PlayerController> pcs = new List<PlayerController>();
    private bool intitialized = false;
    public void OnEnable()
    {
        StartCoroutine(InitializeScene());
    }

    IEnumerator InitializeScene()
    {
        // create local player
        GameObject localPlayer = Instantiate(PlayerPrefab);
        PlayerController localController = localPlayer.GetComponent<PlayerController>();
        localController.Initialize(true, GameSettings.localColor, GameSettings.localID);

        // add other players
        byte[] buffer;
        GameObject player;
        PlayerController pc;
        while (true)
        {
            Debug.Log("Fun");
            while (!NetworkCommunication.DataAvailable) { yield return null; }

            buffer = NetworkCommunication.Receive();
            Debug.Log(buffer.Length);
            if (buffer.Length > 6)
                break;
            // create player
            player = Instantiate(PlayerPrefab);
            pc = player.GetComponent<PlayerController>();
            pc.Initialize(false, new Color(buffer[3], buffer[4], buffer[5]), buffer[2]);
            pcs.Add(pc);
            Debug.Log("added player with ID " + buffer[2]);
        }
        yield return null;

        // all players added
        Debug.Log("enable moving");
        localController.moveable = true;
        for (int i = 0; i < pcs.Count; i++)
        {
            pcs[i].moveable = true;
        }

        intitialized = true;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (intitialized && NetworkCommunication.DataAvailable)
        {
            Debug.Log("thingies");
            byte[] buffer = NetworkCommunication.Receive();
            if (buffer[0] == 0)
            {
                // standard
                
                float yAngle = BitConverter.ToSingle(buffer, 1);
                float posX = BitConverter.ToSingle(buffer, 5);
                float posZ = BitConverter.ToSingle(buffer, 9);
                byte boosted = buffer[13];
                byte ID = buffer[14];
                
                for (int i = 0; i < pcs.Count; i++)
                {
                    if (pcs[i].ID == ID)
                    {
                        pcs[i].transform.position = new Vector3(posX, transform.position.y, posZ);
                        pcs[i].transform.localScale = new Vector3(pcs[i].transform.localScale.x, yAngle, pcs[i].transform.localScale.z);
                        pcs[i].boosted = boosted;
                    }
                }
            }
            else if (buffer[0] == 3)
            {
                byte ID = buffer[1];

                // kill
                for (int i = 0; i < pcs.Count; i++)
                {
                    if (pcs[i].ID == ID)
                    {
                        pcs[i].dead = true;
                        pcs.RemoveAt(i);
                    }
                }
            }
            else if (buffer[0] == 4)
            {
                byte ID = buffer[1];

                // disconnect
                for (int i = 0; i < pcs.Count; i++)
                {
                    if (pcs[i].ID == ID)
                    {
                        pcs[i].dead = true;
                        pcs.RemoveAt(i);
                    }
                }
            }
        }
    }
}