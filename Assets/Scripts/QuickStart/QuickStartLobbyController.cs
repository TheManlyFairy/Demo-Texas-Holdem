using Photon.Pun;
using Photon.Realtime;
using System.Text;
using UnityEngine;

public class QuickStartLobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject quickStartButton;
    [SerializeField]
    GameObject quickCancelButton;
    [SerializeField]
    GameObject loadingButton;
    [SerializeField]
    int roomSize;


    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        quickStartButton.SetActive(true);
        loadingButton.SetActive(false);
    }

    public void QuickStart()
    {
        quickStartButton.SetActive(false);
        quickCancelButton.SetActive(true);
        //PhotonNetwork.JoinRandomRoom();
        CreateRoom();
        Debug.Log("QuickStart");
    }
    /*
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join room");
        CreatRoom();
    }
    */

    void CreateRoom()
    {
        Debug.Log("Creating room now");
        int randomRoomNumber = Random.Range(1, 100);
        StringBuilder roomCode = GenerateRoomCode();
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize };
        if (PhotonNetwork.CreateRoom("Room " + randomRoomNumber, roomOps))
            Debug.Log("Created room: Room " + randomRoomNumber);
    }

    StringBuilder GenerateRoomCode()
    {
        StringBuilder randomCode = new StringBuilder();

        randomCode.Append(Random.Range('A', 'Z'));
        randomCode.Append(Random.Range('A', 'Z'));
        randomCode.Append(Random.Range('A', 'Z'));
        randomCode.Append(Random.Range('A', 'Z'));

        return randomCode;

    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Create room FAILED");
        CreateRoom();
    }

    public void QuickCancel()
    {
        quickCancelButton.SetActive(false);
        quickStartButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }

}
