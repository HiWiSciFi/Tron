using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public List<PlayerController> pcs = new List<PlayerController>();

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
            while (!newNetworkCommunication.DataAvailable) { yield return null; }

            buffer = newNetworkCommunication.Receive();
            if (buffer.Length > 5)
                break;
            // create player
            player = Instantiate(PlayerPrefab);
            pc = player.GetComponent<PlayerController>();
            pc.Initialize(false, new Color(buffer[2], buffer[3], buffer[4]), buffer[1]);
            pcs.Add(pc);
        }
        yield return null;

        // all players added
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (newNetworkCommunication.DataAvailable)
        {
            byte[] buffer = newNetworkCommunication.Receive();
            if (buffer[0] == 0)
            {
                // standard
                byte ID = buffer[1];
                float yAngle = BitConverter.ToSingle(buffer, 2);
                float posX = BitConverter.ToSingle(buffer, 6);
                float posZ = BitConverter.ToSingle(buffer, 10);
                byte boosted = buffer[14];

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