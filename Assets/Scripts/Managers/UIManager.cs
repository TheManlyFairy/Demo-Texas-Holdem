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
        GameManager.OnDealingCards += UpdatePlayerDisplay;
    }

    void UpdatePlayerDisplay()
    {
        playerHandDisplay.SetupPlayerHand(GameManager.CurrentPlayer);
        playerName.text = GameManager.CurrentPlayer.name;
    }

    public void DebugShowPlayer(int index)
    {
        playerHandDisplay.SetupPlayerHand(GameManager.players[index]);
        playerName.text = GameManager.players[index].name;
    }
}
