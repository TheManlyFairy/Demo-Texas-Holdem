using UnityEngine;
using UnityEngine.UI;
public class PlayerDisplay : MonoBehaviour
{
    [SerializeField]
    Image playerIcon;
    [SerializeField]
    Text playerName;
    [SerializeField]
    Text playerRemainingMoney;
    [SerializeField]
    Text playerTotalBet;

    public void SetupPlayer(Player player)
    {
        //Assign player icon here
        playerName.text = player.name;
        playerRemainingMoney.text = "$" + player.money;
        playerTotalBet.text = "$0";
    }
    public void UpdatePlayerMoney(int totalBet, int remainingMoney)
    {
        playerTotalBet.text = "$" + totalBet;
        playerRemainingMoney.text = "$" + remainingMoney;
    }
   
}
