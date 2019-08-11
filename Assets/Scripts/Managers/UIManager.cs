using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public Text playerName;
    public PlayerHandDisplay playerHandDisplay;
    public CommunityHandDisplay communityHandDisplay;
    private void Start()
    {
        PhotonGameManager.OnDealingCards += UpdatePlayerDisplay;
    }

    void UpdatePlayerDisplay()
    {
        playerHandDisplay.SetupPlayerHand(PhotonGameManager.CurrentPlayer);
        playerName.text = PhotonGameManager.CurrentPlayer.name;
    }

    public void DebugShowPlayer(int index)
    {
        playerHandDisplay.SetupPlayerHand(PhotonGameManager.players[index]);
        playerName.text = PhotonGameManager.players[index].name;
    }
}
