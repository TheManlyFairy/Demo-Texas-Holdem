using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class QuickStartRommController : MonoBehaviourPunCallbacks
{

    [SerializeField]
    int multiplayerSceneIndex;

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }


    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room " + PhotonNetwork.CurrentRoom.Name);
        StartGame();
    }

    void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            
            Debug.Log("Starting Game");
            PhotonNetwork.LoadLevel(multiplayerSceneIndex);
        }
    }
}
