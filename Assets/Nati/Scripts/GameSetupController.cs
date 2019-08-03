using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameSetupController : MonoBehaviour
{
    Vector3 randomPosition;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] Text roomName;

    void Start()
    {
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        CreatePlayer(); 
    }

  void CreatePlayer()
    {
        randomPosition = new Vector3(Random.Range(0, 3), Random.Range(0, 3), Random.Range(0, 3));
        Debug.Log("Creating Player");
        PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero + randomPosition, Quaternion.identity);
        
    }
}
