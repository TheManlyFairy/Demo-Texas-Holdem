using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public Text playerName;
    public PlayerHandDisplay playerHandDisplay;
    public CommunityHandDisplay communityHandDisplay;
    public Text playerMoney;
    public Text playerCurrentBet;
    public Text currentPot;
    public Slider betValueSlider;
    public InputField betValueField;
    public Button raiseBet;
    public Button callBet;
    public Button check;
    public Button fold;

    static UIManager instance;


    private void Start()
    {
        instance = this;
    }
    public static void StartGame()
    {
        instance.SetupUIListeners();
    }
    
    void SetupUIListeners()
    {
        PhotonGameManager.OnDealingCards += UpdatePlayerDisplay;
        Dealer.OnInterfaceUpdate += UpdateGameInterface;
        Dealer.OnInterfaceUpdate += UpdatePlayerDisplay;
        betValueSlider.onValueChanged.AddListener(delegate
        {
            int sliderMinimum = Dealer.MinimumBet;
            int sliderMaximum = PhotonGameManager.CurrentPlayer.money;
            int betValue = (int)(betValueSlider.value * sliderMaximum);

            if(betValue >= Dealer.MinimumBet)
            {
                while (betValue % Dealer.MinimumBet != 0)
                    betValue--;
            }
            else
            {
                betValue = Dealer.MinimumBet;
            }
            PhotonGameManager.CurrentPlayer.AmountToBet = betValue;
            betValueField.text = "" + betValue;
        });
        raiseBet.onClick.AddListener(delegate
        {
            PhotonGameManager.CurrentPlayer.Raise();
            betValueSlider.value = 0;
        });
        callBet.onClick.AddListener(delegate
        {
            PhotonGameManager.CurrentPlayer.Call();
            betValueSlider.value = 0;
        });
        check.onClick.AddListener(delegate
        {
            PhotonGameManager.CurrentPlayer.Check();
            betValueSlider.value = 0;
        });
        fold.onClick.AddListener(delegate
        {
            PhotonGameManager.CurrentPlayer.Fold();
            betValueSlider.value = 0;
        });
        PhotonGameManager.OnDealingCards += UpdatePlayerDisplay;
    }
    void UpdateGameInterface()
    {
        playerMoney.text = "Cash: " + PhotonGameManager.CurrentPlayer.money;
        currentPot.text = "Total Cash Prize: " + Dealer.Pot;
    }
    void UpdatePlayerDisplay()
    {
        playerHandDisplay.SetupPlayerHand(PhotonGameManager.CurrentPlayer);
        playerName.text = PhotonGameManager.CurrentPlayer.name;
        playerMoney.text = "Cash: " + PhotonGameManager.CurrentPlayer.money;

        if(PhotonGameManager.CurrentPlayer.TotalBetThisRound<Dealer.HighestBetMade)
        {
            callBet.gameObject.SetActive(true);
            check.gameObject.SetActive(false);
        }
        else
        {
            callBet.gameObject.SetActive(false);
            check.gameObject.SetActive(true);
        }
        playerHandDisplay.SetupPlayerHand(PhotonGameManager.CurrentPlayer);
        playerName.text = PhotonGameManager.CurrentPlayer.name;
    }
    public void DebugShowPlayer(int index)
    {
        playerMoney.text = "Cash: " + PhotonGameManager.players[index].money;
        currentPot.text = "Total Cash Prize: " + Dealer.Pot;
        playerHandDisplay.SetupPlayerHand(PhotonGameManager.players[index]);
        playerName.text = PhotonGameManager.players[index].name;
        playerHandDisplay.SetupPlayerHand(PhotonGameManager.players[index]);
        playerName.text = PhotonGameManager.players[index].name;
    }
}
