using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject PlayerPrefab;

    private void OnEnable()
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
        }
        yield return null;

        // all players added
    }
}