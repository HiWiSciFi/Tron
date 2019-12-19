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
    public InputField IPAddressField;
    public InputField PortField;

    public GameObject ConnectButton;
    public GameObject BackButton;
    public GameObject CancelButton;
    public GameObject PopupPanel;
    public Text PopupText;

    private void Start()
    {
        MainMenuPanel.SetActive(true);
        DrivePanel.SetActive(false);
        newRandomMessage();
        PopupPanel.SetActive(false);
    }

    private const string messagesFile = "Assets/BuildResources/Messages.txt";

    private void newRandomMessage()
    {
        TextReader tr = new StreamReader(messagesFile);
        int NumberOfLines = TotalLines(messagesFile);
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
        PopupPanel.SetActive(true);
        ConnectButton.SetActive(false);
        BackButton.SetActive(false);
        StartCoroutine(connect());
    }

    IEnumerator connect()
    {
        PopupText.text = "Connecting...";
        yield return null;
        int result = newNetworkCommunication.Connect(IPAddressField.text, int.Parse(PortField.text));

        if (result == 0)
        {
                PopupText.text = "Connected";
                yield return new WaitForSeconds(2);
                PopupText.text = "Waiting for Round begin...";
        }
        else if (result == 1)
        {
            PopupText.text = "Could not connect to server";
        }
    }

    public void CancelButtonClicked()
    {
        StopCoroutine(connect());
        PopupPanel.SetActive(false);
        ConnectButton.SetActive(true);
        BackButton.SetActive(true);
    }

    public void QuitButtonClicked()
    {
        Application.Quit();
    }
}