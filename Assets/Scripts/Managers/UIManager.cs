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
        GameManager.OnDealingCards += UpdatePlayerDisplay;
        Dealer.OnInterfaceUpdate += UpdateGameInterface;
        Dealer.OnInterfaceUpdate += UpdatePlayerDisplay;
        betValueSlider.onValueChanged.AddListener(delegate
        {
            int sliderMinimum = Dealer.MinimumBet;
            int sliderMaximum = GameManager.CurrentPlayer.money;
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
            GameManager.CurrentPlayer.AmountToBet = betValue;
            betValueField.text = "" + betValue;
        });
        raiseBet.onClick.AddListener(delegate
        {
            GameManager.CurrentPlayer.Raise();
            betValueSlider.value = 0;
        });
        callBet.onClick.AddListener(delegate
        {
            GameManager.CurrentPlayer.Call();
            betValueSlider.value = 0;
        });
        check.onClick.AddListener(delegate
        {
            GameManager.CurrentPlayer.Check();
            betValueSlider.value = 0;
        });
        fold.onClick.AddListener(delegate
        {
            GameManager.CurrentPlayer.Fold();
            betValueSlider.value = 0;
        });
        PhotonGameManager.OnDealingCards += UpdatePlayerDisplay;
    }
    void UpdateGameInterface()
    {
        playerMoney.text = "Cash: " + GameManager.CurrentPlayer.money;
        currentPot.text = "Total Cash Prize: " + Dealer.Pot;
    }
    void UpdatePlayerDisplay()
    {
        playerHandDisplay.SetupPlayerHand(GameManager.CurrentPlayer);
        playerName.text = GameManager.CurrentPlayer.name;
        playerMoney.text = "Cash: " + GameManager.CurrentPlayer.money;

        if(GameManager.CurrentPlayer.TotalBetThisRound<Dealer.HighestBetMade)
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
        playerMoney.text = "Cash: " + GameManager.players[index].money;
        currentPot.text = "Total Cash Prize: " + Dealer.Pot;
        playerHandDisplay.SetupPlayerHand(GameManager.players[index]);
        playerName.text = GameManager.players[index].name;
        playerHandDisplay.SetupPlayerHand(PhotonGameManager.players[index]);
        playerName.text = PhotonGameManager.players[index].name;
    }
}
