using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public Text playerName;
    //public PlayerHandDisplay playerHandDisplay;
    public CommunityHandDisplay communityHandDisplay;
    public Text playerMoney;
    public Text playerCurrentBet;
    public Text currentPot;
    public List<PlayerDisplay> playerSeats;
    public Button NextRound, RestartGame, QuitGame;
    public GameObject winnerDisplay;
    public TextMeshProUGUI winnerText;
    /*Unused Variables
     * //public Slider betValueSlider;
    //public InputField betValueField;
    //public Button raiseBet;
    //public Button callBet;
    //public Button check;
    //public Button fold;*/

    private void Start()
    {
        instance = this;

        NextRound.onClick.AddListener(delegate { Dealer.StartNextRound(); });
        RestartGame.onClick.AddListener(delegate { PhotonGameManager.instance.StartGame(); });
        QuitGame.onClick.AddListener(delegate { Application.Quit(); });
    }
    public static void StartGame()
    {
        //  instance.SetupUIListeners();
    }

    public static void DeclareWinner(List<Player> winners)
    {
        if (winners.Count == 1)
        {
            instance.winnerText.text = winners[0].name + " wins!";
        }
        else
        {
            instance.winnerText.text = "Tied!";
        }
        instance.winnerDisplay.SetActive(true);
    }

    void UpdateGameInterface()
    {
        playerMoney.text = "Cash: " + PhotonGameManager.CurrentPlayer.money;
        currentPot.text = "Total Cash Prize: " + Dealer.Pot + " $";
    }
    void UpdatePlayerDisplay()
    {
        playerName.text = PhotonGameManager.CurrentPlayer.name;
        playerMoney.text = "Cash: " + PhotonGameManager.CurrentPlayer.money;
        playerName.text = PhotonGameManager.CurrentPlayer.name;
    }
    public void DebugShowPlayer(int index)
    {
        playerMoney.text = "Cash: " + PhotonGameManager.players[index].money;
        currentPot.text = "Total Cash Prize: " + Dealer.Pot;
        // playerHandDisplay.SetupPlayerHand(PhotonGameManager.players[index]);
        playerName.text = PhotonGameManager.players[index].name;
        //  playerHandDisplay.SetupPlayerHand(PhotonGameManager.players[index]);
        playerName.text = PhotonGameManager.players[index].name;
    }

    public void UpdatePot()
    {
        currentPot.text = Dealer.Pot + " $";
    }
    /* Unused Methods
    * //void SetupUIListeners()
    //{
    //    PhotonGameManager.OnDealingCards += UpdatePlayerDisplay;
    //    Dealer.OnInterfaceUpdate += UpdateGameInterface;
    //    Dealer.OnInterfaceUpdate += UpdatePlayerDisplay;
    //    betValueSlider.onValueChanged.AddListener(delegate
    //    {
    //        int sliderMinimum = Dealer.MinimumBet;
    //        int sliderMaximum = PhotonGameManager.CurrentPlayer.money;
    //        int betValue = (int)(betValueSlider.value * sliderMaximum);

    //        if(betValue >= Dealer.MinimumBet)
    //        {
    //            while (betValue % Dealer.MinimumBet != 0)
    //                betValue--;
    //        }
    //        else
    //        {
    //            betValue = Dealer.MinimumBet;
    //        }
    //        PhotonGameManager.CurrentPlayer.AmountToBet = betValue;
    //        betValueField.text = "" + betValue;
    //    });
    //    raiseBet.onClick.AddListener(delegate
    //    {
    //        PhotonGameManager.CurrentPlayer.Raise();
    //        betValueSlider.value = 0;
    //    });
    //    callBet.onClick.AddListener(delegate
    //    {
    //        PhotonGameManager.CurrentPlayer.Call();
    //        betValueSlider.value = 0;
    //    });
    //    check.onClick.AddListener(delegate
    //    {
    //        PhotonGameManager.CurrentPlayer.Check();
    //        betValueSlider.value = 0;
    //    });
    //    fold.onClick.AddListener(delegate
    //    {
    //        PhotonGameManager.CurrentPlayer.Fold();
    //        betValueSlider.value = 0;
    //    });
    //    PhotonGameManager.OnDealingCards += UpdatePlayerDisplay;
    //}*/
}
