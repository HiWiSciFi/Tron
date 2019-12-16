using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainMenuManager : MonoBehaviour
{
    public Text randomMessage;
    public GameObject MainMenuPanel;
    public GameObject DrivePanel;

    private void Start()
    {
        MainMenuPanel.SetActive(true);
        DrivePanel.SetActive(false);
        newRandomMessage();
    }

    private void newRandomMessage()
    {
        TextReader tr = new StreamReader("Assets/BuildResources/Messages.txt");
        int NumberOfLines = TotalLines("Assets/BuildResources/Messages.txt");
        int r = Random.Range(0, NumberOfLines);
        for (int i = 0; i <= r; i++)
        {
            if (i == r)
            {
                randomMessage.text = tr.ReadLine();
                break;
            }
            tr.ReadLine();
        }
        tr.Close();
        tr.Dispose();
    }

    int TotalLines(string filePath)
    {
        using (StreamReader r = new StreamReader(filePath))
        {
            int i = 0;
            while (r.ReadLine() != null) { i++; }
            return i;
        }
    }

    public void GenerateRandomMessage()
    {

    }

    public void PlayButtonClicked()
    {
        MainMenuPanel.SetActive(false);
        DrivePanel.SetActive(true);
    }

    public void BackButtonClicked()
    {
        MainMenuPanel.SetActive(true);
        DrivePanel.SetActive(false);
        newRandomMessage();
    }

    public void ConnectButtonClicked()
    {

    }

    public void QuitButtonClicked()
    {
        Application.Quit();
    }
}