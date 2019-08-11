using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class QuickStartLobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject quickStartButton;
    [SerializeField]
    GameObject quickCancelButton;
    [SerializeField]
    int roomSize;


    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        quickStartButton.SetActive(true);
    }

    public void QuickStart()
    {
        quickStartButton.SetActive(false);
        quickCancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("QuickStart");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join room");
        CreatRoom();
    }

    void CreatRoom()
    {
        Debug.Log("Creating room now");
        int randomRoomNumber = Random.Range(1, 100);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte) roomSize};
        PhotonNetwork.CreateRoom("Room " + randomRoomNumber, roomOps);
        Debug.Log("Created room: " + "Room " + randomRoomNumber);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Create room FAILED");
        CreatRoom();
    }

    public void QuickCancel()
    {
        quickCancelButton.SetActive(false);
        quickStartButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }

}
