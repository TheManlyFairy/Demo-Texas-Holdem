using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameSetupController : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject playerButtonPrefab;
   // [SerializeField] Transform playerButtonsContainer;
    [SerializeField] Text roomName;

    void Start()
    {
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        //CreatePlayer();
       // PhotonGameManager.instance.FindPlayers();
        //CreatePlayerButton();
    }


    void CreatePlayerButton()
    {
        Debug.Log("Creating Player Button");
        GameObject tempButton = PhotonNetwork.Instantiate(playerButtonPrefab.name, Vector3.zero, Quaternion.identity);
       // tempButton.transform.parent = playerButtonsContainer;
    }

    // Not used, Player created only in Client app
    /*void CreatePlayer()
    {
        Debug.Log("Creating Player");
        PhotonNetwork.Instantiate(playerPrefab.name, playerButtonsContainer.position, Quaternion.identity);
    }*/


}
