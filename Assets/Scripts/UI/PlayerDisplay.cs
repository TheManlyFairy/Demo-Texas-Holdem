using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerDisplay : MonoBehaviourPun
{
    [SerializeField]
    Image playerIcon;
    [SerializeField]
    Image turnMarker;
    [SerializeField]
    Text playerName;
    [SerializeField]
    Text playerRemainingMoney;
    [SerializeField]
    Text playerTotalBet;
    [SerializeField]
    GameObject moneyBetFrame;

    public Vector2 IconAnchoredPosition { get { return playerIcon.GetComponent<RectTransform>().anchoredPosition; } }

    public void SetupPlayer(Player player)
    {
        //Assign player icon here
        // playerName.text = player.photonView.ViewID + "";
        playerName.text = player.name;
        playerRemainingMoney.text = "$" + player.money;
        playerTotalBet.text = "$0";
    }
    public void SetupNameOnly(Player player)
    {
        playerName.text = player.name;
    }
    public void UpdatePlayerMoney(int totalBet, int remainingMoney)
    {
        playerTotalBet.text = "$" + totalBet;
        playerRemainingMoney.text = "$" + remainingMoney;
    }
    public void HideDisplayName()
    {
        playerName.gameObject.SetActive(false);
    }
    public void ShowPlayerGameDisplay()
    {
        playerIcon.gameObject.SetActive(true);
        playerName.gameObject.SetActive(true);
        playerRemainingMoney.gameObject.SetActive(true);
        moneyBetFrame.gameObject.SetActive(true);
    }
    public void ShowPlayerTurnMarker()
    {

    }
    public void HidePlayerTurnMarker()
    {

    }
}
